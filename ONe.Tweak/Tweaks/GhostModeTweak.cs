using System.Reflection;
using HarmonyLib;
using Kitchen.ONe.Tweak.Config.Sections;
using Kitchen.ONe.Tweak.Utils;
using UnityEngine;

namespace Kitchen.ONe.Tweak.Tweaks;

/// <summary>
/// Enable ghost mode by turning off player colliders.
/// </summary>
public static class GhostModeTweak
{
    /// <summary>
    /// Keep track of whether the ghost mode is currently enabled or not.
    /// </summary>
    private static bool _ghostModeEnabled;
    
    /// <summary>
    /// Toggle ghost mode by enabling or disabling player colliders.
    /// </summary>
    public static void ToggleGhostMode()
    {
        _ghostModeEnabled = !_ghostModeEnabled;
        
        foreach (var playerView in Object.FindObjectsOfType<PlayerView>())
        {
            var colliders = playerView.GetComponents<Collider>();

            foreach (var collider in colliders)
            {
                collider.enabled = !_ghostModeEnabled;
            }
        }
    }

    /// <summary>
    /// System that can enable or disable ghost mode automatically.
    /// </summary>
    private class GhostModeSystem : TweakRestaurantSystem<GhostModeSystem>
    {
        protected override void OnUpdate()
        {
            // Enable ghost mode on preparation start
            if (Has<SIsNightFirstUpdate>() && GhostModeConfig.Instance.EnableOnPreparationStart.Value && !_ghostModeEnabled)
            {
                ToggleGhostMode();
            }
            
            // Disable ghost mode on preparation end
            if (Has<SIsDayFirstUpdate>() &&  GhostModeConfig.Instance.DisableOnPreparationEnd.Value && _ghostModeEnabled)
            {
                ToggleGhostMode(); 
            }
        }
    }

    /// <summary>
    /// This patch makes it possible to expand map bounds when in ghost mode.
    ///
    /// The main idea is that if the ghost mode is enabled and resizing is also enabled,
    /// whenever someone asks for the current bounds, we give them a bit larger bounds.
    ///
    /// This method is non-destructive: If you save the game or stop playing with the mod,
    /// your bounds will be the same as originally.
    /// </summary>
    [HarmonyPatch(typeof(EnforcePlayerBounds))]
    private static class ResizeBoundsWhenInGhostModePatch
    {
        public static MethodInfo TargetMethod()
        {
            return AccessTools.Property(typeof(EnforcePlayerBounds), "Bounds").GetMethod;
        }
        
        public static void Postfix(ref Bounds __result)
        {
            if (_ghostModeEnabled && GhostModeConfig.Instance.ResizeBoundsWhenEnabled.Value)
            {
                __result = new Bounds(__result.center, __result.size + new Vector3(4, 0, 6));
            }
        }
    }
}
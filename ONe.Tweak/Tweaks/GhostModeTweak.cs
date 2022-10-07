using System.Reflection;
using BepInEx.Configuration;
using HarmonyLib;
using Kitchen.ONe.Tweak.Utils;
using ONe.Tweak;
using Unity.Entities;
using UnityEngine;

namespace Kitchen.ONe.Tweak.Tweaks;

public static class GhostModeTweak
{
    private static bool _ghostModeEnabled = false;
    
    public static void ToggleCollisions()
    {
        _ghostModeEnabled = !_ghostModeEnabled;
        
        foreach (var playerView in GameObject.FindObjectsOfType<PlayerView>())
        {
            var player = playerView.gameObject;
            var colliders = playerView.GetComponents<Collider>();

            foreach (var collider in colliders)
            {
                collider.enabled = !_ghostModeEnabled;
            }
        }
    }

    private class GhostModeSystem : TweakRestaurantSystem<GhostModeSystem>
    {
        protected override void OnUpdate()
        {
            if (Has<SIsNightFirstUpdate>() && GhostModeGUIConfig.EnableOnPreparationStart.Value && !_ghostModeEnabled)
            {
                ToggleCollisions();
            }
            
            if (Has<SIsDayFirstUpdate>() && GhostModeGUIConfig.DisableOnPreparationEnd.Value && _ghostModeEnabled)
            {
                ToggleCollisions(); 
            }
        }
    }

    [HarmonyPatch(typeof(EnforcePlayerBounds))]
    private static class EnforcePlayerBoundsPatch
    {
        public static MethodInfo TargetMethod()
        {
            return AccessTools.Property(typeof(EnforcePlayerBounds), "Bounds").GetMethod;
        }

        public static void Postfix(ref Bounds __result)
        {
            if (_ghostModeEnabled)
            {
                __result = new Bounds(__result.center, __result.size + new Vector3(4, 0, 6));
            }
        }
    }
}

public class GhostModeGUIConfig : TweakGUIConfig
{
    public override string Section => "GhostMode";

    public override string Name => "";

    public static ConfigEntry<bool> EnableOnPreparationStart;

    public static ConfigEntry<bool> DisableOnPreparationEnd;
    
    public static ConfigEntry<bool> ResizeBoundsWhenInGhostMode;

    public override void Init()
    {
        BindButton("Toggle ghost mode", Run);
        EnableOnPreparationStart = Bind("EnableOnPreparationStart", false);
        DisableOnPreparationEnd = Bind("DisableOnPreparationEnd", true);
        DisableOnPreparationEnd = Bind("ResizeBoundsWhenInGhostMode", true);
    }

    private void Run()
    {
        GhostModeTweak.ToggleCollisions();
    }
}
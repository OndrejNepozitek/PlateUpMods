using System.Linq;
using System.Reflection;
using HarmonyLib;
using Kitchen.ONe.Tweak.Config.Sections;
using Kitchen.ONe.Tweak.Utils;
using KitchenData;
using Unity.Entities;

namespace Kitchen.ONe.Tweak.Tweaks;

public static class RestartAnyDayTweak
{
    private static bool _changePopup;
    
    /// <summary>
    /// When the game asks if the player can restart the day due to low day number (3 or less), return true.
    /// </summary>
    [HarmonyPatch(typeof(CheckGameOverFromLife), "RescuedByDay")]
    private static class RescuedByDayPatch
    {
        public static void Postfix(ref bool __result)
        {
            if (CreativeModeConfig.Instance.RestartAnyDay.Value && __result == false)
            {
                // We need to run some code beforehand to make sure that the game can actually be restarted.
                RestartAnyDaySystem.Instance?.RestartDay();
                _changePopup = true;
                __result = true;
            }
        }
    }
    
    /// <summary>
    /// This system does what RescuedByDay usually does when the game can be restarted.
    /// </summary>
    private class RestartAnyDaySystem : TweakRestaurantSystem<RestartAnyDaySystem>
    {
        public void RestartDay()
        {
            World.Add(new COfferRestartDay()
            {
                Reason = LossReason.Patience
            });
            
            var query = this.GetEntityQuery(ComponentType.ReadWrite<SKitchenStatus>());
            var singleton = query.GetSingleton<SKitchenStatus>();
            query.SetSingleton(new SKitchenStatus()
            {
                RemainingLives = singleton.TotalLives,
                TotalLives = singleton.TotalLives
            });
        }
    }
    
    [HarmonyPatch(typeof(GlobalLocalisation))]
    private static class ChangeRestartPopupTextPatch
    {
        public static MethodBase TargetMethod()
        {
            var indexerProperty = AccessTools
                .GetDeclaredProperties(typeof(GlobalLocalisation))
                .First(x => x.GetIndexParameters().Length == 1 &&
                            x.GetIndexParameters()[0].ParameterType == typeof(PopupType)); 

            var getter = indexerProperty.GetMethod; 

            return getter;
        }
        
        public static void Postfix(ref PopupDetails __result, PopupType popup_type)
        {
            if (!_changePopup)
            {
                return;
            }
            _changePopup = false;
            
            if (popup_type == PopupType.RestartRestaurantAfterFailure && CreativeModeConfig.Instance.RestartAnyDay.Value)
            {
                __result.Description = "You can restart the current day because you have the ONe.Tweak mod installed. You can disable this feature in the config.";
            }
        }
    }
}
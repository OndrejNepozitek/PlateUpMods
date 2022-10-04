using System;
using System.Linq;
using System.Reflection;
using BepInEx.Configuration;
using HarmonyLib;
using KitchenData;
using ONe.Tweak;
using Unity.Entities;

namespace Kitchen.ONe.Tweak.Tweaks;

public class RestartAnyDayTweak
{
    [HarmonyPatch(typeof(CheckGameOverFromLife), "RescuedByDay")]
    private class CheckGameOverFromLife_RescuedByDay_Patch
    {
        public static void Postfix(ref bool __result)
        {
            if (RestartAnyDayConfig.IsEnabled)
            {
                RestartAnyDaySystem.Instance.RestartDay();
                __result = true;
            }
        }
    }
    
    [HarmonyPatch(typeof(GlobalLocalisation))]
    private class GlobalLocalisation_Indexer_Patch
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
            if (popup_type == PopupType.RestartRestaurantAfterFailure)
            {
                // TODO: show this message only if the day was actually restarted because of our mod
                __result.Description = "You can restart the current day because you have the ONe.Tweak mod installed. You can disable this feature in the config.";
            }
        }
    }

    private class RestartAnyDaySystem : RestaurantSystem
    {
        public static RestartAnyDaySystem Instance { get; private set; }

        public RestartAnyDaySystem()
        {
            Instance = this;
        }
        
        protected override void OnUpdate()
        {
            
        }

        public void RestartDay()
        {
            World.Add(new COfferRestartDay()
            {
                Reason = LossReason.Patience
            });
            
            var query = this.GetEntityQuery(ComponentType.ReadWrite<SKitchenStatus>());
            var singleton = query.GetSingleton<SKitchenStatus>();
            query.SetSingleton<SKitchenStatus>(new SKitchenStatus()
            {
                RemainingLives = singleton.TotalLives,
                TotalLives = singleton.TotalLives
            });
        }
    }
}

public class RestartAnyDayConfig : TweakGUIConfig
{
    public override string Section => "General";

    public override string Name => "RestartAnyDay";

    private static ConfigEntry<bool> _enableConfig;

    public static bool IsEnabled => _enableConfig.Value;

    public override void Init()
    {
        _enableConfig = Bind("Enabled", false);
    }
}
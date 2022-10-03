using System;
using BepInEx.Configuration;
using HarmonyLib;
using ONe.Tweak;
using Unity.Entities;

namespace Kitchen.ONe.Tweak.Tweaks;

public static class SkipDayTweak
{
    private static bool _shouldRun = false;
    
    public static void Run()
    {
        _shouldRun = true;
    }

    [UpdateInGroup(typeof (TimeManagementGroup))]
    [UpdateAfter(typeof(AdvanceTime))]
    public class SkipDaySystem : RestaurantSystem
    {
        protected override void OnUpdate()
        {
            if (!_shouldRun)
            {
                return;
            }

            _shouldRun = false;

            var advanceTimeSystem = World.GetExistingSystem<AdvanceTime>();
            var becomeNightMethod = AccessTools.Method(typeof(AdvanceTime), "BecomeNight");
            becomeNightMethod.Invoke(advanceTimeSystem, Array.Empty<object>());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            Console.WriteLine("SkipDaySystem.OnDestroy");
            _shouldRun = false;
        }
    }
}

public class SkipDayGUIConfig : TweakGUIConfig
{
    public override string Section => "Creative";
    
    public override string Name => "Skip day";

    public override void Init()
    {
        BindButton("Skip day", Run);
    }

    private void Run()
    {
        SkipDayTweak.Run();
    }
}
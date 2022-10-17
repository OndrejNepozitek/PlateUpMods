using System;
using HarmonyLib;
using Unity.Entities;

namespace Kitchen.ONe.Tweak.Tweaks;

/// <summary>
/// This tweak makes it possible to skip a day.
/// </summary>
public static class SkipDayTweak
{
    private static bool _shouldRun;
    
    public static void Run()
    {
        _shouldRun = true;
    }

    /// <summary>
    /// This system makes it possible to skip a day when requested.
    /// </summary>
    [UpdateInGroup(typeof (TimeManagementGroup))]
    [UpdateAfter(typeof(AdvanceTime))]
    private class SkipDaySystem : RestaurantSystem
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
            _shouldRun = false;
        }
    }
}
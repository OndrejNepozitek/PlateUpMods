using System.Collections.Generic;
using BepInEx.Configuration;
using Kitchen.ONe.Tweak.Tweaks;
using ONe.Tweak;
using UnityEngine;

namespace Kitchen.ONe.Tweak.Config;

public static class ConfigHelper
{
    private static List<TweakGUIConfig> _configs;

    public static void SetUp(ConfigFile config)
    {
        _configs = new List<TweakGUIConfig>()
        {
            new SkipDayGUIConfig(),
            new RestartAnyDayConfig(),
            new StartPracticeGUIConfig(),
            new GhostModeGUIConfig(),
            new SeparateKitchenSeedsGUIConfig(),
        };

        foreach (var tweakConfig in _configs)
        {
            tweakConfig.Config = config;
            tweakConfig.Init();
        }
    }

    public static void Update()
    {
        foreach (var config in _configs)
        {
            config.Update();
        }
    }
}
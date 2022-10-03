using System.Collections.Generic;
using BepInEx.Configuration;
using Kitchen.ONe.Tweak.Tweaks;
using ONe.Tweak;
using UnityEngine;

namespace Kitchen.ONe.Tweak.Config;

public static class ConfigHelper
{
    private static List<TweakCommandConfig> _configs;

    public static void SetUp(ConfigFile config)
    {
        _configs = new List<TweakCommandConfig>()
        {
            new SkipDayCommandConfig(),
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
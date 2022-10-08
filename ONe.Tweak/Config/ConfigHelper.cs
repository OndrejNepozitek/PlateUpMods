using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using Kitchen.ONe.Tweak.Config.Sections;
using Kitchen.ONe.Tweak.Tweaks;
using ONe.Tweak;
using UnityEngine;

namespace Kitchen.ONe.Tweak.Config;

public static class ConfigHelper
{
    private static List<ConfigSection> _configSections;

    public static void SetUp(ConfigFile config)
    {
        _configSections = new List<ConfigSection>()
        {
            new GhostModeConfig(config),
            new PreparationPhaseConfig(config),
            new SeparateSeedsConfig(config),
        };
    }
    
    public static Action GetAction(ConfigEntry<KeyboardShortcut> entry)
    {
        foreach (var config in _configSections)
        {
            var action = config.GetAction(entry);

            if (action != null)
            {
                return action;
            }
        }

        return null;
    }

    public static void Update()
    {
        foreach (var config in _configSections)
        {
            config.Update();
        }
    }
}
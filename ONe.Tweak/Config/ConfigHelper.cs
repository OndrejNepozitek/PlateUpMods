using BepInEx.Configuration;
using Kitchen.ONe.Tweak.Tweaks;
using UnityEngine;

namespace Kitchen.ONe.Tweak.Config;

public static class ConfigHelper
{
    private static ConfigEntry<string> _skipDayConfig;
    private static ConfigEntry<KeyboardShortcut> _skipDayConfigShortcut;

    public static void SetUp(ConfigFile config)
    {
        _skipDayConfig = config.Bind(
            "Creative", "SkipDay", "",
            new ConfigDescription("This value is currently not used", null,
                new ConfigurationManagerAttributes
                {
                    CustomDrawer = SkipDayDrawer,
                    HideSettingName = true,
                    HideDefaultButton = true,
                }));
        _skipDayConfigShortcut = config.Bind("Creative", "SkipDayShortcut", new KeyboardShortcut(KeyCode.None));
    }

    public static void Update()
    {
        if (_skipDayConfigShortcut.Value.IsDown())
        {
            SkipDayTweak.Run();
        }
    }

    private static void SkipDayDrawer(ConfigEntryBase obj)
    {
        if (GUILayout.Button("Skip day", GUILayout.ExpandWidth(true)))
        {
            SkipDayTweak.Run();
        }
    }
}
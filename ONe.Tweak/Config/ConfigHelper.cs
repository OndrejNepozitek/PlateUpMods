using BepInEx.Configuration;
using Kitchen.ONe.Tweak.Tweaks;
using UnityEngine;

namespace Kitchen.ONe.Tweak.Config;

public static class ConfigHelper
{
    public static void SetUp(ConfigFile config)
    {
        var skipDayConfig = config.Bind(
            "Creative", "SkipDay", "",
            new ConfigDescription("This value is currently not used", null,
                new ConfigurationManagerAttributes
                {
                    CustomDrawer = SkipDayDrawer,
                    HideSettingName = true,
                    HideDefaultButton = true,
                }));
    }

    private static void SkipDayDrawer(ConfigEntryBase obj)
    {
        if (GUILayout.Button("Skip day", GUILayout.ExpandWidth(true)))
        {
            SkipDayTweak.Run();
        }
    }
}
using BepInEx.Configuration;
using Kitchen.ONe.Tweak.Tweaks;

namespace Kitchen.ONe.Tweak.Config.Sections;

public class CreativeModeConfig : ConfigSection<CreativeModeConfig>
{
    public override string SectionName => ConfigSections.CreativeMode;

    public CreativeModeConfig(ConfigFile config) : base(config)
    {
        BindCommand("SkipDay", KeyboardShortcut.Empty, SkipDay, "Skip the current day");
    }

    private void SkipDay()
    {
        SkipDayTweak.Run();
    }
}
using BepInEx.Configuration;

namespace Kitchen.ONe.Tweak.Config.Sections;

public class CasualModeConfig : ConfigSection<CasualModeConfig>
{
    public override string SectionName => ConfigSections.CasualMode;
    
    public ConfigEntry<bool> RestartAnyDay { get; }
    
    public CasualModeConfig(ConfigFile config) : base(config)
    {
        RestartAnyDay = Bind("RestartAnyDay", true, "Whether any day can be restarted the same way as the first 3 days.");
    }
}
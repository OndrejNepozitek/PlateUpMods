using BepInEx.Configuration;

namespace Kitchen.ONe.Tweak.Config.Sections;

public class SeparateSeedsConfig : ConfigSection<SeparateSeedsConfig>
{
    public override string SectionName => ConfigSections.SeparateSeeds;
    
    public ConfigEntry<bool> Enabled { get; }
    
    public ConfigEntry<bool> UseRandomSeed { get; }
    
    public ConfigEntry<string> FixedSeed { get; } 
    
    public SeparateSeedsConfig(ConfigFile config) : base(config)
    {
        Enabled = Bind("Enabled", false, "Whether to use one seed for layout generation and a different seed for everything else.");
        UseRandomSeed = Bind("UseRandomSeed", true, "Whether the other seed should be randomly generated or not.");
        FixedSeed = Bind("FixedSeed", "", "If UseRandomSeed is set to false, this seed will be used as the other seed.");
    }
}
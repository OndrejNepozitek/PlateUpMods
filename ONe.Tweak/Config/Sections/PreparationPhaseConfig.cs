using BepInEx.Configuration;
using Kitchen.ONe.Tweak.Tweaks;

namespace Kitchen.ONe.Tweak.Config.Sections;

public class PreparationPhaseConfig : ConfigSection<PreparationPhaseConfig>
{
    public override string SectionName => ConfigSections.PreparationPhase;
    
    public ConfigEntry<bool> RestorePositionsAfterPractice { get; }
    
    public PreparationPhaseConfig(ConfigFile config) : base(config)
    {
        RestorePositionsAfterPractice = Bind("RestorePositionsAfterPractice", true, "Whether to restore player positions after exiting the practice mode.");
        BindCommand("StartPractice", KeyboardShortcut.Empty, StartPractice, "Keyboard shortcut for starting the practice mode from anywhere.");
    }

    private void StartPractice()
    {
        StartPracticeTweak.Run();
    }
}
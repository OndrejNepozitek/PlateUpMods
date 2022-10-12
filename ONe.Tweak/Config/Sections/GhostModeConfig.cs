using BepInEx.Configuration;
using Kitchen.ONe.Tweak.Tweaks;

namespace Kitchen.ONe.Tweak.Config.Sections;

public class GhostModeConfig : ConfigSection<GhostModeConfig>
{
    public override string SectionName => ConfigSections.GhostMode;
    
    public ConfigEntry<bool> EnableOnPreparationStart { get; }
    
    public ConfigEntry<bool> DisableOnPreparationEnd { get; }
    
    public ConfigEntry<bool> ResizeBoundsWhenEnabled { get; }
    
    public GhostModeConfig(ConfigFile config) : base(config)
    {
        EnableOnPreparationStart = Bind("EnableOnPreparationStart", false, "Whether to enable ghost mode at the start of each day's preparation phase.");
        DisableOnPreparationEnd = Bind("DisableOnPreparationEnd", true, "Whether to disable ghost mode at the end of each day's preparation phase.");
        ResizeBoundsWhenEnabled = Bind("ResizeBoundsWhenEnabled", true, "Whether to resize map bounds when in ghost mode. Otherwise, the game teleports you to the front door if you leave the walls.");
        BindCommand("ToggleGhostMode", KeyboardShortcut.Empty, ToggleGhostMode ,"Keyboard shortcut for toggling the ghost mode.");
    }

    private void ToggleGhostMode()
    {
        GhostModeTweak.ToggleGhostMode();
    }
}
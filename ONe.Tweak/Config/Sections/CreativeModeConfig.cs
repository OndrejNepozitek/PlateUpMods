using BepInEx.Configuration;
using Kitchen.ONe.Tweak.Tweaks;

namespace Kitchen.ONe.Tweak.Config.Sections;

public class CreativeModeConfig : ConfigSection<CreativeModeConfig>
{
    public override string SectionName => ConfigSections.CreativeMode;
    
    public ConfigEntry<bool> RestartAnyDay { get; }
    
    public ConfigEntry<bool> EnableAdditionalCards { get; }

    public ConfigEntry<int> AdditionalCustomerDishCards { get; }
    
    public ConfigEntry<int> AdditionalFranchiseCards { get; }
    
    public CreativeModeConfig(ConfigFile config) : base(config)
    {
        RestartAnyDay = Bind("RestartAnyDay", true, "Whether any day can be restart the same way as the first 3 days.");
        
        BindCommand("SkipDay", KeyboardShortcut.Empty, SkipDay, "Skip the current day");
        
        EnableAdditionalCards = Bind("EnableAdditionalCards", false, "Whether choose from additional customer, dish and franchise card.");
        AdditionalCustomerDishCards = Bind("AdditionalCustomerDishCards", 0, "How many additional customer/dish cards to choose from");
        AdditionalFranchiseCards = Bind("AdditionalFranchiseCards", 2, "How many additional customer/dish cards to choose from");
        BindCommand("ShowNextCard", KeyboardShortcut.Empty, ShowNextCard, "");
    }

    private void SkipDay()
    {
        SkipDayTweak.Run();
    }

    private void ShowNextCard()
    {
        SelectFromMoreCardsTweak.ShowNextCard();
    }
}
using BepInEx.Configuration;
using Kitchen.ONe.Tweak.Tweaks;

namespace Kitchen.ONe.Tweak.Config.Sections;

public class CardsConfig : ConfigSection<CardsConfig>
{
    public override string SectionName => ConfigSections.Cards;
    
    public ConfigEntry<bool> EnableAdditionalCards { get; }

    public ConfigEntry<int> AdditionalCustomerDishCards { get; }
    
    public ConfigEntry<int> AdditionalFranchiseCards { get; }
    
    public CardsConfig(ConfigFile config) : base(config)
    {
        EnableAdditionalCards = Bind("EnableAdditionalCards", false, "Whether choose from additional customer, dish and franchise card.");
        AdditionalCustomerDishCards = Bind("AdditionalCustomerDishCards", 0, new ConfigDescription("How many additional customer/dish cards to choose from.", new AcceptableValueRange<int>(0, 10)));
        AdditionalFranchiseCards = Bind("AdditionalFranchiseCards", 2, new ConfigDescription("How many additional franchise cards to choose from.", new AcceptableValueRange<int>(0, 10)));
        BindCommand("ShowNextCard", KeyboardShortcut.Empty, ShowNextCard, "Cycle through all available cards.");
    }
    
    private void ShowNextCard()
    {
        SelectFromMoreCardsTweak.ShowNextCard();
    }
}
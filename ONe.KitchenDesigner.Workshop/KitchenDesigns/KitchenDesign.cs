using Kitchen.Layouts;
using KitchenData;

namespace ONe.KitchenDesigner.KitchenDesigns;

/// <summary>
/// Represents a loaded and decoded kitchen design.
/// </summary>
public class KitchenDesign
{
    public LayoutBlueprint Blueprint { get; }
    
    public LayoutProfile Profile { get; }
    
    public RestaurantSetting Setting { get; }
    
    public KitchenDesign(LayoutBlueprint blueprint, LayoutProfile profile, RestaurantSetting setting)
    {
        Blueprint = blueprint;
        Profile = profile;
        Setting = setting;
    }
}
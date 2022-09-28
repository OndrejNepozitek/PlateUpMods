using Kitchen.Layouts;
using Unity.Entities;

namespace ONe.KitchenDesigner.KitchenDesigns;

public class SeededLayout
{
    public string Seed { get; }
    
    public LayoutBlueprint LayoutBlueprint { get; }
    
    public Entity LayoutEntity { get; }
    
    public Entity MapItem { get; }
    
    public SeededLayout(string seed, LayoutBlueprint layoutBlueprint, Entity layoutEntity, Entity mapItem)
    {
        LayoutBlueprint = layoutBlueprint;
        LayoutEntity = layoutEntity;
        MapItem = mapItem;
        Seed = seed;
    }
}
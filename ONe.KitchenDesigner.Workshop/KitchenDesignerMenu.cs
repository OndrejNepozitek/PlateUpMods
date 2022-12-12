using Kitchen;
using Kitchen.Modules;
using UnityEngine;

namespace Kitchen.ONe.KitchenDesigner.Workshop;

public class KitchenDesignerMenu<T> : Menu<T>
{
    public KitchenDesignerMenu(Transform instanceButtonContainer, ModuleList argsModuleList) : base(instanceButtonContainer, argsModuleList)
    {
        DefaultElementSize.x = 2 * DefaultElementSize.x;
    }

    public override void Setup(int player_id)
    {
        AddLabel("Kitchen Designer");
        var e = AddInfo("Copy your Exported Kitchen Design Description in the text area below:");
        Debug.Log(e.BoundingBox);
        Debug.Log(e.Position);
        e = AddInfo("Status: No custom design provided");
        Debug.Log(e.BoundingBox);
        Debug.Log(e.Position);
        AddButton("Generate", (number) => Debug.Log("Clicked"));
    }
}
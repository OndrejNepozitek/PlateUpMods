using BepInEx;
using ONe.KitchenDesigner.Config;

namespace ONe.KitchenDesigner;

[BepInPlugin(Guid, Name, Version)]
[BepInProcess("PlateUp.exe")]
public class KitchenDesigner : BaseUnityPlugin
{
    private const string Guid = "ONe.KitchenDesigner";
    private const string Name = "ONe.KitchenDesigner";
    private const string Version = "1.0.0";

    private void Awake()
    {
        ConfigHelper.SetUp(Config);
    }
}
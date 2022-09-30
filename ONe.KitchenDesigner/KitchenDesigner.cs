using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ONe.KitchenDesigner.Config;

namespace ONe.KitchenDesigner;

[BepInPlugin(Guid, Name, Version)]
[BepInProcess("PlateUp.exe")]
public class KitchenDesigner : BaseUnityPlugin
{
    private const string Guid = "ONe.KitchenDesigner";
    private const string Name = "ONe.KitchenDesigner";
    private const string Version = "1.0.0";
    
    internal new static ManualLogSource Log;

    private void Awake()
    {
        Log = Logger;
        
        var harmony = new Harmony(Guid);
        harmony.PatchAll();
        
        ConfigHelper.SetUp(Config);
    }
}
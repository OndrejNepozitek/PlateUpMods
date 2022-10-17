using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Kitchen.ONe.Tweak.Config;

namespace Kitchen.ONe.Tweak;

[BepInPlugin(Guid, Name, Version)]
[BepInProcess("PlateUp.exe")]
public class TweakPlugin : BaseUnityPlugin
{
    private const string Guid = "ONe.Tweak";
    private const string Name = "ONe.Tweak";
    private const string Version = "1.0.0";
    
    internal static ManualLogSource Log;

    private void Awake()
    {
        Log = Logger;
        
        var harmony = new Harmony(Guid);
        harmony.PatchAll();
        
        ConfigHelper.SetUp(Config);
    }

    private void Update()
    {   
        ConfigHelper.Update();
    }
}
using System;
using BepInEx.Configuration;
using HarmonyLib;
using ONe.Tweak;
using Unity.Entities;

namespace Kitchen.ONe.Tweak.Tweaks;

public class SeparateKitchenSeedsTweak
{
    [HarmonyPatch(typeof(SetSeededRunOverride), "CreateMapItem")]
    public class ChangeLayoutSeedPatch
    {
        public static void Prefix(ref Seed seed)
        {
            if (SeparateKitchenSeedsGUIConfig.IsEnabled == false)
            {
                return;
            }

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var seedFixers = entityManager.CreateEntityQuery((ComponentType) typeof (CSeededRunInfo));

            if (seedFixers.IsEmpty)
            {
                return;
            }

            var seedFixer = seedFixers.First();
            var seededRunInfo = entityManager.GetComponentData<CSeededRunInfo>(seedFixer);
            seed = Seed.Generate(new Random().Next());
            seededRunInfo.FixedSeed = seed;
            entityManager.SetComponentData(seedFixer, seededRunInfo);
        }
    }
}

public class SeparateKitchenSeedsGUIConfig : TweakGUIConfig
{
    public override string Section => "Seeds";
    
    public override string Name => "Separate kitchen seeds";
    
    private static ConfigEntry<bool> _enableConfig;

    public static bool IsEnabled => _enableConfig.Value;
    
    public override void Init()
    {
        _enableConfig = Bind("Enabled", false);
    }
}
using System;
using HarmonyLib;
using Kitchen.ONe.Tweak.Config.Sections;
using Unity.Entities;

namespace Kitchen.ONe.Tweak.Tweaks;

public static class SeparateKitchenSeedsTweak
{
    [HarmonyPatch(typeof(SetSeededRunOverride), "CreateMapItem")]
    private static class ChangeLayoutSeedPatch
    {
        public static void Prefix(ref Seed seed)
        {
            if (!SeparateSeedsConfig.Instance.Enabled.Value)
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
            seed = string.IsNullOrWhiteSpace(SeparateSeedsConfig.Instance.FixedSeed.Value) || SeparateSeedsConfig.Instance.UseRandomSeed.Value
                ? Seed.Generate(new Random().Next())
                : new Seed(SeparateSeedsConfig.Instance.FixedSeed.Value);
            seededRunInfo.FixedSeed = seed;
            entityManager.SetComponentData(seedFixer, seededRunInfo);
        }
    }
}
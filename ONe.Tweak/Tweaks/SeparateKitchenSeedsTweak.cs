using System;
using HarmonyLib;
using Kitchen.ONe.Tweak.Config.Sections;
using Kitchen.ONe.Tweak.Utils;
using Unity.Entities;

namespace Kitchen.ONe.Tweak.Tweaks;

public static class SeparateKitchenSeedsTweak
{
    private static Seed _originalSeed;
    
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

    [UpdateBefore(typeof(SetSeededRunOverride))]
    private class SameSeedFixSystem : TweakRestaurantSystem<SameSeedFixSystem>
    {
        private EntityQuery _seedFixers;
        private Seed? _previousSeed;

        protected override void Initialise()
        {
            _seedFixers = GetEntityQuery((ComponentType) typeof (CSeededRunInfo));
        }

        protected override void OnUpdate()
        {
            if (!SeparateSeedsConfig.Instance.Enabled.Value)
            {
                _previousSeed = null;
                return;
            }
            
            if (_seedFixers.IsEmpty)
            {
                return;
            }

            var seedFixer = _seedFixers.First();
            var cSeededRunInfo = EntityManager.GetComponentData<CSeededRunInfo>(seedFixer);
            var fixedSeed = cSeededRunInfo.FixedSeed;

            if (cSeededRunInfo.IsSeedOverride && _previousSeed != fixedSeed && _previousSeed != null)
            {
                _originalSeed = fixedSeed;
                cSeededRunInfo.FixedSeed = Kitchen.Seed.Generate(new Random().Next());
                EntityManager.SetComponentData(seedFixer, cSeededRunInfo);
                
                Console.WriteLine($"Detected new seed: {fixedSeed.StrValue}, replaced it with {cSeededRunInfo.FixedSeed.StrValue}");

                _previousSeed = cSeededRunInfo.FixedSeed;
            }
            else
            {
               _previousSeed = fixedSeed; 
            }
        }
    }
}
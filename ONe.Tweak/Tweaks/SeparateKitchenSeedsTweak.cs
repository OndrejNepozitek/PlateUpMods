using System;
using HarmonyLib;
using Kitchen.ONe.Tweak.Config.Sections;
using Kitchen.ONe.Tweak.Utils;
using Unity.Entities;

namespace Kitchen.ONe.Tweak.Tweaks;

/// <summary>
/// This tweak makes it possible to have a different seed for layout and different seed for the rest.
/// 
/// One challenge to overcome was that the game will not generate a layout again if the seed is the same as the previous seed.
/// Problematic sequence of actions:
/// - Have mod disabled
/// - Enter seed blargh
/// - Enable plugin
/// - Enter seed blargh again, expect different separate seed
/// - But the layout does not get generated again because the seed was internally the same as previously.
///
/// Solution:
/// After a seed is changed, replace it with a random seed but remember the requested seed.
/// When a layout should be generated, change the random seed with the requested seed.
/// </summary>
public static class SeparateKitchenSeedsTweak
{
    /// <summary>
    /// This variable keeps track of the seed for which a layout should be generated.
    /// </summary>
    private static Seed? _originalSeed;
    
    /// <summary>
    /// This variable keeps track of the separate seed that was generated for a layout.
    /// </summary>
    private static Seed? _customSeed;
    
    /// <summary>
    /// This system replaces the layout seed with a custom seed, either random or predefined.
    /// </summary>
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
            _customSeed = seed;
            entityManager.SetComponentData(seedFixer, seededRunInfo);
        }
    }
    
    /// <summary>
    /// This patch restores the requested layout seed.
    /// </summary>
    [HarmonyPatch(typeof(SetSeededRunOverride), "CreateSeededRun")]
    private static class RestoreOriginalSeedPatch
    {
        public static void Prefix(ref Seed seed)
        {
            if (!SeparateSeedsConfig.Instance.Enabled.Value)
            {
                return;
            }

            if (_originalSeed != null)
            {
                seed = _originalSeed.Value;
                _originalSeed = null;
            }
        }
    }

    /// <summary>
    /// This system tracks seed changed and replaces the seed with a random one if needed. See the tweak challenge at the top.
    /// </summary>
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

            if (cSeededRunInfo.IsSeedOverride && _previousSeed != fixedSeed && _previousSeed != null && fixedSeed != _customSeed)
            {
                _originalSeed = fixedSeed;
                cSeededRunInfo.FixedSeed = Kitchen.Seed.Generate(new Random().Next());
                EntityManager.SetComponentData(seedFixer, cSeededRunInfo);
                _previousSeed = cSeededRunInfo.FixedSeed;
            }
            else
            {
               _previousSeed = fixedSeed; 
            }

            _customSeed = null;
        }
    }
}
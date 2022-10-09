using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Kitchen.ONe.Tweak.Config.Sections;
using Unity.Collections;
using Unity.Entities;

namespace Kitchen.ONe.Tweak.Tweaks;

/// <summary>
/// This tweak makes it possible to keep your player positions after you exit practice mode,
/// instead of teleporting you to the front door.
/// </summary>
public static class PracticeKeepPositionTweak
{
    /// <summary>
    /// We use this variable to keep track of the positions of player before leaving the practice mode.
    /// </summary>
    private static List<Tuple<CPlayer, CPosition>> _practiceModePositions;

    /// <summary>
    /// This patch is used to detect when the players want to leave the practice mode.
    /// When that happens, compute positions of all players and store them.
    /// </summary>
    [HarmonyPatch(typeof(LeavePracticeMode), "OnUpdate")]
    private static class DetectLeavingPracticePatch
    {
        public static void Prefix(ref EntityQuery ____SingletonEntityQuery_SLeavePracticeView_44, LeavePracticeMode __instance) 
        {
            if (!PreparationPhaseConfig.Instance.RestorePositionsAfterPractice.Value)
            {
                return;
            }
            
            if (____SingletonEntityQuery_SLeavePracticeView_44.IsEmpty)
            {
                return;
            }
            
            if (!____SingletonEntityQuery_SLeavePracticeView_44.GetSingleton<LeavePracticeMode.SLeavePracticeView>().Ready)
            {
                return;
            }

            var players = __instance.EntityManager.CreateEntityQuery((ComponentType) typeof (CPlayer), (ComponentType) typeof (CPosition));
            _practiceModePositions = new List<Tuple<CPlayer, CPosition>>();
            
            using var playerEntities = players.ToEntityArray(Allocator.Temp);
            foreach (var playerEntity in playerEntities)
            {
                var cPlayer = __instance.EntityManager.GetComponentData<CPlayer>(playerEntity);
                var cPosition = __instance.EntityManager.GetComponentData<CPosition>(playerEntity);
                _practiceModePositions.Add(Tuple.Create(cPlayer, cPosition));
            }
        }
    }

    /// <summary>
    /// This patch restores player positions after the game is loaded to the kitchen again.
    /// TODO: What happens if the game fails to load? Is there a better way to do this patch?
    /// </summary>
    [HarmonyPatch(typeof(CreateSceneAutosaveLoad), "OnUpdate")]
    private static class ReloadPositionsPatch
    {
        public static void Postfix()
        {
            if (!PreparationPhaseConfig.Instance.RestorePositionsAfterPractice.Value)
            {
                return;
            }
            
            if (_practiceModePositions == null)
            {
                return;
            }

            var positionsToRestore = _practiceModePositions;
            
            // Set the stored position to null just in case something goes wrong to prevent leftovers
            _practiceModePositions = null;

            // TODO: Maybe try getting the entity manager from a different source?
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var players = entityManager.CreateEntityQuery((ComponentType) typeof (CPlayer), (ComponentType) typeof (CPosition));

            using var playerEntities = players.ToEntityArray(Allocator.Temp);
            foreach (var playerEntity in playerEntities)
            {
                var cPlayer = entityManager.GetComponentData<CPlayer>(playerEntity);
                var positionToRestore = positionsToRestore.FirstOrDefault(x => x.Item1.ID == cPlayer.ID);
                if (positionToRestore == null) continue;
                entityManager.SetComponentData(playerEntity, positionToRestore.Item2);
                entityManager.RemoveComponent<CUnplacedPlayer>(playerEntities);
            }
        }
    }
}
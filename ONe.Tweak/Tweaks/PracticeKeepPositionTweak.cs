using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Kitchen.ONe.Tweak.Config.Sections;
using Unity.Collections;
using Unity.Entities;

namespace Kitchen.ONe.Tweak.Tweaks;

public static class PracticeKeepPositionTweak
{
    private static List<Tuple<CPlayer, CPosition>> _oldPositions;

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
            _oldPositions = new List<Tuple<CPlayer, CPosition>>();
            
            using var playerEntities = players.ToEntityArray(Allocator.Temp);
            foreach (var playerEntity in playerEntities)
            {
                var cPlayer = __instance.EntityManager.GetComponentData<CPlayer>(playerEntity);
                var cPosition = __instance.EntityManager.GetComponentData<CPosition>(playerEntity);
                _oldPositions.Add(Tuple.Create(cPlayer, cPosition));
            }
        }
    }

    [HarmonyPatch(typeof(CreateSceneAutosaveLoad), "OnUpdate")]
    private static class ReloadPositionsPatch
    {
        public static void Postfix()
        {
            if (!PreparationPhaseConfig.Instance.RestorePositionsAfterPractice.Value)
            {
                return;
            }
            
            if (_oldPositions == null)
            {
                return;
            }

            var oldPositionsCopy = _oldPositions;
            _oldPositions = null;

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var players = entityManager.CreateEntityQuery((ComponentType) typeof (CPlayer), (ComponentType) typeof (CPosition));

            using var playerEntities = players.ToEntityArray(Allocator.Temp);
            foreach (var playerEntity in playerEntities)
            {
                var cPlayer = entityManager.GetComponentData<CPlayer>(playerEntity);
                var oldPosition = oldPositionsCopy.FirstOrDefault(x => x.Item1.ID == cPlayer.ID);
                if (oldPosition != null)
                {
                    entityManager.SetComponentData(playerEntity, oldPosition.Item2);
                    entityManager.RemoveComponent<CUnplacedPlayer>(playerEntities);
                }
            }
        }
    }
}
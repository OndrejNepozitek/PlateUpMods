using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Unity.Collections;
using Unity.Entities;

namespace Kitchen.ONe.Tweak.Tweaks;

public class PracticeKeepPositionTweak
{
    private static List<Tuple<CPlayer, CPosition>> oldPositions;

    [HarmonyPatch(typeof(LeavePracticeMode), "OnUpdate")]
    public class DetectLeavingPracticePatch
    {
        public static void Prefix(ref EntityQuery ____SingletonEntityQuery_SLeavePracticeView_44, LeavePracticeMode __instance) 
        {
            if (____SingletonEntityQuery_SLeavePracticeView_44.IsEmpty)
            {
                return;
            }
            
            if (!____SingletonEntityQuery_SLeavePracticeView_44.GetSingleton<LeavePracticeMode.SLeavePracticeView>().Ready)
            {
                return;
            }
            
            Console.WriteLine("Is exiting practice mode");
            
            var players = __instance.EntityManager.CreateEntityQuery((ComponentType) typeof (CPlayer), (ComponentType) typeof (CPosition));
            oldPositions = new List<Tuple<CPlayer, CPosition>>();
            
            using var playerEntities = players.ToEntityArray(Allocator.Temp);
            foreach (var playerEntity in playerEntities)
            {
                var cPlayer = __instance.EntityManager.GetComponentData<CPlayer>(playerEntity);
                var cPosition = __instance.EntityManager.GetComponentData<CPosition>(playerEntity);
                Console.WriteLine($"{cPlayer.Index}, {cPlayer.ID}, {cPosition.Position}");
                oldPositions.Add(Tuple.Create(cPlayer, cPosition));
            }
        }
    }

    [HarmonyPatch(typeof(CreateSceneAutosaveLoad), "OnUpdate")]
    public class ReloadPositionsPatch
    {
        public static void Postfix()
        {
            if (oldPositions == null)
            {
                return;
            }

            var oldPositionsCopy = oldPositions;
            oldPositions = null;
            
            Console.WriteLine("Restoring original player positions");

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var players = entityManager.CreateEntityQuery((ComponentType) typeof (CPlayer), (ComponentType) typeof (CPosition));

            using var playerEntities = players.ToEntityArray(Allocator.Temp);
            foreach (var playerEntity in playerEntities)
            {
                var cPlayer = entityManager.GetComponentData<CPlayer>(playerEntity);
                var cPosition = entityManager.GetComponentData<CPosition>(playerEntity);
                Console.WriteLine($"{cPlayer.Index}, {cPlayer.ID}, {cPosition.Position}");

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
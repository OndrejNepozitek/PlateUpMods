using System.Diagnostics;
using HarmonyLib;
using KitchenData;
using Unity.Collections;
using Unity.Entities;

namespace Kitchen.ONe.Tweak.Tweaks;

public class SelectMoreCardsTweakOld
{
    // [UpdateInGroup(typeof (EndOfDayProgressionGroup), OrderFirst = true)]
    // [UpdateAfter(typeof(CreateProgressionRequests))]
    // private class SelectMoreCards : StartOfNightSystem
    // {
    //     private EntityQuery _singletonEntityQuerySDay54;
    //     
    //
    //     protected override void Initialise()
    //     {
    //         base.Initialise();
    //         this._singletonEntityQuerySDay54 = this.GetEntityQuery(ComponentType.ReadOnly<SDay>());
    //     }
    //
    //     protected override void OnUpdate()
    //     {
    //         if (this.HasSingleton<SIsRestartedDay>())
    //             return;
    //         int day = this._singletonEntityQuerySDay54.GetSingleton<SDay>().Day;
    //         if (!this.HasSingleton<CreateNewKitchen.SKitchenFirstFrame>() && day % 3 == 0 && day != 15)
    //         {
    //             this.AddRequest(UnlockGroup.Generic);
    //             this.AddRequest(UnlockGroup.Dish);
    //         }
    //         if (day == 15)
    //         {
    //             this.AddRequest(UnlockGroup.FranchiseCard);
    //             this.AddRequest(UnlockGroup.FranchiseCard);
    //         }
    //     }
    //
    //     private void AddRequest(UnlockGroup group) => this.EntityManager.SetComponentData<CProgressionRequest>(this.EntityManager.CreateEntity((ComponentType) typeof (CProgressionRequest)), new CProgressionRequest()
    //     {
    //         Group = group
    //     });
    // }

    // private class CycleCards : NightSystem
    // {
    //     private EntityQuery _progressionOption;
    //     private Stopwatch _stopwatch = new Stopwatch();
    //
    //     protected override void Initialise()
    //     {
    //         base.Initialise();
    //         _progressionOption = GetEntityQuery(new QueryHelper().All((ComponentType) typeof (CProgressionOption)));
    //         _stopwatch.Start();
    //     }
    //
    //     protected override void OnUpdate()
    //     {
    //         var entities = GetEntityQuery(typeof(CUnlockSelectPopup));
    //
    //         if (entities.IsEmpty)
    //         {
    //             return;
    //         }
    //
    //         var popupEntity = entities.First();
    //         var buffer = EntityManager.GetBuffer<CUnlockSelectPopupOption>(popupEntity);
    //
    //         if (_stopwatch.ElapsedMilliseconds > 5000)
    //         {
    //             _stopwatch.Restart();
    //             
    //             var tmp1 = buffer[0];
    //             var tmp2 = buffer[1];
    //             buffer[0] = buffer[2];
    //             buffer[1] = buffer[3];
    //             buffer[2] = tmp1;
    //             buffer[3] = tmp2;
    //         }
    //
    //         // using var progressionOptions = _progressionOption.ToEntityArray(Allocator.Temp);
    //         // var displayedCount = 0;
    //         //
    //         // foreach (var entity in progressionOptions)
    //         // {
    //         //     if (Has<CProgressionOption.Displayed>(entity))
    //         //     {
    //         //         displayedCount++;
    //         //     }
    //         // }
    //         //
    //         // if (displayedCount == progressionOptions.Length)
    //         // {
    //         //     for (var i = 0; i < progressionOptions.Length; i++)
    //         //     {
    //         //         var progressionOption = progressionOptions[i];
    //         //
    //         //         if (i < 2)
    //         //         {
    //         //             continue;
    //         //         }
    //         //
    //         //         EntityManager.RemoveComponent<CProgressionOption.Displayed>(progressionOption);
    //         //     }
    //         //     
    //         //     _stopwatch.Start();
    //         // }
    //         // else
    //         // {
    //         //     if (_stopwatch.ElapsedMilliseconds > 5000)
    //         //     {
    //         //         _stopwatch.Restart();
    //         //
    //         //         foreach (var progressionOption in progressionOptions)
    //         //         {
    //         //             if (Has<CProgressionOption.Displayed>(progressionOption))
    //         //             {
    //         //                 EntityManager.RemoveComponent<CProgressionOption.Displayed>(progressionOption);
    //         //                 EntityManager.RemoveComponent<CProgressionOption>(progressionOption);
    //         //             }
    //         //             else
    //         //             {
    //         //                 EntityManager.AddComponent<CProgressionOption.Displayed>(progressionOption);
    //         //             }
    //         //         }
    //         //     }
    //         // }
    //     }
    // }

    // [HarmonyPatch(typeof(UnlockSelectPopupView), "UpdateData")] 
    // private static class UnlockSelectPopupViewPatch
    // {
    //     private static bool _originalValue = false;
    //     
    //     public static void Prefix(ref bool ___IsUpdated)
    //     {
    //         _originalValue = ___IsUpdated;
    //         ___IsUpdated = false;
    //     }
    //     
    //     // public static void Postfix(ref bool ___IsUpdated)
    //     // {
    //     //     ___IsUpdated = _originalValue;
    //     // }
    // }
}
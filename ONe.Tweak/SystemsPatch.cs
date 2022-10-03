// using System;
// using System.Collections.Generic;
// using HarmonyLib;
// using ONe.Tweak;
// using Unity.Entities;
//
// namespace Kitchen.ONe.Tweak;
//
// [HarmonyPatch(typeof(DefaultWorldInitialization), nameof(DefaultWorldInitialization.GetAllSystems))]
// public class SystemsPatch
// {
//     public static void Postfix(ref IReadOnlyList<Type> __result)
//     {
//         var list = new List<Type>(__result) { typeof(TestSystem) };
//         __result = list.AsReadOnly();
//     }
// }
using HarmonyLib;
using Kitchen;
using UnityEngine;

namespace ONe.KitchenDesigner.Workshop;

[HarmonyPatch(typeof(LayoutView), nameof(LayoutView.Initialise))]
public static class LayoutViewNavMeshPatch
{
    [HarmonyPostfix]
    public static void Postfix(LayoutView __instance)
    {
        if (KitchenDesignerWindow.LargeLayoutSupport)
        {
            var boxCollider = __instance.gameObject.GetComponent<BoxCollider>();
            boxCollider.size = new Vector3(300f, 0.01f, 300f);
        }
    }
}
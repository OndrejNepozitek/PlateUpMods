using HarmonyLib;
using Kitchen;
using Unity.Entities;

namespace ONe.KitchenDesigner.KitchenDesigns;

[HarmonyPatch(typeof(SetSeededRunOverride), "CreateSeededRun")]
public class SetSeededRunOverridePatch_CreateSeededRun
{
    public static bool Prefix(Seed seed, Entity pedestal)
    {
        if (!KitchenDesignLoader.ShouldPatchCreateSeededRun)
        {
            return true;
        }

        KitchenDesignLoader.ShouldPatchCreateSeededRun = false;
        KitchenDesignLoader.LoadKitchenDesign(pedestal);
        
        return false;
    }
}

[HarmonyPatch(typeof(SetSeededRunOverride), "OnUpdate")]
public class SetSeededRunOverridePatch_OnUpdate
{
    public static void Postfix()
    {
        if (KitchenDesignLoader.IsWaitingForSetSeededRunUpdate)
        {
            KitchenDesignLoader.IsWaitingForSetSeededRunUpdate = false;
            KitchenDesignLoader.SetSeededRunUpdated();
        }
    }
}
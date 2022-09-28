using BepInEx.Configuration;
using Kitchen;
using KitchenData;
using ONe.KitchenDesigner.KitchenDesigns;
using Unity.Entities;
using UnityEngine;

namespace ONe.KitchenDesigner.Config;

public static class ConfigHelper
{
    private static ConfigEntry<string> _kitchenDesignConfig;
    private static string _kitchenDesignValue = "";
    private static string _kitchenDesignStatus = "";
    private static KitchenDesign _kitchenDesign;

    public static void SetUp(ConfigFile config)
    {
        _kitchenDesignConfig = config.Bind(
            "Kitchen Designer", "Kitchen Design", "",
            new ConfigDescription("Desc", null,
                new ConfigurationManagerAttributes
                {
                    CustomDrawer = KitchenDesignDrawer,
                    HideSettingName = true,
                    HideDefaultButton = true,
                }));
    }

    static void KitchenDesignDrawer(ConfigEntryBase entry)
    {
        GUILayout.BeginVertical();

        GUILayout.Label("Copy your Exported Kitchen Design Description in the text area below:");
        
        var newDesignValue = GUILayout.TextArea(_kitchenDesignValue, GUILayout.Height(70));
        var hasChanges = newDesignValue != _kitchenDesignValue;
        _kitchenDesignValue = newDesignValue;

        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var hasPedestal = !entityManager.CreateEntityQuery(typeof(SSelectedLayoutPedestal)).IsEmpty;
        var canGenerateLayout = true;

        if (hasChanges)
        {
            KitchenDesignDecoder.TryDecode(_kitchenDesignValue, out _kitchenDesign);
        }
        
        if (!hasPedestal)
        {
            canGenerateLayout = false;
            _kitchenDesignStatus = "You must be at least level 6 and be inside the headquarters.";
        } 
        else if (string.IsNullOrEmpty(_kitchenDesignValue))
        {
            canGenerateLayout = false;
            _kitchenDesignStatus = "No custom design provided.";
        }
        else if (_kitchenDesign == null)
        {
            canGenerateLayout = false;
            _kitchenDesignStatus = "Kitchen Design could not be loaded. Please check that you copied it correctly. Also check the console.";
        }
        else if (hasChanges)
        {
            _kitchenDesignStatus = "Kitchen Design loaded, you can click the button below.";
        }

        GUILayout.Label($"Status: {_kitchenDesignStatus}");

        if (canGenerateLayout)
        {
            if (GUILayout.Button("Generate kitchen layout"))
            {
                var success = KitchenDesignLoader.TryLoadKitchenDesign(_kitchenDesign);

                if (success)
                {
                    _kitchenDesignStatus = "Layout generated, you can close this window and play.";
                }
                else
                {
                    _kitchenDesignStatus = "Layout not generated, please see the console for errors.";
                }
            }
        }
        else
        {
            GUILayout.Button("Cannot generate kitchen layout - see status above");
        }


        GUILayout.EndVertical();
    }
}
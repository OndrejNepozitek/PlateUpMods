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
    private static ConfigEntry<bool> _useRandomSeedConfig;
    private static ConfigEntry<string> _fixedSeed;
    
    private static string _kitchenDesignValue = "";
    private static string _kitchenDesignStatusText = "";
    private static Status _kitchenDesignStatus = Status.Normal;
    private static KitchenDesign _kitchenDesign;

    public static void SetUp(ConfigFile config)
    {
        _kitchenDesignConfig = config.Bind(
            "KitchenDesign", "KitchenDesign", "",
            new ConfigDescription("This value is currently not used", null,
                new ConfigurationManagerAttributes
                {
                    CustomDrawer = KitchenDesignDrawer,
                    HideSettingName = true,
                    HideDefaultButton = true,
                }));

        _useRandomSeedConfig = config.Bind("Seeds", "UseRandomSeed", true, "Whether a random seed should be used for new runs with custom designs");
        _fixedSeed = config.Bind("Seeds", "FixedSeed", "", "The seed that will be used if random seeds are disabled");
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
            _kitchenDesignStatus = Status.Failure;
            _kitchenDesignStatusText = "You must be inside the headquarters (and be at least level 6).";
        } 
        else if (string.IsNullOrEmpty(_kitchenDesignValue))
        {
            canGenerateLayout = false;
            _kitchenDesignStatus = Status.Normal;
            _kitchenDesignStatusText = "No custom design provided.";
        }
        else if (_kitchenDesign == null)
        {
            canGenerateLayout = false;
            _kitchenDesignStatus = Status.Failure;
            _kitchenDesignStatusText = "Kitchen Design could not be loaded - the description is not valid. Please check that you copied it correctly. Also check the console.";
        }
        else if (hasChanges)
        {
            _kitchenDesignStatus = Status.Normal;
            _kitchenDesignStatusText = "Kitchen Design loaded, you can click the button below.";
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label($"Status: ", GUILayout.ExpandWidth(false));

        var style = new GUIStyle(GUI.skin.label);

        if (_kitchenDesignStatus == Status.Failure)
        {
            style.normal.textColor = Color.red;
        } 
        else if (_kitchenDesignStatus == Status.Success)
        {
            style.normal.textColor = Color.green;
        }
        
        GUILayout.Label(_kitchenDesignStatusText, style, GUILayout.ExpandWidth(true));

        GUILayout.EndHorizontal();

        if (canGenerateLayout)
        {
            if (GUILayout.Button("Generate kitchen layout"))
            {
                var seed = _useRandomSeedConfig.Value ? null : _fixedSeed.Value;
                var success = KitchenDesignLoader.TryLoadKitchenDesign(_kitchenDesign, seed);

                if (success)
                {
                    _kitchenDesignStatus = Status.Success;
                    _kitchenDesignStatusText = "Layout generated, you can close this window and play.";
                }
                else
                {
                    _kitchenDesignStatus = Status.Failure;
                    _kitchenDesignStatusText = "Layout not generated, please see the console for errors.";
                }
            }
        }
        else
        {
            GUILayout.Button("Cannot generate kitchen layout - see status above");
        }


        GUILayout.EndVertical();
    }

    private enum Status
    {
        Normal,
        Success,
        Failure,
    }
}
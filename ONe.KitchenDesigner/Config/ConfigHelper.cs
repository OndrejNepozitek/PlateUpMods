using System.Linq;
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
    private static State _kitchenDesignState = State.NoDesignProvided;
    private static KitchenDesign _kitchenDesign;
    
    private static readonly State[] FailureStates = new State[]
    {
        State.GeneratingFailure,
        State.IsOutsideHeadquarters,
        State.SeededRunsNotAvailable,
        State.KitchenDesignCouldNotBeLoaded
    };
    
    private static readonly State[] SuccessStates = new State[]
    {
        State.GeneratingSuccess,
    };

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
        var hasSeededRunInfo = !entityManager.CreateEntityQuery((ComponentType)typeof(CSeededRunInfo)).IsEmpty;

        if (hasChanges)
        {
            KitchenDesignDecoder.TryDecode(_kitchenDesignValue, out _kitchenDesign);
        }
        
        if (!hasPedestal)
        {
            _kitchenDesignState = State.IsOutsideHeadquarters;
            _kitchenDesignStatusText = "You must be inside the headquarters.";
        } 
        else if (!hasSeededRunInfo)
        {
            _kitchenDesignState = State.SeededRunsNotAvailable;
            _kitchenDesignStatusText = "Seeded runs not available (you need to be at least level 5).";
        }
        else if (string.IsNullOrEmpty(_kitchenDesignValue))
        {
            _kitchenDesignState = State.NoDesignProvided;
            _kitchenDesignStatusText = "No custom design provided.";
        }
        else if (_kitchenDesign == null)
        {
            _kitchenDesignState = State.KitchenDesignCouldNotBeLoaded;
            _kitchenDesignStatusText = "Kitchen Design could not be loaded - the description is not valid. Please check that you copied it correctly. Also check the console.";
        }
        else if (hasChanges)
        {
            _kitchenDesignState = State.DesignLoaded;
            _kitchenDesignStatusText = "Kitchen Design loaded, you can click the button below.";
        } 
        else if (_kitchenDesignState == State.GeneratingLayout)
        {
            if (!KitchenDesignLoader.IsGenerating)
            {
                if (KitchenDesignLoader.WasLastGenerationSuccessful)
                {
                    _kitchenDesignState = State.GeneratingSuccess;
                    _kitchenDesignStatusText = "Layout generated, you can close this window and play.";
                }
                else
                {
                    _kitchenDesignState = State.GeneratingFailure;
                    _kitchenDesignStatusText = "Layout not generated, please see the console for errors.";
                }
            }
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label($"Status: ", GUILayout.ExpandWidth(false));

        var style = new GUIStyle(GUI.skin.label);

        if (FailureStates.Contains(_kitchenDesignState))
        {
            style.normal.textColor = Color.red;
        } 
        else if (SuccessStates.Contains(_kitchenDesignState))
        {
            style.normal.textColor = Color.green;
        }
        
        GUILayout.Label(_kitchenDesignStatusText, style, GUILayout.ExpandWidth(true));

        GUILayout.EndHorizontal();

        if (_kitchenDesignState == State.DesignLoaded)
        {
            if (GUILayout.Button("Generate kitchen layout"))
            {
                _kitchenDesignState = State.GeneratingLayout;
                var seed = _useRandomSeedConfig.Value ? null : _fixedSeed.Value;
                KitchenDesignLoader.LoadKitchenDesign(_kitchenDesign, seed);
            }
        }
        else
        {
            GUILayout.Button("Cannot generate kitchen layout - see status above");
        }


        GUILayout.EndVertical();
    }

    private enum State
    {
        IsOutsideHeadquarters,
        SeededRunsNotAvailable,
        NoDesignProvided,
        KitchenDesignCouldNotBeLoaded,
        DesignLoaded,
        GeneratingLayout,
        GeneratingSuccess,
        GeneratingFailure,
    }
}
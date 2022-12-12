using HarmonyLib;
using KitchenMods;
using UnityEngine;

namespace ONe.KitchenDesigner.Workshop
{
    internal class KitchenDesigner : IModInitializer
    {
        public const string Version = "1.0.3";

        private static GameObject GameObject { get; set; }
        
        public static KitchenDesignerGUIManager KitchenDesignerGUIManager { get; private set; }
        
        public void PostActivate()
        {
            
        }

        public void PreInject()
        {
            Init();
        }

        public void PostInject()
        {

        }

        private void Init()
        {
            Debug.Log("KitchenDesigner Init");
            
            GameObject = new GameObject("Kitchen Designer");
            KitchenDesignerGUIManager = GameObject.AddComponent<KitchenDesignerGUIManager>();
            Object.DontDestroyOnLoad(GameObject);
            
            
            var harmony = new Harmony("ONe.KitchenDesigner");
            harmony.PatchAll(GetType().Assembly);
        }
    }
}
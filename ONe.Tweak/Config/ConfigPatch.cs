using System;
using System.Linq;
using System.Reflection;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace Kitchen.ONe.Tweak.Config;

[HarmonyPatch]
public static class ConfigPatch
{
    private static MethodInfo _drawKeyboardShortcutMethod;
    private static MethodInfo _entryGetter;
    
    public static bool Prepare(MethodInfo original)
    {
        if (original == null)
        {
            try
            {
                var assembly = AppDomain
                    .CurrentDomain
                    .GetAssemblies()
                    .FirstOrDefault(x => x.GetName().Name == "ConfigurationManager");

                var settingFieldDrawer = assembly.GetType("ConfigurationManager.SettingFieldDrawer");
                var settingEntryBase = assembly.GetType("ConfigurationManager.SettingEntryBase");   
                var configSettingEntry = assembly.GetType("ConfigurationManager.ConfigSettingEntry");
                
                _drawKeyboardShortcutMethod = AccessTools.Method(settingFieldDrawer, "DrawKeyboardShortcut", new []{ settingEntryBase });
                _entryGetter = configSettingEntry.GetProperty("Entry").GetMethod;
                
                return _drawKeyboardShortcutMethod != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        return _drawKeyboardShortcutMethod != null;
    }
    
    public static MethodInfo TargetMethod()
    {
        return _drawKeyboardShortcutMethod;
    }

    public static void Prefix(ref object setting)
    {
        if (_entryGetter == null)
        {
            return;
        }

        if (_entryGetter.Invoke(setting, Array.Empty<object>()) is not ConfigEntry<KeyboardShortcut> entry)
        {
            return;
        }

        var action = ConfigHelper.GetAction(entry);

        if (action == null)
        {
            return;
        }
        
        if (GUILayout.Button("Run", GUILayout.Width(50)))
        {
            action();
        }
    }
}
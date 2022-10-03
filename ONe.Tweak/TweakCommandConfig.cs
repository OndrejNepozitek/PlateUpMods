using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace ONe.Tweak;

public abstract class TweakCommandConfig
{
    public abstract string Section { get; }
    
    public abstract string Name { get; }
    
    public abstract string ButtonText { get; }
    
    public abstract ConfigEntry<KeyboardShortcut> KeyboardConfig { get; protected set; }
    
    public ConfigFile Config { get; internal set; }

    public abstract void Init();

    protected abstract void Run();

    public virtual void Update()
    {
        if (KeyboardConfig.Value.IsDown())
        {
            Run();
        }
    }
    
    protected virtual void ShowCommandGUI(ConfigEntryBase obj)
    {
        if (GUILayout.Button(ButtonText))
        {
            Run();
        }
    }

    protected ConfigEntry<T> Bind<T>(string name, T defaultValue, ConfigDescription configDescription = null)
    {
        return Config.Bind(Section, $"{Name} - {name}", defaultValue, configDescription);
    }

    protected ConfigEntry<KeyboardShortcut> BindMainButton()
    {
        var buttonConfig = Bind("Button", KeyboardShortcut.Empty, new ConfigDescription("This value is currently not used", null,
            new ConfigurationManagerAttributes
            {
                CustomDrawer = ShowCommandGUI,
                HideSettingName = true,
                HideDefaultButton = true,
            }));
        
        return Bind("Keyboard shortcut", KeyboardShortcut.Empty);
    }
}
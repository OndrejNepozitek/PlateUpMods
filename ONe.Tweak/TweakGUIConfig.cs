using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using UnityEngine;

namespace ONe.Tweak;

public abstract class TweakGUIConfig
{
    public abstract string Section { get; }
    
    public abstract string Name { get; }

    public ConfigFile Config { get; internal set; }

    public abstract void Init();

    private readonly List<ButtonConfig> _buttons = new List<ButtonConfig>();

    public virtual void Update()
    {
        foreach (var button in _buttons)
        {
            if (button.Config.Value.IsDown())
            {
                button.Action();
            }
        }
    }

    public Action GetAction(ConfigEntry<KeyboardShortcut> entry)
    {
        foreach (var button in _buttons)
        {
            if (button.Config == entry)
            {
                return button.Action;
            }
        }

        return null;
    }
    
    protected virtual void ShowCommandGUI(ConfigEntryBase obj, ButtonConfig button)
    {
        // if (GUILayout.Button(button.Text))
        // {
        //     button.Action();
        // }

        var domain = AppDomain.CurrentDomain.GetAssemblies().ToList();
    }

    protected ConfigEntry<T> Bind<T>(string name, T defaultValue, ConfigDescription configDescription = null)
    {
        if (!string.IsNullOrEmpty(Name))
        {
            name = $"{Name} - {name}";
        }
        
        return Config.Bind(Section, name, defaultValue, configDescription);
    }

    protected ConfigEntry<KeyboardShortcut> BindButton(string text, Action action)
    {
        var shortcutConfig = Bind("Keyboard shortcut", KeyboardShortcut.Empty);
        var buttonConfig = new ButtonConfig(shortcutConfig, action, text);
        _buttons.Add(buttonConfig);
        
        Bind("Button", KeyboardShortcut.Empty, new ConfigDescription("This value is currently not used", null,
            new ConfigurationManagerAttributes
            {
                CustomDrawer = (config) => ShowCommandGUI(config, buttonConfig),
                HideSettingName = true,
                HideDefaultButton = true,
        }));

        return shortcutConfig;
    }

    protected class ButtonConfig
    {
        public ConfigEntry<KeyboardShortcut> Config { get; }
        
        public Action Action { get; }
        
        public string Text { get; }

        public ButtonConfig(ConfigEntry<KeyboardShortcut> config, Action action, string text)
        {
            Config = config;
            Action = action;
            Text = text;
        }
    }
}
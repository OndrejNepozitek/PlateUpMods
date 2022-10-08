using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;

namespace Kitchen.ONe.Tweak.Config;

public abstract class ConfigSection
{
    public abstract string SectionName { get; }

    public IReadOnlyList<ConfigEntryBase> ConfigEntries => _configEntries.AsReadOnly();

    private readonly List<ConfigEntryBase> _configEntries = new();
    private readonly ConfigFile _config;
    private readonly Dictionary<ConfigEntry<KeyboardShortcut>, Action> actions = new();

    protected ConfigSection(ConfigFile config)
    {
        _config = config;
    }

    public void Update()
    {
        foreach (var pair in actions)
        {
            var config = pair.Key;
            var action = pair.Value;
            
            if (config.Value.IsDown())
            {
                action();
            }
        }
    }

    public Action GetAction(ConfigEntry<KeyboardShortcut> config)
    {
        if (actions.TryGetValue(config, out var action))
        {
            return action;
        }

        return null;
    }

    protected ConfigEntry<T> Bind<T>(string name, T defaultValue, ConfigDescription configDescription)
    {
        configDescription ??= new ConfigDescription(null, null, new ConfigurationManagerAttributes());
        var attributes = GetOrCreateAttributes(ref configDescription);
        attributes.Order = -_configEntries.Count;
        
        var configEntry = _config.Bind(SectionName, name, defaultValue, configDescription);
        _configEntries.Add(configEntry);

        return configEntry; 
    }

    protected ConfigEntry<T> Bind<T>(string name, T defaultValue, string description)
    {
        return Bind(name, defaultValue, new ConfigDescription(description));
    }

    protected ConfigEntry<KeyboardShortcut> BindCommand(string name, KeyboardShortcut defaultValue, Action action, ConfigDescription configDescription)
    {
        var attributes = GetOrCreateAttributes(ref configDescription);
        attributes.DispName = name;
        
        var configEntry = Bind(name + "KeyboardShortcut", defaultValue, configDescription);
        actions[configEntry] = action;

        return configEntry; 
    }
    
    protected ConfigEntry<KeyboardShortcut> BindCommand(string name, KeyboardShortcut defaultValue, Action action, string description)
    {
        return BindCommand(name, defaultValue, action, new ConfigDescription(description));
    }

    private ConfigurationManagerAttributes GetOrCreateAttributes(ref ConfigDescription configDescription)
    {
        if (configDescription.Tags.FirstOrDefault(x => x is ConfigurationManagerAttributes) is not ConfigurationManagerAttributes attrs)
        {
            attrs = new ConfigurationManagerAttributes();
            configDescription = new ConfigDescription(
                configDescription.Description,
                configDescription.AcceptableValues,
                configDescription.Tags.Append(attrs).ToArray()
            );
        }

        return attrs;
    }
}

public abstract class ConfigSection<TConfig> : ConfigSection where TConfig : ConfigSection<TConfig>
{
    public static TConfig Instance { get; private set; }
    
    protected ConfigSection(ConfigFile config) : base(config)
    {
        Instance = (TConfig) this;
    }
}
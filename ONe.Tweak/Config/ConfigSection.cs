using System;
using System.Collections.Generic;
using BepInEx.Configuration;

namespace Kitchen.ONe.Tweak.Config;

public abstract class ConfigSection<TConfig> where TConfig : ConfigSection<TConfig>
{
    public static TConfig Instance { get; private set; }
    
    public abstract string SectionName { get; }

    public IReadOnlyList<ConfigEntryBase> ConfigEntries => _configEntries.AsReadOnly();

    private readonly List<ConfigEntryBase> _configEntries = new List<ConfigEntryBase>();
    private readonly ConfigFile _config;
    private readonly Dictionary<string, Action> actions = new Dictionary<string, Action>();

    protected ConfigSection(ConfigFile config)
    {
        _config = config;
        Instance = (TConfig) this;
    }
    
    protected ConfigEntry<T> Bind<T>(string name, T defaultValue, ConfigDescription configDescription = null)
    {
        var configEntry = _config.Bind(SectionName, name, defaultValue, configDescription);
        _configEntries.Add(configEntry);

        return configEntry;
    }
    
    protected ConfigEntry<KeyboardShortcut> BindCommand(string name, KeyboardShortcut defaultValue, Action action, ConfigDescription configDescription = null)
    {
        var configEntry = Bind(name, defaultValue, configDescription);
        actions[name] = action;

        return configEntry;
    }
}
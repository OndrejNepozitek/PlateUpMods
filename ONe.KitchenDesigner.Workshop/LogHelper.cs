using UnityEngine;

namespace ONe.KitchenDesigner.Workshop;

public static class LogHelper
{
    private const string Prefix = "[Kitchen Designer] ";

    public static void LogError(string text)
    {
        Debug.LogError($"{Prefix}{text}");
    }
    
    public static void LogWarning(string text)
    {
        Debug.LogWarning($"{Prefix}{text}");
    }
    
    public static void Log(string text)
    {
        Debug.Log($"{Prefix}{text}");
    }
}
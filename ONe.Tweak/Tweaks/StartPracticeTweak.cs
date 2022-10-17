using HarmonyLib;

namespace Kitchen.ONe.Tweak.Tweaks;

/// <summary>
/// This tweak makes it possible to start the practice mode from anywhere.
/// </summary>
public static class StartPracticeTweak
{
    private static bool _shouldRun;

    public static void Run()
    {
        _shouldRun = true;
    }
    
    [HarmonyPatch(typeof(StartPracticeMode), "AfterRun")]
    private class StartPracticeModePatch
    {
        public static bool Prefix(ref bool ___ShouldPrompt)
        {
            if (_shouldRun)
            {
                _shouldRun = false;
                ___ShouldPrompt = true;
            }

            return true;
        }
    }
}
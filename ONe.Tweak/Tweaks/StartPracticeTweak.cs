using HarmonyLib;
using ONe.Tweak;

namespace Kitchen.ONe.Tweak.Tweaks;

public static class StartPracticeTweak
{
    private static bool _shouldRun;

    public static void Run()
    {
        _shouldRun = true;
    }
    
    [HarmonyPatch(typeof(StartPracticeMode), "AfterRun")]
    public class StartPracticeModePatch
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
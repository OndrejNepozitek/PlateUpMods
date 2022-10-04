using HarmonyLib;
using ONe.Tweak;

namespace Kitchen.ONe.Tweak.Tweaks;

public class StartPracticeTweak
{
    public static bool ShouldRun;
    
    [HarmonyPatch(typeof(StartPracticeMode), "AfterRun")]
    public class StartPracticeModePatch
    {
        public static bool Prefix(ref bool ___ShouldPrompt)
        {
            if (ShouldRun)
            {
                ShouldRun = false;
                ___ShouldPrompt = true;
            }

            return true;
        }
    }
}

public class StartPracticeGUIConfig : TweakGUIConfig
{
    public override string Section => "Creative";
    
    public override string Name => "Start practice";

    public override void Init()
    {
        BindButton("Start practice", Run);
    }

    private void Run()
    {
        StartPracticeTweak.ShouldRun = true;
    }
}
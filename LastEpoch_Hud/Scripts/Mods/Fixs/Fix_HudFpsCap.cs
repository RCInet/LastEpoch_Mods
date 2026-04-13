using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Fixs
{
    public class Fix_HudFpsCap
    {
        static int savedTargetFps = int.MinValue;
        const int FallbackTargetFps = 30;

        public static void SetHudCapActive(bool active)
        {
            if (active)
            {
                if (savedTargetFps == int.MinValue)
                {
                    savedTargetFps = Application.targetFrameRate;
                    Application.targetFrameRate = FallbackTargetFps;
                }
            }
            else if (savedTargetFps != int.MinValue)
            {
                Application.targetFrameRate = savedTargetFps;
                savedTargetFps = int.MinValue;
            }
        }
    }
}

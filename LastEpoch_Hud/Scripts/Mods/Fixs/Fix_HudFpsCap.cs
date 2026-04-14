using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Fixs
{
    public class Fix_HudFpsCap
    {
        public static bool Enabled = false;

        static int savedTargetFps = int.MinValue;
        const int FallbackTargetFps = 30;

        public static void SetHudCapActive(bool active)
        {
            if (!Enabled)
            {
                if (savedTargetFps != int.MinValue)
                {
                    Application.targetFrameRate = savedTargetFps;
                    savedTargetFps = int.MinValue;
                }
                return;
            }
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

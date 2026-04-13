using HarmonyLib;
using Il2CppGraphicsBackend;
using Il2CppLE.Settings;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Fixs
{
    public class Fix_HudFpsCap
    {
        static GraphicsSettingsProcessor processor;
        static int savedTargetFps = int.MinValue;
        const int FallbackTargetFps = 30;

        public static void SetHudCapActive(bool active)
        {
            if (!processor.IsNullOrDestroyed())
            {
                try
                {
                    processor.TryApplyMenuFPSLimit(active);
                    return;
                }
                catch { }
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

        [HarmonyPatch(
            typeof(GraphicsSettingsProcessor),
            MethodType.Constructor,
            new System.Type[] { typeof(GraphicsSettings) }
        )]
        public class GraphicsSettingsProcessor_ctor
        {
            [HarmonyPostfix]
            static void Postfix(GraphicsSettingsProcessor __instance)
            {
                processor = __instance;
            }
        }
    }
}

using HarmonyLib;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Login
{
    public class Login_AutoLoginOffline
    {
        public static bool CanRun()
        {
            if (!Save_Manager.instance.IsNullOrDestroyed())
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Login.Enable_AutoLoginOffline;
                }
                else { return false; }
            }
            else { return false; }
        }
        public static void Hide_Online(ref LE.UI.Login.UnityUI.LandingZonePanel __instance)
        {
            if (!__instance.playOnlineButton.IsNullOrDestroyed()) { __instance.playOnlineButton.gameObject.SetActive(false); }
            if (!__instance.manageAccountButton.IsNullOrDestroyed()) { __instance.manageAccountButton.gameObject.SetActive(false); }
        }
        public static void AutoClickOffline(ref LE.UI.Login.UnityUI.LandingZonePanel __instance)
        {
            __instance.OnPlayOfflineClicked();
        }
        [HarmonyPatch(typeof(LE.UI.Login.UnityUI.LandingZonePanel), "OnOnEnable")]
        public class LandingZonePanel_OnOnEnable
        {
            [HarmonyPostfix]
            static void Postfix(ref LE.UI.Login.UnityUI.LandingZonePanel __instance)
            {
                if (CanRun())
                {
                    Hide_Online(ref __instance);
                    AutoClickOffline(ref __instance);
                }
            }
        }
    }
}

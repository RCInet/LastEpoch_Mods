using HarmonyLib;

namespace LastEpochMods.Mods
{
    public class Login
    {
        public class Functions
        {
            public static void Hide_Online(LE.UI.Login.UnityUI.LandingZonePanel __instance)
            {
                __instance.playOnlineButton.gameObject.SetActive(false);
                __instance.manageAccountButton.gameObject.SetActive(false);
            }
            public static void AutoClickOffline(LE.UI.Login.UnityUI.LandingZonePanel __instance)
            {
                __instance.OnPlayOfflineClicked();
            }
        }
        public class Hooks
        {
            [HarmonyPatch(typeof(LE.UI.Login.UnityUI.LandingZonePanel), "OnOnEnable")]
            public class LandingZonePanel_OnOnEnable
            {
                [HarmonyPostfix]
                static void Postfix(ref LE.UI.Login.UnityUI.LandingZonePanel __instance)
                {
                    Functions.Hide_Online(__instance);
                    Functions.AutoClickOffline(__instance);
                }
            }
        }
    }
}

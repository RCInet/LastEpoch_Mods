using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Login
{
    public class Login_AutoLoginOffline
    {
        public static bool CanRun()
        {
            bool r = false;
            if (!Save_Manager.instance.IsNullOrDestroyed())
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    r = Save_Manager.instance.data.Login.Enable_AutoLoginOffline;
                }
            }

            return r;
        }

        [HarmonyPatch(typeof(Il2CppLE.UI.Login.UnityUI.LandingZonePanel), "OnOnEnable")]
        public class LandingZonePanel_OnOnEnable
        {
            [HarmonyPostfix]
            static void Postfix(ref Il2CppLE.UI.Login.UnityUI.LandingZonePanel __instance)
            {
                if (CanRun())
                    __instance.OnPlayOfflineClicked();
            }
        }

        [HarmonyPatch(typeof(Il2CppLE.UI.Login.UnityUI.LandingZonePanel), "OnPlayOnlineClicked")]
        public class LandingZonePanel_BlockOnline
        {
            [HarmonyPrefix]
            static bool Prefix() => !CanRun();
        }

        [HarmonyPatch(typeof(Il2Cpp.CharacterSelect), "SwitchOnlineOffline")]
        public class CharacterSelect_BlockSwitch
        {
            [HarmonyPrefix]
            static bool Prefix() => !CanRun();
        }
    }
}

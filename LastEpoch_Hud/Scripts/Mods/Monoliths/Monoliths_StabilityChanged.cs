using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Monoliths
{
    public class Monoliths_StabilityChanged
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Scenes.Monoliths.Enable_MaxStabilityOnStabilityChanged;
                }
                else { return false; }
            }
            else { return false; }
        }

        [HarmonyPatch(typeof(MonolithZoneManager), "OnBonusStabilityChanged")]
        public class MonolithZoneManager_OnBonusStabilityChanged
        {
            [HarmonyPrefix]
            static void Prefix(ref MonolithZoneManager __instance)
            {
                if (CanRun())
                {
                    __instance.bonusStablity = __instance.maxBonusStablity;
                }
            }
        }
    }
}

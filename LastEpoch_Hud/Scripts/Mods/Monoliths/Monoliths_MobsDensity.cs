using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Monoliths
{
    public class Monoliths_MobsDensity
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Scenes.Monoliths.Enable_MobsDensity;
                }
                else { return false; }
            }
            else { return false; }
        }

        [HarmonyPatch(typeof(MonolithRun), "calculateIncreasedRarityAndExperienceFromMods")]
        public class MonolithRun_calculateIncreasedRarityAndExperienceFromMods
        {
            [HarmonyPostfix]
            static void Postfix(ref MonolithRun __instance)
            {
                if (CanRun())
                {
                    __instance.timeline.enemyDensityModifier = Save_Manager.instance.data.Scenes.Monoliths.MobsDensity;
                }
            }
        }
    }
}

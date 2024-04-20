using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Character
{
    public class Character_Favor_Experience_Multiplier
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Character.Cheats.Enable_FavorMultiplier;
                }
                else { return false; }
            }
            else { return false; }
        }
        [HarmonyPatch(typeof(ExperienceTracker), "GainExp")]
        public class ExperienceTracker_GainExp
        {
            [HarmonyPrefix]
            static void Prefix(ref long __2)
            {
                if (CanRun())
                {
                    __2 *= (long)Save_Manager.instance.data.Character.Cheats.FavorMultiplier;
                }
            }
        }
    }
}

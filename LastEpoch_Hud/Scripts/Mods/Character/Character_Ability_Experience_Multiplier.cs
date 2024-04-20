using HarmonyLib;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Character
{
    public class Character_Ability_Experience_Multiplier
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Character.Cheats.Enable_AbilityMultiplier;
                }
                else { return false; }
            }
            else { return false; }
        }
        
        [HarmonyPatch(typeof(ExperienceTracker), "GainExp")]
        public class ExperienceTracker_GainExp
        {
            [HarmonyPrefix]
            static void Prefix(ref long __1)
            {
                if (CanRun())
                {
                    __1 *= (long)Save_Manager.instance.data.Character.Cheats.AbilityMultiplier;
                }
            }
        }
    }
}

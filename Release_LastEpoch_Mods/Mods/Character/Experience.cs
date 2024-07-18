using HarmonyLib;
using LastEpochMods.Managers;

namespace LastEpochMods.Mods.Character
{
    public class Experience
    {
        [HarmonyPatch(typeof(ExperienceTracker), "GainExp")]
        public class ExperienceTracker_GainExp
        {
            [HarmonyPrefix]
            static void Prefix(ExperienceTracker __instance, ref long __0, ref long __1, ref long __2)
            {
                if ((Save_Manager.Data.UserData.Character.Experience.Enable_ExperienceMultiplier) &&
                    (Save_Manager.Data.UserData.Character.Experience.ExperienceMultiplier > 0))
                {
                    __0 *= Save_Manager.Data.UserData.Character.Experience.ExperienceMultiplier;
                }
                if (Save_Manager.Data.UserData.Character.Experience.Enable_AbilityExpMultiplier &&
                    (Save_Manager.Data.UserData.Character.Experience.AbilityExpMultiplier > 0))
                {
                    __1 *= Save_Manager.Data.UserData.Character.Experience.AbilityExpMultiplier;
                }
                if (Save_Manager.Data.UserData.Character.Experience.Enable_FavorExpMultiplier &&
                    (Save_Manager.Data.UserData.Character.Experience.FavorExpMultiplier > 0))
                {
                    __2 *= Save_Manager.Data.UserData.Character.Experience.FavorExpMultiplier;
                }
            }
        }

        /*[HarmonyPatch(typeof(ExperienceTracker), "GainExpDirect")]
        public class ExperienceTracker_GainExpDirect
        {
            [HarmonyPrefix]
            static void Prefix(ExperienceTracker __instance, ref long __0)
            {
                //0 'expAmount'
                Main.logger_instance.Msg("ExperienceTracker GainExpDirect : Value = " + __0);
                if (Save_Manager.Data.UserData.Scene.Scene_Options.Enable_Scenes_Exp_Multiplier)
                {
                    System.Single result = __0 * Save_Manager.Data.UserData.Scene.Scene_Options.Scenes_Exp_Multiplier;
                    __0 *= (long)result;
                    Main.logger_instance.Msg("ExperienceTracker GainExpDirect : New Value = " + __0);
                }
            }
        }

        [HarmonyPatch(typeof(ExperienceTracker), "GainExpFromEnemy")]
        public class ExperienceTracker_GainExpFromEnemy
        {
            [HarmonyPrefix]
            static void Prefix(ExperienceTracker __instance, ref long __0)
            {
                //0 'expAmount'
                Main.logger_instance.Msg("ExperienceTracker GainExpFromEnemy : Value = " + __0);
                if (Save_Manager.Data.UserData.Scene.Scene_Options.Enable_Scenes_Exp_Multiplier)
                {
                    System.Single result = __0 * Save_Manager.Data.UserData.Scene.Scene_Options.Scenes_Exp_Multiplier;
                    __0 *= (long)result;
                    Main.logger_instance.Msg("ExperienceTracker GainExpFromEnemy : New Value = " + __0);
                }
            }
        }*/
    }
}

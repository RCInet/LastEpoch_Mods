using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Skills
{
    public class Skills_Companions
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return true;
                }
                else { return false; }
            }
            else { return false; }
        }
        [HarmonyPatch(typeof(AbilityStatsMutatorManager), "OnStatsUpdate")]
        public class AbilityStatsMutatorManager_OnStatsUpdate
        {
            [HarmonyPostfix]
            static void Postfix(ref AbilityStatsMutatorManager __instance)
            {
                if (CanRun())
                {
                    if (Save_Manager.instance.data.Skills.Companion.Wolf.Enable_SummonMax) { __instance.canSummonWolvesUpToMaxCompanions = true; }
                    if (Save_Manager.instance.data.Skills.Minions.BoneGolems.Enable_addedGolemsPer4Skeletons)
                    {
                        __instance.addedGolemsPer4Skeletons = Save_Manager.instance.data.Skills.Minions.BoneGolems.addedGolemsPer4Skeletons;
                    }
                    if (Save_Manager.instance.data.Skills.Companion.Wolf.Enable_StunImmunity) { __instance.wolfStunImmunity = true; }
                    else { __instance.wolfStunImmunity = false; }
                }
            }
        }
        [HarmonyPatch(typeof(CharacterStats), "getMaximumCompanions")]
        public class CharacterStats_GetMaximumCompanions
        {
            [HarmonyPostfix]
            static void Postfix(CharacterStats __instance, ref int __result)
            {
                if (CanRun())
                {
                    if (Save_Manager.instance.data.Skills.Companion.Enable_Limit)
                    {
                        __result = Save_Manager.instance.data.Skills.Companion.Limit;
                    }
                }
            }
        }
    }
}

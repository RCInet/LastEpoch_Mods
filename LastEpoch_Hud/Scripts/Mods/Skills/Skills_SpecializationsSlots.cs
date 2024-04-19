using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Skills
{
    public class Skills_SpecializationsSlots
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Skills.Enable_SpecializationSlots;
                }
                else { return false; }
            }
            else { return false; }
        }

        [HarmonyPatch(typeof(SkillsPanelManager), "OnEnable")]
        public class SkillsPanelManager_OnEnable
        {
            [HarmonyPrefix]
            static void Prefix(ref SkillsPanelManager __instance)
            {
                if (CanRun())
                {
                    SpecialisedAbilityManager.getNumberOfSpecialisationSlots((int)PlayerFinder.localPlayerLevel());
                }
            }
        }

        [HarmonyPatch(typeof(SpecialisedAbilityManager), "getNumberOfSpecialisationSlots")]
        public class SpecialisedAbilityManager_getNumberOfSpecialisationSlots
        {
            [HarmonyPrefix]
            static bool Prefix(ref byte __result, int __0)
            {
                if (CanRun())
                {
                    __result = (byte)Save_Manager.instance.data.Skills.SpecializationSlots;
                    return false;
                }
                else { return true; }
            }
        }
    }
}

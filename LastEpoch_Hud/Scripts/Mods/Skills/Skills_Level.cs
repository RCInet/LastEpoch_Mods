using HarmonyLib;
using Il2Cpp;

namespace LastEpoch_Hud.Scripts.Mods.Skills
{
    public class Skills_Level
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()) && (!Refs_Manager.player_treedata.IsNullOrDestroyed()))
            {
                if ((!Save_Manager.instance.data.IsNullOrDestroyed()) && (!Refs_Manager.player_treedata.specialisedSkillTrees.IsNullOrDestroyed()))
                {
                    return Save_Manager.instance.data.Skills.Enable_SkillLevel;
                }
                else { return false; }
            }
            else { return false; }
        }

        [HarmonyPatch(typeof(SkillsPanelManager), "OnOpenSkillTree")]
        //[HarmonyPatch(typeof(SkillsPanelManager), "openSkillTree")]
        public class SkillsPanelManager_openSkillTree
        {
            [HarmonyPrefix]
            static void Prefix(ref SkillsPanelManager __instance, SkillTree __0)
            //static void Prefix(ref SkillsPanelManager __instance, Ability __0)
            {
                try
                {
                    if (!__instance.IsNullOrDestroyed())
                    {
                        if ((CanRun()) && (!__0.IsNullOrDestroyed()))
                        {
                            if (!Refs_Manager.player_treedata.specialisedSkillTrees.IsNullOrDestroyed())
                            {
                                foreach (LocalTreeData.SkillTreeData skill_tree_data in Refs_Manager.player_treedata.specialisedSkillTrees)
                                {
                                    if (!skill_tree_data.ability.IsNullOrDestroyed())
                                    {
                                        if (skill_tree_data.ability.abilityName == __0.ability.abilityName)
                                        //if (skill_tree_data.ability.abilityName == __0.abilityName)
                                        {
                                            skill_tree_data.level = (byte)Save_Manager.instance.data.Skills.SkillLevel;
                                            __instance.updateVisuals(false);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch { Main.logger_instance?.Msg("SkillsPanelManager.openSkillTree() ERROR"); }
            }
        }
    }
}

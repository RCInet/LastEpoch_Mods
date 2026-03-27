using HarmonyLib;
using Il2Cpp;

namespace LastEpoch_Hud.Scripts.Mods.Skills
{
    public class Skills_Level
    {
        public static SkillsPanelManager skills_panel_manager = null;

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
        
        [HarmonyPatch(typeof(SkillsPanelManager), "Awake")]
        public class SkillsPanelManager_Awake
        {
            [HarmonyPostfix]
            static void Postfix(ref SkillsPanelManager __instance)
            {
                skills_panel_manager = __instance;
            }
        }
        
        [HarmonyPatch(typeof(SkillsTreesUIManager), "OpenSkillTree")]
        public class SkillsTreesUIManager_OpenSkillTree
        {
            [HarmonyPrefix]
            static void Prefix(ref SkillsTreesUIManager __instance, Ability __0)
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
                                        if (skill_tree_data.ability.abilityName == __0.abilityName)
                                        {
                                            skill_tree_data.level = (byte)Save_Manager.instance.data.Skills.SkillLevel;
                                            if (!skills_panel_manager.IsNullOrDestroyed())
                                            {
                                                skills_panel_manager.updateVisuals(false);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
            }
        }
    }
}

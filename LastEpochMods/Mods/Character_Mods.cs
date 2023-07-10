using HarmonyLib;
using UMA.AssetBundles;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Character_Mods
    {
        public static void Launch_LevelUp()
        {            
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                if ((obj.name == "MainPlayer(Clone)") && (obj.GetActualType() == typeof(ExperienceTracker)))
                {
                    obj.TryCast<ExperienceTracker>().LevelUp(true);
                    break;
                }
            }
        }
        public static void Launch_ExempleBuffCharacter()
        {
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                if ((obj.name == "MainPlayer(Clone)") && (obj.GetActualType() == typeof(StatBuffs)))
                {
                    float duration = 255;
                    SP propertie = SP.Intelligence;
                    float added_value = 255;
                    float increase_value = 255;
                    Il2CppSystem.Collections.Generic.List<float> more_values = null;
                    AT tag = AT.Buff;
                    byte special_tag = 0;

                    obj.TryCast<StatBuffs>().addBuff(duration, propertie, added_value, increase_value, more_values, tag, special_tag);
                }
            }
        }
        public static void Launch()
        {
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                if (obj.name == "MainPlayer(Clone)")
                {
                    System.Type type = obj.GetActualType();                    
                    if (type == typeof(LocalTreeData))
                    {
                        LocalTreeData tree_data = obj.TryCast<LocalTreeData>();
                        if (Config.Data.mods_config.character.Enable_number_of_unlocked_slots) { tree_data.numberOfUnlockedSlots = Config.Data.mods_config.character.number_of_unlocked_slots; }                        
                        if (Config.Data.mods_config.character.Enable_passiveTree_pointsEarnt) { tree_data.passiveTree.pointsEarnt = Config.Data.mods_config.character.passiveTree_pointsEarnt; }
                        if (Config.Data.mods_config.character.Enable_skilltree_level)
                        {
                            foreach (LocalTreeData.SkillTreeData skill_tree_data in tree_data.specialisedSkillTrees)
                            {
                                skill_tree_data.level = Config.Data.mods_config.character.skilltree_level;
                            }
                        }
                    }                    
                }
            }
        }
    }
}

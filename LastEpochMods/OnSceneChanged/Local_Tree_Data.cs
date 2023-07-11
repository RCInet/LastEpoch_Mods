using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniverseLib;

namespace LastEpochMods.OnSceneChanged
{
    public class Local_Tree_Data
    {
        public static void Init()
        {
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(LocalTreeData)))
            {
                if (obj.name == "MainPlayer(Clone)")
                {
                    LocalTreeData tree_data = obj.TryCast<LocalTreeData>();
                    if (Config.Data.mods_config.character.passivetree.Enable_passiveTree_pointsEarnt)
                    {
                        tree_data.passiveTree.pointsEarnt = Config.Data.mods_config.character.passivetree.passiveTree_pointsEarnt;
                    }
                    if (Config.Data.mods_config.character.skilltree.Enable_skilltree_level)
                    {
                        foreach (LocalTreeData.SkillTreeData skill_tree_data in tree_data.specialisedSkillTrees)
                        {
                            skill_tree_data.level = Config.Data.mods_config.character.skilltree.skilltree_level;
                        }
                    }
                }
            }
        }
    }
}

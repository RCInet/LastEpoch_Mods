using MelonLoader;
using UMA.AssetBundles;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Skills_Mods
    {
        public struct skill_structure
        {
            public string name;
            public string id;
        }
        public struct node_structure
        {
            public string name;
            public int id;
        }
        private static LocalTreeData GetCharacter_TreeData()
        {
            LocalTreeData tree_data = null;
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                System.Type type = obj.GetActualType();
                if (type == typeof(LocalTreeData))
                {
                    if (obj.name == "MainPlayer(Clone)")
                    {
                        tree_data = obj.TryCast<LocalTreeData>();
                        break;
                    }
                }
            }

            return tree_data;
        }
        private static System.Collections.Generic.List<skill_structure> GetAllSkills()
        {
            System.Collections.Generic.List<skill_structure> skills = new System.Collections.Generic.List<skill_structure>();
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                System.Type type = obj.GetActualType();
                if (type == typeof(CharacterClassList))
                {
                    CharacterClassList c_class = obj.TryCast<CharacterClassList>();
                    foreach (CharacterClass c in c_class.classes)
                    {
                        foreach (Ability ability in c.knownAbilities)
                        {
                            skills.Add(new skill_structure { name = ability.abilityName, id = ability.playerAbilityID });
                        }
                        foreach (AbilityAndLevel ability_level in c.unlockableAbilities)
                        {
                            skills.Add(new skill_structure { name = ability_level.ability.abilityName, id = ability_level.ability.playerAbilityID });
                        }
                        foreach (Mastery masterie in c.masteries)
                        {
                            foreach (AbilityAndLevel ability_level in masterie.abilities)
                            {
                                skills.Add(new skill_structure { name = ability_level.ability.abilityName, id = ability_level.ability.playerAbilityID });
                            }
                        }
                    }
                    break;
                }
            }

            return skills;
        }
        private static System.Collections.Generic.List<node_structure> GetAllNode(string skill_name, Main main)
        {
            System.Collections.Generic.List<node_structure> nodes = new System.Collections.Generic.List<node_structure>();
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                if (obj.name.Contains(skill_name))
                {
                    //string name = obj.name;
                    System.Type type = obj.GetActualType();
                    if (type == typeof(SkillTreeNode))
                    {
                        //main.LoggerInstance.Msg("SkillTreeNode : Name = " + obj.name);
                        SkillTreeNode tree_node = obj.TryCast<SkillTreeNode>();
                        //main.LoggerInstance.Msg("NodeName = " + tree_node.name);
                        //main.LoggerInstance.Msg("Id = " + tree_node.id);
                        nodes.Add(new node_structure { name = obj.name, id = tree_node.id });
                    }
                }
            }

            return nodes;
        }

        public static void EditNode(string skill_name, int node_id, int points_allocated, Main main)
        {
            LocalTreeData tree_data = GetCharacter_TreeData();
            if (tree_data != null)
            {
                foreach (LocalTreeData.SkillTreeData specialized in tree_data.specialisedSkillTrees)
                {
                    if (skill_name.Contains(specialized.ability.name))
                    {
                        foreach (LocalTreeData.NodeData node_data in specialized.nodes)
                        {
                            if (node_id == node_data.id)
                            {
                                node_data.pointsAllocated = (byte)points_allocated;
                                main.LoggerInstance.Msg("Skill : " + specialized.ability.name + ", NodeId = " + node_id + ", Points = " + points_allocated);
                                break;
                            }                            
                        }
                        break;
                    }
                }
            }
        }
        public static void Helper_Skills_Nodes(Main main)
        {
            //Get All Skills
            main.LoggerInstance.Msg("Get All Skills");
            System.Collections.Generic.List<skill_structure> skills = GetAllSkills();            
            //Get Character TreeData
            main.LoggerInstance.Msg("Get Character TreeData");
            LocalTreeData tree_data = GetCharacter_TreeData();
            if (tree_data != null)
            {
                foreach (LocalTreeData.TreeData specialized in tree_data.specialisedSkillTrees)
                {
                    main.LoggerInstance.Msg("");                   
                    bool found = false;
                    int i = 0;
                    string skill_name = "";
                    foreach (skill_structure s_struct in skills)
                    {
                        if (s_struct.id == specialized.treeID)
                        {
                            skill_name = s_struct.name;
                            main.LoggerInstance.Msg("Skill : Name = " + s_struct.name + ", Id = " + specialized.treeID);
                            found = true;
                            break;
                        }
                        i++;
                    }
                    if (!found) { main.LoggerInstance.Msg("Skill Name not Found, Id = " + specialized.treeID); }
                    System.Collections.Generic.List<node_structure> nodes = GetAllNode(skill_name, main);
                    foreach (LocalTreeData.NodeData n_data in specialized.nodes)
                    {
                        bool node_found = false;
                        if (found)
                        {
                            foreach (var n in nodes)
                            {
                                if (n_data.id == n.id)
                                {
                                    main.LoggerInstance.Msg("Node : Name = " + n.name + ", Id = " + n.id);
                                    node_found = true;
                                    break;
                                }
                            }
                            if (!node_found) { main.LoggerInstance.Msg("Node Name Not Found, Id = " + n_data.id); }
                        }
                        else { main.LoggerInstance.Msg("Node Id = " + n_data.id); }
                    }
                }
            }
        }
    }
}

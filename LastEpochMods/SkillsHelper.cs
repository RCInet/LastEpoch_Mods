using MelonLoader;
using System.Drawing;
using System.Linq;
using UMA.AssetBundles;
using UniverseLib;

namespace LastEpochMods
{
    public class SkillsHelper
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
            if ((skill_name != "") && (skill_name != "BasicPlayerAttack"))
            {
                string skill_name_format = "";
                string skill_name_format2 = "";
                string skill_name_temp = string.Concat(skill_name.Select(x => System.Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');                
                int index = 0;
                foreach (string s in skill_name_temp.Split(' '))
                {
                    if ((s != " ") && (s != ""))
                    {
                        try { int z = System.Convert.ToInt32(s); } //Remove Numbers
                        catch
                        {
                            string temp = "";
                            if (s == "Smelters") { temp = s + "'s"; }
                            else { temp = s; }

                            if (index == 0) { skill_name_format = s; }
                            else { skill_name_format += " " + s; }
                            index++;
                        }
                    }
                }
                int index2 = 0;
                foreach (string s in skill_name_temp.Split(' '))
                {
                    if ((s != " ") && (s != "") && (s != "Summon") && (s != "Slam") && (s != "Rogue"))
                    {
                        try { int z = System.Convert.ToInt32(s); } //Remove Numbers
                        catch
                        {
                            if (index2 == 0) { skill_name_format2 = s; }
                            else { skill_name_format2 += " " + s; }
                            index2++;
                        }                        
                    }
                }
                //main.LoggerInstance.Msg("Search for Node with name : " + skill_name_format);
                foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
                {
                    System.Type type = obj.GetActualType();
                    if (type == typeof(SkillTreeNode))
                    {
                        bool found = false;
                        int skill_name_count = skill_name_format.Split(' ').Count();
                        if (obj.name.Split(' ').Count() >= skill_name_count)
                        {
                            string obj_name = "";
                            for (int i = 0; i < skill_name_count; i++)
                            {
                                if (i == 0) { obj_name = obj.name.Split(' ')[i]; }
                                else { obj_name += " " + obj.name.Split(' ')[i]; }
                            }
                            if (obj_name.ToUpper() == skill_name_format.ToUpper()) { found = true; }
                            if (found)
                            {
                                SkillTreeNode tree_node = obj.TryCast<SkillTreeNode>();
                                nodes.Add(new node_structure { name = obj.name, id = tree_node.id });
                            }
                        }
                        if (!found)
                        {
                            int skill_name_count2 = skill_name_format2.Split(' ').Count();
                            if (obj.name.Split(' ').Count() >= skill_name_count2)
                            {
                                string obj_name = "";
                                for (int i = 0; i < skill_name_count2; i++)
                                {
                                    if (i == 0) { obj_name = obj.name.Split(' ')[i]; }
                                    else { obj_name += " " + obj.name.Split(' ')[i]; }
                                }
                                if (obj_name.ToUpper() == skill_name_format2.ToUpper()) { found = true; }
                                if (found)
                                {
                                    SkillTreeNode tree_node = obj.TryCast<SkillTreeNode>();                                    
                                    nodes.Add(new node_structure { name = obj.name, id = tree_node.id });
                                }
                            }
                        }
                    }
                }
                if (nodes.Count == 0) { main.LoggerInstance.Msg("No Nodes Found for : Name " + skill_name + " : Format1 = " + skill_name_format + ", Format2 = " + skill_name_format2); }
                else { main.LoggerInstance.Msg(nodes.Count + " Nodes Found for : Name " + skill_name + " : Format1 = " + skill_name_format + ", Format2 = " + skill_name_format2); }
            }

            return nodes;
        }
               
        public static void Helper_Skills_Nodes(Main main)
        {
            //Get All Skills
            //Main.logger_instance
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

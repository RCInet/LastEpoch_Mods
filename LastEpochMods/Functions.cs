using UnityEngine;
using UniverseLib;

namespace LastEpochMods
{
    public class Functions
    {
        public static UnityEngine.Object GetObject(string name)
        {
            UnityEngine.Object objet = new UnityEngine.Object();
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                if ((name != "") && (obj.name.Contains(name)))
                {
                    System.Type type = obj.GetActualType();
                    if (type != typeof(TextAsset))
                    {
                        objet = obj;
                        break;
                    }
                }
            }
            return objet;
        }
        public static Il2CppSystem.Collections.Generic.List<UnityEngine.Object> GetCharacter_Objets()
        {
            Il2CppSystem.Collections.Generic.List<UnityEngine.Object> list = new Il2CppSystem.Collections.Generic.List<Object>();
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                if (obj.name == "MainPlayer(Clone)") { list.Add(obj); }
            }

            return list;
        }
                
        //Skills
        /*public static System.Collections.Generic.List<node_structure> GetAllNode(string ability_name, Main main)
        {
            System.Collections.Generic.List<node_structure> nodes = new System.Collections.Generic.List<node_structure>();
            if (ability_name != "")
            {
                foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
                {
                    System.Type type = obj.GetActualType();
                    if (type == typeof(SkillTree))
                    {
                        main.LoggerInstance.Msg("Get SkillTreeNode");
                        SkillTree tree = obj.TryCast<SkillTree>();

                        main.LoggerInstance.Msg("Get SkillTreeNodes");
                        //SkillTreeNode tree_node = obj.TryCast<SkillTreeNode>();

                        if (ability_name == tree.treeID)
                        {
                            main.LoggerInstance.Msg("Node Found : " + obj.name + ", Id = " + tree_node.id);
                            nodes.Add(new node_structure { name = obj.name, id = tree_node.id });
                        }


                        //main.LoggerInstance.Msg("Get SkillTreeNode.Tree");
                        //SkillTree tree = tree_node.tree.TryCast<SkillTree>();
                        //UnityEngine.GameObject obj_tree = tree_node.tree.gameObject;
                        //main.LoggerInstance.Msg("Get SkillTreeNode.Tree.ability");
                        //Ability ability = tree.ability.TryCast<Ability>();

                        //var obj_ability = obj_tree.GetComponent<Ability>();
                        //main.LoggerInstance.Msg("TryCast Ability");
                        //Ability ability = obj_ability.TryCast<Ability>();
                        //main.LoggerInstance.Msg("Ability Name : " + ability.abilityName);
                        //if (ability_name == ability.abilityName)
                        //{
                        //    nodes.Add(new node_structure { name = obj.name, id = tree_node.id });
                        //}
                    }
                }
                if (nodes.Count == 0) { main.LoggerInstance.Msg("No Nodes Found for : " + ability_name); }
                else { main.LoggerInstance.Msg(nodes.Count + " Nodes Found for : " + ability_name); }
            }

            return nodes;
        }*/

    }
}

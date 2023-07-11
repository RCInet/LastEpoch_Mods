using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Local_Tree_Data
    {
        [HarmonyPatch(typeof(LocalTreeData), "fulfilledRequirementExists")]
        public class fulfilledRequirementExists
        {
            [HarmonyPrefix]
            static bool Prefix(LocalTreeData __instance, bool __result, GlobalTreeData.TreeData __0, ref GlobalTreeData.NodeData __1, LocalTreeData.TreeData __2, LocalTreeData.NodeData __3)
            {
                if (Config.Data.mods_config.character.skilltree.Disable_node_requirement)
                {
                    foreach (GlobalTreeData.Requirement req in __1.requirements)
                    {
                        req.requirement = 0;
                    }
                }

                return true;
            }
        }
    }
}

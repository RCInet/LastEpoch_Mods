using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Skills
{
    internal class Skills_Nodes_Req
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Skills.Disable_NodeRequirement;
                }
                else { return false; }
            }
            else { return false; }
        }
        [HarmonyPatch(typeof(LocalTreeData), "fulfilledRequirementExists")]
        public class LocalTreeData_FulfilledRequirementExists
        {
            [HarmonyPrefix]
            static bool Prefix(LocalTreeData __instance, ref bool __result, GlobalTreeData.TreeData __0, ref GlobalTreeData.NodeData __1, LocalTreeData.TreeData __2, LocalTreeData.NodeData __3)
            {
                if (CanRun())
                {
                    __result = true;
                    return false;
                }
                else { return true; }
            }
        }
    }
}

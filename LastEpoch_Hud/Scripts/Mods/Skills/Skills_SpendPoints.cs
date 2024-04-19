using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Skills
{
    public class Skills_SpendPoints
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return true;
                }
                else { return false; }
            }
            else { return false; }
        }
        [HarmonyPatch(typeof(LocalTreeData), "tryToSpendSkillPoint")]
        public class TryToSpendSkillPoint
        {
            private static bool Added_MaxPoint = false;
            private static byte Backup_MaxPoint = 0;

            [HarmonyPrefix]
            static void Prefix(ref LocalTreeData __instance, bool __result, Ability __0, byte __1)
            {
                if (CanRun())
                {
                    Added_MaxPoint = false;
                    if ((!__result) && (Save_Manager.instance.data.Skills.Disable_NodeRequirement))
                    {
                        foreach (LocalTreeData.SkillTreeData tree in __instance.specialisedSkillTrees)
                        {
                            if (tree.ability == __0)
                            {
                                Added_MaxPoint = true;
                                Backup_MaxPoint = tree.additionalMaxPointsFromStats;
                                tree.additionalMaxPointsFromStats = 255;
                                break;
                            }
                        }
                    }
                }
            }
            [HarmonyPostfix]
            static void Postfix(ref LocalTreeData __instance, bool __result, Ability __0, byte __1)
            {
                if ((CanRun()) && (Added_MaxPoint))
                {
                    foreach (LocalTreeData.SkillTreeData tree in __instance.specialisedSkillTrees)
                    {
                        if (tree.ability == __0)
                        {
                            tree.additionalMaxPointsFromStats = Backup_MaxPoint;
                            Added_MaxPoint = false;
                            break;
                        }
                    }
                }
            }
        }
    }
}

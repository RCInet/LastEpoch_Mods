using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Factions.TheWoven
{
    public class Faction_Woven_TreePoints
    {
        public static bool CanRun()
        {
            return Scenes.IsGameScene() && ModSettings.Weaver.TreePoints.Enabled;
        }

        [HarmonyPatch(typeof(LocalTreeData.WeaverTreeData), "getUnspentPoints")]
        public class WeaverTreeData_getUnspentPoints
        {
            [HarmonyPrefix]
            static void Prefix(ref LocalTreeData.WeaverTreeData __instance)
            {
                if (CanRun())
                {
                    __instance.EarnedWeaverPoints = (ushort)ModSettings.Weaver.TreePoints.Value;
                }
            }
        }
    }
}

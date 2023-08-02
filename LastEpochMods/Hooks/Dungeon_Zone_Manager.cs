using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Dungeon_Zone_Manager
    {
        [HarmonyPatch(typeof(DungeonZoneManager), "initialise")]
        public class initialise
        {
            [HarmonyPostfix]
            static void Postfix(ref DungeonZoneManager __instance)
            {
                if (Config.Data.mods_config.scene.Enable_Dungeons_ObjectiveReveal) { __instance.objectiveRevealThresholdModifier = float.MaxValue; }

            }
        }
    }
}

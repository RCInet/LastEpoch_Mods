using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Monolith_Timeline_Panel_Manager
    {
        [HarmonyPatch(typeof(MonolithTimelinePanelManager), "Update")]
        public class Update
        {
            [HarmonyPrefix]
            static bool Prefix(MonolithTimelinePanelManager __instance)
            {
                if (Config.Data.mods_config.scene.Enable_Monolith_EnemyDensity)
                {
                    __instance.run.timeline.enemyDensityModifier = Config.Data.mods_config.scene.Monolith_EnemyDensity;
                }
                return true;
            }
        }
    }
}

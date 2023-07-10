using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Monolith_Zone_Manager
    {
        [HarmonyPatch(typeof(MonolithZoneManager), "initialise")]
        public class initialise
        {
            [HarmonyPostfix]
            static void Postfix(ref MonolithZoneManager __instance, ref StatefulQuestList __0)
            {
                if (Config.Data.mods_config.scene.Enable_Monolith_Overide_Max_Stability) { __instance.maxBonusStablity = Config.Data.mods_config.scene.Max_Stability; }
                if (Config.Data.mods_config.scene.Enable_Monolith_Stability) { __instance.bonusStablity = __instance.maxBonusStablity; }
                if (Config.Data.mods_config.scene.Enable_Monolith_EnnemiesDefeat_OnStart) { __instance.enemiesDefeated = Config.Data.mods_config.scene.Monolith_EnnemiesDefeat_OnStart; }
                if (Config.Data.mods_config.scene.Enable_Monolith_ObjectiveReveal) { __instance.objectiveRevealThresholdModifier = float.MaxValue; }
                if (Config.Data.mods_config.scene.Enable_Monolith_Complete_Objective) { __instance.CompleteObjective(); }
            }
        }

        [HarmonyPatch(typeof(MonolithZoneManager), "OnBonusStabilityChanged")]
        public class OnBonusStabilityChanged
        {
            [HarmonyPrefix]
            static void Prefix(ref MonolithZoneManager __instance)
            {
                if (Config.Data.mods_config.scene.Enable_Monolith_Stability)
                {
                    __instance.bonusStablity = __instance.maxBonusStablity;
                }
            }
        }

        [HarmonyPatch(typeof(EchoWebIsland), "onDiedInEcho")]
        public class onDiedInEcho
        {
            [HarmonyPrefix]
            public static bool Prefix(ref EchoWebIsland __instance, ref MonolithRun run)
            {
                bool result = true;
                if (Config.Data.mods_config.scene.Enable_Monolith_NoDie) { result = false; }

                return result;
            }
        }
    }
}

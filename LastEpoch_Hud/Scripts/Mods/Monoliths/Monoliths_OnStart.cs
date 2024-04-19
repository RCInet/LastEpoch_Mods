using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Monoliths
{
    public class Monoliths_OnStart
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()) &&
                (!Refs_Manager.player_actor.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    if ((Save_Manager.instance.data.Scenes.Monoliths.Enable_MaxStability) ||
                        (Save_Manager.instance.data.Scenes.Monoliths.Enable_MaxStabilityOnStart) ||
                        (Save_Manager.instance.data.Scenes.Monoliths.Enable_MobsDefeatOnStart) ||
                        (Save_Manager.instance.data.Scenes.Monoliths.Enable_ObjectiveReveal) ||
                        (Save_Manager.instance.data.Scenes.Monoliths.Enable_CompleteObjective))
                    {
                        return true;
                    }
                    else { return false; }
                }
                else { return false; }
            }
            else { return false; }
        }

        [HarmonyPatch(typeof(MonolithZoneManager), "initialise")]
        public class MonolithZoneManager_initialise
        {
            [HarmonyPostfix]
            static void Postfix(ref MonolithZoneManager __instance, StatefulQuestList __0)
            {
                if (CanRun())
                {
                    if (Save_Manager.instance.data.Scenes.Monoliths.Enable_MaxStability) { __instance.maxBonusStablity = (int)Save_Manager.instance.data.Scenes.Monoliths.MaxStability; }
                    if (Save_Manager.instance.data.Scenes.Monoliths.Enable_MaxStabilityOnStart) { __instance.bonusStablity = __instance.maxBonusStablity; }
                    if (Save_Manager.instance.data.Scenes.Monoliths.Enable_MobsDefeatOnStart) { __instance.enemiesDefeated = Save_Manager.instance.data.Scenes.Monoliths.MobsDefeatOnStart; }
                    if (Save_Manager.instance.data.Scenes.Monoliths.Enable_ObjectiveReveal) { __instance.objectiveRevealThresholdModifier = float.MaxValue; }
                    if (Save_Manager.instance.data.Scenes.Monoliths.Enable_CompleteObjective)
                    {
                        foreach (Quest quest in __instance.questsThatCompleteZone) { quest.completeQuest(Refs_Manager.player_actor); }
                        __instance.CompleteObjective();
                    }
                }                
            }
        }
    }
}

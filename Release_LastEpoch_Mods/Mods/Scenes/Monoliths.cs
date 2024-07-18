using HarmonyLib;
using LastEpochMods.Managers;

namespace LastEpochMods.Mods.Scenes
{
    public class Monoliths
    {
        public static void RevealIslands()
        {
            if (!Monolith_Timeline_Panel_Manager.manager.IsNullOrDestroyed())
            {
                Monolith_Timeline_Panel_Manager.manager.revealEchoesAroundIslands();
                Monolith_Timeline_Panel_Manager.manager.createEchoWebVisuals(Monolith_Timeline_Panel_Manager.manager.web, 1);
                Monolith_Timeline_Panel_Manager.manager.forceRebuildLayoutAfterFrame();
                Monolith_Timeline_Panel_Manager.manager.webView.hasChanged = true;
            }
        }        

        public class Monolith_Zone_manager
        {
            //Stability, Max Stability, Objectives, Quests, on Start
            [HarmonyPatch(typeof(MonolithZoneManager), "initialise")]
            public class MonolithZoneManager_initialise
            {
                [HarmonyPostfix]
                static void Postfix(ref MonolithZoneManager __instance, StatefulQuestList __0)
                {
                    if (Save_Manager.Data.UserData.Scene.Monoliths.Enable_MaxStability) { __instance.maxBonusStablity = Save_Manager.Data.UserData.Scene.Monoliths.MaxStability; }
                    if (Save_Manager.Data.UserData.Scene.Monoliths.Enable_MaxStability_OnStart) { __instance.bonusStablity = __instance.maxBonusStablity; }
                    if (Save_Manager.Data.UserData.Scene.Monoliths.Enable_EnnemiesDefeat) { __instance.enemiesDefeated = Save_Manager.Data.UserData.Scene.Monoliths.EnnemiesDefeat; }
                    if (Save_Manager.Data.UserData.Scene.Monoliths.Enable_ObjectiveReveal) { __instance.objectiveRevealThresholdModifier = float.MaxValue; }
                    if (Save_Manager.Data.UserData.Scene.Monoliths.Enable_Complete_Objective)
                    {
                        foreach (Quest quest in __instance.questsThatCompleteZone) { quest.completeQuest(PlayerFinder.getPlayerActor()); }
                        __instance.CompleteObjective();
                    }
                }
            }

            //Max Stability on Stability change
            [HarmonyPatch(typeof(MonolithZoneManager), "OnBonusStabilityChanged")]
            public class MonolithZoneManager_OnBonusStabilityChanged
            {
                [HarmonyPrefix]
                static void Prefix(ref MonolithZoneManager __instance)
                {
                    if (Save_Manager.Data.UserData.Scene.Monoliths.Enable_MaxStability_OnStabilityChanged)
                    {
                        __instance.bonusStablity = __instance.maxBonusStablity;
                    }
                }
            }
        }
        public class Monolith_Run_Manager
        {
            /*private static byte GetTimelineId(TimelineID timeline)
            {
                byte result = 0;
                if (timeline == TimelineID.None) { result = 0; }
                else if (timeline == TimelineID.UndeadAbom) { result = 1; }
                else if (timeline == TimelineID.OsprixWithLance) { result = 2; }
                else if (timeline == TimelineID.VoidRahyeh) { result = 3; }
                else if (timeline == TimelineID.FrostLich) { result = 4; }
                else if (timeline == TimelineID.Lagon) { result = 5; }
                else if (timeline == TimelineID.UndeadVsVoid) { result = 6; }
                else if (timeline == TimelineID.Dragons) { result = 7; }
                else if (timeline == TimelineID.Gaspar) { result = 8; }
                else if (timeline == TimelineID.Heorot) { result = 9; }
                else if (timeline == TimelineID.Volcano) { result = 10; }

                return result;
            }

            [HarmonyPatch(typeof(MonolithRunsManager), "onEchoIslandCompleted")]
            public class MonolithRunsManager_onEchoIslandCompleted
            {
                [HarmonyPostfix]
                static void Postfix(MonolithRunsManager __instance, TimelineID __0, int __1, int __2)
                {
                    byte timeline_id = GetTimelineId(__0);
                    Main.logger_instance.Msg("MonolithRunsManager:onEchoIslandCompleted : Timeline = " + timeline_id + ", Difficulty = " + __1);
                    
                    bool found = false;
                    foreach (LE.Data.SavedMonolithRun sv_run in PlayerFinder.getPlayerData().MonolithRuns)
                    {
                        if ((sv_run.TimelineID == timeline_id) && (sv_run.DifficultyIndex == __1))
                        {
                            found = true;
                            Main.logger_instance.Msg("Found Monolith run in save");
                            sv_run.SavedEchoWeb.Corruption += 9999;
                            sv_run.SavedEchoWeb.GazeOfOrobyss += 9999;
                            //PlayerFinder.getPlayerData().SaveData();
                            break;
                        }
                    }
                    if (!found) { Main.logger_instance.Error("Monolith run NOT found in save"); }
                    
                    //__instance.updateSharedRunData();
                }
            }*/
        }
        public class Monolith_Run
        {
            public static MonolithRun Run = null; //ref for Blessing_Rewards
            //Init ref for Blessing_Rewards and set Density
            [HarmonyPatch(typeof(MonolithRun), "calculateIncreasedRarityAndExperienceFromMods")]
            public class MonolithRun_calculateIncreasedRarityAndExperienceFromMods
            {
                [HarmonyPostfix]
                static void Postfix(ref MonolithRun __instance)
                {
                    Run = __instance;
                    if (Save_Manager.Data.UserData.Scene.Monoliths.Enable_EnemyDensity)
                    {
                        __instance.timeline.enemyDensityModifier = Save_Manager.Data.UserData.Scene.Monoliths.EnemyDensity;
                    }
                    else { __instance.timeline.enemyDensityModifier = 1; }
                }
            }
        }
        public class Monolith_UI_Manager
        {
            //Unlock Blessings Slots
            [HarmonyPatch(typeof(LE.Gameplay.Monolith.MonolithUIManager), "OpenBlessingsRewardPanelAfterDelay")]
            public class MonolithUIManager_OpenBlessingsRewardPanelAfterDelay
            {
                [HarmonyPrefix]
                static void Prefix(Cysharp.Threading.Tasks.UniTaskVoid __result, TimelineID __0, int __1, float __2, ref int __3)
                {
                    if (Save_Manager.Data.UserData.Scene.Monoliths.Enable_UnlockSlots)
                    {
                        __3 = Save_Manager.Data.UserData.Scene.Monoliths.UnlockSlots;
                    }
                }
            }
        }
        public class Echo_Web
        {
            //All Islands can be run
            /*[HarmonyPatch(typeof(EchoWeb), "islandCanBeRun")]
            public class EchoWeb_islandCanBeRun
            {
                [HarmonyPostfix]
                static void Postfix(EchoWeb __instance, ref bool __result, EchoWebIsland __0)
                {
                    //__instance.corruption
                    if (Scenes_Manager.GameScene())
                    {
                        __result = true;
                    }
                }
            }*/
            //On Island Completed
            /*[HarmonyPatch(typeof(EchoWeb), "OnIslandCompleted")]
            public class EchoWeb_OnIslandCompleted
            {
                [HarmonyPrefix]
                static void Prefix(ref EchoWeb __instance, int __0, MonolithRun __1, ref SharedMonolithRunsData __2)
                {
                    //__0 'hexIndex'
                    if (Scenes_Manager.GameScene())
                    {
                        Main.logger_instance.Msg("EchoWeb:OnIslandCompleted");

                        //__2.

                        __instance.corruption += 9999;
                        __instance.gazeOfOrobyss += 9999;
                        //__instance.islands.entries[0].
                    }
                }
            }*/
            //Connections
            /*public static bool ForceConnect = false;
            [HarmonyPatch(typeof(EchoWeb), "attemptToConnectToExistingIsland")]
            public class EchoWeb_attemptToConnectToExistingIsland
            {
                [HarmonyPostfix]
                static void Postfix(EchoWeb __instance, ref bool __result, EchoWebIsland __0)
                {
                    if (Scenes_Manager.GameScene())
                    {
                        if (ForceConnect) { __result = true; }
                    }
                }
            } */           
        }
        public class Echo_Web_Island
        {
            //On Die
            [HarmonyPatch(typeof(EchoWebIsland), "onDiedInEcho")]
            public class EchoWebIsland_onDiedInEcho
            {
                [HarmonyPrefix]
                public static bool Prefix(ref EchoWebIsland __instance, ref MonolithRun run)
                {
                    bool result = true;
                    if (Save_Manager.Data.UserData.Scene.Monoliths.Enable_NoLostWhenDie) { result = false; }

                    return result;
                }
            }
        }
        public class Blessing_Reward_Panel_Manager
        {
            public static System.Collections.Generic.List<byte> GetAllTimelineBlessings(TimelineID timeline_id, int difficulty)
            {
                System.Collections.Generic.List<byte> result = new System.Collections.Generic.List<byte>();
                try
                {
                    if (Monolith_Run.Run != null)
                    {
                        foreach (System.Int32 b in Monolith_Run.Run.timeline.difficulties[difficulty].anySlotBlessings)
                        {
                            result.Add((byte)b);
                        }
                        foreach (System.Int32 b in Monolith_Run.Run.timeline.difficulties[difficulty].otherSlotBlessings)
                        {
                            result.Add((byte)b);
                        }
                    }

                }
                catch { }

                return result;
            }
            //Populate Blessings Slots
            [HarmonyPatch(typeof(BlessingRewardPanelManager), "OnOptionsPopulated")]
            public class BlessingRewardPanelManager_OnOptionsPopulated
            {
                [HarmonyPostfix]
                static void Postfix(ref BlessingRewardPanelManager __instance, TimelineID __0, int __1, int __2)
                {
                    System.Collections.Generic.List<byte> available_blessings = GetAllTimelineBlessings(__0, __1);
                    if (available_blessings.Count > 0)
                    {
                        System.Collections.Generic.List<byte> already_list = new System.Collections.Generic.List<byte>();
                        foreach (BlessingOptionContainerUI b_container in __instance.blessingOptions) // rewardBlessingsOptions)
                        {
                            if (b_container.container.HasContent())
                            {
                                already_list.Add(b_container.container.GetContent()[0].data.id[2]);
                            }
                            else
                            {
                                System.Collections.Generic.List<byte> new_list = new System.Collections.Generic.List<byte>();
                                foreach (byte available in available_blessings)
                                {
                                    bool found = false;
                                    foreach (byte already in already_list)
                                    {
                                        if (available == already)
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (!found) { new_list.Add(available); }
                                }
                                try
                                {
                                    byte new_blessing_id = new_list[UnityEngine.Random.Range(0, new_list.Count - 1)];
                                    b_container.container.TryAddItem(PlayerFinder.getPlayerActor().generateItems.initialiseRandomItemData(false, 100, false, ItemLocationTag.None, 34, new_blessing_id, 0, 0, 0, false, 0), 1, Context.DEFAULT);
                                    already_list.Add(new_blessing_id);
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
        }
        public class Monolith_Timeline_Panel_Manager
        {
            public static MonolithTimelinePanelManager manager = null;

            [HarmonyPatch(typeof(MonolithTimelinePanelManager), "OnEnable")]
            public class MonolithTimelinePanelManager_OnEnable
            {
                [HarmonyPostfix]
                public static void Postfix(ref MonolithTimelinePanelManager __instance)
                {
                    manager = __instance;

                    //__instance.web.
                    /*EchoWeb echo_web = __instance.web;
                    int island_count = echo_web.islands.Count;
                    for (int i = 0; i < island_count; i++)
                    {
                        EchoWebIsland entry = echo_web.islands.getEntry(i);
                        if (entry != null)
                        {
                            entry.
                        }
                    }*/
                }
            }
        }
        
        //Gaze
        /*[HarmonyPatch(typeof(LE.Data.SavedEchoWeb), "get_GazeOfOrobyss")]
        public class SavedEchoWeb_get_GazeOfOrobyss
        {
            [HarmonyPostfix]
            static void Postfix(ref LE.Data.SavedEchoWeb __instance, ref int __result)
            {
                if (Scenes_Manager.GameScene())
                {
                    //if (__instance.ToString().Contains("LE.Data.")) { Main.logger_instance.Msg("Gaze : " + __instance.ToString()); }
                    if ((__instance.ToString() == "LE.Data.SavedMonolithRun") || (__instance.ToString() == "LE.Data.SavedEchoWeb"))
                    {
                        __result = 65535;
                    }
                }
            }
        }*/
        
        //Corruption
        /*[HarmonyPatch(typeof(LE.Data.SavedEchoWeb), "set_Corruption")]
        public class SavedEchoWeb_set_Corruption
        {
            [HarmonyPrefix]
            static void Prefix(LE.Data.SavedEchoWeb __instance, ref int __0)
            {
                if (Scenes_Manager.GameScene())
                {
                    //LE.Data.SavedShard
                    //LE.Data.StashCategory
                    //LE.Data.SavedMonolithRun
                    //LE.Data.SavedEchoWeb
                    //LE.Data.SavedMonolithEchoOption
                    //LE.Data.SavedMonolithEchoOption
                    //Main.logger_instance.Msg("Corruption : " + __instance.ToString());
                    if (__instance.ToString() == "LE.Data.SavedEchoWeb")
                    {
                        __0 = 9999;
                        //__result = 9999;
                    }
                }
            }
        }*/
    }
}

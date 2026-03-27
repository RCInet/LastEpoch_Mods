using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Monoliths
{
    [RegisterTypeInIl2Cpp]
    public class Monoliths_CompleteObjective : MonoBehaviour
    {
        public Monoliths_CompleteObjective(System.IntPtr ptr) : base(ptr) { }
        public static Monoliths_CompleteObjective instance { get; private set; }
        
        public static bool started = false;

        public static Vector3 player_position = Vector3.zero;
        public static System.DateTime player_start_time;
        public static System.Double player_wait = 5; //wait 5sec after first spawn

        public static MonolithZoneManager monolith_zone_manager = null;
        public static bool initialized = false;
        public static bool complete = false;

        public static Il2CppLE.Networking.Monolith.ForgeSync forge_sync = null;
        public static bool tp_forge = false;

        public static Il2CppSystem.Collections.Generic.List<Il2CppLE.Networking.Monolith.UnstableRiftSync> rift_sync = new Il2CppSystem.Collections.Generic.List<Il2CppLE.Networking.Monolith.UnstableRiftSync>();
        public static Il2CppSystem.Collections.Generic.List<bool> tp_rift = new Il2CppSystem.Collections.Generic.List<bool>();
        public static int rift_index = 0;

        public static RunePrison prison_sync = null;
        public static bool prison_started = false;
        public static bool tp_prison = false;

        public static TombEntranceLogic tomb_entrance_logic_sync = null;
        public static bool tp_tomb_entrance = false;

        void Awake()
        {
            instance = this;
        }
        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.F10)) { RevealIslands(); } //Debug

            if (monolith_zone_manager.IsNullOrDestroyed()) { initialized = false; started = false; }
            if (initialized)
            {
                if ((!started) && (player_position != Get_PlayerPosition()) && ((Get_PlayerPosition().x != 0f) || (Get_PlayerPosition().y != 0f) || (Get_PlayerPosition().z != 0f)))
                {
                    player_position = Get_PlayerPosition();
                    Main.logger_instance.Msg("Player Spawn at Position : " + player_position);
                    player_start_time = System.DateTime.Now;
                    complete = false;
                    started = true;
                }
                if (started)
                {
                    System.TimeSpan elaspedTime = System.DateTime.Now - player_start_time;
                    System.Double seconds = elaspedTime.TotalSeconds;
                    if (seconds > player_wait)
                    {
                        if ((!monolith_zone_manager.IsNullOrDestroyed()) && (CanRun()) && (!Refs_Manager.epoch_input_manager.IsNullOrDestroyed()))
                        {
                            Kill(monolith_zone_manager.objectiveEnemies, "objectiveEnemies");
                            Kill(monolith_zone_manager.currentAndFormerObjectiveEnemies, "currentAndFormerObjectiveEnemies");
                            Kill(monolith_zone_manager.spawnedEntities, "spawnedEntities");

                            /*if (monolith_zone_manager.runComplete)
                            {
                                if (!Refs_Manager.ground_item_manager.IsNullOrDestroyed())
                                {
                                    foreach (GroundItemManager.GroundItem item in Refs_Manager.ground_item_manager.offlineItems.items)
                                    {
                                        item.location = Get_PlayerPosition();
                                    }
                                    complete = true;
                                }                                
                            }
                            else if (complete) { }
                            else*/
                            if (monolith_zone_manager.defaultObjectiveType == MonolithObjectiveType.ArenaWaves)
                            {
                                if (monolith_zone_manager.waveSpawner.spawnsRemaining.Count > 0)
                                {
                                    GameObject obj = monolith_zone_manager.waveSpawner.spawnsRemaining[0];
                                    if (!obj.IsNullOrDestroyed())
                                    {
                                        obj.transform.position = Get_PlayerPosition();
                                        obj.GetComponent<Dying>().die();
                                    }
                                }
                                else if (!monolith_zone_manager.waveSpawner.isLastWave) { monolith_zone_manager.waveSpawner.spawnWave(); }
                            }
                            else if (monolith_zone_manager.isQuestZone)
                            {
                                //EpochInputManager.ButtonPressBlocked
                            }
                            else if (monolith_zone_manager.isHarbingerFight)
                            {

                            }
                            else if (monolith_zone_manager.isTimelineBossEncounter)
                            {

                            }
                            else if ((!forge_sync.IsNullOrDestroyed()) && (!tp_forge))
                            {
                                Main.logger_instance?.Msg("TP Forge");
                                tp_forge = MoveTo(forge_sync.gameObject.transform.position, tp_forge);
                            }
                            else if ((!rift_sync.IsNullOrDestroyed()) && (rift_index < tp_rift.Count) && (rift_index < rift_sync.Count))
                            {
                                if (!tp_rift[rift_index])
                                {
                                    Main.logger_instance?.Msg("TP Rift");
                                    tp_rift[rift_index] = MoveTo(rift_sync[rift_index].gameObject.transform.position, tp_rift[rift_index]);
                                }
                                else
                                {
                                    Main.logger_instance?.Msg("Next Rift");
                                    rift_index++;
                                }
                            }
                            else if ((!prison_sync.IsNullOrDestroyed()) && (!tp_prison))
                            {
                                Main.logger_instance?.Msg("TP Prison");
                                prison_started = false;
                                tp_prison = MoveTo(prison_sync.gameObject.transform.position, tp_prison);
                            }
                            else if ((!prison_sync.IsNullOrDestroyed()) && (tp_prison) && (!prison_started))
                            {
                                prison_sync.Open();
                                //prison_sync.spawner.Start();

                                prison_started = true;
                            }
                            else if ((!tomb_entrance_logic_sync.IsNullOrDestroyed()) && (!tp_tomb_entrance))
                            {
                                Main.logger_instance?.Msg("TP Tomb Entrance");
                                tp_tomb_entrance = MoveTo(tomb_entrance_logic_sync.gameObject.transform.position, tp_tomb_entrance);
                            }

                            /*if (!monolith_zone_manager.zoneQuestListHolder.IsNullOrDestroyed())
                            {
                                if (!monolith_zone_manager.zoneQuestListHolder.StatefulQuestList.IsNullOrDestroyed())
                                {
                                    foreach (StatefulQuest state_quest in monolith_zone_manager.zoneQuestListHolder.StatefulQuestList.trackedQuests)
                                    {
                                        bool complete = false;
                                        foreach (StatefulObjective objective in state_quest.objectives)
                                        {
                                            if (objective.state != QuestState.Completed) { complete = true; break; }
                                        }
                                        if (complete) { state_quest.quest.completeQuest(Refs_Manager.player_actor); }
                                    }
                                }
                            }*/
                        }
                    }
                }
            }
        }

        public static Vector3 Get_PlayerPosition()
        {
            Vector3 position = Vector3.zero;
            if (!Refs_Manager.player_actor.IsNullOrDestroyed()) { position = Refs_Manager.player_actor.transform.position; }

            return position;
        }
        bool MoveTo(Vector3 position, bool tp)
        {
            bool result = false;

            if ((Get_PlayerPosition() != position) && (!tp) && (position != Vector3.zero))
            {
                Main.logger_instance?.Msg("Try to move Player to : " + position.ToString());
                if (!Refs_Manager.player_moving.IsNullOrDestroyed()) { Refs_Manager.player_moving.MoveToPointNoChecks(position); }
                if (!Refs_Manager.player_actor.IsNullOrDestroyed()) { Refs_Manager.player_actor.transform.position = position; }
                if (Get_PlayerPosition() == position)
                {
                    Main.logger_instance?.Msg("Move Player Success");
                    result = true;
                }
            }

            return result;
        }
        void Kill(Il2CppSystem.Collections.Generic.List<Dying> enemies, string name)
        {
            try
            {
                if (!enemies.IsNullOrDestroyed())
                {
                    if (enemies.Count > 0)
                    {
                        for (int i = 0; i < enemies.Count; i++)
                        {
                            Dying dying = enemies[0];
                            if (!dying.IsNullOrDestroyed())
                            {
                                //dying.gameObject.transform.position = Get_PlayerPosition();
                                Main.logger_instance?.Msg("Kill " + dying.name);
                                dying.die();
                                break;
                            }
                        }
                    }
                }
            }
            catch { Main.logger_instance?.Error("Error MoveAndKill : " + name); }
        }
        
        public static bool CanRun()
        {
            bool result = false;
            if (!Save_Manager.instance.IsNullOrDestroyed())
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    result = Save_Manager.instance.data.Scenes.Monoliths.Enable_CompleteObjective;
                }
            }

            return result;
        }
                
        [HarmonyPatch(typeof(MonolithZoneManager), "initialise")]
        public class MonolithZoneManager_initialise
        {
            [HarmonyPrefix]
            static void Prefix(ref MonolithZoneManager __instance, StatefulQuestList __0)
            {
                Main.logger_instance?.Msg("MonolithZoneManager.initialise() Prefix");
                
                started = false;
                player_position = Get_PlayerPosition();

                forge_sync = null;
                tp_forge = false;

                rift_sync = new Il2CppSystem.Collections.Generic.List<Il2CppLE.Networking.Monolith.UnstableRiftSync>();
                tp_rift = new Il2CppSystem.Collections.Generic.List<bool>();

                prison_sync = null;
                tp_prison = false;

                tomb_entrance_logic_sync = null;
                tp_tomb_entrance = false;

                if (monolith_zone_manager.IsNullOrDestroyed()) { monolith_zone_manager = __instance; }
                initialized = false;
            }
            [HarmonyPostfix]
            static void Postfix(ref MonolithZoneManager __instance, StatefulQuestList __0)
            {
                Main.logger_instance?.Msg("MonolithZoneManager.initialise() Postfix");
                if (CanRun())
                {
                    if (__instance.isQuestZone)
                    {
                        Main.logger_instance?.Msg("MonolithZoneManager.isQuestZone");
                        foreach (Il2Cpp.Quest quest in __instance.questsThatCompleteZone)
                        {
                            Main.logger_instance?.Msg("Complete quest : " + quest.name);
                            quest.completeQuest(Refs_Manager.player_actor);
                        }
                    }
                    if (__instance.isHarbingerFight)
                    {
                        Main.logger_instance?.Msg("MonolithZoneManager.isHarbingerFight");
                    }
                    if (__instance.isTimelineBossEncounter)
                    {
                        Main.logger_instance?.Msg("MonolithZoneManager.isTimelineBossEncounter");
                    }
                    initialized = true;
                }

                //initialized = true;
            }
        }

        [HarmonyPatch(typeof(MonolithZoneManager), "CreatePulseForge")]
        public class MonolithZoneManager_CreatePulseForge
        {
            [HarmonyPostfix]
            static void Postfix(/*ref MonolithZoneManager __instance,*/ ref Il2CppLE.Networking.Monolith.ForgeSync __0)
            { 
                Main.logger_instance?.Msg("MonolithZoneManager.CreatePulseForge()");
                forge_sync = __0;
            }
        }

        [HarmonyPatch(typeof(MonolithZoneManager), "CreatePulseRift")]
        public class MonolithZoneManager_CreatePulseRift
        {
            [HarmonyPostfix]
            static void Postfix(/*ref MonolithZoneManager __instance,*/ ref Il2CppLE.Networking.Monolith.UnstableRiftSync __0)
            {
                Main.logger_instance?.Msg("MonolithZoneManager.CreatePulseRift()");
                rift_sync.Add(__0);
                tp_rift.Add(false);
            }
        }

        [HarmonyPatch(typeof(RunePrison), "Initialise")]
        public class RunePrison_Initialise
        {
            [HarmonyPostfix]
            static void Postfix(ref RunePrison __instance, uint __0)
            {
                Main.logger_instance?.Msg("RunePrison.Initialise() Id = " + __0);
                prison_sync = __instance;
            }
        }

        [HarmonyPatch(typeof(MonolithZoneManager), "CreatePulseBeacon")]
        public class MonolithZoneManager_CreatePulseBeacon
        {
            [HarmonyPostfix]
            static void Postfix(ref BeaconSync __0)
            {
                Main.logger_instance?.Msg("MonolithZoneManager.CreatePulseBeacon()");
            }
        }

        [HarmonyPatch(typeof(MonolithZoneManager), "CreatePulseCache")]
        public class MonolithZoneManager_CreatePulseCache
        {
            [HarmonyPostfix]
            static void Postfix(MonolithZoneManager __instance, Il2CppLE.Gameplay.Monolith.Frontend.MonolithPulse __result, OneShotCacheGameplayObject __0)
            {
                Main.logger_instance?.Msg("MonolithZoneManager.CreatePulseCache()");
            }
        }

        [HarmonyPatch(typeof(MonolithZoneManager), "CreatePulseForCache")]
        public class MonolithZoneManager_CreatePulseForCache
        {
            [HarmonyPostfix]
            static void Postfix(MonolithZoneManager __instance, Il2Cpp.OneShotCacheGameplayObject __0)
            {
                Main.logger_instance?.Msg("MonolithZoneManager.CreatePulseForCache()");
            }
        }

        [HarmonyPatch(typeof(MonolithZoneManager), "CreatePulseShrine", new System.Type[] { typeof(ShrineSync) })]
        public class MonolithZoneManager_CreatePulseShrine_ShrineSync
        {
            [HarmonyPostfix]
            static void Postfix(MonolithZoneManager __instance, Il2CppLE.Gameplay.Monolith.Frontend.MonolithPulse __result, ShrineSync __0)
            {
                Main.logger_instance?.Msg("MonolithZoneManager.CreatePulseShrine() ShrineSync");
            }
        }

        [HarmonyPatch(typeof(MonolithZoneManager), "CreatePulseShrine", new System.Type[] { typeof(GameObject) })]
        public class MonolithZoneManager_CreatePulseShrine_GameObject
        {
            [HarmonyPostfix]
            static void Postfix(Il2Cpp.MonolithZoneManager __instance, Il2CppLE.Gameplay.Monolith.Frontend.MonolithPulse __result, GameObject __0)
            {
                Main.logger_instance?.Msg("MonolithZoneManager.CreatePulseShrine() GameObject");
            }
        }

        [HarmonyPatch(typeof(MonolithZoneManager), "CreatePulseTombEntrance")]
        public class MonolithZoneManager_CreatePulseTombEntrance
        {
            [HarmonyPostfix]
            static void Postfix(/*MonolithZoneManager __instance, Il2CppLE.Gameplay.Monolith.Frontend.MonolithPulse __result,*/ ref TombEntranceLogic __0)
            {
                Main.logger_instance?.Msg("MonolithZoneManager.CreatePulseTombEntrance()");
                tomb_entrance_logic_sync = __0;
            }
        }

        [HarmonyPatch(typeof(MonolithZoneManager), "CreatePulseActor")]
        public class MonolithZoneManager_CreatePulseActor
        {
            [HarmonyPostfix]
            static void Postfix(MonolithZoneManager __instance, Il2Cpp.Actor __0, bool __1) //__1 = showImmediately
            {
                Main.logger_instance?.Msg("MonolithZoneManager.CreatePulseActor() : " + __0.name);
            }
        }

        //Debug
        public static MonolithTimelinePanelManager monolith_timeline_panel_manager = null;

        /*public static void RevealIslands()
        {
            if (!monolith_timeline_panel_manager.IsNullOrDestroyed())
            {
                monolith_timeline_panel_manager.revealEchoesAroundIslands();
                monolith_timeline_panel_manager.createEchoWebVisuals(monolith_timeline_panel_manager.web, 1);
                monolith_timeline_panel_manager.forceRebuildLayoutAfterFrame();
                monolith_timeline_panel_manager.webView.hasChanged = true;
            }
        }*/

        [HarmonyPatch(typeof(MonolithTimelinePanelManager), "OnEnable")]
        public class MonolithTimelinePanelManager_OnEnable
        {
            [HarmonyPostfix]
            public static void Postfix(ref MonolithTimelinePanelManager __instance)
            {
                monolith_timeline_panel_manager = __instance;
            }
        }
    }
}

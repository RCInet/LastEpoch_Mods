using HarmonyLib;
using UnhollowerBaseLib;
using UnityEngine;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Scene_Mods
    {
        #region OnSceneChange               
        //SpawnerPlacementManager
        public static bool Enable_SpawnerPlacementManager_defaultSpawnerDensity = true;
        public static float SpawnerPlacementManager_defaultSpawnerDensity = 10f;
        public static bool Enable_SpawnerPlacementManager_IncreaseExperience = true;
        public static float SpawnerPlacementManager_IncreaseExperience = 255f;
        
        public static void Launch()
        {            
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                System.Type type = obj.GetActualType();
                if (type == typeof(SpawnerPlacementManager))
                {
                    if ((Enable_SpawnerPlacementManager_defaultSpawnerDensity) |
                        (Enable_SpawnerPlacementManager_IncreaseExperience))
                    {
                        SpawnerPlacementManager spawner_manager = obj.TryCast<SpawnerPlacementManager>();
                        if (Enable_SpawnerPlacementManager_defaultSpawnerDensity)
                        {
                            spawner_manager.defaultSpawnerDensity = SpawnerPlacementManager_defaultSpawnerDensity;
                            spawner_manager.alwaysRollSpawnerDensity = false;
                        }
                        if (Enable_SpawnerPlacementManager_IncreaseExperience)
                        {
                            spawner_manager.increasedExperience = SpawnerPlacementManager_IncreaseExperience;
                        }
                    }
                    break;
                }
            }
        }
        #endregion
        #region Functions Patch        
        public class Waypoints_Mods
        {
            public static bool Enable_Waypoint_Unlock = true;

            #region Unlock
            [HarmonyPatch(typeof(UIWaypointStandard), "OnPointerEnter")]
            public class Unlock
            {
                [HarmonyPrefix]
                static void Prefix(UIWaypointStandard __instance, UnityEngine.EventSystems.PointerEventData __0)
                {
                    if (Enable_Waypoint_Unlock)
                    {
                        __instance.isActive = true;
                    }                    
                }
            }
            #endregion
        }
        public class Dungeons_Mods
        {
            public static bool Enable_Dungeons_WithoutKey = true;
            private static bool CreateKey = false; //True = create a key, False = Bypass Result
            public static bool Enable_Dungeons_ObjectiveReveal = true;

            #region Initialize
            [HarmonyPatch(typeof(DungeonZoneManager), "initialise")]
            public class DungeonZoneManager_initialise
            {
                [HarmonyPostfix]
                static void initialise(ref DungeonZoneManager __instance)
                {
                    if (Enable_Dungeons_ObjectiveReveal) { __instance.objectiveRevealThresholdModifier = float.MaxValue; }
                    
                }
            }
            #endregion
            #region EnterWithoutKey
            [HarmonyPatch(typeof(ItemContainersManager), "IsOccupiedWithValidDungeonKey")]
            public class NoKey
            {
                #region CreateKey
                [HarmonyPrefix]
                static void Prefix(ItemContainersManager __instance, bool __result, DungeonID __0)
                {
                    if ((Enable_Dungeons_WithoutKey) && (CreateKey))
                    {
                        if ((__instance != null) && (!__result))
                        {
                            byte item_type = 104;
                            Il2CppStructArray<byte> item_id = new Il2CppStructArray<byte>(22);
                            item_id[0] = 1;
                            item_id[1] = item_type;
                            item_id[2] = 0; //id
                            for (int i = 3; i < item_id.Count; i++) { item_id[i] = 0; }

                            Vector2Int Inventory_position = new Vector2Int { x = 0, y = 0 };
                            Vector2Int Item_size = new Vector2Int { x = 1, y = 1 };
                            if (__0 == DungeonID.Dungeon1)
                            {
                                item_id[2] = 4;
                                ItemData item_data = new ItemData { itemType = item_type, subType = 4, id = item_id };
                                ItemContainerEntry item_container_entry = new ItemContainerEntry(item_data, Inventory_position, Item_size, 1);
                                __instance.dun1Key.content = item_container_entry;
                            }
                            else if (__0 == DungeonID.LightlessArbor)
                            {
                                item_id[2] = 5;
                                ItemData item_data = new ItemData { itemType = item_type, subType = 5, id = item_id };
                                ItemContainerEntry item_container_entry = new ItemContainerEntry(item_data, Inventory_position, Item_size, 1);
                                __instance.lightlessArborDungeonKey.content = item_container_entry;
                            }
                            else if (__0 == DungeonID.SoulfireBastion)
                            {
                                item_id[2] = 6;
                                ItemData item_data = new ItemData { itemType = item_type, subType = 6, id = item_id };
                                ItemContainerEntry item_container_entry = new ItemContainerEntry(item_data, Inventory_position, Item_size, 1);
                                __instance.soulfireBastionDungeonKey.content = item_container_entry;
                            }
                        }
                    }
                }
                #endregion
                #region Bypass result
                [HarmonyPostfix]
                static void Postfix(ref ItemContainersManager __instance, ref bool __result, ref DungeonID __0)
                {
                    if ((Enable_Dungeons_WithoutKey) && (!CreateKey)) { __result = true; }
                }
                #endregion
            }
            #endregion
        }
        public class Monoliths_mods
        {
            public static bool Enable_Monolith_Stability = true;
            public static bool Enable_Monolith_Overide_Max_Stability = false;
            public static int Max_Stability = 100;
            public static bool Enable_Monolith_EnnemiesDefeat_OnStart = true;
            public static float Monolith_EnnemiesDefeat_OnStart = 500f;
            public static bool Enable_Monolith_ObjectiveReveal = true;
            public static bool Enable_Monolith_Complete_Objective = false;            
            
            public static bool Enable_Monolith_NoDie = true;

            #region Initialize
            [HarmonyPatch(typeof(MonolithZoneManager), "initialise")]
            public class DungeonZoneManager_initialise
            {
                [HarmonyPostfix]
                static void initialise(ref MonolithZoneManager __instance, ref StatefulQuestList __0)
                {
                    if (Enable_Monolith_Overide_Max_Stability) { __instance.maxBonusStablity = Max_Stability; }                    
                    if (Enable_Monolith_Stability) { __instance.bonusStablity = __instance.maxBonusStablity; }                    
                    if (Enable_Monolith_EnnemiesDefeat_OnStart) { __instance.enemiesDefeated = Monolith_EnnemiesDefeat_OnStart; }
                    if (Enable_Monolith_ObjectiveReveal) { __instance.objectiveRevealThresholdModifier = float.MaxValue; }                    
                    if (Enable_Monolith_Complete_Objective) { __instance.CompleteObjective(); }
                }
            }
            #endregion
            #region Stability
            [HarmonyPatch(typeof(MonolithZoneManager), "OnBonusStabilityChanged")]
            public class MonolithZoneManager_OnBonusStabilityChanged
            {
                [HarmonyPrefix]
                static void Prefix(ref MonolithZoneManager __instance)
                {
                    if (Enable_Monolith_Stability)
                    {
                        __instance.bonusStablity = __instance.maxBonusStablity;
                    }
                }
            }
            #endregion
            #region No Die
            [HarmonyPatch(typeof(EchoWebIsland), "onDiedInEcho")]
            public class EchoWebIsland_onDiedInEcho
            {
                [HarmonyPrefix]
                public static bool Prefix(ref EchoWebIsland __instance, ref MonolithRun run)
                {
                    bool result = true;
                    if (Enable_Monolith_NoDie) { result =  false; }
                    
                    return result;
                }
            }
            #endregion
        }
        #endregion
    }
}

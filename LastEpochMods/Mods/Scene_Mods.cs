using HarmonyLib;
using Il2CppSystem.Runtime.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;
using UnityEngine;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Scene_Mods
    {
        #region OnSceneChange
        //ItemDrop
        public static bool Enable_DeathItemDrop_goldMultiplier = true;
        public static float DeathItemDrop_goldMultiplier = 255f;
        public static bool Enable_DeathItemDrop_ItemMultiplier = true;
        public static float DeathItemDrop_ItemMultiplier = 2f;
        public static bool Enable_DeathItemDrop_Experience = true;
        public static long DeathItemDrop_Experience = 9999;
        public static bool Enable_DeathItemDrop_AdditionalRare = false;
        public static bool DeathItemDrop_AdditionalRare = true;
        //MonsterRarityMagic (untested)
        public static bool Enable_MonsterRarityMagic_BaseExpMultiplier = false;
        public static float MonsterRarityMagic_BaseExpMultiplier = 255f;
        public static bool Enable_MonsterRarityMagic_ExpMultiplierPerLevel = false;
        public static float MonsterRarityMagic_ExpMultiplierPerLevel = 255f;
        //MonsterRarityRare (untested)
        public static bool Enable_MonsterRarityRare_BaseExpMultiplier = false;
        public static float MonsterRarityRare_BaseExpMultiplier = 255f;
        public static bool Enable_MonsterRarityRare_xpMultiplierPerLevel = false;
        public static float MonsterRarityRare_xpMultiplierPerLevel = 255f;        
        //SpawnerPlacementManager
        public static bool Enable_SpawnerPlacementManager_defaultSpawnerDensity = true;
        public static float SpawnerPlacementManager_defaultSpawnerDensity = 255f;
        public static bool Enable_SpawnerPlacementManager_IncreaseExperience = false;
        public static float SpawnerPlacementManager_IncreaseExperience = 255f;
        
        public static void Launch()
        {
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                System.Type type = obj.GetActualType();
                if (type == typeof(DeathItemDrop))
                {
                    if ((Enable_DeathItemDrop_goldMultiplier) | (Enable_DeathItemDrop_ItemMultiplier) |
                        (Enable_DeathItemDrop_AdditionalRare) | (Enable_DeathItemDrop_Experience))
                    {
                        DeathItemDrop drop = obj.TryCast<DeathItemDrop>();
                        if (Enable_DeathItemDrop_goldMultiplier)
                        {
                            drop.overrideBaseGoldDropChance = true;
                            drop.goldDropChance = 1; //100%
                            drop.goldMultiplier = DeathItemDrop_goldMultiplier;
                        }
                        if (Enable_DeathItemDrop_ItemMultiplier)
                        {
                            drop.overrideBaseItemDropChance = true;
                            drop.itemDropChance = 1; //100%
                            drop.itemMultiplier = DeathItemDrop_ItemMultiplier;
                        }
                        if (Enable_DeathItemDrop_AdditionalRare)
                        {
                            drop.guaranteedAdditionalRare = DeathItemDrop_AdditionalRare;
                        }
                        if (Enable_DeathItemDrop_Experience)
                        {
                            drop.experience = DeathItemDrop_Experience;
                        }
                    }
                }
                else if (type == typeof(MonsterRarityMagic))
                {
                    if ((Enable_MonsterRarityMagic_BaseExpMultiplier) | (Enable_MonsterRarityMagic_ExpMultiplierPerLevel))
                    {
                        MonsterRarityMagic magic_monsters = obj.TryCast<MonsterRarityMagic>();
                        if (Enable_MonsterRarityMagic_BaseExpMultiplier)
                        {
                            magic_monsters.baseExperienceMultiplier = MonsterRarityMagic_BaseExpMultiplier;
                        }
                        if (Enable_MonsterRarityMagic_ExpMultiplierPerLevel)
                        {
                            magic_monsters.experienceMultiplierPerLevel = MonsterRarityMagic_ExpMultiplierPerLevel;
                        }
                    }
                }
                else if (type == typeof(MonsterRarityRare))
                {
                    if ((Enable_MonsterRarityRare_BaseExpMultiplier) |
                        (Enable_MonsterRarityRare_xpMultiplierPerLevel))
                    {
                        MonsterRarityRare rare_monsters = obj.TryCast<MonsterRarityRare>();
                        if (Enable_MonsterRarityRare_BaseExpMultiplier)
                        {
                            rare_monsters.baseExperienceMultiplier = MonsterRarityRare_BaseExpMultiplier;
                        }
                        if (Enable_MonsterRarityRare_xpMultiplierPerLevel)
                        {
                            rare_monsters.experienceMultiplierPerLevel = MonsterRarityRare_xpMultiplierPerLevel;
                        }
                    }
                }                
                else if (type == typeof(SpawnerPlacementManager))
                {
                    if ((Enable_SpawnerPlacementManager_defaultSpawnerDensity) |
                        (Enable_SpawnerPlacementManager_IncreaseExperience))
                    {
                        SpawnerPlacementManager spawner_manager = obj.TryCast<SpawnerPlacementManager>();
                        if (Enable_SpawnerPlacementManager_defaultSpawnerDensity)
                        {
                            spawner_manager.defaultSpawnerDensity = System.Convert.ToInt32(SpawnerPlacementManager_defaultSpawnerDensity);
                            spawner_manager.alwaysRollSpawnerDensity = false;
                        }
                        if (Enable_SpawnerPlacementManager_IncreaseExperience)
                        {
                            spawner_manager.increasedExperience = SpawnerPlacementManager_IncreaseExperience;
                        }
                    }
                }
            }
        }
        #endregion
        #region Functions Patch
        public static bool Enable_Waypoint_Unlock = true;
        public static bool Enable_Dungeons_WithoutKey = true;
        public static bool Enable_Monolith_BossFarm = true;
        public static bool Enable_Monolith_NoDie = true;

        public class Waypoints_Mods
        {
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
            #region Initialize
            /*[HarmonyPatch(typeof(DungeonZoneManager), "initialise")]
            public class DungeonZoneManager_initialise
            {
                [HarmonyPrefix]
                static void Prefix(ref DungeonZoneManager __instance)
                {
                    //Init Dungeon Here                
                    __instance.objectiveRevealThresholdModifier = float.MinValue;
                }
            }*/
            #endregion
            #region EnterWithoutKey
            [HarmonyPatch(typeof(ItemContainersManager), "IsOccupiedWithValidDungeonKey")]
            public class NoKey
            {
                [HarmonyPrefix]
                static void Prefix(ItemContainersManager __instance, bool __result, DungeonID __0)
                {
                    if (Enable_Dungeons_WithoutKey)
                    {
                        if ((__instance != null) && (!__result)) //Add key
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
                                ItemData item_data = new ItemData
                                {
                                    itemType = item_type,
                                    subType = 4,
                                    id = item_id
                                };
                                ItemContainerEntry item_container_entry = new ItemContainerEntry(item_data, Inventory_position, Item_size, 1);
                                __instance.dun1Key.content = item_container_entry;
                            }
                            else if (__0 == DungeonID.LightlessArbor)
                            {
                                item_id[2] = 5;
                                ItemData item_data = new ItemData
                                {
                                    itemType = item_type,
                                    subType = 5,
                                    id = item_id
                                };
                                ItemContainerEntry item_container_entry = new ItemContainerEntry(item_data, Inventory_position, Item_size, 1);
                                __instance.lightlessArborDungeonKey.content = item_container_entry;
                            }
                            else if (__0 == DungeonID.SoulfireBastion)
                            {
                                item_id[2] = 6;
                                ItemData item_data = new ItemData
                                {
                                    itemType = item_type,
                                    subType = 6,
                                    id = item_id
                                };
                                ItemContainerEntry item_container_entry = new ItemContainerEntry(item_data, Inventory_position, Item_size, 1);
                                __instance.soulfireBastionDungeonKey.content = item_container_entry;
                            }
                        }
                    }
                }
            }
            #endregion
        }
        public class Monoliths_mods
        {
            #region Initialize
            /*[HarmonyPatch(typeof(MonolithZoneManager), "initialise")]
            public class DungeonZoneManager_initialise
            {
                [HarmonyPrefix]
                static void Prefix(ref MonolithZoneManager __instance)
                {
                    //Init Monoliths Here                
                    __instance.objectiveRevealThresholdModifier = float.MinValue;
                }
            }*/
            #endregion
            #region BossFarm
            [HarmonyPatch(typeof(MonolithZoneManager), "OnBonusStabilityChanged")]
            public class MonolithZoneManager_OnBonusStabilityChanged
            {
                [HarmonyPrefix]
                static void Prefix(ref MonolithZoneManager __instance)
                {
                    if (Enable_Monolith_BossFarm)
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

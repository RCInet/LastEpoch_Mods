using Il2CppSystem.Runtime.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Scene_Mods
    {
        //ItemDrop
        public static bool Enable_DeathItemDrop_goldMultiplier = true;
        public static int DeathItemDrop_goldMultiplier = 99;
        public static bool Enable_DeathItemDrop_ItemMultiplier = true;
        public static int DeathItemDrop_ItemMultiplier = 99;
        public static bool Enable_DeathItemDrop_Experience = true;
        public static int DeathItemDrop_Experience = 9999;
        public static bool Enable_DeathItemDrop_AdditionalRare = true;
        public static bool DeathItemDrop_AdditionalRare = true;
        //MonsterRarityMagic
        public static bool Enable_MonsterRarityMagic_BaseExpMultiplier = true;
        public static int MonsterRarityMagic_BaseExpMultiplier = 99999;
        public static bool Enable_MonsterRarityMagic_ExpMultiplierPerLevel = true;
        public static int MonsterRarityMagic_ExpMultiplierPerLevel = 99999;
        //MonsterRarityRare
        public static bool Enable_MonsterRarityRare_BaseExpMultiplier = true;
        public static int MonsterRarityRare_BaseExpMultiplier = 99999;
        public static bool Enable_MonsterRarityRare_xpMultiplierPerLevel = true;
        public static int MonsterRarityRare_xpMultiplierPerLevel = 99999;
        //SpawnerPlacementRoom
        public static bool Enable_SpawnerPlacementRoom_intendedSpawnerDensity = true;
        public static int SpawnerPlacementRoom_intendedSpawnerDensity = 99999;
        //SpawnerPlacementManager
        public static bool Enable_SpawnerPlacementManager_defaultSpawnerDensity = true;
        public static int SpawnerPlacementManager_defaultSpawnerDensity = 99999;
        public static bool Enable_SpawnerPlacementManager_alwaysRollSpawnerDensity = true;
        public static bool SpawnerPlacementManager_alwaysRollSpawnerDensity = false;
        public static bool Enable_SpawnerPlacementManager_IncreaseExperience = true;
        public static int SpawnerPlacementManager_IncreaseExperience = 99999;

        public static void Launch()
        {
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                System.Type type = obj.GetActualType();
                if (type == typeof(DeathItemDrop))
                {
                    DeathItemDrop drop = new DeathItemDrop();
                    if ((Enable_DeathItemDrop_goldMultiplier) | (Enable_DeathItemDrop_ItemMultiplier) |
                        (Enable_DeathItemDrop_AdditionalRare) | (Enable_DeathItemDrop_Experience))
                    {
                        drop = obj.TryCast<DeathItemDrop>();
                    }
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
                else if (type == typeof(MonsterRarityMagic))
                {
                    MonsterRarityMagic magic_monsters = new MonsterRarityMagic();
                    if ((Enable_MonsterRarityMagic_BaseExpMultiplier) | (Enable_MonsterRarityMagic_ExpMultiplierPerLevel))
                    {
                        magic_monsters = obj.TryCast<MonsterRarityMagic>();
                    }
                    if (Enable_MonsterRarityMagic_BaseExpMultiplier)
                    {
                        magic_monsters.baseExperienceMultiplier = MonsterRarityMagic_BaseExpMultiplier;
                    }
                    if (Enable_MonsterRarityMagic_ExpMultiplierPerLevel)
                    {
                        magic_monsters.experienceMultiplierPerLevel = MonsterRarityMagic_ExpMultiplierPerLevel;
                    }                    
                }
                else if (type == typeof(MonsterRarityRare))
                {
                    MonsterRarityRare rare_monsters = new MonsterRarityRare();
                    if ((Enable_MonsterRarityRare_BaseExpMultiplier) | (Enable_MonsterRarityRare_xpMultiplierPerLevel))
                    {
                        rare_monsters = obj.TryCast<MonsterRarityRare>();
                    }
                    if (Enable_MonsterRarityRare_BaseExpMultiplier)
                    {
                        rare_monsters.baseExperienceMultiplier = MonsterRarityRare_BaseExpMultiplier;
                    }
                    if (Enable_MonsterRarityRare_xpMultiplierPerLevel)
                    {
                        rare_monsters.experienceMultiplierPerLevel = MonsterRarityRare_xpMultiplierPerLevel;
                    }
                }
                else if (type == typeof(SpawnerPlacementRoom))
                {
                    if (Enable_SpawnerPlacementRoom_intendedSpawnerDensity)
                    {
                        SpawnerPlacementRoom room = obj.TryCast<SpawnerPlacementRoom>();
                        room.intendedSpawnerDensity = SpawnerPlacementRoom_intendedSpawnerDensity;                        
                    }                    
                }
                else if (type == typeof(SpawnerPlacementManager))
                {
                    SpawnerPlacementManager spawner_manager = new SpawnerPlacementManager();
                    if ((Enable_SpawnerPlacementManager_defaultSpawnerDensity) | (Enable_SpawnerPlacementManager_alwaysRollSpawnerDensity) | (Enable_SpawnerPlacementManager_IncreaseExperience))
                    {
                        spawner_manager = obj.TryCast<SpawnerPlacementManager>();
                    }                    
                    if (Enable_SpawnerPlacementManager_defaultSpawnerDensity)
                    {
                        spawner_manager.defaultSpawnerDensity = System.Convert.ToInt32(SpawnerPlacementManager_defaultSpawnerDensity);
                    }
                    if (Enable_SpawnerPlacementManager_alwaysRollSpawnerDensity)
                    {
                        spawner_manager.alwaysRollSpawnerDensity = SpawnerPlacementManager_alwaysRollSpawnerDensity;
                    }
                    if (Enable_SpawnerPlacementManager_IncreaseExperience)
                    {
                        spawner_manager.increasedExperience = SpawnerPlacementManager_IncreaseExperience;
                    }
                }
            }
        }
    }
}

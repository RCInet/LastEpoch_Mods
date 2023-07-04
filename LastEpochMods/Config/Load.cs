using Il2CppSystem.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastEpochMods.Config
{
    public class Load
    {
        public static void Mods()
        {
            Data.mods_config = new Data.Mods_Structure();
            if (!File.Exists(Data.path + Data.filename))
            {
                Data.mods_config = DefaultConfig();
                Save.Mods();
                Main.logger_instance.Msg("Config not found : Default Values");
            }
            else
            {
                Data.mods_config = JsonConvert.DeserializeObject<Data.Mods_Structure>(File.ReadAllText(Data.path + Data.filename));
                Main.logger_instance.Msg("Config Loaded");
            }
            Data.mods_config_duplicate = Data.mods_config; //Use to check for data changed
        }
        private static Data.Mods_Structure DefaultConfig()
        {
            Data.Mods_Structure result = new Data.Mods_Structure
            {
                auto_loot = new Data.AutoLoot_Structure
                {
                    AutoPickup_Key = true,
                    AutoPickup_Craft = true,
                    AutoPickup_UniqueAndSet = false,
                    AutoStore_Materials = true,
                    AutoPickup_Gold = true,
                    AutoPickup_XpTome = true,
                    AutoPickup_Pots = false
                },
                items = new Data.Items_Structure
                {
                    Enable_increase_equipment_droprate = false,
                    increase_equipment_droprate = 255f,
                    Enable_increase_equipmentshards_droprate = true,
                    increase_equipmentshards_droprate = 255f,
                    Enable_increase_uniques_droprate = true,
                    increase_uniques_droprate = 255f,
                    Enable_DeathItemDrop_goldMultiplier = true,
                    DeathItemDrop_goldMultiplier = 255f,
                    Enable_DeathItemDrop_ItemMultiplier = true,
                    DeathItemDrop_ItemMultiplier = 4f,
                    Enable_DeathItemDrop_Experience = false,
                    DeathItemDrop_Experience = 9999,
                    Enable_DeathItemDrop_AdditionalRare = false,
                    DeathItemDrop_AdditionalRare = true,
                    Enable_Rarity = true,
                    GenerateItem_Rarity = 7,
                    Enable_RollImplicit = true,
                    Roll_Implicit = 255,
                    Enable_AffixsValue = true,
                    Roll_AffixValue = 255,
                    Enable_AffixsTier = true,
                    Roll_AffixTier = 7,
                    Enable_ForgingPotencial = true,
                    Roll_ForgingPotencial = 255,
                    Enable_UniqueMod = true,
                    Roll_UniqueMod = 255,
                    Enable_RollLegendayPotencial = true,
                    Roll_Legendary_Potencial = 4, //0 to 4
                    Enable_RollWeaverWill = true,
                    Roll_Weaver_Will = 28, //5 to 28
                    Remove_LevelReq = true,
                    Remove_ClassReq = true,
                    Remove_SubClassReq = true
                },
                scene = new Data.Scene_Structure
                {
                    Enable_Waypoint_Unlock = true,
                    Enable_SpawnerPlacementManager_defaultSpawnerDensity = true,
                    SpawnerPlacementManager_defaultSpawnerDensity = 10f,
                    Enable_SpawnerPlacementManager_IncreaseExperience = true,
                    SpawnerPlacementManager_IncreaseExperience = 255f,
                    Enable_Dungeons_WithoutKey = true,
                    CreateKey = false, //True = create a key, False = Bypass Result
                    Enable_Dungeons_ObjectiveReveal = true,
                    Enable_Monolith_Stability = true,
                    Enable_Monolith_Overide_Max_Stability = false,
                    Max_Stability = 100,
                    Enable_Monolith_EnnemiesDefeat_OnStart = true,
                    Monolith_EnnemiesDefeat_OnStart = 500f,
                    Enable_Monolith_ObjectiveReveal = true,
                    Enable_Monolith_Complete_Objective = false,
                    Enable_Monolith_NoDie = true
                },
                character = new Data.Character_Structure
                {
                    Enable_manaCost = true,
                    Enable_channel_cost = true,
                    Enable_noManaRegenWhileChanneling = true,
                    Enable_stopWhenOutOfMana = true,
                    Enable_RemoveCooldown = true,
                    Enable_number_of_unlocked_slots = false,
                    number_of_unlocked_slots = 5,
                    Enable_passiveTree_pointsEarnt = false,
                    passiveTree_pointsEarnt = 65535,
                    Enable_skilltree_level = false,
                    skilltree_level = 255,
                    Enable_attack_rate = true,
                    attack_rate = 255f,
                    Enable_leach_rate = true,
                    leach_rate = 255f,
                    Enable_Cosmetic_Btn = true
                },
                affixs = new Data.Affixs_Structure
                {
                    Enable_Affixs_Multiplier = true,
                    Affixs_Multiplier = 10
                }
            };

            return result;
        }
    }
}

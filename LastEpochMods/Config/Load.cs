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
                try
                {
                    Data.mods_config = JsonConvert.DeserializeObject<Data.Mods_Structure>(File.ReadAllText(Data.path + Data.filename));
                    Main.logger_instance.Msg("Config Loaded");
                }
                catch
                {
                    Data.mods_config = DefaultConfig();
                    Save.Mods();
                    Main.logger_instance.Msg("Config Outdated : Default Values");
                }                
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
                    AutoPickup_Pots = false,
                    Hide_materials_notifications = true
                },
                items = new Data.Items_Structure
                {
                    Enable_increase_equipment_droprate = false,
                    increase_equipment_droprate = 255f,
                    Enable_increase_equipmentshards_droprate = false,
                    increase_equipmentshards_droprate = 255f,
                    Enable_increase_uniques_droprate = false,
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
                    Enable_SealValue = true,
                    Roll_SealValue = 255,
                    Enable_SealTier = true,
                    Roll_SealTier = 7,
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
                    Remove_SubClassReq = true,
                    Remove_set_req = true,
                    Enable_pickup_range = true,
                    Enable_nb_affixes = true,
                    nb_affixes = 4,
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
                    Enable_Monolith_Stability = false,
                    Enable_Monolith_Overide_Max_Stability = false,
                    Max_Stability = 100,
                    Enable_Monolith_EnnemiesDefeat_OnStart = false,
                    Monolith_EnnemiesDefeat_OnStart = 255f,
                    Enable_Monolith_ObjectiveReveal = false,
                    Enable_Monolith_Complete_Objective = false,
                    Enable_Monolith_NoDie = false,
                    Enable_Monolith_EnemyDensity = false,
                    Monolith_EnemyDensity = 10f,
                    Remove_Fog_Of_War = true
                },
                character = new Data.Character_Structure
                {
                    skills = new Data.Skills_structure
                    {
                        Enable_manaCost = false,
                        Enable_channel_cost = false,
                        Enable_noManaRegenWhileChanneling = false,
                        Enable_stopWhenOutOfMana = false,
                        Enable_RemoveCooldown = false,
                        Enable_All = false,
                        Movements = new Data.Movement_Skills_Structure
                        {
                            Enable_ImmuneDuringMovement = false,
                            Enable_NoTarget = false,
                            Disable_SimplePath = false
                        },
                        HealCost = new Data.Skills_Heal_Cost_Structure
                        {
                            Enable_Transplant = false,
                            Enable_MarrowShards = false,
                            Enable_ReaperForm = false
                        }
                    },
                    skilltree = new Data.SkillTree_structure
                    {
                        Enable_skilltree_level = false,
                        skilltree_level = 255,
                        Disable_node_requirement = true
                    },
                    passivetree = new Data.PassiveTree_structure
                    {
                        Enable_passiveTree_pointsEarnt = false,
                        passiveTree_pointsEarnt = 255
                    },
                    slots = new Data.Slots_structure
                    {
                        Enable_number_of_unlocked_slots = false,
                        number_of_unlocked_slots = 5
                    },
                    companions = new Data.Companion_structure
                    {
                        Enable_companion_limit = false,
                        companion_limit = 255,
                        wolf = new Data.Wolf_structure
                        {
                            Enable_summon_max = true,
                            Enable_override_limit = true,
                            summon_limit = 50
                        },
                        scorpion = new Data.Scorpion_structure
                        {
                            Enable_baby_quantity = true,
                            baby_quantity = 50
                        }
                    },
                    minions = new Data.Minion_structure
                    {
                        skeleton = new Data.Skeleton_structure
                        {
                            Enable_additionalSkeletonsFromPassives = false,
                            additionalSkeletonsFromPassives = 50,
                            Enable_additionalSkeletonsFromSkillTree = false,
                            additionalSkeletonsFromSkillTree = 50,
                            Enable_additionalSkeletonsPerCast = false,
                            additionalSkeletonsPerCast = 50,
                            Enable_chanceToResummonOnDeath = false,
                            chanceToResummonOnDeath = 255,
                            Enable_forceArcher = false,
                            Enable_forceBrawler = false,
                            Enable_forceRogue = false,
                            Enable_forceWarrior = false
                        },
                        wraith = new Data.Wraith_structure
                        {
                            Enable_additionalMaxWraiths = false,
                            additionalMaxWraiths = 50,
                            Enable_delayedWraiths = false,
                            delayedWraiths = 50,
                            Enable_limitedTo2Wraiths = false,
                            Enable_wraithsDoNotDecay = false,
                            Enable_increasedCastSpeed = false,
                            increasedCastSpeed = 255
                        },
                        mage = new Data.Mage_structure
                        {
                            Enable_additionalSkeletonsFromItems = false,
                            additionalSkeletonsFromItems = 50,
                            Enable_additionalSkeletonsFromPassives = false,
                            additionalSkeletonsFromPassives = 50,
                            Enable_additionalSkeletonsFromSkillTree = false,
                            additionalSkeletonsFromSkillTree = 50,
                            Enable_additionalSkeletonsPerCast = false,
                            additionalSkeletonsPerCast = 50,
                            Enable_onlySummonOneMage = false,
                            Enable_singleSummon = false,
                            Enable_forceCryomancer = false,
                            Enable_forceDeathKnight = false,
                            Enable_forcePyromancer = false,
                            Enable_chanceForTwoExtraProjectiles = false,
                            chanceForTwoExtraProjectiles = 255,
                            Enable_doubleProjectiles = false
                        },
                        bone_golem = new Data.BoneGolem_structure
                        {    
                            Enable_addedGolemsPer4Skeletons = false,
                            addedGolemsPer4Skeletons = 10,
                            Enable_twins = false,
                            Enable_selfResurrectChance = false,
                            selfResurrectChance = 255,
                            Enable_increasedFireAuraArea = false,
                            increasedFireAuraArea = 5,
                            Enable_increasedMoveSpeed = false,
                            increasedMoveSpeed = 5,
                            Enable_hasSlamAttack = false,
                            Enable_undeadArmorAura = false,
                            undeadArmorAura = 255,
                            Enable_undeadMovespeedAura = false,
                            undeadMovespeedAura = 255                            
                        },
                        volatile_zombie = new Data.VolatileZombie_structure
                        {
                            Enable_chanceToCastFromMinionDeath = false,
                            chanceToCastFromMinionDeath = 255,
                            Enable_chanceToCastInfernalShadeOnDeath = false,
                            chanceToCastInfernalShadeOnDeath = 255,
                            Enable_chanceToCastMarrowShardsOnDeath = false,
                            chanceToCastMarrowShardsOnDeath = 255
                        },
                        dread_shade = new Data.DreadShade_structure
                        {
                            Enable_Duration = false,
                            Duration = 255,
                            Enable_DisableLimit = false,
                            Enable_DisableHealthDrain = false,
                            Enable_Max = false,
                            max = 255,
                            Enable_ReduceDecay = false,
                            decay = 255,
                            Enable_Radius = false,
                            radius = 5,
                        }
                    },
                    characterstats = new Data.CharacterStats_structure
                    {
                        Enable_attack_rate = false,
                        attack_rate = 255f,
                        Enable_leach_rate = false,
                        leach_rate = 255f,
                        Enable_GodMode = false,
                    },
                    cosmetic = new Data.Cosmetic_structure
                    {
                        Enable_Cosmetic_Btn = true
                    }
                },                
                craft = new Data.Crafting_structure
                {
                    no_cost = true,
                    only_crit = false,
                    override_affix_roll = false,
                    affix_roll = 255
                }
            };

            return result;
        }
    }
}

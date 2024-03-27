using Newtonsoft.Json;
using System.IO;

namespace LastEpochMods.Managers
{
    public class Save_Manager
    {
        public static string path = Directory.GetCurrentDirectory() + @"\Mods\LastEpochMods\";
        public static string filename = "Config.json";

        public class Data
        {
            public static Mods_Structure UserData = new Mods_Structure();
            public static Mods_Structure UserData_duplicate = new Mods_Structure();
                      
            public struct Mods_Structure
            {
                public KeyBinds KeyBinds;
                public CharacterSelectectionMenu CharacterSelectectionMenu;
                public Character Character;
                public Items Items;
                public Scene Scene;
                public Skills Skills;
            }
            //KeyBinds
            public struct KeyBinds
            {
                public UnityEngine.KeyCode BankStashs;
                public UnityEngine.KeyCode HeadhunterBuffs;
            }
            //CharacterMenu
            public struct CharacterSelectectionMenu
            {
                public bool UniqueSubTypeFromSave;
                public bool Enable_UnlockAllIdols;
            }
            //Character
            public struct Character
            {
                public BaseStats BaseStats;
                public Cheats Cheats;
                public PermanentBuffs PermanentBuffs;
                public Experience Experience;
            }
            public struct BaseStats
            {
                public bool Enable_baseStrength;
                public int baseStrength;
                public bool Enable_baseIntelligence;
                public int baseIntelligence;
                public bool Enable_baseVitality;
                public int baseVitality;
                public bool Enable_baseDexterity;
                public int baseDexterity;
                public bool Enable_baseAttunement;
                public int baseAttunement;
            }
            public struct Cheats
            {
                public bool Enable_leech_rate;
                public float leech_rate;
                public bool Enable_AutoPot;
                public float autoPot;
                public bool Enable_GodMode;
                public bool Enable_ChooseBlessingFromBlessingPanel;
                public bool Enable_LowLife;                
            }
            public struct PermanentBuffs
            {
                public bool Enable_MoveSpeed_Buff;
                public float MoveSpeed_Buff_Value;
                public bool Enable_CastSpeed_Buff;
                public float CastSpeed_Buff_Value;
                public bool Enable_AttackSpeed_Buff;
                public float AttackSpeed_Buff_Value;
                public bool Enable_Damage_Buff;
                public float Damage_Buff_Value;
                public bool Enable_ManaRegen_Buff;
                public float ManaRegen_Buff_Value;
                public bool Enable_HealthRegen_Buff;
                public float HealthRegen_Buff_Value;
                public bool Enable_CriticalChance_Buff;
                public float CriticalChance_Buff_Value;
                public bool Enable_CriticalMultiplier_Buff;
                public float CriticalMultiplier_Buff_Value;

                public bool Enable_Str_Buff;
                public float Str_Buff_Value;
                public bool Enable_Int_Buff;
                public float Int_Buff_Value;
                public bool Enable_Dex_Buff;
                public float Dex_Buff_Value;
                public bool Enable_Vit_Buff;
                public float Vit_Buff_Value;
                public bool Enable_Att_Buff;
                public float Att_Buff_Value;
            }
            public struct Experience
            {
                public bool Enable_ExperienceMultiplier;
                public long ExperienceMultiplier;
                public bool Enable_AbilityExpMultiplier;
                public long AbilityExpMultiplier;
                public bool Enable_FavorExpMultiplier;
                public long FavorExpMultiplier;
            }
            //Skills
            public struct Skills
            {
                public bool Enable_RemoveManaCost;
                public bool Enable_RemoveChannelCost;
                public bool Enable_NoManaRegenWhileChanneling;
                public bool Enable_StopWhenOutOfMana;
                public bool Enable_RemoveCooldown;
                public bool Enable_AllSkills;
                public bool Disable_NodeRequirement;

                public MovementSkills MovementSkills;
                public SkillTree SkillTree;
                public PassiveTree PassiveTree;
                public Companion Companion;
                public Minions Minions;
            }
            public struct MovementSkills
            {
                public bool Enable_NoTarget;
                public bool Enable_ImmuneDuringMovement;
                public bool Disable_SimplePath;
            }
            public struct SkillTree
            {
                public bool Enable_Slots;
                public byte Slots;
                public bool Enable_Level;
                public byte Level;
            }
            public struct PassiveTree
            {
                public bool Enable_PointsEarnt;
                public ushort PointsEarnt;
            }
            public struct Companion
            {
                public bool Enable_Limit;
                public int Limit;
                public Wolf Wolf;
                public Scorpion Scorpion;
            }
            public struct Wolf
            {
                public bool Enable_SummonMax;
                public bool Enable_SummonLimit;
                public int SummonLimit;
                public bool Enable_StunImmunity;
            }
            public struct Scorpion
            {
                public bool Enable_BabyQuantity;
                public int BabyQuantity;
            }
            public struct Minions
            {
                public Skeletons Skeletons;
                public Wraiths Wraiths;
                public Mages Mages;                
                public BoneGolems BoneGolems;
                public VolatileZombies VolatileZombies;
                public DreadShades DreadShades;
            }
            public struct Skeletons
            {
                public bool Enable_additionalSkeletonsFromPassives;
                public int additionalSkeletonsFromPassives;
                public bool Enable_additionalSkeletonsFromSkillTree;
                public int additionalSkeletonsFromSkillTree;
                public bool Enable_additionalSkeletonsPerCast;
                public int additionalSkeletonsPerCast;
                public bool Enable_chanceToResummonOnDeath;
                public float chanceToResummonOnDeath;
                public bool Enable_forceArcher;
                public bool Enable_forceBrawler;
                public bool Enable_forceRogue;
                public bool Enable_forceWarrior;
            }
            public struct Wraiths
            {
                public bool Enable_additionalMaxWraiths;
                public int additionalMaxWraiths;
                public bool Enable_delayedWraiths;
                public int delayedWraiths;
                public bool Enable_limitedTo2Wraiths;
                public bool Enable_wraithsDoNotDecay;
                public bool Enable_increasedCastSpeed;
                public float increasedCastSpeed;
            }
            public struct Mages
            {
                public bool Enable_additionalSkeletonsFromItems;
                public int additionalSkeletonsFromItems;
                public bool Enable_additionalSkeletonsFromPassives;
                public int additionalSkeletonsFromPassives;
                public bool Enable_additionalSkeletonsFromSkillTree;
                public int additionalSkeletonsFromSkillTree;
                public bool Enable_additionalSkeletonsPerCast;
                public int additionalSkeletonsPerCast;
                public bool Enable_onlySummonOneMage;
                public bool Enable_singleSummon;
                public bool Enable_forceCryomancer;
                public bool Enable_forceDeathKnight;
                public bool Enable_forcePyromancer;
                public bool Enable_doubleProjectiles;
                public bool Enable_chanceForTwoExtraProjectiles;
                public float chanceForTwoExtraProjectiles;
            }
            public struct BoneGolems
            {
                public bool Enable_addedGolemsPer4Skeletons;
                public int addedGolemsPer4Skeletons;
                public bool Enable_twins;
                public bool Enable_hasSlamAttack;
                public bool Enable_selfResurrectChance;
                public float selfResurrectChance;
                public bool Enable_increasedFireAuraArea;
                public float increasedFireAuraArea;
                public bool Enable_increasedMoveSpeed;
                public float increasedMoveSpeed;
                public bool Enable_undeadArmorAura;
                public float undeadArmorAura;
                public bool Enable_undeadMovespeedAura;
                public float undeadMovespeedAura;
            }
            public struct VolatileZombies
            {
                public bool Enable_chanceToCastFromMinionDeath;
                public float chanceToCastFromMinionDeath;
                public bool Enable_chanceToCastInfernalShadeOnDeath;
                public float chanceToCastInfernalShadeOnDeath;
                public bool Enable_chanceToCastMarrowShardsOnDeath;
                public float chanceToCastMarrowShardsOnDeath;
            }
            public struct DreadShades
            {
                public bool Enable_Duration;
                public float Duration;
                public bool Enable_DisableLimit;
                public bool Enable_DisableHealthDrain;
                public bool Enable_Max;
                public int max;
                public bool Enable_ReduceDecay;
                public float decay;
                public bool Enable_Radius;
                public float radius;
            }
            //Items
            public struct Items
            {
                public AutoPickup AutoPickup;
                public Craft Craft;
                public DropRate DropRate;
                public ItemData ItemData;
                public Headhunter Headhunter;
                public RemoveReq RemoveReq;
                public DropNotification DropNotification;
                public Pickup Pickup;
            }
            public struct AutoPickup
            {
                public bool AutoPickup_Key;
                //public bool AutoPickup_UniqueAndSet;                
                public bool AutoPickup_Materials;
                public bool AutoStore_Materials;
                public bool AutoStore_Materials_WithTimer;
                public bool AutoStore_Materials_WhenOpeningInventory;
                public bool AutoPickup_Pots;
                public bool AutoPickup_XpTome;
                public bool AutoPickup_Gold;
                public bool AutoPickup_Filter;
            }
            public struct Pickup
            {
                public bool Enable_RangePickup;
                public bool RemoveItemNotInFilter;
            }
            public struct DropNotification
            {
                public bool Hide_materials_notifications;
            }
            public struct RemoveReq
            {
                public bool Remove_LevelReq;
                public bool Remove_ClassReq;
                public bool Remove_SubClassReq;
                public bool Remove_set_req;
                public bool Enable_AllAffixsInAllSlots;
            }
            public struct Craft_Implicits
            {
                public bool Enable_Implicit;
                public byte Implicit;
            }
            public struct Craft_Affixs
            {
                public bool Enable_Affix_Tier;
                public byte Tier;
                public bool Enable_Affix_Value;
                public byte Value;
            }
            public struct Craft
            {
                public Craft_Implicits[] Implicits;
                public Craft_Affixs[] Affix;
                public bool Enable_CritChance;
                public float CritChance;
                public bool Enable_LuckyRollChance;
                public float LuckyRollChance;
                public bool NoForgingPotentialCost;
                public bool DontChekCapability;
                public bool Enable_AddForginpotencial;
                public byte AddForginpotencial;
            }
            public struct Headhunter
            {
                public bool enable;
                public int Min_Generated;
                public int Max_Generated;
                public float BuffDuration;
                public float addvalue;
                public float increasedvalue;
                public bool random;
                public bool weaverwill;
                public bool showui;
                public bool base_item_cannotDrop;
                public bool unique_item_canDropRandomly;
            }
            public struct ItemData
            {
                public bool ForceUnique;
                public bool ForceSet;
                public bool ForceLegendary;
                //public bool Enable_Rarity;
                //public byte Roll_Rarity;
                public bool Enable_Implicit;
                public byte Roll_Implicit;
                public bool Enable_ForgingPotencial;
                public int Roll_ForgingPotencial;
                public bool Force_Seal;
                public bool Enable_SealValue;
                public byte Roll_SealValue;
                public bool Enable_SealTier;
                public byte Roll_SealTier;
                //public bool Enable_Min_Affixs;
                public int Min_affixs;
                //public bool Enable_Max_Affixs;
                public int Max_affixs;
                public bool Enable_AffixsValue;
                public byte Roll_AffixValue;
                public bool Enable_AffixsTier;
                public byte Roll_AffixTier;
                public bool Enable_UniqueMod;
                public byte Roll_UniqueMod;
                public bool Enable_LegendayPotencial;
                public int Roll_Legendary_Potencial;
                public bool Enable_WeaverWill;
                public int Roll_Weaver_Will;
            }
            public struct DropRate
            {
                public bool Enable_IncreaseEquipment;
                public float IncreaseEquipment;
                public bool Enable_IncreaseShards;
                public float IncreaseShards;
                public bool Enable_IncreaseUniques;
                public float IncreaseUniques;
            }
            //Scenes
            public struct Scene
            {
                public Scene_Options Scene_Options;
                public Camera Camera;
                public Minimap Minimap;
                public Dungeons Dungeons;
                public Monoliths Monoliths;
            }
            public struct Scene_Options
            {
                public bool Enable_Scenes_Density_Multiplier;
                public float Scenes_Density_Multiplier;
                public bool Enable_Mobs_ItemsMultiplier;
                public float Mobs_ItemsMultiplier;
                public bool Enable_Mobs_ItemsDropChance;
                public float Mobs_ItemsDropChance;
                public bool Enable_Mobs_GoldMultiplier;
                public float Mobs_GoldMultiplier;
                public bool Enable_Mobs_GoldDropChance;
                public float Mobs_GoldDropChance;
                public bool Enable_Waypoints_Unlock;
            }
            public struct Camera
            {
                public bool Enable_OnLoad;
                public float ZoomMin;
                public float ZoomPerScroll;
                public float ZoomSpeed;
                public float Rotation;
                public float OffsetMin;
                public float OffsetMax;
                public float AngleMin;
                public float AngleMax;
            }
            public struct Minimap
            {
                public bool Enable_MaxZoomOut;
                public bool Remove_FogOfWar;
            }
            public struct Dungeons
            {
                public bool EnteringWithoutKey;
            }
            public struct Monoliths
            {
                public bool Enable_MaxStability;
                public int MaxStability;
                public bool Enable_MaxStability_OnStabilityChanged;
                public bool Enable_MaxStability_OnStart;
                public bool Enable_EnemyDensity;
                public float EnemyDensity;                
                public bool Enable_EnnemiesDefeat;
                public float EnnemiesDefeat;
                public bool Enable_UnlockSlots;
                public int UnlockSlots;
                public bool Enable_ObjectiveReveal;
                public bool Enable_Complete_Objective;                
                public bool Enable_NoLostWhenDie;
            }
        }
        public class Load
        {
            public static void Update()
            {
                if (!Initialized) { Init(); }
            }

            public static bool Initialized = false;
            public static async void Init()
            {
                if (!Initialized)
                {
                    Main.logger_instance.Msg("Initialize Save_Manager");
                    Initialized = true;
                    await System.Threading.Tasks.Task.Run(() => LoadMods());
                }
            }
            private static void LoadMods()
            {
                bool error = false;
                if (File.Exists(path + filename))
                {
                    try
                    {
                        Data.UserData = JsonConvert.DeserializeObject<Data.Mods_Structure>(File.ReadAllText(path + filename));
                        CheckErrors();
                    }
                    catch { error = true; }
                }
                else { error = true; }
                if (error)
                {
                    Data.UserData = DefaultConfig();
                    Save.Mods();
                }                
                Data.UserData_duplicate = Data.UserData; //Use to check for data changed
            }
            private static Data.Mods_Structure DefaultConfig()
            {
                Main.logger_instance.Msg("Default Config Loaded");

                var craft_default_implicit = new Data.Craft_Implicits
                {
                    Enable_Implicit = false,
                    Implicit = 255
                };
                var craft_default_affix = new Data.Craft_Affixs
                {
                    Enable_Affix_Tier = false,
                    Tier = 7,
                    Enable_Affix_Value = false,
                    Value = 255,
                };

                Data.Mods_Structure result = new Data.Mods_Structure
                {
                    KeyBinds =
                    {
                        BankStashs = UnityEngine.KeyCode.F3,
                        HeadhunterBuffs = UnityEngine.KeyCode.F2
                    },
                    CharacterSelectectionMenu =
                    {
                        UniqueSubTypeFromSave = false,
                        Enable_UnlockAllIdols = false
                    },
                    Character =
                    {
                        BaseStats =
                        {
                            Enable_baseAttunement = false,
                            baseAttunement = 0,
                            Enable_baseDexterity = false,
                            baseDexterity = 0,
                            Enable_baseIntelligence = false,
                            baseIntelligence = 0,
                            Enable_baseStrength = false,
                            baseStrength = 0,
                            Enable_baseVitality = false,
                            baseVitality = 0
                        },
                        Cheats =
                        {
                            //Enable_attack_rate = false,
                            //attack_rate = 0,
                            Enable_leech_rate = false,
                            leech_rate = 0,
                            Enable_AutoPot = false,
                            autoPot = 0f,
                            Enable_GodMode = false,
                            Enable_ChooseBlessingFromBlessingPanel = false,
                            Enable_LowLife = false
                        },
                        PermanentBuffs =
                        {
                            Enable_MoveSpeed_Buff = false,
                            MoveSpeed_Buff_Value = 1f,
                            Enable_CastSpeed_Buff = false,
                            CastSpeed_Buff_Value = 1f,
                            Enable_AttackSpeed_Buff = false,
                            AttackSpeed_Buff_Value = 1f,
                            Enable_Damage_Buff = false,
                            Damage_Buff_Value = 1f,
                            Enable_HealthRegen_Buff = false,
                            HealthRegen_Buff_Value = 1f,
                            Enable_ManaRegen_Buff = false,
                            ManaRegen_Buff_Value = 1f,
                            Enable_CriticalChance_Buff = false,
                            CriticalChance_Buff_Value = 0f,
                            Enable_CriticalMultiplier_Buff = false,
                            CriticalMultiplier_Buff_Value = 0f,
                            Enable_Str_Buff = false,
                            Str_Buff_Value = 0f,
                            Enable_Int_Buff = false,
                            Int_Buff_Value = 0f,
                            Enable_Vit_Buff = false,
                            Vit_Buff_Value = 0f,
                            Enable_Dex_Buff = false,
                            Dex_Buff_Value = 0f,
                            Enable_Att_Buff = false,
                            Att_Buff_Value = 0f
                        },
                        Experience =
                        {
                            Enable_ExperienceMultiplier = false,
                            ExperienceMultiplier = 1,
                            Enable_AbilityExpMultiplier = false,
                            AbilityExpMultiplier = 1,
                            Enable_FavorExpMultiplier = false,
                            FavorExpMultiplier = 1
                        }
                    },
                    Items =
                    {
                        Craft =
                        {
                            Implicits = new Data.Craft_Implicits[3]
                            {
                                craft_default_implicit,
                                craft_default_implicit,
                                craft_default_implicit
                            },
                            Affix = new Data.Craft_Affixs[4]
                            {
                                craft_default_affix,
                                craft_default_affix,
                                craft_default_affix,
                                craft_default_affix
                            },

                            NoForgingPotentialCost = false,
                            DontChekCapability = false,
                            Enable_AddForginpotencial = false,
                            AddForginpotencial = 255
                        },
                        AutoPickup =
                        {
                            AutoPickup_Materials = false,
                            AutoStore_Materials_WithTimer = false,
                            AutoStore_Materials_WhenOpeningInventory = false,
                            AutoStore_Materials = false,
                            AutoPickup_Gold = false,
                            AutoPickup_Key = false,
                            AutoPickup_Pots = false,
                            //AutoPickup_UniqueAndSet = false,
                            AutoPickup_XpTome = false,
                            AutoPickup_Filter = false                            
                        },
                        Pickup =
                        {
                            Enable_RangePickup = false,
                            RemoveItemNotInFilter = false
                        },
                        DropRate =
                        {
                            Enable_IncreaseEquipment = false,
                            IncreaseEquipment = 0,
                            Enable_IncreaseShards = false,
                            IncreaseShards = 0,
                            Enable_IncreaseUniques = false,
                            IncreaseUniques = 0
                        },
                        ItemData =
                        {
                            ForceUnique = false,
                            ForceSet = false,
                            ForceLegendary = false,
                            //Enable_Rarity = false,
                            //Roll_Rarity = 7,
                            Enable_Implicit = false,
                            Roll_Implicit = 255,
                            Enable_ForgingPotencial = false,
                            Roll_ForgingPotencial = 255,
                            Enable_AffixsTier = false,
                            Roll_AffixTier = 7,
                            Enable_AffixsValue = false,
                            Roll_AffixValue = 255,
                            Force_Seal = false,
                            Enable_SealTier = false,
                            Roll_SealTier = 5,
                            Enable_SealValue = false,
                            Roll_SealValue = 255,
                            Enable_UniqueMod = false,
                            Roll_UniqueMod = 255,
                            Enable_LegendayPotencial = false,
                            Roll_Legendary_Potencial = 4,
                            Enable_WeaverWill = false,
                            Roll_Weaver_Will = 28,
                            Min_affixs = 0,
                            Max_affixs = 4
                        },
                        Headhunter =
                        {
                            enable = true,
                            Min_Generated = 1,
                            Max_Generated = 5,
                            BuffDuration = 20f,
                            addvalue = 1f,
                            increasedvalue = 1f,
                            random = true,
                            weaverwill = false,
                            showui = false,
                            base_item_cannotDrop = true,
                            unique_item_canDropRandomly = true
                        },
                        RemoveReq =
                        {
                            Remove_ClassReq = false,
                            Remove_LevelReq = false,
                            Remove_set_req = false,
                            Remove_SubClassReq = false,
                            Enable_AllAffixsInAllSlots = false
                        },
                        DropNotification =
                        {
                            Hide_materials_notifications = false
                        }
                    },
                    Scene =
                    {
                        Scene_Options =
                        {
                            Enable_Scenes_Density_Multiplier = false,
                            Scenes_Density_Multiplier = 0,
                            Enable_Mobs_ItemsMultiplier = false,
                            Mobs_ItemsMultiplier = 0,
                            Enable_Mobs_ItemsDropChance = false,
                            Mobs_ItemsDropChance = 255,
                            Enable_Mobs_GoldMultiplier = false,
                            Mobs_GoldMultiplier = 0,
                            Enable_Mobs_GoldDropChance = false,
                            Mobs_GoldDropChance = 255,
                            Enable_Waypoints_Unlock = false
                        },
                        Camera =
                        {
                            Enable_OnLoad = false,
                            ZoomMin = -7,
                            ZoomPerScroll = 0.15f,
                            ZoomSpeed = 2.5f,
                            Rotation = 95f,
                            OffsetMin = -1f,
                            OffsetMax = 0.31f,
                            AngleMin = 35,
                            AngleMax = 49
                        },
                        Minimap =
                        {
                            Enable_MaxZoomOut = false,
                            Remove_FogOfWar = false
                        },
                        Dungeons =
                        {
                            EnteringWithoutKey = false
                        },
                        Monoliths =
                        {
                            Enable_MaxStability = false,
                            MaxStability = 0,
                            Enable_MaxStability_OnStabilityChanged = false,
                            Enable_MaxStability_OnStart = false,
                            Enable_EnemyDensity = false,
                            EnemyDensity = 0,
                            Enable_EnnemiesDefeat = false,
                            EnnemiesDefeat = 0,
                            Enable_ObjectiveReveal = false,
                            Enable_Complete_Objective = false,
                            Enable_UnlockSlots = false,
                            UnlockSlots = 3,
                            Enable_NoLostWhenDie = false
                        }
                    },
                    Skills =
                    {
                        Enable_RemoveManaCost = false,
                        Enable_RemoveChannelCost = false,
                        Enable_NoManaRegenWhileChanneling = false,
                        Enable_StopWhenOutOfMana = false,
                        Enable_RemoveCooldown = false,
                        Enable_AllSkills = false,
                        Disable_NodeRequirement = false,
                        MovementSkills =
                        {
                            Enable_ImmuneDuringMovement = false,
                            Enable_NoTarget = false,
                            Disable_SimplePath = false
                        },
                        SkillTree =
                        {
                            Enable_Slots = false,
                            Slots = 5,
                            Enable_Level = false,
                            Level = 0
                        },
                        PassiveTree =
                        {
                            Enable_PointsEarnt = false,
                            PointsEarnt = 0
                        },
                        Companion =
                        {
                            Enable_Limit = false,
                            Limit = 255,
                            Wolf =
                            {
                                Enable_SummonMax = false,
                                Enable_SummonLimit = false,
                                SummonLimit = 255,
                                Enable_StunImmunity = false
                            },
                            Scorpion =
                            {
                                Enable_BabyQuantity = false,
                                BabyQuantity = 255
                            }
                        },
                        Minions =
                        {
                            Skeletons =
                            {
                                Enable_additionalSkeletonsFromPassives = false,
                                additionalSkeletonsFromPassives = 0,
                                Enable_additionalSkeletonsFromSkillTree = false,
                                additionalSkeletonsFromSkillTree = 0,
                                Enable_additionalSkeletonsPerCast = false,
                                additionalSkeletonsPerCast = 0,
                                Enable_chanceToResummonOnDeath = false,
                                chanceToResummonOnDeath = 0,
                                Enable_forceArcher = false,
                                Enable_forceBrawler = false,
                                Enable_forceRogue = false,
                                Enable_forceWarrior = false
                            },
                            Wraiths =
                            {
                                Enable_additionalMaxWraiths = false,
                                additionalMaxWraiths = 0,
                                Enable_delayedWraiths = false,
                                delayedWraiths = 0,
                                Enable_limitedTo2Wraiths = false,
                                Enable_wraithsDoNotDecay = false,
                                Enable_increasedCastSpeed = false,
                                increasedCastSpeed = 0
                            },
                            Mages =
                            {
                                Enable_additionalSkeletonsFromItems = false,
                                additionalSkeletonsFromItems = 0,
                                Enable_additionalSkeletonsFromPassives = false,
                                additionalSkeletonsFromPassives = 0,
                                Enable_additionalSkeletonsFromSkillTree = false,
                                additionalSkeletonsFromSkillTree = 0,
                                Enable_additionalSkeletonsPerCast = false,
                                additionalSkeletonsPerCast = 0,
                                Enable_onlySummonOneMage = false,
                                Enable_singleSummon = false,
                                Enable_forceCryomancer = false,
                                Enable_forceDeathKnight = false,
                                Enable_forcePyromancer = false,
                                Enable_doubleProjectiles = false,
                                Enable_chanceForTwoExtraProjectiles = false,
                                chanceForTwoExtraProjectiles = 0                                
                            },
                            BoneGolems =
                            {
                                Enable_addedGolemsPer4Skeletons = false,
                                addedGolemsPer4Skeletons = 0,
                                Enable_hasSlamAttack = false,                                
                                Enable_increasedFireAuraArea = false,
                                increasedFireAuraArea = 0,
                                Enable_increasedMoveSpeed = false,
                                increasedMoveSpeed = 0,
                                Enable_selfResurrectChance = false,
                                selfResurrectChance = 0,
                                Enable_twins = false,                                
                                Enable_undeadArmorAura = false,
                                undeadArmorAura = 0,
                                Enable_undeadMovespeedAura = false,
                                undeadMovespeedAura = 0
                            },
                            VolatileZombies =
                            {
                                Enable_chanceToCastFromMinionDeath = false,
                                chanceToCastFromMinionDeath = 0,
                                Enable_chanceToCastInfernalShadeOnDeath = false,
                                chanceToCastInfernalShadeOnDeath = 0,
                                Enable_chanceToCastMarrowShardsOnDeath = false,
                                chanceToCastMarrowShardsOnDeath = 0
                            },
                            DreadShades =
                            {
                                Enable_DisableHealthDrain = false,
                                Enable_DisableLimit = false,
                                Enable_Duration = false,
                                Duration = 0,
                                Enable_Max = false,
                                max = 0,
                                Enable_Radius = false,
                                radius = 0,
                                Enable_ReduceDecay = false,
                                decay = 0
                            }
                        }
                    }
                };

                return result;
            }
            private static void CheckErrors()
            {
                bool data_changed = false;
                if (Data.UserData.Items.Craft.Implicits == null)
                {
                    var craft_default_implicit = new Data.Craft_Implicits
                    {
                        Enable_Implicit = false,
                        Implicit = 255
                    };
                    Data.UserData.Items.Craft.Implicits = new Data.Craft_Implicits[3]
                    {
                        craft_default_implicit,
                        craft_default_implicit,
                        craft_default_implicit
                    };
                    data_changed = true;
                }
                if (Data.UserData.Items.Craft.Affix == null)
                {
                    var craft_default_affix = new Data.Craft_Affixs
                    {
                        Enable_Affix_Tier = false,
                        Tier = 0,
                        Enable_Affix_Value = false,
                        Value = 255,
                    };
                    Data.UserData.Items.Craft.Affix = new Data.Craft_Affixs[4]
                    {
                        craft_default_affix,
                        craft_default_affix,
                        craft_default_affix,
                        craft_default_affix
                    };
                    data_changed = true;
                }
                if (data_changed) { Save.Mods(); }
            }
        }
        public class Save
        {
            public static void Mods()
            {
                string jsonString = JsonConvert.SerializeObject(Data.UserData, Formatting.Indented);
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                if (File.Exists(path + filename)) { File.Delete(path + filename); }
                File.WriteAllText(path + filename, jsonString);
            }
        }
    }
}

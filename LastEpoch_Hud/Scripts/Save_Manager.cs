using MelonLoader;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

namespace LastEpoch_Hud.Scripts
{
    [RegisterTypeInIl2Cpp]
    public class Save_Manager : MonoBehaviour
    {
        public Save_Manager(System.IntPtr ptr) : base(ptr) { }
        public static Save_Manager instance { get; private set; }

        public string path = Directory.GetCurrentDirectory() + @"\Mods\" + Main.mod_name + @"\";
        string filename = "Save.json";
        public Data.Mods_Structure data = new Data.Mods_Structure();
        public Data.Mods_Structure data_duplicate = new Data.Mods_Structure();
        public bool initialized = false;

        void Awake()
        {
            instance = this;  
        }
        async void Start()
        {
            await System.Threading.Tasks.Task.Run(() => Load());
        }
        void Update()
        {
            if (initialized) { Check_DataChanged(); }
        }
        
        void Load()
        {
            bool error = false;
            if (File.Exists(path + filename))
            {
                Main.logger_instance?.Msg("Save Manager : Loading file : " + path + filename);                
                try { data = JsonConvert.DeserializeObject<Data.Mods_Structure>(File.ReadAllText(path + filename)); }
                catch
                {
                    Main.logger_instance?.Warning("Save Manager : Error when deserializing the file");
                    error = true;
                }
            }
            else { error = true; }
            if (error)
            {
                Main.logger_instance.Msg("Save Manager : Generate default config");
                data = Get_DefaultConfig();
                Save();
            }
            Check_Update();
            data_duplicate = data; //Use to check for data changed
            Main.logger_instance.Msg("Save Manager : Data initialized");
            initialized = true;
        }
        Data.Mods_Structure Get_DefaultConfig()
        {
            Data.Mods_Structure result = new Data.Mods_Structure
            {
                ModVersion = Main.mod_version,
                KeyBinds =
                {
                    SpawnMysteriousRift = KeyCode.F1,
                    SpawnRiftBeast = KeyCode.F2,
                    BankStashs = KeyCode.F3,
                    SummonBeast = KeyCode.F4
                },
                modsNotInHud =
                {
                    Shrines_Unlimited = false,                      //Unlimited Shrine Click
                    Shrines_Override = false,                       //Change Shrine with another shrines
                    Shrines_Override_id = 0,                        //see Scripts.Mods.Shrines.Shrines_Override.cs
                    Craft_MaxTier = false,                          //Allow craft to T8
                    //Craft_Corrupted = false                         //Allow craft corrupted item
                },
                Login =
                {
                    Enable_AutoLoginOffline = true
                },
                Character =
                {
                    Cheats =
                    {
                        Enable_GodMode = false,
                        Enable_LowLife = false,
                        Enable_CanChooseBlessing = false,
                        Enable_UnlockAllIdolsSlots = false,
                        Enable_AutoPot = false,
                        autoPot = 0f,
                        Enable_DensityMultiplier = false,
                        DensityMultiplier = 0f,
                        Enable_ExperienceMultiplier = false,
                        ExperienceMultiplier = 0f,
                        Enable_AbilityMultiplier = false,
                        AbilityMultiplier = 0f,
                        Enable_FavorMultiplier = false,
                        FavorMultiplier = 0f,
                        Enable_ItemDropChance = false,
                        ItemDropChance = 0f,
                        Enable_ItemDropMultiplier = false,
                        ItemDropMultiplier = 0f,
                        Enable_GoldDropChance = false,
                        GoldDropChance = 0f,
                        Enable_GoldDropMultiplier = false,
                        GoldDropMultiplier = 0f,
                        Enable_MemoryAmberMultiplier = false,
                        MemoryAmberMultiplier = 1,
                        Enable_WaypointsUnlock = false,
                        Enable_TwoHandedWithShield = false,
                        Enable_PotionResplenishment = false,
                    },
                    PermanentBuffs =
                    {
                        Enable_Mod = true,
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
                    Visuals =
                    {
                        Primalist = "Primalist",
                        Mage = "Mage",
                        Sentinel = "Sentinel",
                        Acolyte = "Acolyte",
                        Rogue = "Rogue"
                    }
                },
                Factions =
                {
                    TheWoven =
                    {
                        Enable_TreePoints = false,
                        TreePoints = 0,
                        Enable_FreeRespe = false
                    }
                },
                Items =
                {
                    Drop =
                    {
                        Enable_ForceUnique = false,
                        Enable_ForceSet = false,
                        Enable_ForceLegendary = false,

                        Enable_Implicits = false,
                        Implicits_Min = 0,
                        Implicits_Max = 255,
                        Enable_ForginPotencial = false,
                        ForginPotencial_Min = 0,
                        ForginPotencial_Max = 255,
                        Enable_ForceSeal = false,
                        Enable_SealTier = false,
                        SealTier_Min = 0,
                        SealTier_Max = 6,
                        Enable_SealValue = false,
                        SealValue_Min = 0,
                        SealValue_Max = 255,
                        Enable_AffixCount = false,
                        AffixCount_Min = 0,
                        AffixCount_Max = 4,
                        Enable_IdolAffixCount = false,
                        IdolAffixCount_Min = 0,
                        IdolAffixCount_Max = 2,
                        Enable_AffixTiers = false,
                        AffixTiers_Min = 0,
                        AffixTiers_Max = 6,
                        Enable_AffixValues = false,
                        AffixValues_Min = 0,
                        AffixValues_Max = 255,
                        Enable_UniqueMods = false,
                        UniqueMods_Min = 0,
                        UniqueMods_Max = 255,
                        Enable_LegendaryPotencial = false,
                        LegendaryPotencial_Min = 0,
                        LegendaryPotencial_Max = 4,
                        Enable_WeaverWill = false,
                        WeaverWill_Min = 0,
                        WeaverWill_Max = 28
                    },
                    Pickup =
                    {
                        Enable_AutoPickup_Gold = false,
                        Enable_AutoPickup_Keys = false,
                        Enable_AutoPickup_Potions = false,
                        Enable_AutoPickup_XpTome = false,
                        Enable_AutoPickup_FavorTome = false,
                        Enable_AutoPickup_MemoryAmber = false,
                        Enable_AutoPickup_Materials = false,
                        Enable_AutoPickup_WovenEchoes = false,
                        Enable_AutoPickup_FromFilter = false,
                        Enable_AutoStore_OnDrop = false,
                        Enable_AutoStore_OnInventoryOpen = false,
                        Enable_AutoStore_Timer = false,
                        AutoStore_Timer = 10,
                        Enable_AutoSell_FromFilter = false,
                        Enable_AutoShatter_FromFilter = false,
                        Enable_AutoShatter_UseRune = true,
                        AutoShatter_Chance = 100,
                        AutoShatter_Affix_Chance = 100,
                        AutoShatter_Quantity_Chance = 100,
                        Enable_RangePickup = false,
                        Enable_HideMaterialsNotifications = false
                    },
                    Req =
                    {
                        level = false,
                        classe = false,
                        set = false
                    },
                    CraftingSlot =
                    {
                        Enable_Mod = false,
                        Enable_ForginPotencial = false,
                        ForginPotencial = 255,
                        Enable_Implicit_0 = false,
                        Implicit_0 = 255,
                        Enable_Implicit_1 = false,
                        Implicit_1 = 255,
                        Enable_Implicit_2 = false,
                        Implicit_2 = 255,
                        Enable_Seal_Tier = false,
                        Seal_Tier = 255,
                        Enable_Seal_Value = false,
                        Seal_Value = 255,
                        Enable_Affix_0_Tier = false,
                        Affix_0_Tier = 6,
                        Enable_Affix_0_Value = false,
                        Affix_0_Value = 255,
                        Enable_Affix_1_Tier = false,
                        Affix_1_Tier = 6,
                        Enable_Affix_1_Value = false,
                        Affix_1_Value = 255,
                        Enable_Affix_2_Tier = false,
                        Affix_2_Tier = 6,
                        Enable_Affix_2_Value = false,
                        Affix_2_Value = 255,
                        Enable_Affix_3_Tier = false,
                        Affix_3_Tier = 6,
                        Enable_Affix_3_Value = false,
                        Affix_3_Value = 255,
                        Enable_UniqueMod_0 = false,
                        UniqueMod_0 = 255,
                        Enable_UniqueMod_1 = false,
                        UniqueMod_1 = 255,
                        Enable_UniqueMod_2 = false,
                        UniqueMod_2 = 255,
                        Enable_UniqueMod_3 = false,
                        UniqueMod_3 = 255,
                        Enable_UniqueMod_4 = false,
                        UniqueMod_4 = 255,
                        Enable_UniqueMod_5 = false,
                        UniqueMod_5 = 255,
                        Enable_UniqueMod_6 = false,
                        UniqueMod_6 = 255,
                        Enable_UniqueMod_7 = false,
                        UniqueMod_7 = 255,
                        Enable_LegendaryPotencial = false,
                        LegendaryPotencial = 4,
                        Enable_WeaverWill = false,
                        WeaverWill = 28
                    }
                },
                NewItems =
                {
                    Headhunter =
                    {
                        MinGenerated = 1,
                        MaxGenerated = 5,
                        BuffDuration = 20f,
                        Stack = 0f,
                        AddValue = 1f,
                        IncreasedValue = 1f,
                        WeaverWill = false
                    },
                    Mjolner =
                    {
                        MinTriggerChance = 0.3f,
                        MaxTriggerChance = 0.5f,
                        IntRequirement = 95,
                        StrRequirement = 105,
                        WeaverWill = false,
                        SocketedCooldown = 250,
                        SockectedSkill_0 = "Lightning Nova",
                        SockectedSkill_1 = "Elemental Nova",
                        SockectedSkill_2 = "Storm Bolt"
                    },
                    HeraldOfIce =
                    {
                        VFX = "Runemaster 15.2 Explosion",
                        Enable_Radius = false,
                        Radius = 0f,
                        WeaverWill = false
                    },
                    HeraldOfFire =
                    {
                        VFX = "FireballExplosion",
                        Enable_Radius = false,
                        Radius = 0f,
                        WeaverWill = false
                    },
                    HeraldOfThunder =
                    {
                        VFX = "Runemaster 05c3.1 Runebolt Lightning Explosion",
                        Enable_Radius = false,
                        Radius = 0f,
                        WeaverWill = false
                    },
                    HeraldOfAgony =
                    {
                        VFX = "NemesisSoldierPoison 03.1 PoisonExplosion",
                        Enable_Radius = false,
                        Radius = 0f,
                        WeaverWill = false
                    },
                    HeraldOfPurity =
                    {
                        VFX = "RaptorScreech",
                        Enable_Radius = false,
                        Radius = 0f,
                        WeaverWill = false
                    }
                },
                Scenes =
                {
                    Camera =
                    {
                        Enable_Mod = false,
                        Enable_ZoomMinimum = false,
                        ZoomMinimum = -7f,
                        Enable_ZoomPerScroll = false,
                        ZoomPerScroll = 0f,
                        Enable_ZoomSpeed = false,
                        ZoomSpeed = 2f,
                        Enable_DefaultRotation = false,
                        DefaultRotation = 95f,
                        Enable_OffsetMinimum = false,
                        OffsetMinimum = -1f,
                        Enable_OffsetMaximum = false,
                        OffsetMaximum = 0f,
                        Enable_AngleMinimum = false,
                        AngleMinimum = 35f,
                        Enable_AngleMaximum = false,
                        AngleMaximum = 49f,
                        Enable_LoadOnStart = false
                    },
                    Minimap =
                    {
                        Enable_MaxZoomOut = false,
                        Enable_RemoveFogOfWar = false,
                        Enable_ShowAllItems = false,
                        Enable_ShowItemsFromFilter = false,
                        Icons_Scale = 1f
                    },
                    Dungeons =
                    {
                        Enable_EnterWithoutKey = false
                    },
                    Monoliths =
                    {
                        Monoliths_Corruption = 1000,
                        Enable_MaxStability = false,
                        MaxStability = 0,
                        Enable_MobsDensity = false,
                        MobsDensity = 0,
                        Enable_MobsDefeatOnStart = false,
                        MobsDefeatOnStart = 0,
                        Enable_BlessingSlots = false,
                        BlessingSlots = 3,
                        Enable_MaxStabilityOnStart = false,
                        Enable_MaxStabilityOnStabilityChanged = false,
                        Enable_DropShadeGauntlet = false,
                        Enable_ObjectiveReveal = false,
                        Enable_CompleteObjective = false,
                        Enable_NoLostWhenDie = false,
                        Corruptions =
                        {
                            Enable_Monoliths_Corruptions = false,
                            Fall_Of_The_Outcast = 0,
                            The_Stolen_Lance = 0,
                            The_Black_Sun = 0,
                            Blood_Frost_And_Death = 0,
                            Ending_The_Storm = 0,
                            Fall_Of_The_Empire = 0,
                            Reign_Of_Dragon = 0,
                            The_Last_Ruins = 0,
                            Spirits_Of_Fire = 0
                        },
                        Enable_Islands = false
                    },
                    harbringers =
                    {
                        Enable_AltarWithoutKey = false
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
                        Enable_SpecializationSlots = false,
                        Enable_SkillLevel = false,
                        SkillLevel = 0,
                        Enable_PassivePoints = false,
                        PassivePoints = 0,
                        SpecializationSlots = 0,
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
                },
                Summon =
                {
                    Enable_GodMode = false,
                    Enable_Forever = false,
                    Enable_DontCollide = false
                }
            };

            return result;
        }
        void Check_Update()
        {
            bool data_changed = false;
            if (data.ModVersion != Main.mod_version)
            {
                //Update save when mod update here
                //data.KeyBinds.EternityCache_Past = KeyCode.F4;
                //data.KeyBinds.EternityCache_Future = KeyCode.F5;

                data.ModVersion = Main.mod_version;
                data_changed = true;
            }            
            if (data_changed) { Save(); }
        }
        void Check_DataChanged()
        {
            if (!data.Equals(data_duplicate))
            {
                data_duplicate = data;
                Save();
                if (!Mods_Manager.instance.IsNullOrDestroyed()) { Mods_Manager.instance.change = true; } //Will be removed soon (when i have time to do it ^^) 
            }
        }
        public void Save()
        {
            string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            if (File.Exists(path + filename)) { File.Delete(path + filename); }
            File.WriteAllText(path + filename, jsonString);
        }

        public class Data
        {
            public struct Mods_Structure
            {
                public string ModVersion;
                public KeyBinds KeyBinds;
                public ModsNotInHud modsNotInHud;
                public Login Login;
                public Character Character;
                public Factions Factions;
                public Items Items;
                public NewItems NewItems;
                public Scenes Scenes;
                public Skills Skills;
                public Summon Summon;
            }
            
            //KeyBinds
            public struct KeyBinds
            {
                public UnityEngine.KeyCode SpawnMysteriousRift;
                public UnityEngine.KeyCode SpawnRiftBeast;
                public UnityEngine.KeyCode SummonBeast;
                public UnityEngine.KeyCode BankStashs;
            }
            
            //Options not in hud
            public struct ModsNotInHud
            {
                public bool Shrines_Unlimited;
                public bool Shrines_Override;
                public int Shrines_Override_id;
                public bool Craft_MaxTier;
                //public bool Craft_Corrupted;
            }
            
            //Login
            public struct Login
            {
                public bool Enable_AutoLoginOffline;
            }
            
            //Character
            public struct Character
            {
                public Cheats Cheats;
                public PermanentBuffs PermanentBuffs;
                public Visuals Visuals;
            }
            public struct Cheats
            {
                public bool Enable_GodMode;
                public bool Enable_LowLife;
                public bool Enable_CanChooseBlessing;
                public bool Enable_UnlockAllIdolsSlots;                
                public bool Enable_AutoPot;
                public float autoPot;
                public bool Enable_DensityMultiplier;
                public float DensityMultiplier;
                public bool Enable_ExperienceMultiplier;
                public float ExperienceMultiplier;
                public bool Enable_AbilityMultiplier;
                public float AbilityMultiplier;
                public bool Enable_FavorMultiplier;
                public float FavorMultiplier;
                public bool Enable_ItemDropChance;
                public float ItemDropChance;
                public bool Enable_ItemDropMultiplier;
                public float ItemDropMultiplier;
                public bool Enable_GoldDropChance;
                public float GoldDropChance;
                public bool Enable_GoldDropMultiplier;
                public float GoldDropMultiplier;
                public bool Enable_MemoryAmberMultiplier;
                public uint MemoryAmberMultiplier;
                public bool Enable_WaypointsUnlock;
                public bool Enable_TwoHandedWithShield;
                public bool Enable_PotionResplenishment;
            }
            public struct PermanentBuffs
            {
                public bool Enable_Mod;
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
            public struct Visuals
            {
                public string Primalist;
                public string Mage;
                public string Sentinel;
                public string Acolyte;
                public string Rogue;
            }

            //Summon
            public struct Summon
            {
                public bool Enable_GodMode;
                public bool Enable_Forever;
                public bool Enable_DontCollide;
            }
            
            //Items
            public struct Items
            {
                public Drop Drop;
                public Pickup Pickup;
                public Req Req;
                public CraftingSlot CraftingSlot;                
            }
            public struct Drop
            {
                public bool Enable_ForceUnique;
                public bool Enable_ForceSet;
                public bool Enable_ForceLegendary;

                public bool Enable_Implicits;
                public float Implicits_Min;
                public float Implicits_Max;

                public bool Enable_ForginPotencial;
                public float ForginPotencial_Min;
                public float ForginPotencial_Max;

                public bool Enable_ForceSeal;

                public bool Enable_SealTier;
                public float SealTier_Min;
                public float SealTier_Max;

                public bool Enable_SealValue;
                public float SealValue_Min;
                public float SealValue_Max;

                public bool Enable_AffixCount;
                public float AffixCount_Min;
                public float AffixCount_Max;

                public bool Enable_IdolAffixCount;
                public float IdolAffixCount_Min;
                public float IdolAffixCount_Max;

                public bool Enable_AffixTiers;
                public float AffixTiers_Min;
                public float AffixTiers_Max;

                public bool Enable_AffixValues;
                public float AffixValues_Min;
                public float AffixValues_Max;

                public bool Enable_UniqueMods;
                public float UniqueMods_Min;
                public float UniqueMods_Max;

                public bool Enable_LegendaryPotencial;
                public float LegendaryPotencial_Min;
                public float LegendaryPotencial_Max;

                public bool Enable_WeaverWill;
                public float WeaverWill_Min;
                public float WeaverWill_Max;
            }
            public struct Pickup
            {
                public bool Enable_AutoPickup_Gold;
                public bool Enable_AutoPickup_Keys;
                public bool Enable_AutoPickup_Potions;
                public bool Enable_AutoPickup_XpTome;
                public bool Enable_AutoPickup_FavorTome;
                public bool Enable_AutoPickup_MemoryAmber;
                public bool Enable_AutoPickup_Materials;
                public bool Enable_AutoPickup_WovenEchoes;
                public bool Enable_AutoPickup_FromFilter;

                public bool Enable_AutoStore_OnDrop;
                public bool Enable_AutoStore_OnInventoryOpen;
                public bool Enable_AutoStore_Timer;
                public int AutoStore_Timer;

                public bool Enable_AutoSell_FromFilter;

                public bool Enable_AutoShatter_FromFilter;
                public bool Enable_AutoShatter_UseRune;
                public int AutoShatter_Chance;
                public int AutoShatter_Affix_Chance;
                public int AutoShatter_Quantity_Chance;

                public bool Enable_RangePickup;
                public bool Enable_HideMaterialsNotifications;
            }
            public struct Req
            {
                public bool level;
                public bool classe;
                public bool set;
            }
            public struct CraftingSlot
            {
                public bool Enable_Mod;  //added by https://github.com/RolandSolymosi

                public bool Enable_ForginPotencial;
                public float ForginPotencial;

                public bool Enable_Implicit_0;
                public float Implicit_0;
                public bool Enable_Implicit_1;
                public float Implicit_1;
                public bool Enable_Implicit_2;
                public float Implicit_2;

                public bool Enable_Seal_Tier;
                public int Seal_Tier;
                public bool Enable_Seal_Value;
                public float Seal_Value;

                public bool Enable_Affix_0_Tier;
                public int Affix_0_Tier;
                public bool Enable_Affix_0_Value;
                public float Affix_0_Value;

                public bool Enable_Affix_1_Tier;
                public int Affix_1_Tier;
                public bool Enable_Affix_1_Value;
                public float Affix_1_Value;

                public bool Enable_Affix_2_Tier;
                public int Affix_2_Tier;
                public bool Enable_Affix_2_Value;
                public float Affix_2_Value;

                public bool Enable_Affix_3_Tier;
                public int Affix_3_Tier;
                public bool Enable_Affix_3_Value;
                public float Affix_3_Value;
                                
                public bool Enable_UniqueMod_0;
                public float UniqueMod_0;
                public bool Enable_UniqueMod_1;
                public float UniqueMod_1;
                public bool Enable_UniqueMod_2;
                public float UniqueMod_2;
                public bool Enable_UniqueMod_3;
                public float UniqueMod_3;
                public bool Enable_UniqueMod_4;
                public float UniqueMod_4;
                public bool Enable_UniqueMod_5;
                public float UniqueMod_5;
                public bool Enable_UniqueMod_6;
                public float UniqueMod_6;
                public bool Enable_UniqueMod_7;
                public float UniqueMod_7;

                public bool Enable_LegendaryPotencial;
                public int LegendaryPotencial;

                public bool Enable_WeaverWill;
                public int WeaverWill;
            }
            
            //NewItems
            public struct NewItems
            {
                public Headhunter Headhunter;
                public Mjolner Mjolner;
                public HeraldOfIce HeraldOfIce;
                public HeraldOfFire HeraldOfFire;
                public HeraldOfThunder HeraldOfThunder;
                public HeraldOfAgony HeraldOfAgony;
                public HeraldOfPurity HeraldOfPurity;
            }
            public struct Headhunter
            {
                public int MinGenerated;
                public int MaxGenerated;
                public float BuffDuration;
                public float Stack;
                public float AddValue;
                public float IncreasedValue;
                public bool WeaverWill;
            }
            public struct Mjolner
            {
                public float MinTriggerChance;
                public float MaxTriggerChance;
                public int IntRequirement;
                public int StrRequirement;
                public bool WeaverWill;
                public System.Double SocketedCooldown;
                public string SockectedSkill_0;
                public string SockectedSkill_1;
                public string SockectedSkill_2;
            }
            public struct HeraldOfIce
            {
                public string VFX;
                public bool Enable_Radius;
                public float Radius;
                public bool WeaverWill;
            }
            public struct HeraldOfFire
            {
                public string VFX;
                public bool Enable_Radius;
                public float Radius;
                public bool WeaverWill;
            }
            public struct HeraldOfThunder
            {
                public string VFX;
                public bool Enable_Radius;
                public float Radius;
                public bool WeaverWill;
            }
            public struct HeraldOfAgony
            {
                public string VFX;
                public bool Enable_Radius;
                public float Radius;
                public bool WeaverWill;
            }
            public struct HeraldOfPurity
            {
                public string VFX;
                public bool Enable_Radius;
                public float Radius;
                public bool WeaverWill;
            }

            //Factions
            public struct Factions
            {
                public TheWoven TheWoven;
            }
            public struct TheWoven
            {
                public bool Enable_TreePoints;
                public int TreePoints;
                public bool Enable_FreeRespe;
            }

            //Scenes
            public struct Scenes
            {
                public Camera Camera;
                public Dungeons Dungeons;
                public Minimap Minimap;
                public Monoliths Monoliths;
                public Harbringers harbringers;
            }
            public struct Camera
            {
                public bool Enable_Mod;

                public bool Enable_ZoomMinimum;
                public float ZoomMinimum;

                public bool Enable_ZoomPerScroll;
                public float ZoomPerScroll;

                public bool Enable_ZoomSpeed;
                public float ZoomSpeed;

                public bool Enable_DefaultRotation;
                public float DefaultRotation;

                public bool Enable_OffsetMinimum;
                public float OffsetMinimum;

                public bool Enable_OffsetMaximum;
                public float OffsetMaximum;

                public bool Enable_AngleMinimum;
                public float AngleMinimum;

                public bool Enable_AngleMaximum;
                public float AngleMaximum;

                public bool Enable_LoadOnStart;
            }
            public struct Dungeons
            {
                public bool Enable_EnterWithoutKey;
            }
            public struct Minimap
            {
                public bool Enable_MaxZoomOut;
                public bool Enable_RemoveFogOfWar;
                public bool Enable_ShowAllItems;
                public bool Enable_ShowItemsFromFilter;
                public float Icons_Scale;
            }
            public struct Monoliths
            {
                public bool Enable_MaxStability;
                public float MaxStability;

                public int Monoliths_Corruption;

                public bool Enable_MobsDensity;
                public float MobsDensity;

                public bool Enable_MobsDefeatOnStart;
                public float MobsDefeatOnStart;

                public bool Enable_BlessingSlots;
                public int BlessingSlots;

                public bool Enable_MaxStabilityOnStart;
                public bool Enable_MaxStabilityOnStabilityChanged;
                public bool Enable_ObjectiveReveal;
                public bool Enable_DropShadeGauntlet;
                public bool Enable_CompleteObjective;
                public bool Enable_NoLostWhenDie;
                public Monoliths_Corruptions Corruptions;
                public bool Enable_Islands;
            }
            public struct Monoliths_Corruptions
            {
                public bool Enable_Monoliths_Corruptions;
                public int Fall_Of_The_Outcast;
                public int The_Stolen_Lance;
                public int The_Black_Sun;
                public int Blood_Frost_And_Death;
                public int Ending_The_Storm;
                public int Fall_Of_The_Empire;
                public int Reign_Of_Dragon;
                public int The_Last_Ruins;
                public int The_Age_Of_Winter;
                public int Spirits_Of_Fire;
            }
            public struct Harbringers
            {
                public bool Enable_AltarWithoutKey;
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

                public bool Enable_SpecializationSlots;
                public float SpecializationSlots;

                public bool Enable_SkillLevel;
                public float SkillLevel;

                public bool Enable_PassivePoints;
                public float PassivePoints;

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
        }
    }
}

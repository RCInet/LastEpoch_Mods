using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastEpochMods.Config
{    
    public class Data
    {
        public static bool mods_config_loaded = false;
        public static string path = System.IO.Directory.GetCurrentDirectory() + @"\Mods\LastEpochMods\";
        public static string filename = "config.json";
        public static Mods_Structure mods_config = new Mods_Structure();
        public static Mods_Structure mods_config_duplicate = new Mods_Structure();

        public struct Mods_Structure
        {
            public AutoLoot_Structure auto_loot;
            public Items_Structure items;
            public Scene_Structure scene;
            public Character_Structure character;
            public Crafting_structure craft;
        }
        public struct Crafting_structure
        {
            public bool no_cost;
            public bool only_crit;
            public bool override_affix_roll;
            public int affix_roll;
        }        
        public struct Character_Structure
        {
            public Skills_structure skills;
            public SkillTree_structure skilltree;
            public PassiveTree_structure passivetree;
            public Companion_structure companions;
            public Minion_structure minions;
            public CharacterStats_structure characterstats;
            public Cosmetic_structure cosmetic;
            public Slots_structure slots;
        }
        public struct Slots_structure
        {
            public bool Enable_number_of_unlocked_slots;
            public byte number_of_unlocked_slots;
        }
        public struct Cosmetic_structure
        {
            public bool Enable_Cosmetic_Btn;
        }
        public struct CharacterStats_structure
        {
            public bool Enable_attack_rate;
            public float attack_rate;
            public bool Enable_leach_rate;
            public float leach_rate;
            public bool Enable_GodMode;
        }
        public struct Skills_structure
        {
            public bool Enable_manaCost;
            public bool Enable_channel_cost;
            public bool Enable_noManaRegenWhileChanneling;
            public bool Enable_stopWhenOutOfMana;
            public bool Enable_RemoveCooldown;
            public bool Enable_All;
            public Movement_Skills_Structure Movements;
            public Skills_Heal_Cost_Structure HealCost;
        }
        public struct Skills_Heal_Cost_Structure
        {
            public bool Enable_Transplant;
            public bool Enable_MarrowShards;
            public bool Enable_ReaperForm;
        }
        public struct Movement_Skills_Structure
        {
            public bool Enable_NoTarget;
            public bool Enable_ImmuneDuringMovement;
            public bool Disable_SimplePath;
        }
        public struct SkillTree_structure
        {
            public bool Enable_skilltree_level;
            public byte skilltree_level;
            public bool Disable_node_requirement;
        }
        public struct PassiveTree_structure
        {
            public bool Enable_passiveTree_pointsEarnt;
            public ushort passiveTree_pointsEarnt;
        }
        public struct Companion_structure
        {
            public bool Enable_companion_limit;
            public int companion_limit;
            public Wolf_structure wolf;
            public Scorpion_structure scorpion;
        }
        public struct Wolf_structure
        {
            public bool Enable_summon_max;
            public bool Enable_override_limit;
            public int summon_limit;
        }
        public struct Scorpion_structure
        {
            public bool Enable_baby_quantity;
            public int baby_quantity;
        }
        public struct Minion_structure
        {
            public Skeleton_structure skeleton;
            public Mage_structure mage;
            public Wraith_structure wraith;
            public BoneGolem_structure bone_golem;
            public VolatileZombie_structure volatile_zombie;
            public DreadShade_structure dread_shade;
        }
        public struct Skeleton_structure
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
        public struct Wraith_structure
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
        public struct Mage_structure
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
        public struct BoneGolem_structure
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
        public struct VolatileZombie_structure
        {
            public bool Enable_chanceToCastFromMinionDeath;
            public float chanceToCastFromMinionDeath;
            public bool Enable_chanceToCastInfernalShadeOnDeath;
            public float chanceToCastInfernalShadeOnDeath;
            public bool Enable_chanceToCastMarrowShardsOnDeath;
            public float chanceToCastMarrowShardsOnDeath;
        }
        public struct DreadShade_structure
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
        public struct Scene_Structure
        {
            public bool Enable_Waypoint_Unlock;
            public bool Enable_SpawnerPlacementManager_defaultSpawnerDensity;
            public float SpawnerPlacementManager_defaultSpawnerDensity;
            public bool Enable_SpawnerPlacementManager_IncreaseExperience;
            public float SpawnerPlacementManager_IncreaseExperience;
            public bool Enable_Dungeons_WithoutKey;
            public bool CreateKey; //True = create a key, False = Bypass Result
            public bool Enable_Dungeons_ObjectiveReveal;
            public bool Enable_Monolith_Stability;
            public bool Enable_Monolith_Overide_Max_Stability;
            public int Max_Stability;
            public bool Enable_Monolith_EnnemiesDefeat_OnStart;
            public float Monolith_EnnemiesDefeat_OnStart;
            public bool Enable_Monolith_ObjectiveReveal;
            public bool Enable_Monolith_Complete_Objective;
            public bool Enable_Monolith_EnemyDensity;
            public float Monolith_EnemyDensity;
            public bool Enable_Monolith_NoDie;
            public bool Remove_Fog_Of_War;
        }
        public struct AutoLoot_Structure
        {
            public bool AutoPickup_Key;
            public bool AutoPickup_Craft;
            public bool AutoPickup_UniqueAndSet;
            public bool AutoStore_Materials;
            public bool AutoPickup_Gold;
            public bool AutoPickup_XpTome;
            public bool AutoPickup_Pots;
            public bool Hide_materials_notifications;
        }
        public struct Items_Structure
        {
            public bool Enable_increase_equipment_droprate;
            public float increase_equipment_droprate;
            public bool Enable_increase_equipmentshards_droprate;
            public float increase_equipmentshards_droprate;
            public bool Enable_increase_uniques_droprate;
            public float increase_uniques_droprate;
            public bool Enable_DeathItemDrop_goldMultiplier;
            public float DeathItemDrop_goldMultiplier;
            public bool Enable_DeathItemDrop_ItemMultiplier;
            public float DeathItemDrop_ItemMultiplier;
            public bool Enable_DeathItemDrop_Experience;
            public long DeathItemDrop_Experience;
            public bool Enable_DeathItemDrop_AdditionalRare;
            public bool DeathItemDrop_AdditionalRare;
            public bool Enable_Rarity;
            public byte GenerateItem_Rarity;
            public bool Enable_RollImplicit;
            public byte Roll_Implicit;
            public bool Enable_SealValue;
            public byte Roll_SealValue;
            public bool Enable_SealTier;
            public byte Roll_SealTier;
            public bool Enable_AffixsValue;
            public byte Roll_AffixValue;
            public bool Enable_AffixsTier;
            public byte Roll_AffixTier;
            public bool Enable_ForgingPotencial;
            public int Roll_ForgingPotencial;
            public bool Enable_UniqueMod;
            public byte Roll_UniqueMod;
            public bool Enable_RollLegendayPotencial;
            public int Roll_Legendary_Potencial;
            public bool Enable_RollWeaverWill;
            public int Roll_Weaver_Will;
            public bool Remove_LevelReq;
            public bool Remove_ClassReq;
            public bool Remove_SubClassReq;
            public bool Enable_pickup_range;
            public bool Enable_nb_affixes;
            public bool Remove_set_req;
            public int nb_affixes;
        }
    }
}

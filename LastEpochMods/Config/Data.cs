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
            public Affixs_Structure affixs;
        }
        public struct Affixs_Structure
        {
            public bool Enable_Affixs_Multiplier;
            public int Affixs_Multiplier;
        }
        public struct Character_Structure
        {
            public Skills_structure skills;
            public SkillTree_structure skilltree;
            public PassiveTree_structure passivetree;
            public Companion_structure companions;
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
        }
        public struct Skills_structure
        {
            public bool Enable_manaCost;
            public bool Enable_channel_cost;
            public bool Enable_noManaRegenWhileChanneling;
            public bool Enable_stopWhenOutOfMana;
            public bool Enable_RemoveCooldown;
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
            public bool Enable_Monolith_NoDie;
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
        }
    }
}

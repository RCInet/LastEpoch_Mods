using Il2CppSystem.Collections.Generic;
using MelonLoader;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Character_Mods
    {
        //ItemDrop (untested)
        public static bool Enable_increase_equipment_droprate = false;
        public static float increase_equipment_droprate = 1;
        public static bool Enable_increase_equipmentshards_droprate = false;
        public static float increase_equipmentshards_droprate = 1;
        public static bool Enable_increase_uniques_droprate = false;
        public static float increase_uniques_droprate = 10;        
        //Ability List
        public static bool Enable_channel_cost = true;
        public static float channel_cost = 0;
        public static bool Enable_manaCost = true;
        public static float manaCost = 0;
        public static bool Enable_manaCostPerDistance = true;
        public static float manaCostPerDistance = 0;
        public static bool Enable_minimumManaCost = true;
        public static float minimumManaCost = 0;
        public static bool Enable_noManaRegenWhileChanneling = true;
        public static bool noManaRegenWhileChanneling = false;
        public static bool Enable_stopWhenOutOfMana = true;
        public static bool stopWhenOutOfMana = false;
        //Tree Data        
        public static bool Enable_number_of_unlocked_slots = false;
        public static byte number_of_unlocked_slots = 5;
        public static bool Enable_passiveTree_pointsEarnt = false;
        public static ushort passiveTree_pointsEarnt = 65535;
        public static bool Enable_skilltree_level = false;
        public static byte skilltree_level = 255;        
        //CharacterStats
        public static bool Enable_attack_rate = true;
        public static float attack_rate = 255;
        public static bool Enable_attributes = false;
        public static int attributes_str = 99999999;
        public static int attributes_int = 99999999;
        public static int attributes_vita = 99999999;
        public static int attributes_dext = 99999999;
        public static int attributes_atte = 99999999;        

        public static void Launch_LevelUp()
        {
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                if ((obj.name == "MainPlayer(Clone)") && (obj.GetActualType() == typeof(ExperienceTracker)))
                {
                    obj.TryCast<ExperienceTracker>().LevelUp(true);
                    break;
                }
            }
        }
        public static void Launch_ExempleBuffCharacter()
        {
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                if ((obj.name == "MainPlayer(Clone)") && (obj.GetActualType() == typeof(StatBuffs)))
                {
                    float duration = 255;
                    SP propertie = SP.Intelligence;
                    float added_value = 255;
                    float increase_value = 255;
                    Il2CppSystem.Collections.Generic.List<float> more_values = null;
                    AT tag = AT.Buff;
                    byte special_tag = 0;

                    obj.TryCast<StatBuffs>().addBuff(duration, propertie, added_value, increase_value, more_values, tag, special_tag);
                }
            }
        }
        public static void Launch()
        {
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                if (obj.name == "MainPlayer(Clone)")
                {
                    System.Type type = obj.GetActualType();
                    if (type == typeof(ItemDropBonuses))
                    {
                        ItemDropBonuses item_drop_bonus = obj.TryCast<ItemDropBonuses>();
                        if (Enable_increase_equipment_droprate)
                        {
                            for (int i = 0; i < item_drop_bonus.increasedEquipmentDroprates.Count; i++)
                            {
                                item_drop_bonus.increasedEquipmentDroprates[i] = increase_equipment_droprate;
                            }
                        }
                        if (Enable_increase_equipmentshards_droprate)
                        {
                            for (int z = 0; z < item_drop_bonus.increasedEquipmentShardDroprates.Count; z++)
                            {
                                item_drop_bonus.increasedEquipmentShardDroprates[z] = increase_equipmentshards_droprate;
                            }
                        }
                        if (Enable_increase_uniques_droprate)
                        {
                            item_drop_bonus.increasedUniqueDropRate = increase_uniques_droprate;
                        }
                    }                    
                    else if (type == typeof(PlayerAbilityList))
                    {
                        PlayerAbilityList player_ability_list = obj.TryCast<PlayerAbilityList>();
                        foreach (Ability ability in player_ability_list.equippedAbilities)
                        {
                            if (Enable_channel_cost) { ability.channelCost = channel_cost; }
                            if (Enable_manaCost) { ability.manaCost = manaCost; }
                            if (Enable_manaCostPerDistance) { ability.manaCostPerDistance = manaCostPerDistance; }
                            if (Enable_minimumManaCost) { ability.minimumManaCost = minimumManaCost; }
                            if (Enable_noManaRegenWhileChanneling) { ability.noManaRegenWhileChanneling = noManaRegenWhileChanneling; }
                            if (Enable_stopWhenOutOfMana) { ability.stopWhenOutOfMana = stopWhenOutOfMana; }
                        }
                    }
                    else if (type == typeof(LocalTreeData))
                    {
                        LocalTreeData tree_data = obj.TryCast<LocalTreeData>();
                        if (Enable_number_of_unlocked_slots) { tree_data.numberOfUnlockedSlots = number_of_unlocked_slots; }                        
                        if (Enable_passiveTree_pointsEarnt) { tree_data.passiveTree.pointsEarnt = passiveTree_pointsEarnt; }
                        if (Enable_skilltree_level)
                        {
                            foreach (LocalTreeData.SkillTreeData skill_tree_data in tree_data.specialisedSkillTrees)
                            {
                                skill_tree_data.level = skilltree_level;
                            }
                        }
                    }
                    else if (type == typeof(CharacterStats))
                    {
                        CharacterStats char_stats = obj.TryCast<CharacterStats>();
                        if (Enable_attack_rate) { char_stats.attackRate = attack_rate; }
                        if (Enable_attributes)
                        {
                            foreach (CharacterStats.AttributeValuePair attribute in char_stats.attributes)
                            {
                                if (attribute.attribute.attributeName == CoreAttribute.Attribute.Strength)
                                {
                                    attribute.value = attributes_str;
                                }
                                else if (attribute.attribute.attributeName == CoreAttribute.Attribute.Intelligence)
                                {
                                    attribute.value = attributes_int;
                                }
                                else if (attribute.attribute.attributeName == CoreAttribute.Attribute.Vitality)
                                {
                                    attribute.value = attributes_vita;
                                }
                                else if (attribute.attribute.attributeName == CoreAttribute.Attribute.Dexterity)
                                {
                                    attribute.value = attributes_dext;
                                }
                                else if (attribute.attribute.attributeName == CoreAttribute.Attribute.Attunement)
                                {
                                    attribute.value = attributes_atte;
                                }
                            }
                        }
                    }                    
                }
            }
        }
    }
}

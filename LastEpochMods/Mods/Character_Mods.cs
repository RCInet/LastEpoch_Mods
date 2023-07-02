using HarmonyLib;
using UMA.AssetBundles;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Character_Mods
    {
        //Ability List
        public static bool Enable_manaCost = true;
        public static bool Enable_channel_cost = true;        
        public static bool Enable_noManaRegenWhileChanneling = true;
        public static bool Enable_stopWhenOutOfMana = true;
        public static bool Enable_RemoveCooldown = true;
        //Tree Data        
        public static bool Enable_number_of_unlocked_slots = false;
        public static byte number_of_unlocked_slots = 5;
        public static bool Enable_passiveTree_pointsEarnt = false;
        public static ushort passiveTree_pointsEarnt = 65535;
        public static bool Enable_skilltree_level = false;
        public static byte skilltree_level = 255;        
        //CharacterStats
        public static bool Enable_attack_rate = true;
        public static float attack_rate = 255f;
        public static bool Enable_leach_rate = true;
        public static float leach_rate = 255f;

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
                    if (type == typeof(LocalTreeData))
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
                }
            }
        }
        
        public class Stats_Mods
        {
            [HarmonyPatch(typeof(CharacterStats), "Update")]
            public class Character_Stats
            {
                [HarmonyPostfix]
                static void Postfix(ref CharacterStats __instance)
                {                    
                    if (Enable_attack_rate) { __instance.attackRate = attack_rate; }
                    if (Enable_leach_rate) { __instance.increasedLeechRate = leach_rate; }
                    if (Enable_attributes)
                    {
                        foreach (CharacterStats.AttributeValuePair attribute in __instance.attributes)
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
        public class Ability_Mods
        {
            public class Cooldown
            {
                [HarmonyPatch(typeof(CharacterMutator), "OnAbilityUse")]
                public class Ability_Cooldown_Prefix
                {
                    [HarmonyPrefix]
                    static bool Postfix(CharacterMutator __instance, AbilityInfo __0, ref AbilityMutator __1, ref float __2, UnityEngine.Vector3 __3, bool __4)
                    {
                        if ( (Enable_RemoveCooldown) && (__1 != null)) { __1.RemoveCooldown(); }

                        return true;
                    }
                }
            }
            public class ManaCost
            {
                [HarmonyPatch(typeof(CharacterStats), "onStartedUsingAbility")]
                public class Character_Stats
                {
                    [HarmonyPrefix]
                    static bool Prefix(CharacterStats __instance, AbilityInfo __0, ref Ability __1, UnityEngine.Vector3 __2)
                    {
                        if (Enable_channel_cost) { __1.channelCost = 0f; }
                        if (Enable_manaCost)
                        {
                            __1.manaCost = 0f;
                            __1.minimumManaCost = 0f;
                            __1.manaCostPerDistance = 0f;
                        }
                        if (Enable_noManaRegenWhileChanneling) { __1.noManaRegenWhileChanneling = false; }
                        if (Enable_stopWhenOutOfMana) { __1.stopWhenOutOfMana = false; }

                        return true;
                    }
                }
            }
            public class SkillTree
            {

            }
        }
    }
}

using HarmonyLib;
using UMA.AssetBundles;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Character_Mods
    {
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
                        if (Config.Data.mods_config.character.Enable_number_of_unlocked_slots) { tree_data.numberOfUnlockedSlots = Config.Data.mods_config.character.number_of_unlocked_slots; }                        
                        if (Config.Data.mods_config.character.Enable_passiveTree_pointsEarnt) { tree_data.passiveTree.pointsEarnt = Config.Data.mods_config.character.passiveTree_pointsEarnt; }
                        if (Config.Data.mods_config.character.Enable_skilltree_level)
                        {
                            foreach (LocalTreeData.SkillTreeData skill_tree_data in tree_data.specialisedSkillTrees)
                            {
                                skill_tree_data.level = Config.Data.mods_config.character.skilltree_level;
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
                    if (Config.Data.mods_config.character.Enable_attack_rate) { __instance.attackRate = Config.Data.mods_config.character.attack_rate; }
                    if (Config.Data.mods_config.character.Enable_leach_rate) { __instance.increasedLeechRate = Config.Data.mods_config.character.leach_rate; }
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
                        if ( (Config.Data.mods_config.character.Enable_RemoveCooldown) && (__1 != null)) { __1.RemoveCooldown(); }

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
                        if (Config.Data.mods_config.character.Enable_channel_cost) { __1.channelCost = 0f; }
                        if (Config.Data.mods_config.character.Enable_manaCost)
                        {
                            __1.manaCost = 0f;
                            __1.minimumManaCost = 0f;
                            __1.manaCostPerDistance = 0f;
                        }
                        if (Config.Data.mods_config.character.Enable_noManaRegenWhileChanneling) { __1.noManaRegenWhileChanneling = false; }
                        if (Config.Data.mods_config.character.Enable_stopWhenOutOfMana) { __1.stopWhenOutOfMana = false; }

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

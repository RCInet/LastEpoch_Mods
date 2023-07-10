using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastEpochMods.Hooks
{
    public class Character_Stats
    {
        public static bool Enable_attributes = false;
        public static int attributes_str = 99999999;
        public static int attributes_int = 99999999;
        public static int attributes_vita = 99999999;
        public static int attributes_dext = 99999999;
        public static int attributes_atte = 99999999;

        [HarmonyPatch(typeof(CharacterStats), "Update")]
        public class Update
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
}

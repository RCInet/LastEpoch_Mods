using HarmonyLib;

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
                if (Config.Data.mods_config.character.characterstats.Enable_attack_rate) { __instance.attackRate = Config.Data.mods_config.character.characterstats.attack_rate; }
                if (Config.Data.mods_config.character.characterstats.Enable_leach_rate)
                {
                    __instance.TryCast<BaseStats>().increasedLeechRate = Config.Data.mods_config.character.characterstats.leach_rate;
                }
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
        
        [HarmonyPatch(typeof(CharacterStats), "getMaximumCompanions")]
        public class getMaximumCompanions
        {
            [HarmonyPostfix]
            static void Postfix(CharacterStats __instance, ref int __result)
            {
                if (Config.Data.mods_config.character.companions.Enable_companion_limit)
                {
                    __result = Config.Data.mods_config.character.companions.companion_limit;
                }                    
            }
        }

    }
}

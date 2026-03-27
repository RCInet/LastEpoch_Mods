using HarmonyLib;
using Il2Cpp;

namespace LastEpoch_Hud.Scripts.Mods.Craft
{
    public class Craft_Locales
    {
        //Corrupted Item
        public const string item_corrupted_key = "CraftingForgeButton_Title_Corruption_AlreadyCorrupted"; //LastEpoch v1.4
        public static string item_is_corrupted = "item_corrupted";
        //Craft to T8
        public const string affix_is_maxed_key = "Crafting_ForgeButton_Title_AffixMaxed_2"; //LastEpoch v1.3.1.1
        public static string affix_is_maxed = "affix_maxed";

        [HarmonyPatch(typeof(Localization), "TryGetText")]
        public class Localization_TryGetText
        {
            [HarmonyPrefix]
            static bool Prefix(ref bool __result, string __0)
            {
                bool result = true;
                //Main.logger_instance.Msg("TryGetText : Key = " + __0);
                if ((__0 == affix_is_maxed_key) || (__0 == item_corrupted_key))
                {
                    __result = true;
                    result = false;
                }

                return result;
            }
        }

        [HarmonyPatch(typeof(Localization), "GetText")]
        public class Localization_GetText
        {
            [HarmonyPrefix]
            static bool Prefix(ref string __result, string __0)
            {
                bool result = true;
                //Main.logger_instance.Msg("GetText : Key = " + __0);
                switch (__0)
                {
                    case item_corrupted_key: { __result = item_is_corrupted; result = false; break; }
                    case affix_is_maxed_key: { __result = affix_is_maxed; result = false; break; }
                }

                return result;
            }
        }
    }
}

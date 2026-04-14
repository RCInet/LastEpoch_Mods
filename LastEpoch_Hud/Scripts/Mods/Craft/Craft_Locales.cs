using LastEpoch_Hud.Scripts.Mods.Localization;

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

        public static void RegisterLocales()
        {
            LocalizationOverride.Register(item_corrupted_key, () => item_is_corrupted);
            LocalizationOverride.Register(affix_is_maxed_key, () => affix_is_maxed);
        }
    }
}

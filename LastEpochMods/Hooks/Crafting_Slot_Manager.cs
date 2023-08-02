using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Crafting_Slot_Manager
    {
        [HarmonyPatch(typeof(CraftingSlotManager), "OnCraftingAttempted")]
        public class OnCraftingAttempted
        {
            [HarmonyPrefix]
            static bool Prefix(CraftingSlotManager __instance, bool __0, Il2CppSystem.Collections.Generic.List<Stats.Stat> __1, int __2, bool __3, ref bool __4, bool __5, int __6, ref int __7, ref int __8)
            {
                if (Config.Data.mods_config.craft.only_crit) { __4 = true; }
                if (Config.Data.mods_config.craft.override_affix_roll) { __7 = Config.Data.mods_config.craft.affix_roll; }

                return true;
            }
        }
    }
}

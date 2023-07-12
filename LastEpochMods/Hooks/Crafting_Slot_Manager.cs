using HarmonyLib;
using LE.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using static AffixList;

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
                //need to use rollaffixes functions 
                if (Config.Data.mods_config.craft.override_affix_roll) { __7 = Config.Data.mods_config.craft.affix_roll; }

                return true;
            }
        }
    }
}

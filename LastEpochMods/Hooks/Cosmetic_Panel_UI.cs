using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastEpochMods.Hooks
{
    public class Cosmetic_Panel_UI
    {
        [HarmonyPatch(typeof(CosmeticPanelUI), "OnEnable")]
        public class OnEnable
        {
            [HarmonyPostfix]
            static void Postfix(CosmeticPanelUI __instance)
            {
                Mods.Cosmetics.IsCosmeticPanelOpen = true;
            }
        }
        
        [HarmonyPatch(typeof(CosmeticPanelUI), "OnDisable")]
        public class OnDisable
        {
            [HarmonyPostfix]
            static void Postfix(CosmeticPanelUI __instance)
            {
                Mods.Cosmetics.IsCosmeticPanelOpen = false;
            }
        }
    }
}

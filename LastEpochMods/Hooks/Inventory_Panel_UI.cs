using HarmonyLib;
using UnityEngine;

namespace LastEpochMods.Hooks
{
    public class Inventory_Panel_UI
    {        
        [HarmonyPatch(typeof(InventoryPanelUI), "Start")]
        public class Start
        {
            [HarmonyPostfix]
            static void Postfix(ref InventoryPanelUI __instance)
            {
                Mods.Cosmetics.Inventory.__instance = __instance;
                foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(TabUIElement)))
                {
                    if (obj.name == "AppearanceTab")
                    {
                        Mods.Cosmetics.Inventory.tabUIElement = obj.TryCast<TabUIElement>();
                        break;
                    }
                }
                foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.CanvasGroup)))
                {
                    if (obj.name == "AppearanceTab")
                    {
                        Mods.Cosmetics.Inventory.canvasGroup = obj.TryCast<UnityEngine.CanvasGroup>();
                        break;
                    }
                }
                Mods.Cosmetics.Inventory.ShowHideTab();
            }
        }

        [HarmonyPatch(typeof(InventoryPanelUI), "OpenInventoryPanel")]
        public class OpenInventoryPanel
        {
            [HarmonyPrefix]
            static bool Prefix(ref InventoryPanelUI __instance, bool __0)
            {
                Mods.Cosmetics.Inventory.IsOpen = true;
                Mods.Cosmetics.Inventory.ShowHideTab();                

                return true;
            }
        }

        [HarmonyPatch(typeof(InventoryPanelUI), "OnDisable")]
        public class OnDisable
        {
            [HarmonyPostfix]
            static void Postfix(ref InventoryPanelUI __instance)
            {
                Mods.Cosmetics.Inventory.IsOpen = false;
            }
        }
    }
}

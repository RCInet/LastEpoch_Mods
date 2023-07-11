using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Inventory_Panel_UI
    {
        [HarmonyPatch(typeof(InventoryPanelUI), "OnEnable")]
        public class OnEnable
        {
            [HarmonyPostfix]
            static void Postfix(InventoryPanelUI __instance)
            {
                bool disable = true;
                if (Config.Data.mods_config.character.cosmetic.Enable_Cosmetic_Btn) { disable = false; }
                foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(TabUIElement)))
                {
                    if (obj.name == "AppearanceTab")
                    {
                        TabUIElement appearance_tab = obj.TryCast<TabUIElement>();
                        appearance_tab.isDisabled = disable;
                        break;
                    }
                }
                foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.CanvasGroup)))
                {
                    if (obj.name == "AppearanceTab")
                    {
                        UnityEngine.CanvasGroup canvas_group = obj.TryCast<UnityEngine.CanvasGroup>();
                        canvas_group.enabled = disable;
                        break;
                    }
                }
            }
        }
    }
}

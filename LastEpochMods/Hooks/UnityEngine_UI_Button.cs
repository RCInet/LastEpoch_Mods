using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class UnityEngine_UI_Button
    {
        [HarmonyPatch(typeof(UnityEngine.UI.Button), "Press")]
        public class Press
        {
            [HarmonyPostfix]
            static void Postfix(ref UnityEngine.UI.Button __instance)
            {
                //Main.logger_instance.Msg("Btn : " + __instance.name);

                bool shop_open = Mods.Cosmetics.IsShopOpen();
                bool cosmetic_select_open = Mods.Cosmetics.IsCometicSelectOpen();
                if (Mods.Cosmetics.IsCosmeticPanelOpen)
                {
                    if (__instance.name == "OpenShop")
                    {
                        Mods.Cosmetics.CloseInventory();
                        Mods.Cosmetics.OpenShop();
                    }
                    else if (__instance.name == "GetPoints") { Mods.Cosmetics.GetPoints(); }
                }                
                if (shop_open)
                {
                    if ((__instance.name == "Close_Button") || (__instance.name == "Armory"))
                    {
                        Mods.Cosmetics.CloseShop();
                        Mods.Cosmetics.OpenInventory();
                    }
                }
                if (cosmetic_select_open)
                {
                    if (__instance.name == "OpenCosmeticStore")
                    {
                        Mods.Cosmetics.CloseSelect();
                        Mods.Cosmetics.OpenShop();
                    }
                    //if (__instance.name == "Close_Button") { Mods.Cosmetics.OpenInventory(); }
                }
            }
        }
    }
}

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
                if (__instance.name == "OpenShop") { Mods.Cosmetics.OpenShop(); }
                else if (__instance.name == "GetPoints") { Mods.Cosmetics.GetPoints(); }
                else if (__instance.name == "Cosmetics") { Mods.Cosmetics.SelectCosmeticTab(); }
                else if (__instance.name == "OpenCosmeticStore") { Mods.Cosmetics.OpenShop(); }
                else if (__instance.name == "buyButton") { Mods.Cosmetics.BuyCosmetic(); }
                else if (__instance.name == "Close Button") { Mods.Cosmetics.CloseShop(); }
                else if (__instance.name == "getPointsButton") { Mods.Cosmetics.GetPoints(); }
                else if (__instance.name == "tabButton") { Mods.Cosmetics.ShowTab(); }
            }
        }
    }
}

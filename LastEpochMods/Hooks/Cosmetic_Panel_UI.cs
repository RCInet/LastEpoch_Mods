using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Cosmetic_Panel_UI
    {
        [HarmonyPatch(typeof(CosmeticPanelUI), "OnEnable")]
        public class OnEnable
        {
            [HarmonyPostfix]
            static void Postfix(ref CosmeticPanelUI __instance)
            {                
                Mods.Cosmetics.MtxStore.Get();
                Mods.Cosmetics.Panel.Hide_Text_GetPoint(__instance);
                Mods.Cosmetics.Panel.Hide_Btn_GetPoint(__instance);
                //Mods.Cosmetics.Panel.Hide_Btn_OpenShop(__instance);   
                Mods.Cosmetics.Panel.IsOpen = true;
                Mods.Cosmetics.Panel.__instance = __instance;
            }
        }
        
        [HarmonyPatch(typeof(CosmeticPanelUI), "OnDisable")]
        public class OnDisable
        {
            [HarmonyPostfix]
            static void Postfix(CosmeticPanelUI __instance)
            {
                Mods.Cosmetics.Panel.IsOpen = false;
                Mods.Cosmetics.Panel.__instance = null;
            }
        }
    }
}

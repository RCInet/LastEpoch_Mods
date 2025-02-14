using HarmonyLib;
using UnityEngine;
using Il2Cpp;

namespace LastEpoch_Hud.Scripts.Mods.Cosmetics
{
    public class Cosmetics_Unlock
    {
        public static TabUIElement tab_element = null;
        public static UIPanel mtx_shop_panel = null;
        public static Il2CppLE.UI.MTXStore.MTXStoreController mtx_store_controller = null;

        public static void UnlockTab()
        {
            if ((tab_element.IsNullOrDestroyed()) && (!Refs_Manager.InventoryPanelUI.IsNullOrDestroyed()))
            {
                if (!Refs_Manager.InventoryPanelUI.tabController.IsNullOrDestroyed())
                {
                    foreach (TabUIElement tab in Refs_Manager.InventoryPanelUI.tabController.tabElements)
                    {
                        if (tab.gameObject.name == "AppearanceTab") { tab_element = tab; break; }
                    }
                }
            }
            if (!tab_element.IsNullOrDestroyed())
            {
                tab_element.isDisabled = false;
                tab_element.canvasGroup.TryCast<Behaviour>().enabled = false;
            }
        }
        public static void SetupShopBtn()
        {
            if (!Refs_Manager.InventoryPanelUI.IsNullOrDestroyed())
            {
                CosmeticPanelUI cosmetic_panel_ui = Refs_Manager.InventoryPanelUI.cosmeticPanel.GetComponent<CosmeticPanelUI>();
                cosmetic_panel_ui.openShopButton.onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                cosmetic_panel_ui.openShopButton.onClick.AddListener(OpenMtxAction);
            }
        }
        private static void SetupMtxRef()
        {
            if (!Refs_Manager.game_uibase.IsNullOrDestroyed())
            {
                if ((!Refs_Manager.game_uibase.mtxShopPanel.IsNullOrDestroyed()) &&
                (mtx_shop_panel.IsNullOrDestroyed()))
                { mtx_shop_panel = Refs_Manager.game_uibase.mtxShopPanel; }
            }
        }
        private static readonly System.Action OpenMtxAction = new System.Action(OpenMtx);
        public static void OpenMtx()
        {
            if (mtx_shop_panel.IsNullOrDestroyed()) { SetupMtxRef(); }
            if (!mtx_shop_panel.IsNullOrDestroyed())
            {
                mtx_shop_panel.instance.active = true;
                mtx_shop_panel.isOpen = true;
            }
        }
        /*private static readonly System.Action CloseMtxAction = new System.Action(CloseMtx);
        public static void CloseMtx()
        {
            if (mtx_shop_panel.IsNullOrDestroyed()) { SetupMtxRef(); }
            if (!mtx_shop_panel.IsNullOrDestroyed())
            {
                mtx_shop_panel.instance.active = false;
                mtx_shop_panel.isOpen = false;
            }
        }*/
        [HarmonyPatch(typeof(InventoryPanelUI), "OnEnable")]
        public class InventoryPanelUI_OnEnable
        {
            [HarmonyPostfix]
            static void Postfix(ref InventoryPanelUI __instance)
            {
                UnlockTab();
                SetupShopBtn();
            }
        }
        [HarmonyPatch(typeof(Il2CppLE.UI.MTXStore.MTXStoreController), "OnEnterState")]
        public class MTXStoreController_OnEnterState
        {
            [HarmonyPostfix]
            static void Postfix(ref Il2CppLE.UI.MTXStore.MTXStoreController __instance, Il2CppLE.UI.MTXStore.MTXStoreController.MTXStoreUIState __0, Il2CppLE.UI.MTXStore.MTXStoreController.MTXStoreUIState __1)
            {
                mtx_store_controller = __instance;
                __instance.loadingUIOverlay.active = false;
                __instance.loadingStore = false;
            }
        }        
    }
}

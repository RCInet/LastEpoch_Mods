using HarmonyLib;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Cosmetics
    {
        public class EnableBtn
        {
            [HarmonyPatch(typeof(InventoryPanelUI), "OnEnable")]
            public class OnEnable
            {
                [HarmonyPostfix]
                static void Postfix(ref InventoryPanelUI __instance)
                {
                    foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(TabUIElement)))
                    {
                        System.Type type = obj.GetActualType();
                        if (obj.name == "AppearanceTab")
                        {
                            obj.TryCast<TabUIElement>().isDisabled = false;                            
                            break;
                        }
                    }
                }
            }
        }

        /*private static InventoryPanelUI inventory_panel_ui = null;
        private static UnityEngine.GameObject blessing_panel = null;
        private static UnityEngine.GameObject inventory_panel = null;
        private static UnityEngine.GameObject cosmetic_panel = null;
        private static void Init_Refs()
        {
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(InventoryPanelUI)))
            {
                System.Type type = obj.GetActualType();
                if (obj.name == "InventoryPanel(Clone)")
                {
                    inventory_panel_ui = obj.TryCast<InventoryPanelUI>();
                    blessing_panel = inventory_panel_ui.blessingPanel;
                    inventory_panel = inventory_panel_ui.inventoryPanel;
                    cosmetic_panel = inventory_panel_ui.cosmeticPanel;                    
                    break;
                }
            }
        }*/

        /*public static void GetAllCosmetics()
        {
            if (cosmetic_panel == null) { Init_Refs(); }
            if (cosmetic_panel != null)
            {
                cosmetic_panel.petSlots.Clear();
                foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(CosmeticItemSlot)))
                {
                    System.Type type = obj.GetActualType();
                    if (type == typeof(CosmeticItemSlot))
                    {
                        CosmeticItemSlot cosmetic_item_slot = obj.TryCast<CosmeticItemSlot>();
                        CosmeticType cosmetic_type = cosmetic_item_slot.cosmeticType;
                        if (cosmetic_type == CosmeticType.PET)
                        {
                            cosmetic_panel.petSlots.Add(cosmetic_item_slot);
                        }
                        else if (cosmetic_type == CosmeticType.PORTAL)
                        {
                            
                        }
                        else if (cosmetic_type == CosmeticType.ITEM)
                        {
                            foreach (CosmeticItemSlot slot in cosmetic_panel.equipSlots)
                            {
                                if ((obj.name == "Helmet") && (cosmetic_item_slot.cosmeticSlot == LE.Networking.Cosmetics.CosmeticEquipSlot.Helm))
                                {
                                    
                                }
                            }
                        }
                    }
                }
            }
        }*/
    }
}

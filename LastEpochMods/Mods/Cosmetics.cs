using UnityEngine;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Cosmetics
    {
        //InventoryUi
        public static bool IsCosmeticPanelOpen = false;
        public static void OpenShop()
        {
            try
            {
                foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.GameObject)))
                {
                    if (obj.name == "MTXShopPanel(Clone)")
                    {
                        obj.TryCast<UnityEngine.GameObject>().gameObject.active = true;
                        break;
                    }
                }
            }
            catch { }
        }
        public static void OpenInventory()
        {
            try
            {
                foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.GameObject)))
                {
                    if (obj.name == "InventoryPanel(Clone)")
                    {
                        obj.TryCast<UnityEngine.GameObject>().gameObject.active = true;
                        break;
                    }
                }
            }
            catch { }
        }
        public static void CloseInventory()
        {
            try
            {
                foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.GameObject)))
                {
                    if (obj.name == "InventoryPanel(Clone)")
                    {
                        obj.TryCast<UnityEngine.GameObject>().gameObject.active = false;
                        break;
                    }
                }
            }
            catch { }
        }
        public static void GetPoints()
        {            
            //Set Points
        }

        //ShopUi
        public static bool IsShopOpen()
        {
            bool result = false;
            try
            {
                foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.GameObject)))
                {
                    if (obj.name == "MTXShopPanel(Clone)")
                    {
                        result = obj.TryCast<UnityEngine.GameObject>().gameObject.active;
                        break;
                    }
                }
            }
            catch { }

            return result;
        }
        public static void CloseShop()
        {
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.GameObject)))
            {
                if (obj.name == "MTXShopPanel(Clone)")
                {
                    obj.TryCast<UnityEngine.GameObject>().gameObject.active = false;
                    break;
                }
            }
        }

        //Cosmetic Item Select
        public static bool IsCometicSelectOpen()
        {
            bool result = false;
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.GameObject)))
            {
                if (obj.name == "InventoryPanel(Clone)")
                {
                    UnityEngine.GameObject flyout_holder = null;
                    for (int i = 0; i < obj.TryCast<UnityEngine.GameObject>().transform.childCount; i++)
                    {
                        string obj_name = obj.TryCast<UnityEngine.GameObject>().transform.GetChild(i).gameObject.name;
                        if (obj_name == "FlyoutHolder")
                        {
                            flyout_holder = obj.TryCast<UnityEngine.GameObject>().transform.GetChild(i).gameObject;                            
                            bool found = false;
                            for (int j = 0; j < flyout_holder.transform.childCount; j++)
                            {
                                string obj2_name = obj.TryCast<UnityEngine.GameObject>().transform.GetChild(i).gameObject.transform.GetChild(j).gameObject.name;
                                if (flyout_holder.transform.GetChild(j).gameObject.name == "Flyout Selection Window")
                                {
                                    result = obj.TryCast<UnityEngine.GameObject>().transform.GetChild(i).gameObject.transform.GetChild(j).gameObject.active;                                                                        
                                    found = true;
                                    break;
                                }
                            }
                            if (!found) { Main.logger_instance.Msg("Found Flyout Selection Window is null"); }
                            break;
                        }
                    }
                    if (flyout_holder == null) { Main.logger_instance.Msg("Flyout Holder Not Found"); }

                    break;
                }
            }

            return result;
        }
        public static void CloseSelect()
        {
            try
            {
                foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.GameObject)))
                {
                    if (obj.name == "InventoryPanel(Clone)")
                    {
                        UnityEngine.GameObject game_object = obj.TryCast<UnityEngine.GameObject>();
                        UnityEngine.GameObject flyout_holder = null;
                        for (int i = 0; i < game_object.transform.childCount; i++)
                        {
                            if (game_object.transform.GetChild(i).gameObject.name == "FlyoutHolder")
                            {
                                flyout_holder = game_object.transform.GetChild(i).TryCast<UnityEngine.GameObject>().gameObject;
                                break;
                            }
                        }
                        if (flyout_holder != null)
                        {
                            for (int i = 0; i < flyout_holder.transform.childCount; i++)
                            {
                                if (flyout_holder.transform.GetChild(i).gameObject.name == "Flyout Selection Window")
                                {
                                    flyout_holder.transform.GetChild(i).TryCast<UnityEngine.GameObject>().gameObject.active = false;
                                    break;
                                }
                            }
                        }

                        break;
                    }
                }
            }
            catch { }
        }
        public static void AddCosmeticToSlot(CosmeticItemObject cosmetic_obj)
        {
            foreach (UnityEngine.Object new_obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(CosmeticItemSlot)))
            {
                string new_slot_name = "";
                System.Type type = new_obj.GetActualType();
                if (type == typeof(CosmeticItemSlot))
                {
                    try
                    {
                        new_slot_name = new_obj.TryCast<CosmeticItemSlot>().gameObject.name;
                        if (current_slot_name == new_slot_name)
                        {
                            Main.logger_instance.Msg("Debug : Slot Found : " + new_slot_name);
                            CosmeticItemSlot cosmetic_item_slot = new_obj.TryCast<CosmeticItemSlot>();

                            cosmetic_item_slot.cosmeticDisplayImage = cosmetic_obj.itemImage;
                            //cosmetic_item_slot.ItemTypeInfoTooltip = cosmetic_obj.ItemTypeInfoTooltip;
                            cosmetic_item_slot.SetCosmetic(cosmetic_obj.storedCosmetic);
                            //cosmetic_item_slot.SetSprite(cosmetic_obj.itemImage.sprite);
                            cosmetic_item_slot.SetItemTypeTooltip(cosmetic_obj.storedCosmetic);
                            //cosmetic_item_slot.


                            //cosmetic_obj.enabled = true;
                            //cosmetic_obj.currentSelection = ;
                            //cosmetic_obj.overlayFadeImage

                            /*for (int i = 0; i < cosmetic_item_slot.transform.childCount; i++)
                            {
                                UnityEngine.GameObject game_object = cosmetic_item_slot.transform.GetChild(i).gameObject;
                                if (game_object.name == "CosmeticItemImage")
                                {
                                    game_object.GetComponent<UnityEngine.UI.Image>().sprite = cosmetic_obj.itemImage.sprite;
                                    break;
                                }
                            }*/
                            //break;
                        }
                    }
                    catch { Main.logger_instance.Msg("Error"); }
                }
            }
        }



        public static string current_slot_name = "";
        public static void RemoveShopDropdown()
        {
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.GameObject)))
            {
                if (obj.name == "CosmeticsStore")
                {
                    UnityEngine.GameObject game_object = obj.TryCast<UnityEngine.GameObject>();
                    for (int i = 0; i < game_object.transform.childCount; i++)
                    {
                        if (game_object.transform.GetChild(i).gameObject.name == "Dropdown")
                        {
                            game_object.transform.GetChild(i).TryCast<UnityEngine.GameObject>().gameObject.active = false;
                            break;
                        }
                    }
                }
            }
        }
        public static void BuyCosmetic()
        {
            Main.logger_instance.Msg("Debug : BuyCosmetic");
            //Add Cosmetic to cosmeticsmanager
        }
        public static void ShowTab()
        {
            Main.logger_instance.Msg("Debug : ShowTab");
        }
        public static void SelectCosmeticTab()
        {
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(TabUIController)))
            {
                if (obj.name == "Header")
                {
                    obj.TryCast<TabUIController>().Select(2, false);
                    break;
                }
            }
        }

        public struct cosmetic_list_structure
        {
            public System.Collections.Generic.List<LE.Services.Cosmetics.CosmeticItem> cosmetics_items;
            public System.Collections.Generic.List<LE.Services.Cosmetics.CosmeticBackSlot> cosmetics_backslot;
            public System.Collections.Generic.List<LE.Services.Cosmetics.CosmeticPet> cosmetics_pets;
            public System.Collections.Generic.List<LE.Services.Cosmetics.CosmeticPortal> cosmetics_portals;
        }
        public struct cosmetic_slot_structure
        {
            public CosmeticItemSlot slot;
            public string name;
        }

        public class Get
        {
            public static cosmetic_list_structure Cosmetics()
            {
                cosmetic_list_structure cosmetic_list = new cosmetic_list_structure
                {
                    cosmetics_items = new System.Collections.Generic.List<LE.Services.Cosmetics.CosmeticItem>(),
                    cosmetics_backslot = new System.Collections.Generic.List<LE.Services.Cosmetics.CosmeticBackSlot>(),
                    cosmetics_pets = new System.Collections.Generic.List<LE.Services.Cosmetics.CosmeticPet>(),
                    cosmetics_portals = new System.Collections.Generic.List<LE.Services.Cosmetics.CosmeticPortal>()
                };
                foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(LE.Services.Cosmetics.Cosmetic)))
                {
                    System.Type type = obj.GetActualType();
                    if (type == typeof(LE.Services.Cosmetics.CosmeticItem)) { cosmetic_list.cosmetics_items.Add(obj.TryCast<LE.Services.Cosmetics.CosmeticItem>()); }
                    else if (type == typeof(LE.Services.Cosmetics.CosmeticBackSlot)) { cosmetic_list.cosmetics_backslot.Add(obj.TryCast<LE.Services.Cosmetics.CosmeticBackSlot>()); }
                    else if (type == typeof(LE.Services.Cosmetics.CosmeticPet)) { cosmetic_list.cosmetics_pets.Add(obj.TryCast<LE.Services.Cosmetics.CosmeticPet>()); }
                    else if (type == typeof(LE.Services.Cosmetics.CosmeticPortal)) { cosmetic_list.cosmetics_portals.Add(obj.TryCast<LE.Services.Cosmetics.CosmeticPortal>()); }
                }

                Main.logger_instance.Msg("Found : " + cosmetic_list.cosmetics_items.Count + " items");
                Main.logger_instance.Msg("Found : " + cosmetic_list.cosmetics_backslot.Count + " back");
                Main.logger_instance.Msg("Found : " + cosmetic_list.cosmetics_pets.Count + " pets");
                Main.logger_instance.Msg("Found : " + cosmetic_list.cosmetics_portals.Count + " portal");

                return cosmetic_list;
            }
            public static System.Collections.Generic.List<cosmetic_slot_structure> Slots()
            {
                System.Collections.Generic.List<cosmetic_slot_structure> cosmetic_slot_list = new System.Collections.Generic.List<cosmetic_slot_structure>();
                Il2CppSystem.Collections.Generic.List<CosmeticItemSlot> cosmetic_slots = null;
                Il2CppSystem.Collections.Generic.List<CosmeticItemSlot> pet_slots = null;
                CosmeticItemSlot portal_slots = null;
                foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(InventoryPanelUI)))
                {
                    System.Type type = obj.GetActualType();
                    if (obj.name == "InventoryPanel(Clone)")
                    {
                        cosmetic_slots = obj.TryCast<InventoryPanelUI>().cosmeticPanel.GetComponent<CosmeticPanelUI>().equipSlots;
                        foreach (CosmeticItemSlot slot in cosmetic_slots)
                        {
                            cosmetic_slot_list.Add(new cosmetic_slot_structure
                            {
                                name = slot.cosmeticSlotName,
                                slot = slot
                            });
                        }

                        pet_slots = obj.TryCast<InventoryPanelUI>().cosmeticPanel.GetComponent<CosmeticPanelUI>().petSlots;
                        foreach (CosmeticItemSlot slot in cosmetic_slots)
                        {
                            cosmetic_slot_list.Add(new cosmetic_slot_structure
                            {
                                name = slot.cosmeticSlotName,
                                slot = slot
                            });
                        }

                        portal_slots = obj.TryCast<InventoryPanelUI>().cosmeticPanel.GetComponent<CosmeticPanelUI>().portalSlot;
                        cosmetic_slot_list.Add(new cosmetic_slot_structure
                        {
                            name = portal_slots.cosmeticSlotName,
                            slot = portal_slots
                        });

                        break;
                    }
                }

                return cosmetic_slot_list;
            }
        }
        public class Add
        {
            public static void CosmeticToShop()
            {
                try
                {
                    foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.GameObject)))
                    {
                        if (obj.name == "MTXShopPanel(Clone)")
                        {
                            //Get child "Panels" "All Panel"

                            LE.MicrotransactionSystem.ItemForSale item_for_sale = new LE.MicrotransactionSystem.ItemForSale();
                            LE.Services.Cosmetics.Cosmetic cosmetic = new LE.Services.Cosmetics.Cosmetic();
                            LE.UI.MTXStore.AllPanel panel = new LE.UI.MTXStore.AllPanel();
                            panel.CreateTile(item_for_sale, true, cosmetic);
                            break;
                        }
                    }
                }
                catch { }
            }
            public static void DefaultCosmetic()
            {
                cosmetic_list_structure cosmetics = Get.Cosmetics();
                foreach (cosmetic_slot_structure slot_struct in Get.Slots())
                {
                    if (slot_struct.slot.cosmeticSlotName == "Back Cosmetics")
                    {
                        slot_struct.slot.SetCosmetic(cosmetics.cosmetics_backslot[0].TryCast<LE.Services.Cosmetics.Cosmetic>());
                    }
                    else if (slot_struct.slot.cosmeticSlotName == "Pet Slot 1")
                    {
                        slot_struct.slot.SetCosmetic(cosmetics.cosmetics_pets[0].TryCast<LE.Services.Cosmetics.Cosmetic>());
                    }
                    else if (slot_struct.slot.cosmeticSlotName == "Pet Slot 2")
                    {
                        slot_struct.slot.SetCosmetic(cosmetics.cosmetics_pets[1].TryCast<LE.Services.Cosmetics.Cosmetic>());
                    }
                    else if (slot_struct.slot.cosmeticSlotName == "Portal Cosmetics")
                    {
                        slot_struct.slot.SetCosmetic(cosmetics.cosmetics_portals[0].TryCast<LE.Services.Cosmetics.Cosmetic>());
                    }
                }
            }
        }
    }
}

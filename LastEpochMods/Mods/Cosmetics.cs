using HarmonyLib;
using LE.Deprecated;
using LE.Services.Cosmetics;
using UnityEngine.UI;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Cosmetics
    {
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

        public class EnableBtn
        {
            [HarmonyPatch(typeof(InventoryPanelUI), "OnEnable")]
            public class OnEnable
            {
                [HarmonyPostfix]
                static void Postfix(InventoryPanelUI __instance)
                {
                    bool disable = true;
                    if (Config.Data.mods_config.character.Enable_Cosmetic_Btn) { disable = false; }
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

            [HarmonyPatch(typeof(BottomScreenMenu), "OnEnable")]
            public class OnEnableMenuBtns
            {
                [HarmonyPostfix]
                static void Postfix(BottomScreenMenu __instance)
                {
                    bool disable = true;
                    if (Config.Data.mods_config.character.Enable_Cosmetic_Btn) { disable = false; }
                    foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.UI.Button)))
                    {
                        if (obj.name == "Cosmetics")
                        {
                            Selectable appearance_tab = obj.TryCast<Selectable>();
                            appearance_tab.interactable = !disable;
                            break;
                        }
                    }
                    foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.CanvasGroup)))
                    {
                        if (obj.name == "Cosmetics")
                        {
                            UnityEngine.CanvasGroup canvas_group = obj.TryCast<UnityEngine.CanvasGroup>();
                            canvas_group.enabled = disable;
                            break;
                        }
                    }
                }
            }

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
                    if (type == typeof(CosmeticItem)) { cosmetic_list.cosmetics_items.Add(obj.TryCast<CosmeticItem>()); }
                    else if (type == typeof(CosmeticBackSlot)) { cosmetic_list.cosmetics_backslot.Add(obj.TryCast<CosmeticBackSlot>()); }
                    else if (type == typeof(CosmeticPet)) { cosmetic_list.cosmetics_pets.Add(obj.TryCast<CosmeticPet>()); }
                    else if (type == typeof(CosmeticPortal)) { cosmetic_list.cosmetics_portals.Add(obj.TryCast<CosmeticPortal>()); }
                }

                Main.logger_instance.Msg("Found : " + cosmetic_list.cosmetics_items.Count + " items");
                Main.logger_instance.Msg("Found : " + cosmetic_list.cosmetics_backslot.Count + " back");
                Main.logger_instance.Msg("Found : " + cosmetic_list.cosmetics_pets.Count + " pets");
                Main.logger_instance.Msg("Found : " + cosmetic_list.cosmetics_portals.Count + " portal");

                return cosmetic_list;
            }
            public static System.Collections.Generic.List<cosmetic_slot_structure> Slots()
            {
                System.Collections.Generic.List<cosmetic_slot_structure>  cosmetic_slot_list = new System.Collections.Generic.List<cosmetic_slot_structure>();
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

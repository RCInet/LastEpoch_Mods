using HarmonyLib;
using MelonLoader;
using UnhollowerBaseLib;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Character
{
    [RegisterTypeInIl2Cpp]
    public class Character_Blessings : MonoBehaviour
    {
        public static Character_Blessings instance { get; private set; }
        public Character_Blessings(System.IntPtr ptr) : base(ptr) { }

        void Awake()
        {
            instance = this;
            if (Main.debug) { Main.logger_instance.Msg("Character_Blessings : Awake"); }
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Character_Blessings_Hooks.SelectBlessing();
            }
        }
    }
    public class Character_Blessings_Hooks
    {
        public static void SelectBlessing()
        {
            if (IsBlessingOpen())
            {
                if ((CanRun()) && (selected_slot != null)) { AddBlessingToCharacter(selected_slot.referenceBlessingID); }
                else if (selected_slot != null) { selected_slot = null; }
            }
        }

        private static readonly int base_id = 34;
        private static int timeline_id = -1;
        private static bool adding_blessings = false;
        private static InventoryBlessingSlotUI selected_slot = null;

        private static void AddBlessingToCharacter(int blessing_id)
        {
            ItemDataUnpacked item = CreateBlessing(blessing_id);
            if ((timeline_id > -1) && (IsBlessingDiscovered(blessing_id)) && (!Refs_Manager.player_data_tracker.IsNullOrDestroyed()) && (item != null)) //&& (!active_blessing_slot.lockedSlot.gameObject.active)
            {
                bool found = false;
                ushort container_id = (ushort)(timeline_id + 33);
                foreach (LE.Data.ItemLocationPair item_pair in Refs_Manager.player_data_tracker.charData.SavedItems)
                {
                    if (item_pair.ContainerID == container_id)
                    {
                        if (item_pair.Data.Count > 7)
                        {
                            if (item_pair.Data[1] == 34)
                            {
                                item_pair.Data[2] = (byte)blessing_id;
                                item_pair.Data[5] = item.implicitRolls[0];
                                item_pair.Data[6] = item.implicitRolls[1];
                                item_pair.Data[7] = item.implicitRolls[2];
                                found = true;
                                break;
                            }
                            else { Main.logger_instance.Msg("Not a Blessing"); }
                            break;
                        }
                    }
                }
                if (!found) { Refs_Manager.player_data_tracker.charData.SavedItems.Add(CreateBlessingData(item, container_id)); }
                Refs_Manager.player_data_tracker.charData.SaveData();
            }
            UpdateActiveSlot(item);
        }
        private static LE.Data.ItemLocationPair CreateBlessingData(ItemDataUnpacked item, ushort container_id)
        {
            Il2CppStructArray<byte> Data = new Il2CppStructArray<byte>(11);
            Data[0] = 2;
            Data[1] = item.itemType;
            Data[2] = (byte)item.subType;
            Data[3] = 0;
            Data[4] = 0;
            Data[5] = item.implicitRolls[0];
            Data[6] = item.implicitRolls[1];
            Data[7] = item.implicitRolls[2];
            Data[8] = 0;
            Data[9] = 0;
            Data[10] = 0;

            LE.Data.ItemLocationPair new_blessing = new LE.Data.ItemLocationPair
            {
                ContainerID = container_id,
                Data = Data,
                FormatVersion = 2,
                InventoryPosition = new LE.Data.ItemInventoryPosition(0, 0),
                Quantity = 1,
                TabID = 0
            };

            return new_blessing;
        }
        private static ItemDataUnpacked CreateBlessing(int blessing_id)
        {
            ItemDataUnpacked item = null;
            if (!Refs_Manager.item_list.IsNullOrDestroyed())
            {
                bool ItemList_found = false;
                int index = 0;
                foreach (ItemList.BaseEquipmentItem n_item in Refs_Manager.item_list.EquippableItems)
                {
                    if (n_item.baseTypeID == base_id) { ItemList_found = true; break; }
                    index++;
                }
                if (ItemList_found)
                {
                    foreach (ItemList.EquipmentItem eq_item in Refs_Manager.item_list.EquippableItems[index].subItems)
                    {
                        if (eq_item.subTypeID == blessing_id)
                        {
                            item = new ItemDataUnpacked
                            {
                                LvlReq = 0,
                                classReq = ItemList.ClassRequirement.Any,
                                itemType = (byte)base_id,
                                subType = (ushort)blessing_id,
                                rarity = (byte)0,
                                sockets = (byte)0,
                                uniqueID = (ushort)0
                            };
                            item.randomiseImplicitRolls();
                            item.RefreshIDAndValues();
                            break;
                        }
                    }
                }
            }
            else { Main.logger_instance.Error("Create Blessings"); }

            return item;
        }
        private static bool IsBlessingDiscovered(int id)
        {
            bool result = false;
            if (!Refs_Manager.player_data_tracker.IsNullOrDestroyed())
            {
                foreach (int discovered_id in Refs_Manager.player_data_tracker.charData.BlessingsDiscovered)
                {
                    if (id == discovered_id) { result = true; break; }
                }
            }

            return result;
        }
        private static void UpdateActiveSlot(ItemDataUnpacked item)
        {
            InventoryBlessingSlotUI active_blessing_slot = GetActiveSlot();
            if (active_blessing_slot != null) //Update Ui
            {
                if (active_blessing_slot.lockedSlot.gameObject.active) { active_blessing_slot.lockedSlot.gameObject.active = false; }
                OneSlotItemContainer one_slot_container = active_blessing_slot.blessingUIContainer.container.TryCast<OneSlotItemContainer>();
                one_slot_container.Clear();
                one_slot_container.TryAddItem(item, 1, Context.DEFAULT);
                active_blessing_slot.blessingUIContainer.forceUpdate = true;
            }
        }
        private static InventoryBlessingSlotUI GetActiveSlot()
        {
            InventoryBlessingSlotUI active_blessing_slot = null;
            foreach (InventoryBlessingSlotUI active_slot in InventoryPanelUI.instance.activeBlessingSlots)
            {
                if (active_slot.timelineID == timeline_id) { active_blessing_slot = active_slot; break; }
            }

            return active_blessing_slot;
        }

        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Character.Cheats.Enable_CanChooseBlessing;
                }
                else { return false; }
            }
            else { return false; }
        }
        public static bool IsBlessingOpen()
        {
            bool result = false;
            if (!Refs_Manager.BlessingsPanel.IsNullOrDestroyed())
            {
                result = Refs_Manager.BlessingsPanel.active;
            }
            return result;
        }
        public static void DiscoverAllBlessings()
        {
            if (!adding_blessings)
            {
                adding_blessings = true;
                if (!Refs_Manager.item_list.IsNullOrDestroyed())
                {
                    int base_id = 34;
                    int index = 0;
                    bool found = false;
                    foreach (ItemList.BaseEquipmentItem n_item in Refs_Manager.item_list.EquippableItems)
                    {
                        if (n_item.baseTypeID == base_id) { found = true; break; }
                        index++;
                    }
                    if ((found) && (!Refs_Manager.player_data_tracker.IsNullOrDestroyed()))
                    {
                        foreach (ItemList.EquipmentItem item in Refs_Manager.item_list.EquippableItems[index].subItems)
                        {
                            bool already_in_player = false;
                            foreach (int blessing_id in Refs_Manager.player_data_tracker.charData.BlessingsDiscovered)
                            {
                                if (blessing_id == item.subTypeID)
                                {
                                    already_in_player = true;
                                    break;
                                }
                            }
                            if (!already_in_player) { Refs_Manager.player_data_tracker.charData.BlessingsDiscovered.Add(item.subTypeID); }
                        }
                        Refs_Manager.player_data_tracker.charData.SaveData();
                    }
                }
                else { Main.logger_instance.Error("Discover all blessings"); }
                adding_blessings = false;
            }
        }

        [HarmonyPatch(typeof(InventoryPanelUI), "SelectTimelineForBlessingDisplay")]
        public class InventoryPanelUI_SelectTimelineForBlessingDisplay
        {
            [HarmonyPrefix]
            static void Postfix(InventoryPanelUI __instance, int __0)
            {
                if (CanRun()) { timeline_id = __0; }
            }
        }

        [HarmonyPatch(typeof(InventoryBlessingSlotUI), "UnityEngine_EventSystems_IPointerEnterHandler_OnPointerEnter")]
        public class InventoryBlessingSlotUI_UnityEngine_EventSystems_IPointerEnterHandler_OnPointerEnter
        {
            [HarmonyPrefix]
            static void Postfix(ref InventoryBlessingSlotUI __instance, UnityEngine.EventSystems.PointerEventData __0)
            {
                selected_slot = null;
                if (CanRun())
                {
                    if (__instance.blessingUIContainer.identifier.ToString() == "UNDEFINED") { selected_slot = __instance; }
                }
            }
        }

        [HarmonyPatch(typeof(InventoryBlessingSlotUI), "UnityEngine_EventSystems_IPointerExitHandler_OnPointerExit")]
        public class InventoryBlessingSlotUI_UnityEngine_EventSystems_IPointerExitHandler_OnPointerExit
        {
            [HarmonyPrefix]
            static void Postfix(InventoryBlessingSlotUI __instance, UnityEngine.EventSystems.PointerEventData __0)
            {
                if (CanRun()) { selected_slot = null; }
            }
        }
    }
}

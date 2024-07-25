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

        private readonly int base_id = 34;
        private readonly int base_container = 33;
        private InventoryBlessingSlotUI selected_active_slot = null;
        private InventoryBlessingSlotUI selected_discovered_slot = null;

        void Awake()
        {
            instance = this;
        }
        void Update()
        {
            if (IsBlessingOpen())
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (temp_selected_active_slot != null) { selected_active_slot = temp_selected_active_slot; }
                    else if ((temp_selected_discovered_slot != null) && (selected_active_slot != null))
                    {
                        selected_discovered_slot = temp_selected_discovered_slot;
                        int blessing_id = selected_discovered_slot.referenceBlessingID;
                        selected_discovered_slot = null;
                        ItemDataUnpacked item = CreateBlessing(blessing_id);
                        if ((timeline_id > -1) && (IsBlessingDiscovered(blessing_id)) && (!Refs_Manager.player_data_tracker.IsNullOrDestroyed()) && (item != null)) //&& (!active_blessing_slot.lockedSlot.gameObject.active)
                        {
                            bool found = false;
                            ushort container_id = (ushort)(timeline_id + base_container);
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
                        if (selected_active_slot.lockedSlot.gameObject.active) { selected_active_slot.lockedSlot.gameObject.active = false; }
                        OneSlotItemContainer one_slot_container = selected_active_slot.blessingUIContainer.container.TryCast<OneSlotItemContainer>();
                        one_slot_container.Clear();
                        one_slot_container.TryAddItem(item, 1, Context.DEFAULT);
                        selected_active_slot.blessingUIContainer.forceUpdate = true;
                    }
                }
            }
            else
            {
                timeline_id = -1;
                selected_active_slot = null;
                selected_discovered_slot = null;
            }
        }        
        ItemDataUnpacked CreateBlessing(int blessing_id)
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
        LE.Data.ItemLocationPair CreateBlessingData(ItemDataUnpacked item, ushort container_id)
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
        public static LE.Data.BlessingData CreateBlessingDataForSave(ushort subtype)
        {
            LE.Data.BlessingData result = new LE.Data.BlessingData
            {
                SubtypeId = subtype,
                ImplicitValue = UnityEngine.Random.Range(0f, 1f),
                ImplicitRollByte0 = (byte)UnityEngine.Random.Range(0f, 255f),
                ImplicitRollByte1 = (byte)UnityEngine.Random.Range(0f, 255f),
                ImplicitRollByte2 = (byte)UnityEngine.Random.Range(0f, 255f)
            };

            return result;
        }

        bool IsBlessingDiscovered(int id)
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

        private static int timeline_id = -1;
        private static bool adding_blessings = false;
        private static InventoryBlessingSlotUI temp_selected_active_slot = null;
        private static InventoryBlessingSlotUI temp_selected_discovered_slot = null;

        private static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()) && (IsBlessingOpen()))
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

                Hud_Manager.Hud_Base.Resume_Click(); //Close Hud

                //Unlock all timelines
                if (!ItemContainersManager.Instance.IsNullOrDestroyed())
                {
                    System.Collections.Generic.List<TimelineID> timelines_id = new System.Collections.Generic.List<TimelineID>();
                    timelines_id.Add(TimelineID.UndeadAbom);
                    timelines_id.Add(TimelineID.OsprixWithLance);
                    timelines_id.Add(TimelineID.VoidRahyeh);
                    timelines_id.Add(TimelineID.FrostLich);
                    timelines_id.Add(TimelineID.Lagon);
                    timelines_id.Add(TimelineID.UndeadVsVoid);
                    timelines_id.Add(TimelineID.Dragons);
                    timelines_id.Add(TimelineID.Gaspar);
                    timelines_id.Add(TimelineID.Heorot);
                    timelines_id.Add(TimelineID.Volcano);
                    
                    if (BlessingRewardPanelManager.instance.IsNullOrDestroyed())
                    {
                        BlessingRewardPanelManager.onOptionsPopulated(TimelineID.UndeadAbom, 0, 3);
                    }
                    if (!BlessingRewardPanelManager.instance.IsNullOrDestroyed())
                    {
                        GameObject ui = BlessingRewardPanelManager.instance.gameObject;
                        ui.active = true;
                        foreach (TimelineID t_id in timelines_id)
                        {
                            ItemContainersManager.Instance.populateBlessingOptions(t_id, 0, 3, 2);
                            BlessingRewardPanelManager.instance._selectedOption = 1;
                            BlessingRewardPanelManager.instance.ConfirmSelection();
                        }
                        ui.active = false;
                    }
                    else { Main.logger_instance.Error("BlessingRewardPanelManager.instance is null"); }
                }
                //Add all blessing
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
                            //Add BlessingsDiscovered
                            /*bool blessing_already_in_player = false;
                            foreach (int blessing_id in Refs_Manager.player_data_tracker.charData.BlessingsDiscovered)
                            {
                                if (blessing_id == item.subTypeID)
                                {
                                    blessing_already_in_player = true;
                                    break;
                                }
                            }
                            if (!blessing_already_in_player) { Refs_Manager.player_data_tracker.charData.BlessingsDiscovered.Add(item.subTypeID); }
                            */

                            //Add OpenBlessings
                            bool blessing_data_already_in_player = false;
                            foreach (LE.Data.BlessingData blessing_data in Refs_Manager.player_data_tracker.charData.OpenBlessings)
                            {
                                if (blessing_data.SubtypeId == item.subTypeID)
                                {
                                    blessing_data_already_in_player = true;
                                    break;
                                }
                            }
                            if (!blessing_data_already_in_player) { Refs_Manager.player_data_tracker.charData.OpenBlessings.Add(CreateBlessingDataForSave(System.Convert.ToUInt16(item.subTypeID))); }
                        }
                        Refs_Manager.player_data_tracker.charData.SaveData();
                    }
                    else { Main.logger_instance.Error("Blessings not found in itemlist"); }
                }
                else { Main.logger_instance.Error("ItemList is null"); }

                

                adding_blessings = false;
            }
        }

        [HarmonyPatch(typeof(InventoryPanelUI), "SelectTimelineForBlessingDisplay")]
        public class InventoryPanelUI_SelectTimelineForBlessingDisplay
        {
            [HarmonyPrefix]
            static void Postfix(int __0)
            {
                timeline_id = -1;
                if (CanRun())
                {
                    timeline_id = __0;
                }
            }
        }
        
        [HarmonyPatch(typeof(InventoryBlessingSlotUI), "UnityEngine_EventSystems_IPointerEnterHandler_OnPointerEnter")]
        public class InventoryBlessingSlotUI_UnityEngine_EventSystems_IPointerEnterHandler_OnPointerEnter
        {
            [HarmonyPrefix]
            static void Postfix(ref InventoryBlessingSlotUI __instance)
            {
                temp_selected_discovered_slot = null;
                temp_selected_active_slot = null;
                if (CanRun())
                {
                    string slot_name = __instance.gameObject.name;
                    if (slot_name.Contains("BlessingInventoryDisplayButton"))
                    {
                        temp_selected_discovered_slot = __instance;
                    }
                    else if ((slot_name.Contains("Blessing")) && (!slot_name.Contains("Inventory")))
                    {
                        temp_selected_active_slot = __instance;
                    }                    
                }
            }
        }

        [HarmonyPatch(typeof(InventoryBlessingSlotUI), "UnityEngine_EventSystems_IPointerExitHandler_OnPointerExit")]
        public class InventoryBlessingSlotUI_UnityEngine_EventSystems_IPointerExitHandler_OnPointerExit
        {
            [HarmonyPrefix]
            static void Postfix()
            {
                temp_selected_discovered_slot = null;
                temp_selected_active_slot = null;
            }
        }
    }
}

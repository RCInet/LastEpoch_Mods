using HarmonyLib;
using LastEpochMods.Managers;
using UnhollowerBaseLib;
using UnityEngine;

namespace LastEpochMods.Mods.Character
{
    public class Cheats
    {
        private static PlayerHealth player_base_health;
        private static HealthPotion health_potion;

        private static CharacterDataTracker character_data_tracker = null;
        private static ItemList item_list = null;

        public class LowLife
        {
            [HarmonyPatch(typeof(CharacterStats), "OnUpdateTick")]
            public class CharacterStats_UpdateStats
            {
                [HarmonyPostfix]
                static void Postfix(ref CharacterStats __instance, float __0)
                {
                    if ((Scenes_Manager.GameScene()) && (Save_Manager.Data.UserData.Character.Cheats.Enable_LowLife))
                    {
                        __instance.atLowHealth = true;
                        //__instance.TryCast<BaseStats>().atLowHealth = true;
                    }
                }
            }
        }
        public class CharStats
        {
            [HarmonyPatch(typeof(CharacterStats), "UpdateStats")]
            public class CharacterStats_UpdateStats
            {
                [HarmonyPostfix]
                static void Postfix(ref CharacterStats __instance)
                {
                    if ((Scenes_Manager.GameScene()) && (Save_Manager.Data.UserData.Character.Cheats.Enable_leech_rate))
                    {
                        __instance.increasedLeechRate = Save_Manager.Data.UserData.Character.Cheats.leech_rate;
                        //__instance.TryCast<BaseStats>().increasedLeechRate = Save_Manager.Data.UserData.Character.Cheats.leech_rate;
                    }
                }
            }
        }
        public class Masteries
        {
            public static bool IsMastered()
            {
                bool result = false;
                try
                {
                    if (PlayerFinder.getLocalTreeData().chosenMastery > 0) { result = true; }
                }
                catch { }

                return result;
            }
            public static void Reset()
            {
                PlayerFinder.getLocalTreeData().chosenMastery = 0;
                CharacterDataTracker DataTracker = PlayerFinder.getPlayerDataTracker();
                if (DataTracker != null)
                {
                    DataTracker.charData.ChosenMastery = 0;
                    DataTracker.charData.SaveData();
                }
            }
            public static void ChooseNewOne()
            {
                Managers.GUI_Manager.PauseMenu.Functions.Close();
                //UIBase.instance.openMasteryPanel(false);
                Managers.GUI_Manager.Base.Refs.Game_UIBase.openMasteryPanel(false);
            }
        }
        public class LevelUp
        {
            private static readonly int max_level = 100;
            public static bool AlreadyMaxLevel()
            {
                bool result = false;
                try
                {
                    if (PlayerFinder.getExperienceTracker().CurrentLevel == max_level) { result = true; }
                }
                catch { Main.logger_instance.Error("Character:Cheats -> Error trying to check character level"); }

                return result;
            }
            public static void Once()
            {
                try
                {
                    if (!AlreadyMaxLevel()) { PlayerFinder.getExperienceTracker().LevelUp(true); }
                    else { Main.logger_instance.Msg("Character:Cheats -> Character already max level"); }
                }
                catch { Main.logger_instance.Error("Character:Cheats -> Error trying to LevelUp"); }
            }
            public static void ToLevel(int level)
            {
                try
                {
                    ExperienceTracker exp_tracker = PlayerFinder.getExperienceTracker();
                    for (int i = exp_tracker.CurrentLevel; i < level; i++) { exp_tracker.LevelUp(true); }
                }
                catch { Main.logger_instance.Error("Character:Cheats -> Error trying to LevelUp to " + level); }
            }
            public static void ToMax()
            {
                try
                {
                    ExperienceTracker exp_tracker = PlayerFinder.getExperienceTracker();
                    for (int i = exp_tracker.CurrentLevel; i < max_level; i++) { exp_tracker.LevelUp(true); }
                }
                catch { Main.logger_instance.Error("Character:Cheats -> Error trying to LevelUp to " + max_level); }
            }
        }
        public class GodMode
        {
            public static void Update()
            {
                if ((Scenes_Manager.GameScene()) && (Save_Manager.Data.UserData.Character.Cheats.Enable_GodMode))
                {
                    try
                    {
                        if (player_base_health.IsNullOrDestroyed()) { player_base_health = PlayerFinder.getLocalPlayerHealth(); } //PlayerFinder.getLocalPlayerHealth().TryCast<BaseHealth>(); }
                        if (!player_base_health.IsNullOrDestroyed())
                        {
                            player_base_health.damageable = !Save_Manager.Data.UserData.Character.Cheats.Enable_GodMode;
                            player_base_health.canDie = !Save_Manager.Data.UserData.Character.Cheats.Enable_GodMode;
                        }
                    }
                    catch { }
                }
            }
        }
        public class AutoPot
        {
            public static void Update()
            {
                if ((Scenes_Manager.GameScene()) &&
                    (Save_Manager.Data.UserData.Character.Cheats.Enable_AutoPot))
                {
                    try
                    {
                        if (player_base_health.IsNullOrDestroyed()) { player_base_health = PlayerFinder.getLocalPlayerHealth(); } //.TryCast<BaseHealth>(); }
                        if (!player_base_health.IsNullOrDestroyed())
                        {
                            int player_health_percent = (int)(player_base_health.currentHealth / player_base_health.maxHealth * 100);
                            int auto_pot_percent = (int)(Save_Manager.Data.UserData.Character.Cheats.autoPot / 255 * 100);
                            if (player_health_percent < auto_pot_percent)
                            {
                                if (health_potion.IsNullOrDestroyed()) { health_potion = PlayerFinder.getPlayerActor().gameObject.GetComponent<HealthPotion>(); }
                                if (!health_potion.IsNullOrDestroyed()) { health_potion.UsePotion(); }
                            }
                        }
                    }
                    catch { }
                }
            }
        }
        public class Blessings
        {
            private static bool adding_blessings = false;
            public static void DiscoverAllBlessings()
            {
                if (!adding_blessings)
                {
                    adding_blessings = true;
                    try
                    {
                        if (!item_list.IsNullOrDestroyed())
                        {
                            int base_id = 34;
                            int index = 0;
                            bool found = false;
                            foreach (ItemList.BaseEquipmentItem n_item in item_list.EquippableItems)
                            {
                                if (n_item.baseTypeID == base_id) { found = true; break; }
                                index++;
                            }
                            if (found)
                            {
                                if (!character_data_tracker.IsNullOrDestroyed())
                                {
                                    foreach (ItemList.EquipmentItem item in item_list.EquippableItems[index].subItems)
                                    {
                                        bool already_in_player = false;
                                        foreach (int blessing_id in character_data_tracker.charData.BlessingsDiscovered)
                                        {
                                            if (blessing_id == item.subTypeID)
                                            {
                                                already_in_player = true;
                                                break;
                                            }
                                        }
                                        if (!already_in_player) { character_data_tracker.charData.BlessingsDiscovered.Add(item.subTypeID); }
                                    }
                                    character_data_tracker.charData.SaveData();
                                }
                                else { Main.logger_instance.Error("Character Data is null"); }
                            }
                        }
                        else { Main.logger_instance.Error("itemlist is null"); }
                    }
                    catch { }
                    adding_blessings = false;
                }
            }

            public class Select
            {
                private static readonly int base_id = 34;
                private static readonly int base_container = 33;
                private static InventoryBlessingSlotUI selected_active_slot = null;
                private static InventoryBlessingSlotUI selected_discovered_slot = null;
                
                public static void Update()
                {
                    if (character_data_tracker.IsNullOrDestroyed()) { character_data_tracker = PlayerFinder.localPlayerDataTracker; }
                    if (item_list.IsNullOrDestroyed()) { item_list = ItemList.instance; }

                    if (GUI_Manager.BlessingsPanel.Functions.IsBlessingOpen())
                    {
                        //if (Input.GetKeyDown(KeyCode.Mouse0))
                        //{
                            if (temp_selected_active_slot != null) { selected_active_slot = temp_selected_active_slot; }
                            else if ((temp_selected_discovered_slot != null) && (selected_active_slot != null))
                            {
                                selected_discovered_slot = temp_selected_discovered_slot;
                                int blessing_id = selected_discovered_slot.referenceBlessingID;
                                selected_discovered_slot = null;
                                ItemDataUnpacked item = CreateBlessing(blessing_id);
                                if ((timeline_id > -1) && (IsBlessingDiscovered(blessing_id)) && (!character_data_tracker.IsNullOrDestroyed()) && (item != null)) //&& (!active_blessing_slot.lockedSlot.gameObject.active)
                                {
                                    bool found = false;
                                    ushort container_id = (ushort)(timeline_id + base_container);
                                    foreach (LE.Data.ItemLocationPair item_pair in character_data_tracker.charData.SavedItems)
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
                                    if (!found) { character_data_tracker.charData.SavedItems.Add(CreateBlessingData(item, container_id)); }
                                    character_data_tracker.charData.SaveData();
                                }
                                if (selected_active_slot.lockedSlot.gameObject.active) { selected_active_slot.lockedSlot.gameObject.active = false; }
                                OneSlotItemContainer one_slot_container = selected_active_slot.blessingUIContainer.container.TryCast<OneSlotItemContainer>();
                                one_slot_container.Clear();
                                one_slot_container.TryAddItem(item, 1, Context.DEFAULT);
                                selected_active_slot.blessingUIContainer.forceUpdate = true;
                            }
                        //}
                    }
                    else
                    {
                        timeline_id = -1;
                        selected_active_slot = null;
                        selected_discovered_slot = null;
                    }
                }
                public static ItemDataUnpacked CreateBlessing(int blessing_id)
                {
                    ItemDataUnpacked item = null;
                    if (!item_list.IsNullOrDestroyed())
                    {
                        bool ItemList_found = false;
                        int index = 0;
                        foreach (ItemList.BaseEquipmentItem n_item in item_list.EquippableItems)
                        {
                            if (n_item.baseTypeID == base_id) { ItemList_found = true; break; }
                            index++;
                        }
                        if (ItemList_found)
                        {
                            foreach (ItemList.EquipmentItem eq_item in item_list.EquippableItems[index].subItems)
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
                public static LE.Data.ItemLocationPair CreateBlessingData(ItemDataUnpacked item, ushort container_id)
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
                        ImplicitValue = UnityEngine.Random.Range(0f, 255f),
                        ImplicitRollByte0 = (byte)UnityEngine.Random.Range(0f, 255f),
                        ImplicitRollByte1 = (byte)UnityEngine.Random.Range(0f, 255f),
                        ImplicitRollByte2 = (byte)UnityEngine.Random.Range(0f, 255f)
                    };

                    return result;
                }

                public static bool IsBlessingDiscovered(int id)
                {
                    bool result = false;
                    if (!character_data_tracker.IsNullOrDestroyed())
                    {
                        foreach (int discovered_id in character_data_tracker.charData.BlessingsDiscovered)
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
                    if ((Scenes_Manager.GameScene()) && (GUI_Manager.BlessingsPanel.Functions.IsBlessingOpen()))
                    {
                        return Save_Manager.Data.UserData.Character.Cheats.Enable_ChooseBlessingFromBlessingPanel;
                    }
                    else { return false; }
                }
                public static void DiscoverAllBlessings()
                {
                    if (!adding_blessings)
                    {
                        adding_blessings = true;
                        if (!item_list.IsNullOrDestroyed())
                        {
                            int base_id = 34;
                            int index = 0;
                            bool found = false;
                            foreach (ItemList.BaseEquipmentItem n_item in item_list.EquippableItems)
                            {
                                if (n_item.baseTypeID == base_id) { found = true; break; }
                                index++;
                            }
                            if ((found) && (!character_data_tracker.IsNullOrDestroyed()))
                            {
                                foreach (ItemList.EquipmentItem item in item_list.EquippableItems[index].subItems)
                                {
                                    //Add BlessingsDiscovered
                                    bool blessing_already_in_player = false;
                                    foreach (int blessing_id in character_data_tracker.charData.BlessingsDiscovered)
                                    {
                                        if (blessing_id == item.subTypeID)
                                        {
                                            blessing_already_in_player = true;
                                            break;
                                        }
                                    }
                                    if (!blessing_already_in_player) { character_data_tracker.charData.BlessingsDiscovered.Add(item.subTypeID); }

                                    //Add OpenBlessings
                                    bool blessing_data_already_in_player = false;
                                    foreach (LE.Data.BlessingData blessing_data in character_data_tracker.charData.OpenBlessings)
                                    {
                                        if (blessing_data.SubtypeId == item.subTypeID)
                                        {
                                            blessing_data_already_in_player = true;
                                            break;
                                        }
                                    }
                                    if (!blessing_data_already_in_player) { character_data_tracker.charData.OpenBlessings.Add(CreateBlessingDataForSave(System.Convert.ToUInt16(item.subTypeID))); }
                                }
                                character_data_tracker.charData.SaveData();
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
        public class Materials
        {
            public static void GetAllRunesX99()
            {
                try
                {
                    int item_type = 102;
                    foreach (ItemList.NonEquipmentItem item in GetItemList(item_type).subItems)
                    {
                        ForceDrop(item_type, item.subTypeID, 99);
                    }
                }
                catch { }
            }
            public static void GetAllGlyphsX99()
            {
                try
                {
                    int item_type = 103;
                    foreach (ItemList.NonEquipmentItem item in GetItemList(item_type).subItems)
                    {
                        ForceDrop(item_type, item.subTypeID, 99);
                    }
                }
                catch { }
            }
            public static void GetAllShardsX10()
            {
                try
                {
                    int item_type = 101;
                    foreach (ItemList.NonEquipmentItem item in GetItemList(item_type).subItems)
                    {
                        ForceDrop(item_type, item.subTypeID, 10);
                    }
                }
                catch { }
            }

            private static GroundItemManager ground_item_manager = null;
            private static void Get_GroundItemManager()
            {
                try
                {
                    //ground_item_manager = GroundItemManager.instance;
                    foreach (Object obj in Object.FindObjectsOfType<GroundItemManager>())
                    {
                        ground_item_manager = obj.TryCast<GroundItemManager>();
                        break;
                    }
                }
                catch { }
            }
            private static void ForceDrop(int type, int subtype, int quantity)
            {
                try
                {
                    if (ground_item_manager.IsNullOrDestroyed()) { Get_GroundItemManager(); }
                    if (!ground_item_manager.IsNullOrDestroyed())
                    {
                        Actor player = null;
                        try { player = PlayerFinder.getPlayerActor(); }
                        catch { }
                        if (player != null)
                        {
                            ItemDataUnpacked item = new ItemDataUnpacked
                            {
                                LvlReq = 0,
                                classReq = ItemList.ClassRequirement.Any,
                                itemType = (byte)type,
                                subType = (ushort)subtype,
                                rarity = (byte)0,
                                sockets = (byte)0,
                                uniqueID = (ushort)0
                            };
                            item.RefreshIDAndValues();
                            ItemData final_item = item.TryCast<ItemData>();
                            bool backup_autopickup_mat = Save_Manager.Data.UserData.Items.AutoPickup.AutoPickup_Materials;
                            Save_Manager.Data.UserData.Items.AutoPickup.AutoPickup_Materials = true;
                            bool backup_autostore_mat = Save_Manager.Data.UserData.Items.AutoPickup.AutoStore_Materials;
                            Save_Manager.Data.UserData.Items.AutoPickup.AutoStore_Materials = true;
                            for (int i = 0; i < quantity; i++)
                            {
                                ground_item_manager.dropItemForPlayer(player, final_item, player.position(), false);
                            }
                            Save_Manager.Data.UserData.Items.AutoPickup.AutoPickup_Materials = backup_autopickup_mat;
                            Save_Manager.Data.UserData.Items.AutoPickup.AutoStore_Materials = backup_autostore_mat;
                        }
                        else { Main.logger_instance.Error("Player Not Found"); }
                    }
                    else { Main.logger_instance.Error("Ground Item Manager Not Found"); }
                }
                catch { }
            }
            private static ItemList.BaseNonEquipmentItem GetItemList(int type_id)
            {
                bool found = false;
                int index = 0;
                if (!ItemList.instance.IsNullOrDestroyed())
                {
                    foreach (ItemList.BaseNonEquipmentItem n_item in ItemList.instance.nonEquippableItems)
                    {
                        if (n_item.baseTypeID == type_id) { found = true; break; }
                        index++;
                    }
                }
                else { Main.logger_instance.Msg("Itemlist is null"); }
                if (found) { return ItemList.instance.nonEquippableItems[index]; }
                else
                {
                    Main.logger_instance.Msg("List with type = " + type_id + " not found");
                    return null;
                }
            }
        }
    }
}

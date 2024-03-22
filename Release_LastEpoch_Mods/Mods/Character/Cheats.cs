using HarmonyLib;
using LastEpochMods.Managers;
using UnhollowerBaseLib;
using UnityEngine;

namespace LastEpochMods.Mods.Character
{
    public class Cheats
    {        
        public class LowLife
        {
            [HarmonyPatch(typeof(CharacterStats), "OnUpdateTick")]
            public class CharacterStats_UpdateStats
            {
                [HarmonyPostfix]
                static void Postfix(ref CharacterStats __instance, float __0)
                {
                    try
                    {
                        if (Save_Manager.Data.UserData.Character.Cheats.Enable_LowLife)
                        {
                            __instance.TryCast<BaseStats>().atLowHealth = true;
                        }
                    }
                    catch { Main.logger_instance.Error("LowLife CharacterStats:OnUpdateTick"); }
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
                    try
                    {
                        if (Scenes_Manager.GameScene())
                        {
                            /*if (Save_Manager.Data.UserData.Character.Cheats.Enable_attack_rate)
                            {
                                __instance.attackRate = Save_Manager.Data.UserData.Character.Cheats.attack_rate;
                            }*/
                            if (Save_Manager.Data.UserData.Character.Cheats.Enable_leech_rate)
                            {
                                __instance.TryCast<BaseStats>().increasedLeechRate = Save_Manager.Data.UserData.Character.Cheats.leech_rate;
                            }
                            
                        }
                    }
                    catch { Main.logger_instance.Error("CharacterStats:UpdateStats Error"); }
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
                if (Managers.Scenes_Manager.GameScene())
                {
                    try
                    {
                        BaseHealth Health = PlayerFinder.getLocalPlayerHealth();
                        Health.damageable = !Save_Manager.Data.UserData.Character.Cheats.Enable_GodMode;
                        Health.canDie = !Save_Manager.Data.UserData.Character.Cheats.Enable_GodMode;
                    }
                    catch { Main.logger_instance.Error("GodMode Error -> can't get Local_Player_Health"); }
                }
            }
        }
        public class AutoPot
        {
            public static void Update()
            {
                if (Managers.Scenes_Manager.GameScene())
                {
                    try
                    {
                        if (Managers.Save_Manager.Data.UserData.Character.Cheats.Enable_AutoPot)
                        {
                            BaseHealth player_base_health = PlayerFinder.getLocalPlayerHealth().TryCast<BaseHealth>();
                            int player_health_percent = (int)(player_base_health.currentHealth / player_base_health.maxHealth * 100);
                            int auto_pot_percent = (int)(Managers.Save_Manager.Data.UserData.Character.Cheats.autoPot / 255 * 100);
                            if (player_health_percent < auto_pot_percent)
                            {
                                PlayerFinder.getPlayerActor().gameObject.GetComponent<HealthPotion>().UsePotion();
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
                        if (ItemList.instance != null)
                        {
                            int base_id = 34;
                            int index = 0;
                            bool found = false;
                            foreach (ItemList.BaseEquipmentItem n_item in ItemList.instance.EquippableItems)
                            {
                                if (n_item.baseTypeID == base_id) { found = true; break; }
                                index++;
                            }
                            if (found)
                            {
                                if (PlayerFinder.localPlayerDataTracker.charData != null)
                                {
                                    LE.Data.CharacterData character_data = PlayerFinder.localPlayerDataTracker.charData;
                                    foreach (ItemList.EquipmentItem item in ItemList.instance.EquippableItems[index].subItems)
                                    {
                                        bool already_in_player = false;
                                        foreach (int blessing_id in character_data.BlessingsDiscovered)
                                        {
                                            if (blessing_id == item.subTypeID)
                                            {
                                                already_in_player = true;
                                                break;
                                            }
                                        }
                                        if (!already_in_player) { character_data.BlessingsDiscovered.Add(item.subTypeID); }
                                    }
                                    character_data.SaveData();
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
                private static int timeline_id = -1;
                private static InventoryBlessingSlotUI selected_slot = null;
                private static int base_id = 34;

                public static void Update()
                {
                    if (Save_Manager.Data.UserData.Character.Cheats.Enable_ChooseBlessingFromBlessingPanel)
                    {
                        if (Scenes_Manager.GameScene())
                        {

                            if (!GUI_Manager.BlessingsPanel.Functions.IsBlessingOpen()) { selected_slot = null; }
                            if (selected_slot != null)
                            {
                                if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Mouse0)) { AddBlessingToCharacter(selected_slot.referenceBlessingID); }
                            }
                        }
                        else { selected_slot = null; }
                    }
                }

                private static void AddBlessingToCharacter(int blessing_id)
                {
                    try
                    {
                        ItemDataUnpacked item = CreateBlessing(blessing_id);
                        CharacterDataTracker character_data_tracker = PlayerFinder.localPlayerDataTracker;
                        LE.Data.CharacterData character_data = character_data_tracker.charData;
                        if ((timeline_id > -1) && (IsBlessingDiscovered(blessing_id)) && (character_data != null) && (item != null)) //&& (!active_blessing_slot.lockedSlot.gameObject.active)
                        {
                            bool found = false;
                            ushort container_id = (ushort)(timeline_id + 33);
                            foreach (LE.Data.ItemLocationPair item_pair in character_data.SavedItems)
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
                            if (!found) { character_data.SavedItems.Add(CreateBlessingData(item, container_id)); }
                            character_data.SaveData();
                        }
                        UpdateActiveSlot(item);
                    }
                    catch { }
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
                    try
                    {
                        if (ItemList.instance != null)
                        {
                            bool ItemList_found = false;
                            int index = 0;
                            foreach (ItemList.BaseEquipmentItem n_item in ItemList.instance.EquippableItems)
                            {
                                if (n_item.baseTypeID == base_id) { ItemList_found = true; break; }
                                index++;
                            }
                            if (ItemList_found)
                            {
                                foreach (ItemList.EquipmentItem eq_item in ItemList.instance.EquippableItems[index].subItems)
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
                        else { Main.logger_instance.Error("ItemList is null"); }
                    }
                    catch { }

                    return item;
                }
                private static bool IsBlessingDiscovered(int id)
                {
                    bool result = false;
                    try
                    {
                        if (PlayerFinder.localPlayerDataTracker != null)
                        {
                            foreach (int discovered_id in PlayerFinder.localPlayerDataTracker.charData.BlessingsDiscovered)
                            {
                                if (id == discovered_id) { result = true; break; }
                            }
                        }
                    }
                    catch { }

                    return result;
                }
                private static void UpdateActiveSlot(ItemDataUnpacked item)
                {
                    try
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
                    catch { }
                }
                private static InventoryBlessingSlotUI GetActiveSlot()
                {
                    InventoryBlessingSlotUI active_blessing_slot = null;
                    try
                    {
                        foreach (InventoryBlessingSlotUI active_slot in GUI_Manager.InventoryPanel.Refs.InventoryPanelUI.activeBlessingSlots)
                        {
                            if (active_slot.timelineID == timeline_id) { active_blessing_slot = active_slot; break; }
                        }
                    }
                    catch { }

                    return active_blessing_slot;
                }

                [HarmonyPatch(typeof(InventoryPanelUI), "SelectTimelineForBlessingDisplay")]
                public class InventoryPanelUI_SelectTimelineForBlessingDisplay
                {
                    [HarmonyPrefix]
                    static void Postfix(InventoryPanelUI __instance, int __0)
                    {
                        try
                        {
                            if (Save_Manager.Data.UserData.Character.Cheats.Enable_ChooseBlessingFromBlessingPanel)
                            {
                                if (Scenes_Manager.GameScene()) { timeline_id = __0; }
                            }
                        }
                        catch { }
                    }
                }

                [HarmonyPatch(typeof(InventoryBlessingSlotUI), "UnityEngine_EventSystems_IPointerEnterHandler_OnPointerEnter")]
                public class InventoryBlessingSlotUI_UnityEngine_EventSystems_IPointerEnterHandler_OnPointerEnter
                {
                    [HarmonyPrefix]
                    static void Postfix(ref InventoryBlessingSlotUI __instance, UnityEngine.EventSystems.PointerEventData __0)
                    {
                        selected_slot = null;
                        try
                        {
                            if (Save_Manager.Data.UserData.Character.Cheats.Enable_ChooseBlessingFromBlessingPanel)
                            {
                                if ((Scenes_Manager.GameScene()) && (__instance != null))
                                {
                                    if (__instance.blessingUIContainer.identifier.ToString() == "UNDEFINED") { selected_slot = __instance; }
                                    //else { selected_slot = null; }
                                }
                            }
                        }
                        catch { }
                    }
                }

                [HarmonyPatch(typeof(InventoryBlessingSlotUI), "UnityEngine_EventSystems_IPointerExitHandler_OnPointerExit")]
                public class InventoryBlessingSlotUI_UnityEngine_EventSystems_IPointerExitHandler_OnPointerExit
                {
                    [HarmonyPrefix]
                    static void Postfix(InventoryBlessingSlotUI __instance, UnityEngine.EventSystems.PointerEventData __0)
                    {
                        try
                        {
                            if (Save_Manager.Data.UserData.Character.Cheats.Enable_ChooseBlessingFromBlessingPanel)
                            {
                                if ((Scenes_Manager.GameScene()) && (__instance != null))
                                {
                                    selected_slot = null;
                                }
                            }
                        }
                        catch { }
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

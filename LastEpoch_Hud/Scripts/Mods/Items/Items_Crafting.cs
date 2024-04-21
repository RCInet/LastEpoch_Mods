using HarmonyLib;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_Crafting
    {
        public class Current
        {
            public static ItemData item = null;
            public static AffixSlotForge slot = null;
            public static int affix_id = -1;
            public static int affix_tier = -1;
        }
        public class Get
        {
            public static void CurrentFromCraftingManager(ref Il2CppSystem.Object __0)
            {
                if (Crafting_Main_Item_Container.container.IsNullOrDestroyed()) { Crafting_Main_Item_Container.container = __0.TryCast<OneItemContainer>(); }
                if ((!Crafting_Main_Item_Container.container.IsNullOrDestroyed()) && (Current.item.IsNullOrDestroyed()))
                {
                    if (!Crafting_Main_Item_Container.container.content.IsNullOrDestroyed())
                    {
                        Current.item = Crafting_Main_Item_Container.container.content.data;
                    }
                }
            }
            public static void CurrentFomSlotManager()
            {
                if ((Crafting_Main_Item_Container.container.IsNullOrDestroyed()) && (!Scripts.Refs_Manager.craft_slot_manager.IsNullOrDestroyed()))
                {
                    Crafting_Main_Item_Container.container = Scripts.Refs_Manager.craft_slot_manager.main;
                }
                if ((!Crafting_Main_Item_Container.container.IsNullOrDestroyed()) && (Current.item.IsNullOrDestroyed()))
                {
                    if (!Crafting_Main_Item_Container.container.content.IsNullOrDestroyed())
                    {
                        Current.item = Crafting_Main_Item_Container.container.content.data;
                    }
                }
            }
            public static int TierFromItemData(ItemData item_data, int affix_id)
            {
                int result = -1;
                if (!item_data.IsNullOrDestroyed())
                {
                    foreach (ItemAffix affix in item_data.affixes)
                    {
                        if (affix.affixId == affix_id)
                        {
                            result = affix.affixTier;
                            break;
                        }
                    }
                }

                return result;
            }
            public static bool IsIdol(ItemData item)
            {
                bool result = false;
                if ((item.itemType > 24) && (item.itemType < 34)) { result = true; }
                
                return result;
            }
        }
        public class Add
        {
            public static void Affix(ref CraftingMaterialsPanelUI craft_mat_panel, AffixList.Affix affix, BaseStats.ModType affix_modifier)
            {
                GameObject obj = Object.Instantiate(craft_mat_panel.shardAffixPrefab, Vector3.zero, Quaternion.identity);
                ShardAffixListElement element = obj.GetComponent<ShardAffixListElement>();
                if (!element.IsNullOrDestroyed())
                {
                    element.affix = affix;
                    element.affixTitle = affix.affixTitle;
                    if (affix.type == AffixList.AffixType.SUFFIX)
                    {
                        element.affixType = "Suffix";
                        element.prefixArrow.active = false;
                        element.suffixArrow.active = true;
                    }
                    else
                    {
                        element.affixType = "Prefix";
                        element.prefixArrow.active = true;
                        element.suffixArrow.active = false;
                    }                    
                    element.shardItemName = affix.affixName;
                    element.shardName = affix.affixDisplayName;
                    element.shardType = affix.affixId;
                    element.improvementTMP.text = affix_modifier.ToString();
                    element.init = true;
                    craft_mat_panel.shardAffixList.Add(element);
                }
            }
        }
        public class Crafting_Manager
        {
            public static bool EditingItem = false;
            public static bool first_time = true;

            //Select Item
            [HarmonyPatch(typeof(CraftingManager), "OnMainItemChange")]
            public class CraftingManager_OnMainItemChange
            {
                [HarmonyPostfix]
                static void Postfix(ref CraftingManager __instance, ref Il2CppSystem.Object __0, ref ItemContainerEntryHandler __1)
                {
                    //if (Main.debug) { Main.logger_instance.Msg("CraftingManager : OnMainItemChange : Postfix"); }
                    if ((Scenes.IsGameScene()) && (!EditingItem))
                    {
                        Get.CurrentFromCraftingManager(ref __0);
                        if ((!Current.item.IsNullOrDestroyed()) && (first_time) &&
                            (!Save_Manager.instance.IsNullOrDestroyed()))
                        {
                            first_time = false;
                            EditingItem = true;
                            if (!Save_Manager.instance.data.IsNullOrDestroyed())
                            {
                                if (Save_Manager.instance.data.Items.CraftingSlot.Enable_ForginPotencial)
                                {
                                    Current.item.forgingPotential = (byte)Save_Manager.instance.data.Items.CraftingSlot.ForginPotencial;
                                }

                                System.Collections.Generic.List<bool> implicits_enables = new System.Collections.Generic.List<bool>();
                                implicits_enables.Add(Save_Manager.instance.data.Items.CraftingSlot.Enable_Implicit_0);
                                implicits_enables.Add(Save_Manager.instance.data.Items.CraftingSlot.Enable_Implicit_1);
                                implicits_enables.Add(Save_Manager.instance.data.Items.CraftingSlot.Enable_Implicit_2);
                                System.Collections.Generic.List<float> implicits_values = new System.Collections.Generic.List<float>();
                                implicits_values.Add(Save_Manager.instance.data.Items.CraftingSlot.Implicit_0);
                                implicits_values.Add(Save_Manager.instance.data.Items.CraftingSlot.Implicit_1);
                                implicits_values.Add(Save_Manager.instance.data.Items.CraftingSlot.Implicit_2);

                                for (int z = 0; z < Current.item.implicitRolls.Count; z++)
                                {
                                    if (implicits_enables[z]) { Current.item.implicitRolls[z] = (byte)implicits_values[z]; }
                                }
                                implicits_enables.Clear();
                                implicits_values.Clear();

                                System.Collections.Generic.List<bool> affix_tier_enables = new System.Collections.Generic.List<bool>();
                                affix_tier_enables.Add(Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_0_Tier);
                                affix_tier_enables.Add(Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_1_Tier);
                                affix_tier_enables.Add(Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_2_Tier);
                                affix_tier_enables.Add(Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_3_Tier);
                                System.Collections.Generic.List<float> affix_tier_values = new System.Collections.Generic.List<float>();
                                affix_tier_values.Add(Save_Manager.instance.data.Items.CraftingSlot.Affix_0_Tier);
                                affix_tier_values.Add(Save_Manager.instance.data.Items.CraftingSlot.Affix_1_Tier);
                                affix_tier_values.Add(Save_Manager.instance.data.Items.CraftingSlot.Affix_2_Tier);
                                affix_tier_values.Add(Save_Manager.instance.data.Items.CraftingSlot.Affix_3_Tier);
                                System.Collections.Generic.List<bool> affix_value_enables = new System.Collections.Generic.List<bool>();
                                affix_value_enables.Add(Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_0_Value);
                                affix_value_enables.Add(Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_1_Value);
                                affix_value_enables.Add(Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_2_Value);
                                affix_value_enables.Add(Save_Manager.instance.data.Items.CraftingSlot.Enable_Affix_3_Value);
                                System.Collections.Generic.List<float> affix_value_values = new System.Collections.Generic.List<float>();
                                affix_value_values.Add(Save_Manager.instance.data.Items.CraftingSlot.Affix_0_Value);
                                affix_value_values.Add(Save_Manager.instance.data.Items.CraftingSlot.Affix_1_Value);
                                affix_value_values.Add(Save_Manager.instance.data.Items.CraftingSlot.Affix_2_Value);
                                affix_value_values.Add(Save_Manager.instance.data.Items.CraftingSlot.Affix_3_Value);

                                int k = 0;
                                for (int z = 0; z < Current.item.affixes.Count; z++)
                                {
                                    if (Current.item.affixes[z].isSealedAffix)
                                    {
                                        if (Save_Manager.instance.data.Items.CraftingSlot.Enable_Seal_Tier) { Current.item.affixes[z].affixTier = (byte)Scripts.Save_Manager.instance.data.Items.CraftingSlot.Seal_Tier; }
                                        if (Save_Manager.instance.data.Items.CraftingSlot.Enable_Seal_Value) { Current.item.affixes[z].affixRoll = (byte)Scripts.Save_Manager.instance.data.Items.CraftingSlot.Seal_Value; }
                                    }
                                    else if ((k < affix_tier_enables.Count) && (k < affix_tier_values.Count) &&
                                        (k < affix_value_enables.Count) && (k < affix_value_values.Count))
                                    {
                                        if (affix_tier_enables[k]) { Current.item.affixes[z].affixTier = (byte)affix_tier_values[k]; }
                                        if (affix_value_enables[k]) { Current.item.affixes[z].affixRoll = (byte)affix_value_values[k]; }
                                        k++;
                                    }
                                }
                                affix_tier_enables.Clear();
                                affix_tier_values.Clear();
                                affix_value_enables.Clear();
                                affix_value_values.Clear();

                                if (Current.item.rarity > 6)
                                {
                                    System.Collections.Generic.List<bool> unique_mods_enables = new System.Collections.Generic.List<bool>();
                                    unique_mods_enables.Add(Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_0);
                                    unique_mods_enables.Add(Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_1);
                                    unique_mods_enables.Add(Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_2);
                                    unique_mods_enables.Add(Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_3);
                                    unique_mods_enables.Add(Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_4);
                                    unique_mods_enables.Add(Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_5);
                                    unique_mods_enables.Add(Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_6);
                                    unique_mods_enables.Add(Save_Manager.instance.data.Items.CraftingSlot.Enable_UniqueMod_7);
                                    System.Collections.Generic.List<float> unique_mods_values = new System.Collections.Generic.List<float>();
                                    unique_mods_values.Add(Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_0);
                                    unique_mods_values.Add(Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_1);
                                    unique_mods_values.Add(Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_2);
                                    unique_mods_values.Add(Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_3);
                                    unique_mods_values.Add(Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_4);
                                    unique_mods_values.Add(Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_5);
                                    unique_mods_values.Add(Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_6);
                                    unique_mods_values.Add(Save_Manager.instance.data.Items.CraftingSlot.UniqueMod_7);
                                    for (int z = 0; z < Current.item.uniqueRolls.Count; z++)
                                    {
                                        if (unique_mods_enables[z]) { Current.item.uniqueRolls[z] = (byte)unique_mods_values[z]; }
                                    }
                                    unique_mods_enables.Clear();
                                    unique_mods_values.Clear();

                                    if (Save_Manager.instance.data.Items.CraftingSlot.Enable_LegendaryPotencial)
                                    { Current.item.legendaryPotential = (byte)Save_Manager.instance.data.Items.CraftingSlot.LegendaryPotencial; }

                                    if (Save_Manager.instance.data.Items.CraftingSlot.Enable_WeaverWill)
                                    { Current.item.weaversWill = (byte)Save_Manager.instance.data.Items.CraftingSlot.WeaverWill; }
                                }

                                Current.item.RefreshIDAndValues();
                            }
                            EditingItem = false;
                        }
                    }
                }
            }

            [HarmonyPatch(typeof(CraftingManager), "OnMainItemRemoved")]
            public class CraftingManager_OnMainItemRemoved
            {
                [HarmonyPostfix]
                static void Postfix(CraftingManager __instance, Il2CppSystem.Object __0, ItemContainerEntryHandler __1)
                {
                    //if (Main.debug) { Main.logger_instance.Msg("CraftingManager : OnMainItemRemoved : Postfix"); }
                    Current.item = null;
                    Crafting_Main_Item_Container.container = null;
                    Current.affix_id = -1;
                    Current.slot = null;
                    Current.affix_tier = -1;
                    Crafting_Upgrade_Button.btn = null;
                    first_time = true;
                }
            }

            //Unlock Craft
            [HarmonyPatch(typeof(CraftingManager), "CheckForgeCapability")]
            public class CheckForgeCapability
            {
                [HarmonyPostfix]
                static void Postfix(ref CraftingManager __instance, ref bool __result, ref System.String __0, ref System.Boolean __1)
                {
                    //if (Main.debug) { Main.logger_instance.Msg("CraftingManager : CheckForgeCapability : Postfix : str = " + __0); }
                    if ((Scenes.IsGameScene()) && (!Refs_Manager.craft_slot_manager.IsNullOrDestroyed()))
                    {
                        Get.CurrentFomSlotManager();
                        if (!Current.item.IsNullOrDestroyed())
                        {
                            if (__0.ToLower().Contains("uniques"))// (__0 == "Can't Forge Uniques") //Can't Forge Uniques
                            {
                                //Check tier maxed
                                Refs_Manager.craft_slot_manager.prefixFullOfMax = false;
                                Refs_Manager.craft_slot_manager.suffixFullOfMax = false;
                                Refs_Manager.craft_slot_manager.canForge = true;
                                __0 = "Legendary Craft";
                                __1 = false;
                                __result = true;
                            }
                            /*else if (__0 == "No Forging Potential")
                            {                    
                                __0 = "No Cost";
                                __1 = false;
                                __result = true;
                            }*/
                            else if (__0 == "Affix is Maxed")
                            {
                                int tier = -1;
                                foreach (var affix in Current.item.affixes)
                                {
                                    if (affix.affixId == Current.affix_id)
                                    {
                                        tier = affix.affixTier;
                                        break;
                                    }
                                }
                                if ((tier > -1) && (tier < 6))
                                {
                                    if (tier == 4) { __0 = "Forge T6"; }
                                    else if (tier == 5) { __0 = "Forge T7"; }
                                    Refs_Manager.craft_slot_manager.prefixFullOfMax = false;
                                    Refs_Manager.craft_slot_manager.suffixFullOfMax = false;
                                    __1 = false;
                                    __result = true;
                                }
                            }
                            //No forgin Potencial Cost
                            //__instance.debugNoForgingPotentialCost = true;
                        }
                    }
                }
            }
        }
        public class Crafting_Main_Ui
        {
            public static CraftingMainUI main_ui = null;

            [HarmonyPatch(typeof(CraftingMainUI), "Initialize")]
            public class CraftingMainUI_Initialize
            {
                [HarmonyPostfix]
                static void Postifx(ref CraftingMainUI __instance, ref bool __result)
                {
                    main_ui = __instance;
                }
            }
        }
        public class Crafting_Main_Item_Container
        {
            public static OneItemContainer container = null;
            public static RectTransform rect_transform = null;
            public static Vector2 default_sizedelta = Vector2.zero;
            public static bool backup_initialized = false;

            //Idols can be added to slot (Fix item size)
            [HarmonyPatch(typeof(CraftingMainItemContainer), "CanReceiveItem")]
            public class CraftingMainItemContainer_CanReceiveItem
            {
                [HarmonyPostfix]
                static void Postifx(ref CraftingMainItemContainer __instance, ref bool __result, ItemData __0, int __1)
                {
                    if (Scenes.IsGameScene())
                    {
                        if ((!backup_initialized) && (!Crafting_Main_Ui.main_ui.IsNullOrDestroyed()))
                        {
                            rect_transform = Crafting_Main_Ui.main_ui.gameObject.GetComponent<RectTransform>();
                            default_sizedelta = rect_transform.sizeDelta;
                            backup_initialized = true;
                        }
                        if ((backup_initialized) && (!rect_transform.IsNullOrDestroyed()))
                        {
                            float slots_w = 2;
                            if (__0.itemType == 29) { slots_w = 3; } //Grand Idols
                            else if (__0.itemType == 31) { slots_w = 4; } //Ornate Idols
                            __instance.size = new Vector2Int((int)slots_w, 4); //Set slot size
                            rect_transform.sizeDelta = new Vector2(((slots_w / 2) * default_sizedelta.x), default_sizedelta.y); //Set size delta
                        }
                        if (Get.IsIdol(__0)) { __result = true; } //Allow Idols
                    }
                }
            }
        }
        public class Crafting_Upgrade_Button
        {
            public static CraftingUpgradeButton btn = null;

            //Unlock Craft Button when tier < T7
            [HarmonyPatch(typeof(CraftingUpgradeButton), "UpdateButton")]
            public class CraftingUpgradeButton_UpdateButton
            {
                [HarmonyPrefix]
                static void Prefix(CraftingUpgradeButton __instance, int __0, ref bool __1)
                {
                    //if (Main.debug) { Main.logger_instance.Msg("CraftingUpgradeButton : UpdateButton : Prefix"); }
                    if (Scenes.IsGameScene())
                    {
                        Get.CurrentFomSlotManager();
                        if (!Current.item.IsNullOrDestroyed())
                        {
                            AffixSlotForge temp = __instance.gameObject.GetComponentInParent<AffixSlotForge>();
                            if (!temp.IsNullOrDestroyed())
                            {
                                int affix_id = temp.affixID;
                                int tier = Get.TierFromItemData(Current.item, affix_id);

                                if (Get.IsIdol(Current.item)) { __1 = false; }
                                else if ((tier > -1) && (tier < 6)) { __1 = true; }
                            }
                        }
                    }
                }
            }

            //Get slot, tier and button ref
            [HarmonyPatch(typeof(CraftingUpgradeButton), "UpgradeButtonClicked")]
            public class CraftingUpgradeButton_UpgradeButtonClicked
            {
                [HarmonyPostfix]
                static void Postfix(ref CraftingUpgradeButton __instance)
                {
                    //if (Main.debug) { Main.logger_instance.Msg("CraftingUpgradeButton : UpgradeButtonClicked : Postfix"); }
                    if (Scenes.IsGameScene())
                    {
                        Get.CurrentFomSlotManager();
                        if (!Current.item.IsNullOrDestroyed())
                        {
                            btn = __instance;
                            Current.slot = __instance.gameObject.GetComponentInParent<AffixSlotForge>();
                            Current.affix_tier = Get.TierFromItemData(Current.item, Current.affix_id);
                        }
                    }
                }
            }
        }
        public class Affix_Slot_Forge
        {
            //Get slot and button ref
            [HarmonyPatch(typeof(AffixSlotForge), "SlotClicked")]
            public class AffixSlotForge_SlotClicked
            {
                [HarmonyPostfix]
                static void Postfix(ref AffixSlotForge __instance)
                {
                    //if (Main.debug) { Main.logger_instance.Msg("AffixSlotForge : SlotClicked : Postfix"); }
                    if (Scenes.IsGameScene())
                    {
                        Current.slot = __instance;
                        GameObject upgrade = Functions.GetChild(__instance.gameObject, "upgradeAvailable");
                        if (!upgrade.IsNullOrDestroyed())
                        {
                            Crafting_Upgrade_Button.btn = upgrade.GetComponent<CraftingUpgradeButton>();
                        }
                    }
                }
            }
        }
        public class Crafting_Materials_Panel_UI
        {
            //Add all affixs in list
            [HarmonyPatch(typeof(CraftingMaterialsPanelUI), "AddShardsFromList")]
            public class CraftingMaterialsPanelUI_AddShardsFromList
            {
                [HarmonyPostfix]
                static void Postfix(CraftingMaterialsPanelUI __instance, ref UnhollowerBaseLib.Il2CppReferenceArray<AffixList.Affix> __0)
                {
                    //if (Main.debug) { Main.logger_instance.Msg("CraftingMaterialsPanelUI : AddShardsFromList : Postfix"); }
                    if (!Refs_Manager.item_list.IsNullOrDestroyed())
                    {
                        int count = Refs_Manager.item_list.affixList.singleAffixes.Count + Refs_Manager.item_list.affixList.multiAffixes.Count;
                        UnhollowerBaseLib.Il2CppReferenceArray<AffixList.Affix> new_list = new UnhollowerBaseLib.Il2CppReferenceArray<AffixList.Affix>(count);
                        int i = 0;
                        Refs_Manager.item_list.affixList.isIdolAffix = AffixList.Filter.Either;
                        foreach (AffixList.SingleAffix single_affix in Refs_Manager.item_list.affixList.singleAffixes)
                        {
                            new_list.AddItem(single_affix.TryCast<AffixList.Affix>());
                            i++;
                        }
                        i = 0;
                        foreach (AffixList.MultiAffix multi_affix in Refs_Manager.item_list.affixList.multiAffixes)
                        {
                            new_list.AddItem(multi_affix.TryCast<AffixList.Affix>());
                            i++;
                        }
                        __0 = new_list;
                    }
                }
            }

            //All Affix in materials panel (fix idols)
            [HarmonyPatch(typeof(CraftingMaterialsPanelUI), "PopulateShardList")]
            public class CraftingMaterialsPanelUI_PopulateShardList
            {
                [HarmonyPostfix]
                static void Postfix(CraftingMaterialsPanelUI __instance)
                {
                    //if (Main.debug) { Main.logger_instance.Msg("CraftingMaterialsPanelUI : PopulateShardList : Postfix"); }
                    System.Collections.Generic.List<int> already = new System.Collections.Generic.List<int>();
                    foreach (ShardAffixListElement affix_element in __instance.shardAffixList)
                    {
                        already.Add(affix_element.affix.affixId);
                    }
                    int lost = 0;
                    if (!Refs_Manager.item_list.IsNullOrDestroyed())
                    {
                        if (!Refs_Manager.item_list.affixList.IsNullOrDestroyed())
                        {
                            foreach (AffixList.SingleAffix affix in Refs_Manager.item_list.affixList.singleAffixes)
                            {
                                if ((!already.Contains(affix.affixId)) && (!__instance.shardAffixPrefab.IsNullOrDestroyed()))
                                {
                                    Add.Affix(ref __instance, affix, affix.modifierType);
                                    lost++;
                                }
                            }
                            foreach (AffixList.MultiAffix affix in Refs_Manager.item_list.affixList.multiAffixes)
                            {
                                if (!already.Contains(affix.affixId))
                                {
                                    Add.Affix(ref __instance, affix, affix.affixProperties[0].modifierType);
                                    lost++;
                                }
                            }
                        }                        
                    }
                }
            }            

            //Fix shards for Unique Set Legendary and Idols
            [HarmonyPatch(typeof(CraftingMaterialsPanelUI), "RefreshAffixList")]
            public class CraftingMaterialsPanelUI_RefreshAffixList
            {
                [HarmonyPostfix]
                static void Postfix(ref CraftingMaterialsPanelUI __instance)
                {
                    //if (Main.debug) { Main.logger_instance.Msg("CraftingMaterialsPanelUI : RefreshAffixList : Postfix"); }                    
                    if (Scenes.IsGameScene())
                    {                        
                        Get.CurrentFomSlotManager();
                        if (!Current.item.IsNullOrDestroyed())
                        {
                            GameObject unused_header = __instance.unusedAffixesHeader.gameObject;
                            GameObject unused_holder = __instance.unusedAffixesHolder.gameObject;
                            GameObject incompatible_header = __instance.incompatibleAffixesHeader.gameObject;
                            GameObject incompatible_holder = __instance.incompatibleAffixesHolder.gameObject;
                            GameObject hidden_holder = __instance.hiddenAffixHolder.gameObject;

                            bool type_found = false;
                            EquipmentType equip_type = EquipmentType.HELMET;
                            foreach (ItemList.BaseEquipmentItem item in Scripts.Refs_Manager.item_list.EquippableItems)
                            {
                                if (item.baseTypeID == Current.item.itemType)
                                {
                                    equip_type = item.type;
                                    type_found = true;
                                    break;
                                }
                            }

                            int added = 0;
                            System.Collections.Generic.List<GameObject> unused_list = new System.Collections.Generic.List<GameObject>();
                            System.Collections.Generic.List<GameObject> uncompatible_list = new System.Collections.Generic.List<GameObject>();
                            System.Collections.Generic.List<GameObject> hidden_list = new System.Collections.Generic.List<GameObject>();

                            if ((Get.IsIdol(Current.item)) && (type_found))
                            {
                                for (int i = 0; i < hidden_holder.transform.childCount; i++)
                                {
                                    GameObject affix_obj = hidden_holder.transform.GetChild(i).gameObject;
                                    ShardAffixListElement element = affix_obj.GetComponent<ShardAffixListElement>();
                                    if (((element.affixType == "Prefix") && (__instance.affixFilterType == CraftingMaterialsPanelUI.AffixFilterType.PREFIX)) ||
                                        ((element.affixType == "Suffix") && (__instance.affixFilterType == CraftingMaterialsPanelUI.AffixFilterType.SUFFIX)) ||
                                        (__instance.affixFilterType == CraftingMaterialsPanelUI.AffixFilterType.ANY))
                                    {
                                        if (element.affix.affixName.ToLower().Contains("idol"))
                                        {
                                            if (element.affix.canRollOn.Contains(equip_type)) { unused_list.Add(affix_obj); }
                                            else { uncompatible_list.Add(affix_obj); }                                            
                                        }
                                    }
                                }
                            }
                            else if ((Current.item.rarity > 6) && (type_found)) //Unique set legendary
                            {
                                for (int i = 0; i < incompatible_holder.transform.childCount; i++)
                                {
                                    GameObject affix_obj = incompatible_holder.transform.GetChild(i).gameObject;
                                    ShardAffixListElement element = affix_obj.GetComponent<ShardAffixListElement>();
                                    if (element.affix.canRollOn.Contains(equip_type)) { unused_list.Add(affix_obj); }
                                    else { hidden_list.Add(affix_obj); }
                                }
                                for (int i = 0; i < hidden_holder.transform.childCount; i++)
                                {
                                    GameObject affix_obj = hidden_holder.transform.GetChild(i).gameObject;
                                    ShardAffixListElement element = affix_obj.GetComponent<ShardAffixListElement>();
                                    if (((element.affixType == "Prefix") && (__instance.affixFilterType == CraftingMaterialsPanelUI.AffixFilterType.PREFIX)) ||
                                        ((element.affixType == "Suffix") && (__instance.affixFilterType == CraftingMaterialsPanelUI.AffixFilterType.SUFFIX)) ||
                                        (__instance.affixFilterType == CraftingMaterialsPanelUI.AffixFilterType.ANY))
                                    {
                                        if (element.affix.canRollOn.Contains(equip_type)) { unused_list.Add(affix_obj); }
                                    }
                                }                                
                            }

                            foreach (GameObject item in unused_list)
                            {
                                Functions.GetChild(item, "Button").active = true;
                                item.transform.SetParent(unused_holder.transform);
                                //item.transform.parent = unused_holder.transform;
                                added++;
                            }
                            unused_list.Clear();
                            
                            foreach (GameObject item in uncompatible_list)
                            {
                                Functions.GetChild(item, "Button").active = true;
                                item.transform.SetParent(incompatible_holder.transform);
                                //item.transform.parent = incompatible_holder.transform;
                            }
                            uncompatible_list.Clear();
                            
                            foreach (GameObject item in hidden_list)
                            {
                                item.transform.SetParent(hidden_holder.transform);
                                //item.transform.parent = hidden_holder.transform;
                            }
                            hidden_list.Clear();

                            if (unused_holder.transform.childCount > 0)
                            {
                                __instance.noShardNotice.gameObject.active = false;
                                unused_header.active = true;
                            }
                            else { unused_header.active = false; }

                            for (int i = 0; i < unused_holder.transform.childCount; i++)
                            {
                                unused_holder.transform.GetChild(i).gameObject.active = true;
                            }

                            if (incompatible_holder.transform.childCount > 0) { incompatible_header.active = true; }
                            else { incompatible_header.active = false; }

                            for (int i = 0; i < incompatible_holder.transform.childCount; i++)
                            {
                                incompatible_holder.transform.GetChild(i).gameObject.active = true;
                            }
                        }
                    }
                }
            }
        }
        public class Crafting_Modifier_Item_Container
        {
            [HarmonyPatch(typeof(CraftingModifierItemContainer), "CanReceiveItem")]
            public class CraftingModifierItemContainer_CanReceiveItem
            {
                [HarmonyPrefix]
                static bool Prefix(CraftingModifierItemContainer __instance, ref bool __result, ItemData __0, int __1)
                {
                    bool __return = true;
                    if (Scenes.IsGameScene())
                    {
                        Get.CurrentFomSlotManager();
                        if (!Current.item.IsNullOrDestroyed())
                        {
                            if ((Get.IsIdol(Current.item)) || (Current.item.rarity > 6))
                            {                                
                                __result = true;
                                __return = false;
                            }
                        }
                    }

                    return __return;
                }
            }
        }
        public class Crafting_Slot_Manager
        {
            [HarmonyPatch(typeof(CraftingSlotManager), "Forge")]
            public class CraftingSlotManager_Forge
            {
                [HarmonyPrefix]
                static void Prefix(CraftingSlotManager __instance)
                {
                    if (Scenes.IsGameScene())
                    {
                        Get.CurrentFomSlotManager();
                        if (!Current.item.IsNullOrDestroyed())
                        {
                            Current.affix_tier = -1;
                            Current.affix_id = __instance.appliedAffixID;
                            if (Current.affix_id > -1)
                            {
                                foreach (ItemAffix affix in Current.item.affixes)
                                {
                                    if (affix.affixId == Current.affix_id)
                                    {
                                        Current.affix_tier = affix.affixTier;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                [HarmonyPostfix]
                static void Postfix(CraftingSlotManager __instance)
                {
                    //if (Main.debug) { Main.logger_instance.Msg("CraftingSlotManager : Forge : Postfix"); }
                    if (Scenes.IsGameScene())
                    {
                        Get.CurrentFomSlotManager();
                        if ((!Current.item.IsNullOrDestroyed()) && (Current.affix_id > -1))
                        {
                            bool legendary = Current.item.isUniqueSetOrLegendary();
                            bool idol = Get.IsIdol(Current.item);
                            if (legendary) { Current.item.rarity = 9; }
                            if (Current.affix_tier > -1)
                            {
                                if (!idol)
                                {
                                    bool force_upgrade = false;
                                    foreach (ItemAffix affix in Current.item.affixes)
                                    {
                                        if (affix.affixId == Current.affix_id)
                                        {
                                            if ((Current.affix_tier == affix.affixTier) && (Current.affix_tier < 6))
                                            {
                                                force_upgrade = true;
                                                affix.affixTier++;
                                                affix.affixRoll = (byte)Random.Range(0f, 255f);
                                            }
                                            break;
                                        }
                                    }
                                    if (force_upgrade)
                                    {
                                        Current.item.RefreshIDAndValues();
                                        if ((Current.affix_tier < 5) && (!Crafting_Upgrade_Button.btn.IsNullOrDestroyed()))
                                        {
                                            Crafting_Upgrade_Button.btn.UpgradeButtonClicked();
                                        }
                                        else
                                        {
                                            Current.affix_tier++;
                                            Crafting_Upgrade_Button.btn.gameObject.active = false;
                                        }
                                    }
                                }
                            }
                            else if ((legendary) || (idol))
                            {
                                int nb_prefix = 0;
                                int nb_suffix = 0;
                                foreach (ItemAffix item_affix in Current.item.affixes)
                                {
                                    if (item_affix.affixType == AffixList.AffixType.PREFIX) { nb_prefix++; }
                                    else if (item_affix.affixType == AffixList.AffixType.SUFFIX) { nb_suffix++; }
                                }
                                AffixList.AffixType new_affix_type = AffixList.instance.GetAffixType(Current.affix_id);
                                if (((new_affix_type == AffixList.AffixType.PREFIX) && (nb_prefix < 2)) ||
                                    ((new_affix_type == AffixList.AffixType.SUFFIX) && (nb_suffix < 2)))
                                {
                                    Current.affix_tier = 0;
                                    if (!Current.slot.IsNullOrDestroyed()) { Current.slot.affixID = (ushort)Current.affix_id; }
                                    ItemAffix affix = new ItemAffix
                                    {
                                        affixId = (ushort)Current.affix_id,
                                        affixTier = (byte)Current.affix_tier,
                                        affixRoll = (byte)Random.Range(0f, 255f),
                                        affixType = new_affix_type
                                    };
                                    Current.item.affixes.Add(affix);
                                    Current.item.RefreshIDAndValues();

                                    if (!idol)
                                    {
                                        //Fix Slots Changed
                                        int index = nb_prefix;
                                        if (index == 2) { index += nb_suffix; }
                                        if (index < __instance.affixSlots.Count)
                                        {
                                            GameObject upgrade = Functions.GetChild(__instance.affixSlots[index].gameObject, "upgradeAvailable");
                                            if (!upgrade.IsNullOrDestroyed())
                                            {
                                                upgrade.GetComponent<CraftingUpgradeButton>().UpgradeButtonClicked();

                                                Crafting_Upgrade_Button.btn = upgrade.GetComponent<CraftingUpgradeButton>();
                                                Crafting_Upgrade_Button.btn.gameObject.active = true;
                                                Crafting_Upgrade_Button.btn.UpgradeButtonClicked();
                                            }
                                        }
                                        //Select slot
                                        /*if (!Crafting_Upgrade_Button.btn.IsNullOrDestroyed())
                                        {
                                            Crafting_Upgrade_Button.btn.gameObject.active = true;
                                            Crafting_Upgrade_Button.btn.UpgradeButtonClicked();
                                        }*/
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

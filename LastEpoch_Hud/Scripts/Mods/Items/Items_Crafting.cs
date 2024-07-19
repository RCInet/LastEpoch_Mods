using HarmonyLib;
using UnityEngine;
using static MelonLoader.MelonLogger;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_Crafting
    {
        public static bool enable_forgin_potencial_cost = false;

        public class Current
        {
            public static ItemData item = null;
            public static AffixSlotForge slot = null;
            public static CraftingUpgradeButton btn = null;
        }
        public class Get
        {
            public static int Tier(ItemData item_data, int affix_id)
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
            public static bool IsCraftable(ItemData item)
            {
                bool result = false;
                if (item.itemType < 34) { result = true; }

                return result;
            }
            public static bool IsPrefixFull(ItemData item)
            {
                int nb_max = 0;
                bool idol = IsIdol(item);
                foreach (ItemAffix affix in item.affixes)
                {
                    if ((affix.affixType == AffixList.AffixType.PREFIX) && ((idol) || ((!idol) && (affix.affixTier > 5))))
                    {
                        nb_max++;
                    }
                }
                if (nb_max > 1) { return true; }
                else { return false; }
            }
            public static bool IsSuffixFull(ItemData item)
            {
                int nb_max = 0;
                bool idol = IsIdol(item);
                foreach (ItemAffix affix in item.affixes)
                {
                    if ((affix.affixType == AffixList.AffixType.SUFFIX) && ((idol) || ((!idol) && (affix.affixTier > 5))))
                    {
                        nb_max++;
                    }
                }
                if (nb_max > 1) { return true; }
                else { return false; }
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
                    else if (affix.type == AffixList.AffixType.PREFIX)
                    {
                        element.affixType = "Prefix";
                        element.prefixArrow.active = true;
                        element.suffixArrow.active = false;
                    }
                    else if (affix.type == AffixList.AffixType.SPECIAL)
                    {
                        Main.logger_instance.Msg(affix.affixName + " is Special");
                        element.affixType = "Special";
                        element.prefixArrow.active = false;
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
        public class Locales
        {
            public static string affix_is_maxed_key = "Crafting_ForgeButton_Title_AffixMaxed";
            public static string affix_is_maxed = "maxed_craft";
            public static string cant_craft_unique_key = "Crafting_ForgeButton_Title_Uniques";
            public static string cant_craft_unique = "unique_craft";
            public static string no_forgin_potencial_key = "Crafting_ForgeButton_Title_NoPotential";
            public static string no_forgin_potencial = "no_forgin_potencial_craft";

            [HarmonyPatch(typeof(Localization), "TryGetText")]
            public class Localization_TryGetText
            {
                [HarmonyPrefix]
                static bool Prefix(ref bool __result, string __0) //, Il2CppSystem.String __1)
                {
                    //Main.logger_instance.Msg("Localization:TryGetText key = " + __0);
                    bool result = true;
                    if ((__0 == affix_is_maxed_key) || (__0 == cant_craft_unique_key) ||
                        (__0 == no_forgin_potencial_key))
                    {
                        __result = true;
                        result = false;
                    }

                    return result;
                }
            }

            [HarmonyPatch(typeof(Localization), "GetText")]
            public class Localization_GetText
            {
                [HarmonyPrefix]
                static bool Prefix(ref string __result, string __0)
                {
                    //Main.logger_instance.Msg("Localization:GetText key = " + __0);
                    bool result = true;
                    if (__0 == affix_is_maxed_key)
                    {
                        __result = affix_is_maxed;
                        result = false;
                    }
                    else if (__0 == cant_craft_unique_key)
                    {
                        __result = cant_craft_unique;
                        result = false;
                    }
                    else if (__0 == no_forgin_potencial_key)
                    {
                        __result = no_forgin_potencial;
                        result = false;
                    }

                    return result;
                }
            }
        }
        public class Crafting_Manager
        {
            public static CraftingManager crafting_manager = null;
            public static bool EditingItem = false;
            public static bool first_time = true;
            
            //Select Item
            [HarmonyPatch(typeof(CraftingManager), "OnMainItemChange")]
            public class CraftingManager_OnMainItemChange
            {
                [HarmonyPostfix]
                static void Postfix(ref CraftingManager __instance, ref Il2CppSystem.Object __0, ref ItemContainerEntryHandler __1)
                {
                    if (crafting_manager.IsNullOrDestroyed()) { crafting_manager = __instance; };
                    if (!__0.IsNullOrDestroyed())
                    {
                        OneItemContainer container = __0.TryCast<OneItemContainer>();
                        if (!container.IsNullOrDestroyed())
                        {
                            if (!container.content.IsNullOrDestroyed()) { Current.item = container.content.data; }
                        }
                    }
                    if ((Scenes.IsGameScene()) && (!EditingItem)) // && (!Crafting_Slot_Manager.forgin))
                    {
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

                                int nb_prefix = 0;
                                int nb_suffix = 0;
                                foreach (ItemAffix affix in Current.item.affixes)
                                {
                                    if (affix.isSealedAffix)
                                    {
                                        if (Save_Manager.instance.data.Items.CraftingSlot.Enable_Seal_Tier) { affix.affixTier = (byte)Scripts.Save_Manager.instance.data.Items.CraftingSlot.Seal_Tier; }
                                        if (Save_Manager.instance.data.Items.CraftingSlot.Enable_Seal_Value) { affix.affixRoll = (byte)Scripts.Save_Manager.instance.data.Items.CraftingSlot.Seal_Value; }
                                    }
                                    else
                                    {
                                        int result = -1;
                                        if ((affix.affixType == AffixList.AffixType.PREFIX) && (nb_prefix < 2))
                                        {
                                            result = 0 + nb_prefix;
                                            nb_prefix++;
                                        }
                                        else if ((affix.affixType == AffixList.AffixType.SUFFIX) && (nb_suffix < 2))
                                        {
                                            result = 2 + nb_suffix;
                                            nb_suffix++;
                                        }

                                        if ((result > -1) && (result < 4))
                                        {
                                            if ((result < affix_tier_enables.Count) && (result < affix_tier_values.Count) &&
                                                (result < affix_value_enables.Count) && (result < affix_value_values.Count))
                                            {
                                                if (affix_tier_enables[result]) { affix.affixTier = (byte)affix_tier_values[result]; }
                                                if (affix_value_enables[result]) { affix.affixRoll = (byte)affix_value_values[result]; }
                                                
                                            }
                                        }
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
                    if (crafting_manager.IsNullOrDestroyed()) { crafting_manager = __instance; };
                    Current.item = null;                    
                    Current.slot = null;
                    Current.btn = null;
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
                    if (crafting_manager.IsNullOrDestroyed()) { crafting_manager = __instance; };
                    int affix_id = __instance.appliedAffixID;
                    int affix_tier = Get.Tier(Current.item, affix_id);
                    if ((!Refs_Manager.craft_slot_manager.IsNullOrDestroyed()) && (!Current.item.IsNullOrDestroyed()))
                    {
                        if (__0 == Locales.cant_craft_unique)
                        {
                            if ((affix_id > -1) && (affix_tier < 6))
                            {
                                Refs_Manager.craft_slot_manager.prefixFullOfMax = Get.IsPrefixFull(Current.item);
                                Refs_Manager.craft_slot_manager.suffixFullOfMax = Get.IsSuffixFull(Current.item);
                                                                
                                Refs_Manager.craft_slot_manager.canForge = true;
                                __0 = "Legendary Craft";
                                __1 = false;
                                __result = true;
                            }
                            else if (affix_id == -1) { __0 = "Select an affix"; }
                            else if (affix_tier > 5) { __0 = "Maxed"; }
                        }
                        /*else if (__0 == "No Forging Potential")
                        {                    
                            __0 = "No Cost";
                            __1 = false;
                            __result = true;
                        }*/
                        else if (__0 == Locales.affix_is_maxed) //(__0 == "Affix is Maxed")
                        {
                            if (affix_tier == 4) { __0 = "Forge T6"; }
                            else if (affix_tier == 5) { __0 = "Forge T7"; }
                            if ((affix_tier > 3) && (affix_tier < 6))
                            {
                                Refs_Manager.craft_slot_manager.prefixFullOfMax = Get.IsPrefixFull(Current.item);
                                Refs_Manager.craft_slot_manager.suffixFullOfMax = Get.IsSuffixFull(Current.item);
                                __1 = false;
                                __result = true;
                            }
                            else if (affix_tier > 5) { __0 = "Maxed"; }
                            else { __0 = "Use Upgrade Button"; }
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

                    if (!main_ui.IsNullOrDestroyed()) //&& (Crafting_Main_Item_Container.rect_transform.IsNullOrDestroyed()))
                    {
                        Crafting_Main_Item_Container.rect_transform = main_ui.gameObject.GetComponent<RectTransform>();
                    }                    
                }
            }
        }
        public class Crafting_Main_Item_Container
        {
            public static CraftingMainItemContainer main_item_container = null;
            public static RectTransform rect_transform = null;
            public static Vector2Int default_size = Vector2Int.zero;
            public static Vector2 default_sizedelta = Vector2.zero;
            public static Vector2 default_localscale = Vector2.zero;
            public static bool backup_initialized = false;

            //Fix slot size
            [HarmonyPatch(typeof(OneSlotItemContainer), "TryAddItem", new System.Type[] { typeof(ItemData), typeof(int), typeof(Context) })]
            public class OneSlotItemContainer_TryAddItem
            {
                [HarmonyPrefix]
                static void Prefix(ref OneSlotItemContainer __instance, bool __result, ItemData __0)
                {
                    if (Scenes.IsGameScene())
                    {
                        if ((__instance.ToString() == "CraftingMainItemContainer") && (!rect_transform.IsNullOrDestroyed()))
                        {
                            if (!backup_initialized)
                            {
                                default_size = __instance.size;
                                default_sizedelta = rect_transform.sizeDelta;
                                default_localscale = rect_transform.localScale;
                                backup_initialized = true;
                            }
                            if (backup_initialized)
                            {
                                if ((__0.itemType == 29) || (__0.itemType == 31))
                                {
                                    __instance.size = new Vector2Int((2 * default_size.x), default_size.y);
                                    rect_transform.sizeDelta = new Vector2((2 * default_sizedelta.x), default_sizedelta.y);
                                    rect_transform.localScale = new Vector3((default_localscale.x / 2), default_localscale.y);
                                }
                                else
                                {
                                    __instance.size = default_size;
                                    rect_transform.sizeDelta = default_sizedelta;
                                    rect_transform.localScale = default_localscale;
                                }
                            }
                        }
                    }
                }
            }
            
            //Idols can be added to slot
            [HarmonyPatch(typeof(CraftingMainItemContainer), "CanReceiveItem")]
            public class CraftingMainItemContainer_CanReceiveItem
            {
                [HarmonyPostfix]
                static void Postifx(ref CraftingMainItemContainer __instance, ref bool __result, ItemData __0, int __1)
                {
                    main_item_container = __instance;
                    if (Get.IsCraftable(__0)) { __result = true; } //Allow all item type < 34
                }
            }
        }
        public class Crafting_Upgrade_Button
        {
            //Unlock Craft Button when tier < T7
            [HarmonyPatch(typeof(CraftingUpgradeButton), "UpdateButton")]
            public class CraftingUpgradeButton_UpdateButton
            {
                [HarmonyPrefix]
                static void Prefix(ref CraftingUpgradeButton __instance, int __0, ref bool __1)
                {
                    if ((Scenes.IsGameScene()) && (!Current.item.IsNullOrDestroyed()))
                    {
                        AffixSlotForge temp = __instance.gameObject.GetComponentInParent<AffixSlotForge>();
                        if (!temp.IsNullOrDestroyed())
                        {
                            int affix_id = temp.affixID;
                            int tier = Get.Tier(Current.item, affix_id);

                            if (Get.IsIdol(Current.item)) { __1 = false; }
                            else if ((tier > -1) && (tier < 6)) { __1 = true; }
                        }
                    }
                }
            }

            //Update slot and button ref
            [HarmonyPatch(typeof(CraftingUpgradeButton), "UpgradeButtonClicked")]
            public class CraftingUpgradeButton_UpgradeButtonClicked
            {
                [HarmonyPrefix]
                static void Prefix(ref CraftingUpgradeButton __instance)
                {
                    if (Scenes.IsGameScene())
                    {
                        Current.btn = __instance;
                        Current.slot = __instance.gameObject.GetComponentInParent<AffixSlotForge>();
                    }
                }
            }
        }
        public class Affix_Slot_Forge
        {
            //Update slot and button ref
            [HarmonyPatch(typeof(AffixSlotForge), "SlotClicked")]
            public class AffixSlotForge_SlotClicked
            {
                [HarmonyPostfix]
                static void Postfix(ref AffixSlotForge __instance)
                {
                    if (Scenes.IsGameScene())
                    {
                        Current.slot = __instance;
                        GameObject upgrade = Functions.GetChild(__instance.gameObject, "upgradeAvailable");
                        if (!upgrade.IsNullOrDestroyed())
                        {
                            Current.btn = upgrade.GetComponent<CraftingUpgradeButton>();
                        }
                    }
                }
            }
        }
        public class Crafting_Materials_Panel_UI
        {
            public static bool ShowAffixs(CraftingMaterialsPanelUI.AffixFilterType filter, ShardAffixListElement element, int nb_prefix, int nb_suffix, bool idol)
            {
                int max_prefix_suffix = 2;
                if (idol) { max_prefix_suffix = 1; }
                if ((filter == CraftingMaterialsPanelUI.AffixFilterType.ANY) ||
                    ((element.affixType == "Prefix") && (filter == CraftingMaterialsPanelUI.AffixFilterType.PREFIX) && (nb_prefix < max_prefix_suffix)) ||
                    ((element.affixType == "Suffix") && (filter == CraftingMaterialsPanelUI.AffixFilterType.SUFFIX) && (nb_suffix < max_prefix_suffix)) ||
                    ((element.affixType == "Special") && ((idol) || (Save_Manager.instance.data.modsNotInHud.Enable_Craft_ShowSpecialAffixs))))
                { return true; }
                else { return false; }
            }
            
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
                        //int count = Refs_Manager.item_list.affixList.singleAffixes.Count + Refs_Manager.item_list.affixList.multiAffixes.Count;
                        //UnhollowerBaseLib.Il2CppReferenceArray<AffixList.Affix> new_list = new UnhollowerBaseLib.Il2CppReferenceArray<AffixList.Affix>(count);
                        UnhollowerBaseLib.Il2CppReferenceArray<AffixList.Affix> new_list = new UnhollowerBaseLib.Il2CppReferenceArray<AffixList.Affix>(1);
                        Refs_Manager.item_list.affixList.isIdolAffix = AffixList.Filter.Either;
                        foreach (AffixList.Affix affix in __0)
                        {
                            new_list.AddItem(affix);
                        }
                        foreach (AffixList.SingleAffix single_affix in Refs_Manager.item_list.affixList.singleAffixes)
                        {
                            AffixList.Affix affix = single_affix.TryCast<AffixList.Affix>();
                            if (!new_list.Contains(affix)) { new_list.AddItem(affix); }
                        }
                        foreach (AffixList.MultiAffix multi_affix in Refs_Manager.item_list.affixList.multiAffixes)
                        {
                            AffixList.Affix affix = multi_affix.TryCast<AffixList.Affix>();
                            if (!new_list.Contains(affix)) { new_list.AddItem(affix); }
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
                    System.Collections.Generic.List<int> already = new System.Collections.Generic.List<int>();
                    foreach (ShardAffixListElement affix_element in __instance.shardAffixList)
                    {
                        already.Add(affix_element.affix.affixId);
                    }
                    if (!Refs_Manager.item_list.IsNullOrDestroyed())
                    {
                        if (!Refs_Manager.item_list.affixList.IsNullOrDestroyed())
                        {
                            foreach (AffixList.SingleAffix affix in Refs_Manager.item_list.affixList.singleAffixes)
                            {
                                if ((!already.Contains(affix.affixId)) && (!__instance.shardAffixPrefab.IsNullOrDestroyed()))
                                {
                                    Add.Affix(ref __instance, affix, affix.modifierType);
                                }
                            }
                            foreach (AffixList.MultiAffix affix in Refs_Manager.item_list.affixList.multiAffixes)
                            {
                                if (!already.Contains(affix.affixId))
                                {
                                    Add.Affix(ref __instance, affix, affix.affixProperties[0].modifierType);
                                }
                            }
                        }                        
                    }
                }
            }            

            //Fix shards for Unique Set Legendary and Idols //Have to be edited to fix req (ex : warpath)
            [HarmonyPatch(typeof(CraftingMaterialsPanelUI), "RefreshAffixList")]
            public class CraftingMaterialsPanelUI_RefreshAffixList
            {
                [HarmonyPostfix]
                static void Postfix(ref CraftingMaterialsPanelUI __instance)
                {
                    if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
                    {
                        if (!Current.item.IsNullOrDestroyed())
                        {
                            GameObject applied_holder = __instance.appliedAffixesHolder.gameObject;
                            GameObject unused_header = __instance.unusedAffixesHeader.gameObject;
                            GameObject unused_holder = __instance.unusedAffixesHolder.gameObject;
                            GameObject incompatible_header = __instance.incompatibleAffixesHeader.gameObject;
                            GameObject incompatible_holder = __instance.incompatibleAffixesHolder.gameObject;
                            GameObject hidden_holder = __instance.hiddenAffixHolder.gameObject;

                            bool type_found = false;
                            EquipmentType equip_type = EquipmentType.HELMET;
                            string item_base_name = "";
                            ItemList.ClassRequirement item_class_req = ItemList.ClassRequirement.None;
                            foreach (ItemList.BaseEquipmentItem item in Scripts.Refs_Manager.item_list.EquippableItems)
                            {
                                if (item.baseTypeID == Current.item.itemType)
                                {
                                    equip_type = item.type;
                                    item_base_name = item.BaseTypeName;
                                    type_found = true;
                                    foreach (ItemList.EquipmentItem eq_item in item.subItems)
                                    {
                                        if (eq_item.subTypeID == Current.item.subType)
                                        {                                            
                                            item_class_req = eq_item.classRequirement;
                                            break;
                                        }
                                    }                                    
                                    break;
                                }
                            }

                            System.Collections.Generic.List<GameObject> unused_list = new System.Collections.Generic.List<GameObject>();
                            System.Collections.Generic.List<GameObject> uncompatible_list = new System.Collections.Generic.List<GameObject>();
                            System.Collections.Generic.List<GameObject> hidden_list = new System.Collections.Generic.List<GameObject>();

                            int nb_prefix = 0;
                            int nb_suffix = 0;
                            foreach (ItemAffix aff in Current.item.affixes)
                            {
                                if (aff.affixType == AffixList.AffixType.PREFIX) { nb_prefix++; }
                                else if (aff.affixType != AffixList.AffixType.SUFFIX) {  nb_suffix++; }
                            }
                                                        
                            bool idol = Get.IsIdol(Current.item);
                            if (type_found)
                            {
                                for (int i = 0; i < unused_holder.transform.childCount; i++)
                                {
                                    GameObject affix_obj = unused_holder.transform.GetChild(i).gameObject;
                                    ShardAffixListElement element = affix_obj.GetComponent<ShardAffixListElement>();
                                    
                                    if (ShowAffixs(__instance.affixFilterType, element, nb_prefix, nb_suffix, idol))
                                    {
                                        if ((element.affix.CanRollOnItemType(Current.item.itemType, item_class_req)) ||
                                            (Save_Manager.instance.data.modsNotInHud.Enable_Craft_BypassReq))
                                        {
                                            unused_list.Add(affix_obj);
                                        }
                                        else { uncompatible_list.Add(affix_obj); }
                                    }
                                    else { hidden_list.Add(affix_obj); }
                                }

                                for (int i = 0; i < incompatible_holder.transform.childCount; i++)
                                {
                                    GameObject affix_obj = incompatible_holder.transform.GetChild(i).gameObject;
                                    ShardAffixListElement element = affix_obj.GetComponent<ShardAffixListElement>();
                                    if (ShowAffixs(__instance.affixFilterType, element, nb_prefix, nb_suffix, idol))
                                    {
                                        if ((element.affix.CanRollOnItemType(Current.item.itemType, item_class_req)) ||
                                            (Save_Manager.instance.data.modsNotInHud.Enable_Craft_BypassReq))
                                        {
                                            unused_list.Add(affix_obj);
                                        }
                                        else { uncompatible_list.Add(affix_obj); }
                                    }
                                    else { hidden_list.Add(affix_obj); }
                                }
                                for (int i = 0; i < hidden_holder.transform.childCount; i++)
                                {
                                    GameObject affix_obj = hidden_holder.transform.GetChild(i).gameObject;
                                    ShardAffixListElement element = affix_obj.GetComponent<ShardAffixListElement>();
                                    if (ShowAffixs(__instance.affixFilterType, element, nb_prefix, nb_suffix, idol))
                                    {
                                        if ((element.affix.CanRollOnItemType(Current.item.itemType, item_class_req)) ||
                                            (Save_Manager.instance.data.modsNotInHud.Enable_Craft_BypassReq))
                                        {
                                            unused_list.Add(affix_obj);
                                        }
                                        else { uncompatible_list.Add(affix_obj); }
                                    }
                                    else { hidden_list.Add(affix_obj); }
                                }
                            }

                            //Applied                            
                            for (int i = 0; i < applied_holder.transform.childCount; i++)
                            {
                                if (idol) { Functions.GetChild(applied_holder.transform.GetChild(i).gameObject, "Button").active = false; }
                                else if (Current.item.rarity > 6)
                                {
                                    int affix_id = applied_holder.transform.GetChild(i).gameObject.GetComponent<ShardAffixListElement>().affix.affixId;
                                    bool can_select = false;
                                    foreach (ItemAffix affix in Current.item.affixes)
                                    {
                                        if (affix.affixId == affix_id)
                                        {
                                            if (affix.affixTier < 6) { can_select = true; }
                                            break;
                                        }
                                    }
                                    Functions.GetChild(applied_holder.transform.GetChild(i).gameObject, "Button").active = can_select;
                                }
                                applied_holder.transform.GetChild(i).gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                            }

                            //Unused
                            foreach (GameObject item in unused_list) { item.transform.SetParent(unused_holder.transform); }
                            unused_list.Clear();

                            if (unused_holder.transform.childCount > 0)
                            {
                                __instance.noShardNotice.gameObject.active = false;
                                unused_header.active = true;
                            }
                            else { unused_header.active = false; }

                            for (int i = 0; i < unused_holder.transform.childCount; i++)
                            {
                                unused_holder.transform.GetChild(i).gameObject.active = true;                                
                                Functions.GetChild(unused_holder.transform.GetChild(i).gameObject, "Button").active = true;
                                unused_holder.transform.GetChild(i).gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                            }
                            
                            //Incompatible
                            foreach (GameObject item in uncompatible_list) { item.transform.SetParent(incompatible_holder.transform); }
                            uncompatible_list.Clear();

                            if ((Save_Manager.instance.data.modsNotInHud.Enable_Craft_IncompatibleAffixs) || (idol))
                            {
                                if (incompatible_holder.transform.childCount > 0) { incompatible_header.active = true; }
                                else { incompatible_header.active = false; }

                                for (int i = 0; i < incompatible_holder.transform.childCount; i++)
                                {
                                    incompatible_holder.transform.GetChild(i).gameObject.active = true;                                    
                                    Functions.GetChild(incompatible_holder.transform.GetChild(i).gameObject, "Button").active = true;
                                    incompatible_holder.transform.GetChild(i).gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                                }
                            }
                            else
                            {
                                incompatible_header.active = false;
                                incompatible_holder.active = false;
                            }
                            
                            //Hidden
                            foreach (GameObject item in hidden_list) { item.transform.SetParent(hidden_holder.transform); }
                            hidden_list.Clear();

                            //Use to Debug
                            Main.logger_instance.Msg("Unused count = " + unused_holder.transform.childCount);
                            Main.logger_instance.Msg("Incompatible count = " + incompatible_holder.transform.childCount);
                            Main.logger_instance.Msg("Hidden count = " + hidden_holder.transform.childCount);
                        }
                    }
                }
            }
        }
        public class Shard_Affix_List_Element
        {
            //Craft without shards for unique set legendary and idol
            [HarmonyPatch(typeof(ShardAffixListElement), "setQuantityAndUpdateText")]
            public class ShardAffixListElement_setQuantityAndUpdateText
            {
                [HarmonyPrefix]
                static void Prefix(/*ShardAffixListElement __instance,*/ ref int __0)
                {
                    if (Current.item !=null)
                    {
                        if ((Get.IsIdol(Current.item)) || (Current.item.isUniqueSetOrLegendary()))
                        {
                            if (__0 == 0) { __0 = 1; }
                        }
                    }                    
                }
            }
        }
        public class Crafting_Slot_Manager
        {
            public static bool forgin = false;
            public static int affix_id = -1;
            public static int affix_tier = -1;
            
            public static void Update_Slot(CraftingSlotManager __instance, float affix_id)
            {
                foreach (AffixSlotForge slot in __instance.affixSlots)
                {
                    if (slot.AffixID == affix_id)
                    {
                        Current.slot = slot;
                        break;
                    }
                }
            }
            public static void Update_UpgradeBtn()
            {
                if (!Current.slot.IsNullOrDestroyed())
                {
                    GameObject upgrade = Functions.GetChild(Current.slot.gameObject, "upgradeAvailable");
                    if (!upgrade.IsNullOrDestroyed())
                    {
                        Current.btn = upgrade.GetComponent<CraftingUpgradeButton>();
                    }
                }
            }

            [HarmonyPatch(typeof(CraftingSlotManager), "Forge")]
            public class CraftingSlotManager_Forge
            {
                [HarmonyPrefix]
                static void Prefix(ref CraftingSlotManager __instance)
                {
                    forgin = true;
                    __instance.forgeButton.gameObject.active = false;
                    affix_id = -1;
                    affix_tier = -1;
                    if ((Scenes.IsGameScene()) && (!Current.item.IsNullOrDestroyed()))
                    {
                        affix_id = __instance.appliedAffixID;
                        affix_tier = Get.Tier(Current.item, affix_id);
                        if (Current.slot.IsNullOrDestroyed()) { Update_Slot(__instance, affix_id); }
                        Update_UpgradeBtn();
                    }
                }

                [HarmonyPostfix]
                static void Postfix(ref CraftingSlotManager __instance)
                {
                    if (Scenes.IsGameScene())
                    {
                        if ((!Current.item.IsNullOrDestroyed()) && (affix_id > -1))
                        {
                            bool legendary = Current.item.isUniqueSetOrLegendary();
                            bool idol = Get.IsIdol(Current.item);
                            if (legendary) { Current.item.rarity = 9; }
                            if (affix_tier > -1) //update affix
                            {
                                if (!idol)
                                {
                                    bool force_upgrade = false;
                                    foreach (ItemAffix affix in Current.item.affixes)
                                    {
                                        if (affix.affixId == affix_id)
                                        {
                                            if ((affix_tier == affix.affixTier) && (affix_tier < 6))
                                            {
                                                bool error = false;
                                                if ((!legendary) && (enable_forgin_potencial_cost))
                                                {
                                                    int min = 0;
                                                    int max = 0;
                                                    if (affix_tier == 4) { min = 1; max = 23; }
                                                    else if (affix_tier == 5) { min = 1; max = 27; }
                                                    if (Current.item.forgingPotential >= (max - 1)) { Current.item.forgingPotential -= (byte)Random.RandomRangeInt(min, max); }
                                                    else
                                                    {
                                                        error = true; //Don't increment affix
                                                        Main.logger_instance.Error("You need " + (max - 1) + " forgin potencial on this item to craft T" + (affix_tier + 2));
                                                    }
                                                }
                                                if (!error)
                                                {
                                                    force_upgrade = true;
                                                    affix.affixTier++;
                                                    affix_tier = (int)affix.affixTier;
                                                    affix.affixRoll = (byte)Random.Range(0f, 255f);
                                                }
                                            }
                                            break;
                                        }
                                    }
                                    if (force_upgrade)
                                    {
                                        Current.item.RefreshIDAndValues();
                                        if (!Current.btn.IsNullOrDestroyed())
                                        {
                                            if (affix_tier < 6) { Current.btn.UpgradeButtonClicked(); }
                                            else { Current.btn.gameObject.active = false; }
                                        }
                                        else { Main.logger_instance.Error("Crafting_Upgrade_Button is null"); }
                                    }
                                }
                            }
                            else if (((legendary) || (idol)) && (!Current.slot.IsNullOrDestroyed()) && (!Current.btn.IsNullOrDestroyed())) //add affix
                            {
                                int nb_prefix = 0;
                                int nb_suffix = 0;
                                bool already_contain_affix = false;
                                foreach (ItemAffix item_affix in Current.item.affixes)
                                {
                                    if (item_affix.affixId == affix_id) { already_contain_affix = true; break; }
                                    if (item_affix.affixType == AffixList.AffixType.PREFIX) { nb_prefix++; }
                                    else if (item_affix.affixType == AffixList.AffixType.SUFFIX) { nb_suffix++; }
                                }
                                if (!already_contain_affix)
                                {
                                    AffixList.AffixType new_affix_type = AffixList.instance.GetAffixType(affix_id);
                                    if (((new_affix_type == AffixList.AffixType.PREFIX) && (nb_prefix < 2)) ||
                                        ((new_affix_type == AffixList.AffixType.SUFFIX) && (nb_suffix < 2)))
                                    {
                                        ItemAffix affix = new ItemAffix
                                        {
                                            affixId = (ushort)affix_id,
                                            affixTier = (byte)0,
                                            affixRoll = (byte)Random.Range(0f, 255f),
                                            affixType = new_affix_type
                                        };
                                        Current.item.affixes.Add(affix);
                                        Current.item.RefreshIDAndValues();
                                        if (!idol)
                                        {
                                            int index = nb_prefix;
                                            if (index == 2) { index += nb_suffix; }
                                            if (index < __instance.affixSlots.Count)
                                            {
                                                Current.slot = __instance.affixSlots[index];
                                                if (!Current.slot.IsNullOrDestroyed())
                                                {
                                                    Current.slot.affixID = (ushort)affix_id;
                                                    GameObject upgrade = Functions.GetChild(Current.slot.gameObject, "upgradeAvailable");
                                                    if (!upgrade.IsNullOrDestroyed())
                                                    {
                                                        upgrade.GetComponent<CraftingUpgradeButton>().UpgradeButtonClicked();

                                                        Current.btn = upgrade.GetComponent<CraftingUpgradeButton>();
                                                        Current.btn.gameObject.active = true;
                                                        Current.btn.UpgradeButtonClicked();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    __instance.forgeButton.gameObject.active = true;
                    forgin = false;
                }
            }
        }
    }
}

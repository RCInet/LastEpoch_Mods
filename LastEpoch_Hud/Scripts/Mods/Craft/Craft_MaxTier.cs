using HarmonyLib;
using Il2Cpp;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Craft
{
    [RegisterTypeInIl2Cpp]
    public class Craft_MaxTier : MonoBehaviour
    {
        public Craft_MaxTier(System.IntPtr ptr) : base(ptr) { }
        public static Craft_MaxTier instance { get; private set; }

        public static CraftingSlotManager crafting_slot_manager = null;
        public static ItemData item = null;

        void Awake()
        {
            instance = this;
        }

        public static void UpdateAffixs() //Show All Affix
        {            
            if ((!item.IsNullOrDestroyed()) && (!crafting_slot_manager.IsNullOrDestroyed()))
            {
                int prefix = 0;
                int suffix = 0;
                bool seal_primordial = false; //first t8 move to seal primordial (can be a suffix if you craft a suffix first)
                int seal_primordial_id = -1;
                foreach (ItemAffix affix in item.affixes)
                {
                    bool found = false;                    
                    AffixSlotForge slot = null;
                    if ((affix.affixType == AffixList.AffixType.PREFIX) && ((!affix.IsSealed) || (affix.affixTier == 7)))
                    {
                        if ((affix.affixTier == 7) && (!seal_primordial))
                        {
                            seal_primordial = true;
                            seal_primordial_id = affix.affixId;
                        }
                        else
                        {
                            AffixSlotForge.AffixSlotID slot_id = AffixSlotForge.AffixSlotID.PREFIX_ONE;
                            if (prefix == 1) { slot_id = AffixSlotForge.AffixSlotID.PREFIX_TWO; }
                            slot = Get.Slot(slot_id);
                            found = true;
                            prefix++;
                        }
                    }
                    else if ((affix.affixType == AffixList.AffixType.SUFFIX) && ((!affix.IsSealed) || (affix.affixTier == 7)))
                    {
                        if ((affix.affixTier == 7) && (!seal_primordial))
                        {
                            seal_primordial = true;
                            seal_primordial_id = affix.affixId;
                        }
                        else
                        {
                            AffixSlotForge.AffixSlotID slot_id = AffixSlotForge.AffixSlotID.SUFFIX_ONE;
                            if (suffix == 1) { slot_id = AffixSlotForge.AffixSlotID.SUFFIX_TWO; }
                            slot = Get.Slot(slot_id);
                            found = true;
                            suffix++;
                        }
                    }
                    if ((found) && (!slot.IsNullOrDestroyed()))
                    {
                        UnityEngine.Color color = crafting_slot_manager.defAffixColor; //T1 to T4
                        if (affix.affixTier == 4) { color = crafting_slot_manager.maxCraftColor; } //T5
                        else if (affix.affixTier > 4) { color = crafting_slot_manager.exaltColor; } //T6 to T8
                        slot.SetVisuals(affix, AffixList.instance.GetAffix(affix.affixId), false, color);
                        slot.gameObject.active = true;
                    }
                }
                if ((prefix == 1) || (suffix == 1))
                {
                    foreach (AffixSlotForge slot in crafting_slot_manager.affixSlots)
                    {
                        if (((slot.slotID == AffixSlotForge.AffixSlotID.PREFIX_TWO) && (prefix == 1)) ||
                            ((slot.slotID == AffixSlotForge.AffixSlotID.SUFFIX_TWO) && (suffix == 1)))
                        {
                            slot.gameObject.active = true;
                        }
                    }
                }
                if ((seal_primordial) && (seal_primordial_id > -1) && (!crafting_slot_manager.sealedPrimordialAffixHolder.IsNullOrDestroyed()))
                {
                    GameObject seal_primo_name_obj = Functions.GetChild(crafting_slot_manager.sealedPrimordialAffixHolder.gameObject, "AffixName");
                    if (!seal_primo_name_obj.IsNullOrDestroyed())
                    {
                        seal_primo_name_obj.GetComponent<TextMeshProUGUI>().text = AffixList.instance.GetAffixName(seal_primordial_id);
                    }
                }
            }
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
                if ((item.itemType > 24) && (item.itemType < 34))
                {
                    result = true;
                }

                return result;
            }
            public static int MaxForginCost(int tier)
            {
                int forgin_cost = 30; //To T6
                if (tier == 5) { forgin_cost = 36; } //To T7
                else if (tier == 6) { forgin_cost = 42; } //To T8

                return forgin_cost;
            }
            public static AffixSlotForge Slot(AffixSlotForge.AffixSlotID slot_id)
            {
                AffixSlotForge result = null;
                if (!crafting_slot_manager.IsNullOrDestroyed())
                {
                    foreach (AffixSlotForge slot in crafting_slot_manager.affixSlots)
                    {
                        if (slot.slotID == slot_id)
                        {
                            result = slot;
                            break;
                        }
                    }                    
                }
                else { Main.logger_instance.Error("crafting_slot_manager is null"); }

                return result;
            }
        }
        
        [HarmonyPatch(typeof(CraftingManager), "OnMainItemChange")]
        public class CraftingManager_OnMainItemChange
        {
            [HarmonyPostfix]
            static void Postfix(ref CraftingManager __instance, ref Il2CppSystem.Object __0, ref ItemContainerEntryHandler __1)
            {
                if (!__0.IsNullOrDestroyed())
                {
                    item = null;
                    OneItemContainer item_container = __0.TryCast<OneItemContainer>();
                    if (!item_container.IsNullOrDestroyed())
                    {
                        if (!item_container.content.IsNullOrDestroyed())
                        {
                            item = item_container.content.data;
                            UpdateAffixs();
                        }
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
                item = null;
            }
        }

        [HarmonyPatch(typeof(CraftingManager), "CheckForgeCapability")]
        public class CheckForgeCapability
        {
            [HarmonyPostfix]
            static void Postfix(ref CraftingManager __instance, ref bool __result, ref System.String __0, ref System.Boolean __1, ref System.Boolean __2, ref System.String __3)
            {
                //Main.logger_instance.Msg("CraftingManager:CheckForgeCapability(); __0 = " + __0);
                if ((__0 == Craft_Locales.affix_is_maxed) && (!item.IsNullOrDestroyed()))
                {
                    int affix_id = __instance.appliedAffixID;
                    int affix_tier = Get.Tier(item, affix_id);
                    int max_forgin_potencial = Get.MaxForginCost(affix_tier);
                    if (!crafting_slot_manager.IsNullOrDestroyed()) { crafting_slot_manager.maxForgingPotentialText.text = "<size=13><color=#FF0000>-" + max_forgin_potencial; }
                    if (item.forgingPotential > max_forgin_potencial)
                    {
                        if (affix_tier < 7) //T8
                        {
                            __0 = "Upgrade Affix";
                            __1 = false;
                            __2 = false;
                            __3 = "";
                            __result = true;
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(CraftingUpgradeButton), "UpdateButton")]
        public class CraftingUpgradeButton_UpdateButton
        {
            [HarmonyPrefix]
            static void Prefix(ref CraftingUpgradeButton __instance, int __0, ref bool __1)
            {
                if ((Scenes.IsGameScene()) && (!item.IsNullOrDestroyed()) && (__0 > -1))
                {
                    if (Get.IsIdol(item)) { __1 = false; }
                    else
                    {
                        int tier = Get.Tier(item, __0);
                        if ((tier > -1) && (tier < 7) && (item.forgingPotential > Get.MaxForginCost(tier))) { __1 = true; }
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(UIBase), "openCraftingPanel")]
        public class UIBase_openCraftingPanel
        {
            [HarmonyPostfix]
            static void Postfix()
            {
                UpdateAffixs();
            }
        }

        [HarmonyPatch(typeof(CraftingSlotManager), "Awake")]
        public class CraftingSlotManager_Awake
        {
            [HarmonyPostfix]
            static void Postfix(ref CraftingSlotManager __instance)
            {
                crafting_slot_manager = __instance;                
            }
        }

        [HarmonyPatch(typeof(CraftingSlotManager), "Forge")]
        public class CraftingSlotManager_Forge
        {
            [HarmonyPrefix]
            static bool Prefix(ref CraftingSlotManager __instance)
            {
                bool result = true;
                if ((Scenes.IsGameScene()) && (!item.IsNullOrDestroyed()))
                {
                    bool glyph_of_hope = false;                             //25% no forgin potencial cost                 
                    bool glyph_of_chaos = false;                            //Change affix
                    bool glyph_of_order = false;                            //don't roll when upgrade
                    //bool glyph_of_despair = false;                          //50% chance to seal (don't work well)
                    bool glyph_of_envy = false;                             //Change subtype

                    OneItemContainer glyph_container = __instance.GetSupport();
                    if (!glyph_container.IsNullOrDestroyed())
                    {
                        ItemData glyph_item = glyph_container.getItem();
                        if (!glyph_item.IsNullOrDestroyed())
                        {
                            switch (glyph_item.subType)
                            {
                                case 0: { glyph_of_hope = true; break; }
                                case 1: { glyph_of_chaos = true; break; }
                                case 2: { glyph_of_order = true; break; }
                                //case 3: { glyph_of_despair = true; break; }
                                case 5: { glyph_of_envy = true; break; }

                            }
                        }
                    }

                    int affix_id = __instance.appliedAffixID;
                    int affix_tier = Get.Tier(item, affix_id);
                    if ((affix_tier > 3) && (affix_tier < 7))
                    {
                        bool seal = false;
                        /*if (glyph_of_despair)
                        {
                            //int roll = Random.RandomRangeInt(0, 2); //50% (0-1)
                            //if (roll == 0) { seal = true; }
                        }*/
                        if (seal)
                        {
                            int index = 0;
                            foreach (ItemAffix affix in item.affixes)
                            {
                                if (affix.affixId == affix_id) { break; }
                                index++;
                            }
                            item.SealAffix(item.affixes[index]);
                        }
                        else
                        {
                            foreach (ItemAffix affix in item.affixes)
                            {
                                if (affix.affixId == affix_id)
                                {
                                    if (glyph_of_chaos)
                                    {
                                        System.Collections.Generic.List<AffixList.SingleAffix> affix_list = new System.Collections.Generic.List<AffixList.SingleAffix>();
                                        foreach (AffixList.SingleAffix aff in AffixList.instance.singleAffixes)
                                        {
                                            if ((aff.type == affix.affixType) && (aff.CanRollOn(item)))
                                            //if ((aff.type == affix.affixType) && (aff.CanRollOnItemType(item.itemType, item.TryCast<ItemDataUnpacked>().classReq)))
                                            {
                                                affix_list.Add(aff);
                                            }
                                        }
                                        if (affix_list.Count > 0)
                                        {
                                            affix.affixId = (ushort)affix_list[Random.RandomRangeInt(0, affix_list.Count)].affixId;
                                        }
                                        else { Main.logger_instance.Error("No affix found for glyph of chaos"); }
                                    }
                                    if (!glyph_of_order) { affix.affixRoll = (byte)Random.Range(0f, 255f); }
                                    affix.affixTier++;
                                    break;
                                }
                            }
                        }
                        bool no_cost = false;
                        if (glyph_of_hope)
                        {
                            int roll = Random.RandomRangeInt(0, 5); //25% (0-4)
                            if (roll == 0) { no_cost = true; }
                        }
                        if (!no_cost) { item.forgingPotential -= (byte)Random.RandomRangeInt(1, Get.MaxForginCost(affix_tier)); }
                        if (glyph_of_envy)
                        {
                            System.Collections.Generic.List<int> subtype_list = new System.Collections.Generic.List<int>();
                            foreach (ItemList.BaseEquipmentItem base_item in ItemList.instance.EquippableItems)
                            {
                                if (base_item.baseTypeID == item.itemType)
                                {
                                    foreach (ItemList.EquipmentItem sub_item in base_item.subItems)
                                    {
                                        if (item.TryCast<ItemDataUnpacked>().classReq == sub_item.classRequirement)
                                        {
                                            if (sub_item.subTypeID != item.subType)
                                            {
                                                subtype_list.Add(sub_item.subTypeID);
                                            }
                                        }
                                    }
                                }
                            }
                            if (subtype_list.Count > 0)
                            {
                                item.subType = (ushort)subtype_list[Random.RandomRangeInt(0, subtype_list.Count)];
                            }
                            else { Main.logger_instance.Error("No subtype found for glyph of envy"); }
                        }
                        item.RefreshIDAndValues();
                        result = false;
                    }
                }
                
                return result;
            }
        }
    }
}

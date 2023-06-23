using HarmonyLib;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Items_Mods
    {
        #region OnSceneChange
        public class Basic
        {
            public static bool EquipmentItem_UnlockDropForAll = false;
            public static bool EquipmentItem_UnlockDropForUndropableOnly = false; //Lock Dropable Item
            public static bool EquipmentItem_RemoveClassReq = false;
            public static bool EquipmentItem_EditLevelReq = false;
            public static int EquipmentItem_SetLevelReq = 0;

            private static ItemList basic_items_list = null;
            private static bool basics_found = false;
            private static void InitBasicItemList()
            {
                if (!basics_found)
                {
                    try
                    {
                        basic_items_list = ItemList.get();
                        if (basic_items_list != null)
                        {
                            basics_found = true;
                            Main.logger_instance.Msg("Basic item list found : " + basic_items_list.name);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Main.logger_instance.Msg("Error Basic item list");
                    }
                }
            }
            public static void Launch()
            {
                if (!basics_found) { InitBasicItemList(); }             
                if (basic_items_list != null)
                {
                    for (int i = 0; i < 34; i++)
                    {
                        if ((i != 11) && (i != 24))
                        {
                            Il2CppSystem.Collections.Generic.List<ItemList.EquipmentItem> items = basic_items_list.GetEquipmentSubItems(i);
                            foreach (var item in items)
                            {
                                if (EquipmentItem_UnlockDropForAll) { item.cannotDrop = false; }
                                else if (EquipmentItem_UnlockDropForUndropableOnly)
                                {
                                    if (item.cannotDrop) { item.cannotDrop = false; }
                                    else { item.cannotDrop = true; }
                                }
                                if (EquipmentItem_RemoveClassReq)
                                {
                                    item.classRequirement = ItemList.ClassRequirement.None;
                                }
                                if (EquipmentItem_EditLevelReq)
                                {
                                    item.levelRequirement = EquipmentItem_SetLevelReq;
                                }
                            }
                        }
                    }
                }
            }
        }
        public class Unique
        {
            public static bool UniqueList_Entry_UnlockDropForAll = false;
            public static bool UniqueList_Entry_UnlockDropForUndropableOnly = false; //Lock Dropable Item
            public static bool Enable_LegendaryPotentialLevelMod = false;
            public static int UniqueList_Entry_LegendaryPotentialLevel = 0;
            
            private static UniqueList unique_items_list = null;
            private static bool uniques_found = false;
            private static void InitUniqueItemList()
            {
                if (!uniques_found)
                {
                    try
                    {
                        unique_items_list = UniqueList.get();
                        if (unique_items_list != null)
                        {
                            uniques_found = true;
                            Main.logger_instance.Msg("Unique item list found : " + unique_items_list.name);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Main.logger_instance.Msg("Error Unique item list");
                    }
                }
            }
            public static void Launch()
            {
                if (!uniques_found) { InitUniqueItemList(); }
                if (unique_items_list != null)
                {
                    Il2CppSystem.Collections.Generic.List<UniqueList.Entry> Uniques_List_Entry = unique_items_list.uniques;
                    foreach (UniqueList.Entry item in Uniques_List_Entry)
                    {
                        if (UniqueList_Entry_UnlockDropForAll) { item.canDropRandomly = true; }
                        else if (UniqueList_Entry_UnlockDropForUndropableOnly)
                        {
                            if (!item.canDropRandomly) { item.canDropRandomly = true; }
                            else { item.canDropRandomly = false; }
                        }
                        if (Enable_LegendaryPotentialLevelMod)
                        {
                            item.effectiveLevelForLegendaryPotential = (byte)UniqueList_Entry_LegendaryPotentialLevel;
                        }
                        if (Mods.UniqueMods.Enable_UniqueMods)
                        {
                            foreach (Mods.UniqueMods.unique_mod m in Mods.UniqueMods.Uniques_Mods)
                            {
                                if (m.id == item.uniqueID) { item.mods = m.mods; break; }
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #region Functions Patch
        public class AutoLoot
        {
            #region Items
            public static bool AutoPickup_Key = true;
            public static bool AutoPickup_Craft = true;
            public static bool AutoPickup_UniqueAndSet = false;
            public static bool AutoStore_Materials = true;

            [HarmonyPatch(typeof(GroundItemManager), "dropItemForPlayer")]
            public class Items
            {
                [HarmonyPostfix]
                static void Postfix1(GroundItemManager __instance, Actor __0, ItemData __1, UnityEngine.Vector3 __2, bool __3)
                {
                    if (__instance != null)
                    {
                        System.UInt32 item_id = __instance.nextItemId - 1;
                        bool pickup = false;
                        if ((AutoPickup_Key) && (Item.isKey(__1.itemType))) { pickup = true; }
                        else if ((AutoPickup_Craft) && (ItemList.isCraftingItem(__1.itemType))) { pickup = true; }
                        else if ((AutoPickup_UniqueAndSet) && (Item.rarityIsUniqueSetOrLegendary(__1.rarity))) { pickup = true; }

                        if (pickup)
                        {
                            __instance.pickupItem(__0, item_id);
                            if ((AutoStore_Materials) && (ItemList.isCraftingItem(__1.itemType)))
                            {
                                InventoryPanelUI.instance.StoreMaterialsButtonPress();
                            }
                        }
                    }
                }
            }
            #endregion
            #region Gold
            public static bool AutoPickup_Gold = true;

            [HarmonyPatch(typeof(GroundItemManager), "dropGoldForPlayer")]
            public class Gold
            {
                [HarmonyPostfix]
                static void Postfix2(GroundItemManager __instance, Actor __0, int __1, UnityEngine.Vector3 __2, bool __3)
                {
                    if ((__instance != null) && (AutoPickup_Gold))
                    {
                        System.UInt32 gold_id = __instance.nextGoldId - 1;
                        foreach (GoldPickupInteraction gold_pickup_interaction in __instance.activeGoldPiles)
                        {
                            if (gold_pickup_interaction.id == gold_id)
                            {
                                __instance.pickupGold(__0, gold_id, gold_pickup_interaction);
                                break;
                            }
                        }
                    }
                }
            }
            #endregion            
            #region XpTome
            public static bool AutoPickup_XpTome = true;

            [HarmonyPatch(typeof(GroundItemManager), "dropXPTomeForPlayer")]
            public class XpTome
            {
                [HarmonyPostfix]
                static void Postfix3(GroundItemManager __instance, Actor __0, int __1, UnityEngine.Vector3 __2, bool __3)
                {
                    if ((__instance != null) && (AutoPickup_XpTome))
                    {
                        System.UInt32 tome_id = __instance.nextXpTomeId - 1;
                        foreach (PickupExperiencePotionInteraction pick_exp_pot_interaction in __instance.activeXPTomes)
                        {
                            if (pick_exp_pot_interaction.id == tome_id)
                            {
                                __instance.pickupXPTome(__0, tome_id, pick_exp_pot_interaction);
                                break;
                            }
                        }
                    }
                }
            }
            #endregion
            #region Pots
            public static bool AutoPickup_Pots = false;

            [HarmonyPatch(typeof(GroundItemManager), "dropPotionForPlayer")]
            public class Potions
            {
                [HarmonyPostfix]
                static void Postfix(GroundItemManager __instance, Actor __0, UnityEngine.Vector3 __1, bool __2)
                {
                    if ((__instance != null) && (AutoPickup_Pots))
                    {
                        System.UInt32 pot_id = __instance.nextPotionId - 1;
                        foreach (PotionPickupInteraction pick_pot_interaction in __instance.activePotions)
                        {
                            if (pick_pot_interaction.id == pot_id)
                            {
                                __instance.pickupPotion(__0, pot_id, pick_pot_interaction);
                                break;
                            }
                        }
                    }
                }
            }
            #endregion            
        }
        public class WeaversWill
        {
            [HarmonyPatch(typeof(ItemData), "RollWeaversWill")]
            public class MaxRoll
            {
                [HarmonyPrefix]
                static void Prefix(ItemData __instance, int __result, UniqueList.Entry __0, int __1, int __2)
                {                    
                    __instance.weaversWill = 28;
                    //ItemDataUnpacked item = __instance.getAsUnpacked();
                    //item.weaversWill = 28;
                    //__1 = 28;
                    //__2 = 100;
                    //__result = 28;
                }
            }
        }
        #endregion
    }
}

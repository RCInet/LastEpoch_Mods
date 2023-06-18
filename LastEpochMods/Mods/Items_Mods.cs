using HarmonyLib;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Items_Mods
    {
        public class Basic
        {
            public static bool EquipmentItem_UnlockDropForAll = false;
            public static bool EquipmentItem_UnlockDropForUndropableOnly = false; //Lock Dropable Item
            public static bool EquipmentItem_RemoveClassReq = true;
            public static bool EquipmentItem_EditLevelReq = true;
            public static int EquipmentItem_SetLevelReq = 0;

            public static void Launch()
            {
                UnityEngine.Object obj = Functions.GetObject("MasterItemsList");
                System.Type type = obj.GetActualType();
                if (type == typeof(ItemList))
                {
                    ItemList item_list = obj.TryCast<ItemList>();
                    for (int i = 0; i < 34; i++)
                    {
                        if ((i != 11) && (i != 24))
                        {
                            Il2CppSystem.Collections.Generic.List<ItemList.EquipmentItem> items = item_list.GetEquipmentSubItems(i);
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

            public static void Launch()
            {
                UnityEngine.Object obj = Functions.GetObject("UniqueList");
                System.Type type = obj.GetActualType();
                if (type == typeof(UniqueList))
                {
                    UniqueList unique_list = obj.TryCast<UniqueList>();
                    Il2CppSystem.Collections.Generic.List<UniqueList.Entry> Uniques_List_Entry = unique_list.uniques;
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
            /*[HarmonyPatch(typeof(ItemData), "RollWeaversWill")]
            public class MaxRoll
            {
                [HarmonyPrefix]
                static void Prefix(ItemData __instance, int __result, UniqueList.Entry __0, int __1, int __2)
                {
                    UnityExplorer.ExplorerCore.Log("RollWeaversWill");
                    __1 = 28;
                    __2 = 100;
                    __result = 28;
                }
            }*/
        }
    }
}

using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Ground_Item_Manager
    {
        [HarmonyPatch(typeof(GroundItemManager), "dropItemForPlayer")]
        public class dropItemForPlayer
        {
            [HarmonyPostfix]
            static void Postfix(ref GroundItemManager __instance, ref Actor __0, ref ItemData __1, ref UnityEngine.Vector3 __2, ref bool __3)
            {
                if (__instance != null)
                {
                    System.UInt32 item_id = __instance.nextItemId - 1;
                    bool pickup = false;
                    if ((Config.Data.mods_config.auto_loot.AutoPickup_Key) && (Item.isKey(__1.itemType))) { pickup = true; }
                    else if ((Config.Data.mods_config.auto_loot.AutoPickup_Craft) && (ItemList.isCraftingItem(__1.itemType))) { pickup = true; }
                    else if ((Config.Data.mods_config.auto_loot.AutoPickup_UniqueAndSet) && (Item.rarityIsUniqueSetOrLegendary(__1.rarity))) { pickup = true; }

                    if (pickup)
                    {
                        __instance.pickupItem(__0, item_id);
                        if ((Config.Data.mods_config.auto_loot.AutoStore_Materials) && (ItemList.isCraftingItem(__1.itemType)))
                        {
                            InventoryPanelUI.instance.StoreMaterialsButtonPress();
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(GroundItemManager), "dropGoldForPlayer")]
        public class dropGoldForPlayer
        {
            [HarmonyPostfix]
            static void Postfix(ref GroundItemManager __instance, ref Actor __0, ref int __1, ref UnityEngine.Vector3 __2, ref bool __3)
            {
                if ((__instance != null) && (Config.Data.mods_config.auto_loot.AutoPickup_Gold))
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

        [HarmonyPatch(typeof(GroundItemManager), "dropXPTomeForPlayer")]
        public class dropXPTomeForPlayer
        {
            [HarmonyPostfix]
            static void Postfix(ref GroundItemManager __instance, ref Actor __0, ref int __1, ref UnityEngine.Vector3 __2, ref bool __3)
            {
                if ((__instance != null) && (Config.Data.mods_config.auto_loot.AutoPickup_XpTome))
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

        [HarmonyPatch(typeof(GroundItemManager), "dropPotionForPlayer")]
        public class dropPotionForPlayer
        {
            [HarmonyPostfix]
            static void Postfix(GroundItemManager __instance, Actor __0, UnityEngine.Vector3 __1, bool __2)
            {
                if ((__instance != null) && (Config.Data.mods_config.auto_loot.AutoPickup_Pots))
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
    }
}

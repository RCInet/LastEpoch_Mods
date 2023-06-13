using BeardedManStudios.Threading;
using Il2CppSystem.Threading.Tasks;
using UnityEngine;

namespace LastEpochMods.Mods
{
    public class AutoLoot
    {
        public static bool AutoPickup_Key = true;
        public static bool AutoPickup_Craft = true;
        public static bool AutoPickup_UniqueAndSet = false;
        public static bool AutoStore_Materials = true;
        public static bool AutoPickup_Gold = true;

        [HarmonyLib.HarmonyPatch(typeof(GroundItemManager))]
        public class AutoPickup
        {
            [HarmonyLib.HarmonyPostfix]
            [HarmonyLib.HarmonyPatch("dropItemForPlayer")]
            static void Postfix(GroundItemManager __instance, Actor __0, ItemData __1, UnityEngine.Vector3 __2, bool __3)
            {
                if (__instance != null)
                {
                    System.UInt32 item_id = __instance.nextItemId - 1;
                    bool pickup = false;
                    if ((AutoPickup_Key) && (Item.isKey(__1.itemType))) { pickup = true; }
                    else if((AutoPickup_Craft) && (ItemList.isCraftingItem(__1.itemType))) { pickup = true; }
                    else if((AutoPickup_UniqueAndSet) && (Item.rarityIsUniqueSetOrLegendary(__1.rarity))) { pickup = true; }

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

            [HarmonyLib.HarmonyPostfix]
            [HarmonyLib.HarmonyPatch("dropGoldForPlayer")]
            static void Postfix(GroundItemManager __instance, Actor __0, int __1, UnityEngine.Vector3 __2, bool __3)
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
    }
}

using HarmonyLib;
using LastEpochMods.Managers;

namespace LastEpochMods.Mods.Items
{
    public class RangePickup
    {
        public static void MoveItemToPlayer(GroundItemManager.GroundItem item)
        {
            Actor player = PlayerFinder.getPlayerActor();
            item.location = player.position();
        }

        [HarmonyPatch(typeof(GroundItemLabel), "ClickedItem")]
        public class ClickedItem
        {
            [HarmonyPrefix]
            static bool Prefix(ref GroundItemLabel __instance)
            {
                if (Save_Manager.Data.UserData.Items.Pickup.Enable_RangePickup)
                {
                    ItemDataUnpacked item_data = __instance.getItemData();
                    Il2CppSystem.Collections.Generic.List<GroundItemManager.GroundItem> ground_item_list = GroundItemManager.instance.offlineItems.items;
                    bool found = false;
                    uint id = 0;
                    Actor player = PlayerFinder.getPlayerActor();

                    foreach (GroundItemManager.GroundItem ground_item in ground_item_list)
                    {
                        if (item_data.Pointer == ground_item.itemData.Pointer)
                        {
                            found = true;
                            id = ground_item.id;
                            MoveItemToPlayer(ground_item);
                            break;
                        }
                    }
                    if (found)
                    {
                        Main.logger_instance.Msg("Try to Pickup Item");
                        GroundItemManager.instance.pickupItem(player, id);
                        return false;
                    }
                    else
                    {
                        Main.logger_instance.Msg("Item not Found");
                        return true;
                    }
                }
                else { return true; }
            }
        }
    }
}

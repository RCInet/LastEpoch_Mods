using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_RangePickup
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()) &&
                (!Refs_Manager.ground_item_manager.IsNullOrDestroyed()) &&
                (!Refs_Manager.player_actor.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Items.Pickup.Enable_RangePickup;
                }
                else { return false; }
            }
            else { return false; }
        }
        
        [HarmonyPatch(typeof(GroundItemLabel), "ClickedItem")]
        public class ClickedItem
        {
            [HarmonyPrefix]
            static bool Prefix(ref GroundItemLabel __instance)
            {
                if (CanRun())
                {
                    bool found = false;
                    ItemDataUnpacked item_data = __instance.getItemData();
                    foreach (GroundItemManager.GroundItem ground_item in Refs_Manager.ground_item_manager.offlineItems.items)
                    {
                        if (item_data.Pointer == ground_item.itemData.Pointer)
                        {
                            found = true;
                            ground_item.location = Refs_Manager.player_actor.position();
                            GroundItemManager.instance.pickupItem(Refs_Manager.player_actor, ground_item.id);
                            break;
                        }
                    }
                    if (found) { return false; }
                    else { return true; }
                }
                else { return true; }
            }
        }
    }
}

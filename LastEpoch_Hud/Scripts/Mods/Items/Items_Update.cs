namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_Update
    {
        public static void Reqs()
        {
            Items_Req_Class.Enable();
            Items_Req_Level.Enable();
            if ((Items_Req_Class.need_update) ||
                (Items_Req_Level.need_update))
            {
                Items_Req_Class.need_update = false;
                Items_Req_Level.need_update = false;
                AllItems();
            }
        }
        public static void AllItems()
        {
            if (!Refs_Manager.ground_item_manager.IsNullOrDestroyed())
            {
                if (!Refs_Manager.ground_item_manager.offlineItems.items.IsNullOrDestroyed())
                {
                    foreach (GroundItemManager.GroundItem item in Refs_Manager.ground_item_manager.offlineItems.items)
                    {
                        item.itemData.RefreshIDAndValues();
                    }
                }
            }
            if (!Refs_Manager.item_containers_manager.IsNullOrDestroyed())
            {
                if (!Refs_Manager.item_containers_manager.equipment.containers.IsNullOrDestroyed())
                {
                    foreach (OneSlotItemContainerWithCharacterRequirements slot in Refs_Manager.item_containers_manager.equipment.containers)
                    {
                        if (!slot.content.IsNullOrDestroyed()) { slot.content.data.RefreshIDAndValues(); }
                    }
                }
                if (!Refs_Manager.item_containers_manager.inventory.content.IsNullOrDestroyed())
                {
                    foreach (ItemContainerEntry item_container_entry in Refs_Manager.item_containers_manager.inventory.content)
                    {
                        item_container_entry.data.RefreshIDAndValues();
                    }
                }
                /*if (!Refs_Manager.item_containers_manager.stash.stashItemContainers.IsNullOrDestroyed())
                {
                    foreach (ItemContainer item_container in Refs_Manager.item_containers_manager.stash.stashItemContainers)
                    {
                        if (!item_container.content.IsNullOrDestroyed())
                        {
                            foreach (ItemContainerEntry item_container_entry in item_container.content)
                            {
                                item_container_entry.data.RefreshIDAndValues();
                            }
                        }
                    }
                }*/
            }
        }
    }
}

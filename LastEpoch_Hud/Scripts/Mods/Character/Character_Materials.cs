namespace LastEpoch_Hud.Scripts.Mods.Character
{
    public class Character_Materials
    {
        public static void GetAllRunesX99()
        {
            if (!Refs_Manager.item_list.IsNullOrDestroyed())
            {
                Hud_Manager.Hud_Base.Resume_Click(); //Close Hud
                int item_type = 102;
                ItemList.BaseNonEquipmentItem items = GetItemList(item_type);
                if (!items.IsNullOrDestroyed())
                {
                    foreach (ItemList.NonEquipmentItem item in items.subItems)
                    {
                        ForceDrop(item_type, item.subTypeID, 99);
                    }
                }                
            }
        }
        public static void GetAllGlyphsX99()
        {
            if (!Refs_Manager.item_list.IsNullOrDestroyed())
            {
                Hud_Manager.Hud_Base.Resume_Click(); //Close Hud
                int item_type = 103;
                foreach (ItemList.NonEquipmentItem item in GetItemList(item_type).subItems)
                {
                    ForceDrop(item_type, item.subTypeID, 99);
                }
            }
        }
        public static void GetAllShardsX10()
        {
            if (!Refs_Manager.item_list.IsNullOrDestroyed())
            {
                Hud_Manager.Hud_Base.Resume_Click(); //Close Hud
                int item_type = 101;
                foreach (ItemList.NonEquipmentItem item in GetItemList(item_type).subItems)
                {
                    ForceDrop(item_type, item.subTypeID, 10);
                }
            }
        }
        private static void ForceDrop(int type, int subtype, int quantity)
        {
            if ((!Refs_Manager.ground_item_manager.IsNullOrDestroyed()) &&
                (!Refs_Manager.player_actor.IsNullOrDestroyed()))
            {
                ItemDataUnpacked item = new ItemDataUnpacked
                {
                    LvlReq = 0,
                    classReq = ItemList.ClassRequirement.Any,
                    itemType = (byte)type,
                    subType = (ushort)subtype,
                    rarity = (byte)0,
                    sockets = (byte)0,
                    uniqueID = (ushort)0
                };
                item.RefreshIDAndValues();
                ItemData final_item = item.TryCast<ItemData>();
                for (int i = 0; i < quantity; i++)
                {
                    Refs_Manager.ground_item_manager.dropItemForPlayer(Refs_Manager.player_actor, final_item, Refs_Manager.player_actor.position(), false);
                }
            }
            else { Main.logger_instance.Error("Ground Item Manager Not Found"); }
        }
        private static ItemList.BaseNonEquipmentItem GetItemList(int type_id)
        {
            ItemList.BaseNonEquipmentItem result = null;            
            if (!Refs_Manager.item_list.IsNullOrDestroyed())
            {
                int index = 0;
                foreach (ItemList.BaseNonEquipmentItem n_item in Refs_Manager.item_list.nonEquippableItems)
                {
                    if (n_item.baseTypeID == type_id)
                    {
                        result = Refs_Manager.item_list.nonEquippableItems[index];
                        break;
                    }
                    index++;
                }
                if (result.IsNullOrDestroyed()) { Main.logger_instance.Error("Character_Materials : List with type = " + type_id + " not found"); }
            }
            else { Main.logger_instance.Error("Character_Materials : Itemlist is null"); }
            
            return result;
        }
    }
}

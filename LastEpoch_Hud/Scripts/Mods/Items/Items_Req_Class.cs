namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_Req_Class
    {
        public struct req_class_structure
        {
            public int type;
            public int base_id;
            public ItemList.ClassRequirement class_req;
        }
        public static bool need_update = false;
        public static void Enable()
        {
            if (CanRun())
            {
                if (backup.IsNullOrDestroyed()) { Backup(); }
                if (!backup.IsNullOrDestroyed())
                {
                    if (Save_Manager.instance.data.Items.Req.classe) { Remove(); }
                    else if (req_removed) { Reset(); }
                    need_update = true;
                }
            }
        }

        private static bool req_removed = false;
        private static System.Collections.Generic.List<req_class_structure> backup = null;
        private static void Backup()
        {
            if (CanRun())
            {
                backup = new System.Collections.Generic.List<req_class_structure>();
                foreach (ItemList.BaseEquipmentItem base_item in Refs_Manager.item_list.EquippableItems)
                {
                    foreach (ItemList.EquipmentItem item in base_item.subItems)
                    {
                        req_class_structure req = new req_class_structure();
                        req.type = base_item.baseTypeID;
                        req.base_id = item.subTypeID;
                        req.class_req = item.classRequirement;
                        backup.Add(req);
                    }
                }
            }
        }
        private static void Remove()
        {
            if ((CanRun()) && (!backup.IsNullOrDestroyed()))
            {
                foreach (ItemList.BaseEquipmentItem base_item in Refs_Manager.item_list.EquippableItems)
                {
                    foreach (ItemList.EquipmentItem item in base_item.subItems)
                    {
                        item.classRequirement = ItemList.ClassRequirement.None;
                    }
                }
                req_removed = true;
            }
        }
        private static void Reset()
        {
            if ((CanRun()) && (!backup.IsNullOrDestroyed()))
            {
                foreach (ItemList.BaseEquipmentItem base_item in Refs_Manager.item_list.EquippableItems)
                {
                    foreach (ItemList.EquipmentItem item in base_item.subItems)
                    {
                        foreach (req_class_structure backup_req in backup)
                        {
                            if ((backup_req.type == base_item.baseTypeID) && (backup_req.base_id == item.subTypeID))
                            {
                                item.classRequirement = backup_req.class_req;
                                break;
                            }
                        }
                    }
                }
                req_removed = false;
            }
        }        
        private static bool CanRun()
        {
            if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!Refs_Manager.item_list.IsNullOrDestroyed()))
            {
                if (Save_Manager.instance.initialized) { return true; }
                else { return false; }
            }
            else { return false; }
        }
    }
}

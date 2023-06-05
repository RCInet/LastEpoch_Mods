using UniverseLib;

namespace LastEpochMods.Mods
{
    public class ItemsDrop
    {
        public static void UnlockForAllBasic(Main main)
        {
            UnityEngine.Object obj = Functions.GetObject("MasterItemsList");
            System.Type type = obj.GetActualType();
            if (type == typeof(ItemList))
            {
                ItemList item_list = obj.TryCast<ItemList>();
                foreach (Db.Get.Type.Type_Structure type_struct in Db.Get.Type.TypesArray)
                {
                    if (type_struct.Id < 34)
                    {
                        if ((type_struct.Id != 11) && (type_struct.Id != 24)) //Fist and Crosbow
                        {
                            Il2CppSystem.Collections.Generic.List<ItemList.EquipmentItem> items = item_list.GetEquipmentSubItems(type_struct.Id);
                            foreach (var item in items)
                            {
                                if (item.cannotDrop)
                                {
                                    main.LoggerInstance.Msg("Basic : " + item.name + " : Can Drop Now");
                                    item.cannotDrop = false;
                                }                                
                            }
                        }
                    }
                    else { break; }
                }
            }
        }
        public static void UnlockForAllUniques(Main main)
        {
            UnityEngine.Object obj2 = Functions.GetObject("UniqueList");
            System.Type type2 = obj2.GetActualType();
            if (type2 == typeof(UniqueList))
            {
                UniqueList unique_list = obj2.TryCast<UniqueList>();
                Il2CppSystem.Collections.Generic.List<UniqueList.Entry> Uniques_List_Entry = unique_list.uniques;
                foreach (UniqueList.Entry ul_entry in Uniques_List_Entry)
                {
                    if (!ul_entry.canDropRandomly)
                    {
                        main.LoggerInstance.Msg("Unique : " + ul_entry.name + " : Can Drop Now");
                        ul_entry.canDropRandomly = true;
                    }
                }
            }
        }
    }
}

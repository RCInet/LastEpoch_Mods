using UniverseLib;

namespace LastEpochMods.Mods
{
    public class ItemsReq
    {
        public static void Remove(Main main) //Remove Level and Class Req
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
                            string base_type = type_struct.Name;
                            Il2CppSystem.Collections.Generic.List<ItemList.EquipmentItem> items = item_list.GetEquipmentSubItems(type_struct.Id);
                            foreach (var item in items)
                            {
                                if (item.classRequirement != ItemList.ClassRequirement.None)
                                {
                                    main.LoggerInstance.Msg("Remove Class Req : " + item.name);
                                    item.classRequirement = ItemList.ClassRequirement.None;
                                }
                                if (item.levelRequirement > 0)
                                {
                                    main.LoggerInstance.Msg("Remove Level Req : " + item.name);
                                    item.levelRequirement = 0;
                                }                               
                            }                                                      
                        }
                    }
                    else { break; }
                }
            }
        }
    }
}

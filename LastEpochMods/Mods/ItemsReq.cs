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
                for (int i = 0; i < 34; i++)
                {
                    if ((i != 11) && (i != 24))
                    {
                        Il2CppSystem.Collections.Generic.List<ItemList.EquipmentItem> items = item_list.GetEquipmentSubItems(i);
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
            }
        }
    }
}

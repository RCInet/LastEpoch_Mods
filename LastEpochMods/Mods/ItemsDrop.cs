using LastEpochMods.Db.Json;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class ItemsDrop
    {
        public static bool OnlyUndropablesBasic = false;
        public static void UnlockForAllBasic(Main main)
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
                            if (item.cannotDrop)
                            {
                                main.LoggerInstance.Msg("Basic : " + item.name + " : Can Drop Now");
                                item.cannotDrop = false;
                            }
                            else if (OnlyUndropablesBasic)
                            {
                                main.LoggerInstance.Msg("Basic : " + item.name + " : Can't Drop");
                                item.cannotDrop = true;
                            }
                        }
                    }
                }
            }
        }
        public static bool OnlyUndropablesUnique = false;
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
                    else if (OnlyUndropablesUnique)
                    {
                        main.LoggerInstance.Msg("Unique : " + ul_entry.name + " : Can't Drop");
                        ul_entry.canDropRandomly = false;
                    }
                }
            }
        }
    }
}

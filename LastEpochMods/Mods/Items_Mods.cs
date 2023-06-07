using LastEpochMods.Db.Json;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class Items_Mods
    {
        public class Basic
        {
            public static bool EquipmentItem_UnlockDropForAll = false;
            public static bool EquipmentItem_UnlockDropForUndropableOnly = false; //Lock Dropable Item
            public static bool EquipmentItem_RemoveClassReq = true;
            public static bool EquipmentItem_EditLevelReq = true;
            public static int EquipmentItem_SetLevelReq = 0;

            public static void Launch()
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
                                if (EquipmentItem_UnlockDropForAll) { item.cannotDrop = false; }
                                else if (EquipmentItem_UnlockDropForUndropableOnly)
                                {
                                    if (item.cannotDrop) { item.cannotDrop = false; }
                                    else { item.cannotDrop = true; }
                                }
                                if (EquipmentItem_RemoveClassReq)
                                {
                                    item.classRequirement = ItemList.ClassRequirement.None;
                                }
                                if (EquipmentItem_EditLevelReq)
                                {
                                    item.levelRequirement = EquipmentItem_SetLevelReq;
                                }
                            }
                        }
                    }
                }
            }
        }
        public class Unique
        {
            public static bool UniqueList_Entry_UnlockDropForAll = false;
            public static bool UniqueList_Entry_UnlockDropForUndropableOnly = false; //Lock Dropable Item
            public static bool Enable_LegendaryPotentialLevelMod = false;
            public static int UniqueList_Entry_LegendaryPotentialLevel = 0;
            public static bool UniqueList_Entry_Enable_UniqueMods = false;
            public static System.Collections.Generic.List<unique_mod> Uniques_Mods = new System.Collections.Generic.List<unique_mod>();
            public struct unique_mod
            {
                public int id;
                public Il2CppSystem.Collections.Generic.List<UniqueItemMod> mods;
            }

            public static void Launch()
            {
                UnityEngine.Object obj = Functions.GetObject("UniqueList");
                System.Type type = obj.GetActualType();
                if (type == typeof(UniqueList))
                {
                    UniqueList unique_list = obj.TryCast<UniqueList>();
                    Il2CppSystem.Collections.Generic.List<UniqueList.Entry> Uniques_List_Entry = unique_list.uniques;
                    foreach (UniqueList.Entry item in Uniques_List_Entry)
                    {
                        if (UniqueList_Entry_UnlockDropForAll) { item.canDropRandomly = true; }
                        else if (UniqueList_Entry_UnlockDropForUndropableOnly)
                        {
                            if (!item.canDropRandomly) { item.canDropRandomly = true; }
                            else { item.canDropRandomly = false; }
                        }
                        if (Enable_LegendaryPotentialLevelMod)
                        {
                            item.effectiveLevelForLegendaryPotential = (byte)UniqueList_Entry_LegendaryPotentialLevel;
                        }
                        if (UniqueList_Entry_Enable_UniqueMods)
                        {
                            foreach (unique_mod m in Uniques_Mods)
                            {
                                if (m.id == item.uniqueID) { item.mods = m.mods; break; }
                            }
                        }
                    }
                }
            }
        }
    }
}

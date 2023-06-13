using HarmonyLib;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;
using UnityEngine;

namespace LastEpochMods.Mods
{
    public class Dungeons
    {
        [HarmonyLib.HarmonyPatch(typeof(ItemContainersManager))]
        public class NoKey
        {
            [HarmonyLib.HarmonyPrefix]
            [HarmonyLib.HarmonyPatch("IsOccupiedWithValidDungeonKey")]
            static void Prefix(ItemContainersManager __instance, bool __result, DungeonID __0)
            {
                if ((__instance != null) && (!__result)) //Add key
                {
                    byte item_type = 104;
                    Il2CppStructArray<byte> item_id = new Il2CppStructArray<byte>(22);
                    item_id[0] = 1;
                    item_id[1] = item_type;
                    item_id[2] = 0; //id
                    for (int i = 3; i < item_id.Count; i++) { item_id[i] = 0; }
                    
                    Vector2Int Inventory_position = new Vector2Int { x = 0, y = 0 };
                    Vector2Int Item_size = new Vector2Int { x = 1, y = 1 };
                    if ((__0 == DungeonID.Dungeon1) && (__instance.dun1Key.content == null))
                    {
                        item_id[2] = 4;
                        ItemData item_data = new ItemData
                        {
                            itemType = item_type,
                            subType = 4,
                            id = item_id                            
                        };
                        ItemContainerEntry item_container_entry = new ItemContainerEntry(item_data, Inventory_position, Item_size, 1);
                        __instance.dun1Key.content = item_container_entry;
                    }
                    else if ((__0 == DungeonID.LightlessArbor) && (__instance.lightlessArborDungeonKey.content == null))
                    {
                        item_id[2] = 5;
                        ItemData item_data = new ItemData
                        {
                            itemType = item_type,
                            subType = 5,
                            id = item_id
                        };
                        ItemContainerEntry item_container_entry = new ItemContainerEntry(item_data, Inventory_position, Item_size, 1);
                        __instance.dun1Key.content = item_container_entry;
                    }
                    else if ((__0 == DungeonID.SoulfireBastion) && (__instance.soulfireBastionDungeonKey.content == null))
                    {
                        item_id[2] = 6;
                        ItemData item_data = new ItemData
                        {
                            itemType = item_type,
                            subType = 6,
                            id = item_id
                        };
                        ItemContainerEntry item_container_entry = new ItemContainerEntry(item_data, Inventory_position, Item_size, 1);
                        __instance.dun1Key.content = item_container_entry;
                    }
                }
            }
        }
    }
}

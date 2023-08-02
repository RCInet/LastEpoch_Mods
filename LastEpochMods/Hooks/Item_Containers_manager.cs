using HarmonyLib;
using UniverseLib;

namespace LastEpochMods.Hooks
{
    public class Item_Containers_manager
    {
        [HarmonyPatch(typeof(ItemContainersManager), "getGearCountForSetID")]
        public class getGearCountForSetID
        {
            [HarmonyPostfix]
            static void Postfix(ItemContainersManager __instance, ref int __result, byte __0)
            {
                if (Config.Data.mods_config.items.Remove_set_req) { __result = 10; }
            }
        }

        [HarmonyPatch(typeof(ItemContainersManager), "IsOccupiedWithValidDungeonKey")]
        public class IsOccupiedWithValidDungeonKey
        {
            #region CreateKey
            [HarmonyPrefix]
            static void Prefix(ItemContainersManager __instance, bool __result, DungeonID __0)
            {
                if ((Config.Data.mods_config.scene.Enable_Dungeons_WithoutKey) && (Config.Data.mods_config.scene.CreateKey))
                {
                    if ((__instance != null) && (!__result))
                    {
                        byte item_type = 104;
                        UnhollowerBaseLib.Il2CppStructArray<byte> item_id = new UnhollowerBaseLib.Il2CppStructArray<byte>(22);
                        item_id[0] = 1;
                        item_id[1] = item_type;
                        item_id[2] = 0; //id
                        for (int i = 3; i < item_id.Count; i++) { item_id[i] = 0; }

                        UnityEngine.Vector2Int Inventory_position = new UnityEngine.Vector2Int { x = 0, y = 0 };
                        UnityEngine.Vector2Int Item_size = new UnityEngine.Vector2Int { x = 1, y = 1 };
                        if (__0 == DungeonID.Dungeon1)
                        {
                            item_id[2] = 4;
                            ItemData item_data = new ItemData { itemType = item_type, subType = 4, id = item_id };
                            ItemContainerEntry item_container_entry = new ItemContainerEntry(item_data, Inventory_position, Item_size, 1);
                            __instance.dun1Key.content = item_container_entry;
                        }
                        else if (__0 == DungeonID.LightlessArbor)
                        {
                            item_id[2] = 5;
                            ItemData item_data = new ItemData { itemType = item_type, subType = 5, id = item_id };
                            ItemContainerEntry item_container_entry = new ItemContainerEntry(item_data, Inventory_position, Item_size, 1);
                            __instance.lightlessArborDungeonKey.content = item_container_entry;
                        }
                        else if (__0 == DungeonID.SoulfireBastion)
                        {
                            item_id[2] = 6;
                            ItemData item_data = new ItemData { itemType = item_type, subType = 6, id = item_id };
                            ItemContainerEntry item_container_entry = new ItemContainerEntry(item_data, Inventory_position, Item_size, 1);
                            __instance.soulfireBastionDungeonKey.content = item_container_entry;
                        }
                    }
                }
            }
            #endregion
            #region Bypass result
            [HarmonyPostfix]
            static void Postfix(ref ItemContainersManager __instance, ref bool __result, ref DungeonID __0)
            {
                if ((Config.Data.mods_config.scene.Enable_Dungeons_WithoutKey) && (!Config.Data.mods_config.scene.CreateKey)) { __result = true; }
            }
            #endregion
        }
    }
}

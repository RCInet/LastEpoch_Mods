using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Item_Drop_Bonuses
    {
        [HarmonyPatch(typeof(ItemDropBonuses), "getIncreasedRarityDropRate")]
        public class getIncreasedRarityDropRate
        {
            [HarmonyPrefix]
            static bool Prefix(ref ItemDropBonuses __instance, float __result, float __0)
            {
                if (Config.Data.mods_config.items.Enable_increase_equipment_droprate)
                {
                    for (int i = 0; i < __instance.increasedEquipmentDroprates.Count; i++)
                    {
                        __instance.increasedEquipmentDroprates[i] = Config.Data.mods_config.items.increase_equipment_droprate;
                    }
                }
                if (Config.Data.mods_config.items.Enable_increase_equipmentshards_droprate)
                {
                    for (int z = 0; z < __instance.increasedEquipmentShardDroprates.Count; z++)
                    {
                        __instance.increasedEquipmentShardDroprates[z] = Config.Data.mods_config.items.increase_equipmentshards_droprate;
                    }
                }
                if (Config.Data.mods_config.items.Enable_increase_uniques_droprate)
                {
                    __instance.increasedUniqueDropRate = Config.Data.mods_config.items.increase_uniques_droprate;
                }

                return true;
            }
        }
    }
}

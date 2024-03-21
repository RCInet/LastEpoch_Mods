using HarmonyLib;
using LastEpochMods.Managers;

namespace LastEpochMods.Mods.Items
{
    public class DropRate
    {
        [HarmonyPatch(typeof(ItemDropBonuses), "onStatsUpdate")]
        public class ItemDropBonuses_onStatsUpdate
        {
            [HarmonyPostfix]
            static void Postfix(ref ItemDropBonuses __instance)
            {
                if (Save_Manager.Data.UserData.Items.DropRate.Enable_IncreaseEquipment)
                {
                    for (int i = 0; i < __instance.increasedEquipmentDroprates.Count; i++)
                    {
                        __instance.increasedEquipmentDroprates[i] = Save_Manager.Data.UserData.Items.DropRate.IncreaseEquipment;
                    }
                }
                if (Save_Manager.Data.UserData.Items.DropRate.Enable_IncreaseShards)
                {
                    for (int z = 0; z < __instance.increasedEquipmentShardDroprates.Count; z++)
                    {
                        if (__instance.increasedEquipmentShardDroprates[z] == Save_Manager.Data.UserData.Items.DropRate.IncreaseShards)
                        { break; }
                        __instance.increasedEquipmentShardDroprates[z] = Save_Manager.Data.UserData.Items.DropRate.IncreaseShards;
                    }
                }
                if (Save_Manager.Data.UserData.Items.DropRate.Enable_IncreaseUniques)
                {
                    __instance.increasedUniqueDropRate = Save_Manager.Data.UserData.Items.DropRate.IncreaseUniques;
                }
            }
        }
    }
}

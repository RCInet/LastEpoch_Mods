using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Notifications_
    {
        [HarmonyPatch(typeof(Notifications), "MaterialAdded")]
        public class MaterialAdded
        {
            [HarmonyPrefix]
            static bool Prefix(string __0)
            {
                if (Config.Data.mods_config.auto_loot.Hide_materials_notifications) { return false; }
                else { return true; }
            }
        }        

        [HarmonyPatch(typeof(Notifications), "RuneOrGlyphAddedNotification")]
        public class RuneOrGlyphAddedNotification
        {
            [HarmonyPrefix]
            static bool Prefix(string __0, int __1)
            {
                if (Config.Data.mods_config.auto_loot.Hide_materials_notifications) { return false; }
                else {  return true; }
            }
        }

        [HarmonyPatch(typeof(Notifications), "MultiShardsAdded")]
        public class MultiShardsAdded
        {
            [HarmonyPrefix]
            static bool Prefix(int __0)
            {
                if (Config.Data.mods_config.auto_loot.Hide_materials_notifications) { return false; }
                else { return true; }
            }
        }

        [HarmonyPatch(typeof(Notifications), "ShardAdded")]
        public class ShardAdded
        {
            [HarmonyPrefix]
            static bool Prefix(int __0, int __1)
            {
                if (Config.Data.mods_config.auto_loot.Hide_materials_notifications) { return false; }
                else { return true; }
            }
        }
        
        [HarmonyPatch(typeof(Notifications), "showBufferedCraftingNotifications")]
        public class showBufferedCraftingNotifications
        {
            [HarmonyPrefix]
            static bool Prefix(ref Notifications __instance)
            {
                //__instance.SingleShardNotification = null;                
                //__instance.GenericNotification = null;                

                if (Config.Data.mods_config.auto_loot.Hide_materials_notifications) { return false; }
                else { return true; }
            }
        }
    }
}

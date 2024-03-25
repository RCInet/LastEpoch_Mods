using HarmonyLib;
using LastEpochMods.Managers;

namespace LastEpochMods.Mods.Items
{
    public class DropNotifications
    {
        public static void OnSceneWasLoaded()
        {
            if ((Save_Manager.Data.UserData.Items.DropNotification.Hide_materials_notifications) &&
                (!shard_notification_parent.IsNullOrDestroyed()))
            {
                shard_notification_parent.gameObject.active = false;
            }
            else { shard_notification_parent.gameObject.active = true; }
        }

        private static ShardNotificationParent shard_notification_parent = null;
        [HarmonyPatch(typeof(ShardNotificationParent), "Awake")]
        public class ShardNotificationParent_Awake
        {
            [HarmonyPostfix]
            static void Postfix(ref ShardNotificationParent __instance)
            {
                shard_notification_parent = __instance;
            }
        }
        
        [HarmonyPatch(typeof(Notifications), "MaterialAdded")]
        public class Notifications_MaterialAdded
        {
            [HarmonyPrefix]
            static bool Prefix(string __0)
            {
                if (Save_Manager.Data.UserData.Items.DropNotification.Hide_materials_notifications)
                { return false; }
                else { return true; }
            }
        }

        [HarmonyPatch(typeof(Notifications), "RuneOrGlyphAddedNotification")]
        public class Notifications_RuneOrGlyphAddedNotification
        {
            [HarmonyPrefix]
            static bool Prefix(string __0, int __1)
            {
                if (Save_Manager.Data.UserData.Items.DropNotification.Hide_materials_notifications) { return false; }
                else { return true; }
            }
        }

        [HarmonyPatch(typeof(Notifications), "MultiShardsAdded")]
        public class Notifications_MultiShardsAdded
        {
            [HarmonyPrefix]
            static bool Prefix(int __0)
            {
                if (Save_Manager.Data.UserData.Items.DropNotification.Hide_materials_notifications)
                {
                    return false;
                }
                else { return true; }
            }
        }

        [HarmonyPatch(typeof(Notifications), "ShardAdded")]
        public class Notifications_ShardAdded
        {
            [HarmonyPrefix]
            static bool Prefix(int __0, int __1)
            {
                try
                {
                    if (Save_Manager.Data.UserData.Items.DropNotification.Hide_materials_notifications)
                    {
                        return false;
                    }
                    else { return true; }
                }
                catch { return true; }                
            }
        }

        [HarmonyPatch(typeof(Notifications), "showBufferedCraftingNotifications")]
        public class Notifications_showBufferedCraftingNotifications
        {
            [HarmonyPrefix]
            static bool Prefix(ref Notifications __instance)
            {
                if (Save_Manager.Data.UserData.Items.DropNotification.Hide_materials_notifications)
                {
                    return false;
                }
                else { return true; }
            }
        }
    }
}

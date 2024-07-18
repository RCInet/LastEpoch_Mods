using HarmonyLib;
using LastEpochMods.Managers;

namespace LastEpochMods.Mods.Scenes
{
    public class Options
    {
        //Density && Exp
        [HarmonyPatch(typeof(SpawnerPlacementManager), "Start")]
        public class SpawnerPlacementManager_Start
        {
            [HarmonyPrefix]
            public static void Prefix(ref SpawnerPlacementManager __instance)
            {
                try
                {
                    if (Save_Manager.Data.UserData.Scene.Scene_Options.Enable_Scenes_Density_Multiplier)
                    {
                        __instance.defaultSpawnerDensity = Save_Manager.Data.UserData.Scene.Scene_Options.Scenes_Density_Multiplier;
                        __instance.alwaysRollSpawnerDensity = false;
                    }
                }
                catch { }
            }
        }

        //Mobs Items && Gold (Multiplier, DropChance) && Exp (Multiplier)
        [HarmonyPatch(typeof(DeathItemDrop), "Start")]
        public class DeathItemDrop_Start
        {
            [HarmonyPostfix]
            static void Postfix(ref DeathItemDrop __instance)
            {
                if (Save_Manager.Data.UserData.Scene.Scene_Options.Enable_Mobs_GoldMultiplier)
                {
                    __instance.goldMultiplier = Save_Manager.Data.UserData.Scene.Scene_Options.Mobs_GoldMultiplier;
                }
                if (Save_Manager.Data.UserData.Scene.Scene_Options.Enable_Mobs_GoldDropChance)
                {
                    __instance.overrideBaseGoldDropChance = true;
                    __instance.goldDropChance = Save_Manager.Data.UserData.Scene.Scene_Options.Mobs_GoldDropChance;
                }

                if (Save_Manager.Data.UserData.Scene.Scene_Options.Enable_Mobs_ItemsMultiplier)
                {
                    __instance.itemMultiplier = Save_Manager.Data.UserData.Scene.Scene_Options.Mobs_ItemsMultiplier;
                }
                if (Save_Manager.Data.UserData.Scene.Scene_Options.Enable_Mobs_ItemsDropChance)
                {
                    __instance.overrideBaseItemDropChance = true;
                    __instance.itemDropChance = Save_Manager.Data.UserData.Scene.Scene_Options.Mobs_ItemsDropChance;
                }
            }
        }               

        //Waypoints
        [HarmonyPatch(typeof(UIWaypointStandard), "OnPointerEnter")]
        public class UIWaypointStandard_OnPointerEnter
        {
            [HarmonyPrefix]
            static void Prefix(UIWaypointStandard __instance, UnityEngine.EventSystems.PointerEventData __0)
            {
                if (Save_Manager.Data.UserData.Scene.Scene_Options.Enable_Waypoints_Unlock)
                {
                    __instance.isActive = true;
                }
            }
        }
    }
}

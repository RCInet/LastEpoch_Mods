using HarmonyLib;
using LastEpochMods.Managers;

namespace LastEpochMods.Mods.Scenes
{
    public class Dungeons
    {
        [HarmonyPatch(typeof(ItemContainersManager), "IsOccupiedWithValidDungeonKey")]
        public class ItemContainersManager_IsOccupiedWithValidDungeonKey
        {
            [HarmonyPostfix]
            static void Postfix(ref ItemContainersManager __instance, ref bool __result, ref DungeonID __0)
            {
                if (Save_Manager.Data.UserData.Scene.Dungeons.EnteringWithoutKey) { __result = true; }
            }
        }
    }
}

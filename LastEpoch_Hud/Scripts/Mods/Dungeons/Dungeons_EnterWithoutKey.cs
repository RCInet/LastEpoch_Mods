using HarmonyLib;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Dungeons
{
    public class Dungeons_EnterWithoutKey
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Scenes.Dungeons.Enable_EnterWithoutKey;
                }
                else { return false; }
            }
            else { return false; }
        }
        
        [HarmonyPatch(typeof(ItemContainersManager), "IsOccupiedWithValidDungeonKey")]
        public class ItemContainersManager_IsOccupiedWithValidDungeonKey
        {
            [HarmonyPostfix]
            static void Postfix(ref bool __result)
            {
                if (CanRun()) { __result = true; }
            }
        }
    }
}

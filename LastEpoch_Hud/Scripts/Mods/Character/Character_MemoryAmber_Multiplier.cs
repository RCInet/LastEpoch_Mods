using HarmonyLib;
using Il2CppLE.Factions;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Character
{
    public class Character_MemoryAmber_Multiplier
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()) && (!Refs_Manager.player_actor.IsNullOrDestroyed()))
            {
                return Save_Manager.instance.data.Character.Cheats.Enable_MemoryAmberMultiplier;
            }
            else { return false; }
        }

        [HarmonyPatch(typeof(Il2CppLE.Factions.PickupableObjectsManager), "CreatePickupableObjectForPlayer", new System.Type[] { typeof(PickupableObjectType), typeof(Il2CppLE.Factions.PickupableObjectSet), typeof(Vector3), typeof(uint) })]
        //[HarmonyPatch(typeof(Il2CppLE.Factions.PickupableObjectsManager), "CreatePickupableObjectForPlayer", new System.Type[] { typeof(Il2CppLE.Factions.PickupableObjectSet), typeof(Vector3), typeof(uint) })]
        public class PickupableObjectsManager_CreatePickupableObjectForPlayer2
        {
            [HarmonyPrefix]
            static void Prefix(PickupableObjectType __0, ref uint __3)
            {
                if (CanRun() && __0 == PickupableObjectType.MemoryAmber)
                {
                    __3 = Save_Manager.instance.data.Character.Cheats.MemoryAmberMultiplier * __3;
                }
            }
        }
    }
}

using HarmonyLib;
using Il2CppLE.Factions;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_AutoPickup_MemoryAmber
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()) && (!Refs_Manager.player_actor.IsNullOrDestroyed()))
            {
                return Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_MemoryAmber;
            }
            else { return false; }
        }

        [HarmonyPatch(typeof(Il2CppLE.Factions.PickupableObjectsManager), "CreatePickupableObjectForPlayer", new System.Type[] { typeof(PickupableObjectType), typeof(Il2CppLE.Factions.PickupableObjectSet), typeof(Vector3), typeof(uint) })]
        //[HarmonyPatch(typeof(Il2CppLE.Factions.PickupableObjectsManager), "CreatePickupableObjectForPlayer", new System.Type[] { typeof(Il2CppLE.Factions.PickupableObjectSet), typeof(Vector3), typeof(uint) })]
        public class PickupableObjectsManager_CreatePickupableObjectForPlayer2
        {
            [HarmonyPostfix]
            static void Postfix(ref Il2CppLE.Factions.PickupableObjectsManager __instance, Il2CppLE.Factions.PickupableObjectSet __1, ref Vector3 __2, uint __3)
            {
                if (CanRun())
                {
                    bool pick = false;
                    System.UInt32 id = __1.NextID - 1;
                    foreach (Il2CppSystem.Collections.Generic.KeyValuePair<Il2CppLE.Factions.PickupableObjectType, Il2CppSystem.Collections.Generic.List<Il2CppLE.Factions.PickupableObject>> obj in __1.pickupables)
                    {
                        int index = 0;
                        foreach (Il2CppLE.Factions.PickupableObject pickupable_obj in obj.Value)
                        {
                            if (pickupable_obj.Id == id)
                            {
                                __2 = Refs_Manager.player_actor.position();
                                __instance.PickupObject(__1, pickupable_obj, index);
                                pick = true;
                                break;
                            }
                            index++;
                        }
                        if (pick) { break; }
                    }
                }
            }
        }
    }
}

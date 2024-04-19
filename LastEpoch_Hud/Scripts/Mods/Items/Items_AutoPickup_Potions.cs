using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_AutoPickup_Potions
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()) &&
                (!Refs_Manager.player_actor.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_Potions;
                }
                else { return false; }
            }
            else { return false; }
        }
        [HarmonyPatch(typeof(GroundItemManager), "dropPotionForPlayer")]
        public class dropPotionForPlayer
        {
            [HarmonyPrefix]
            static void Prefix(GroundItemManager __instance, Actor __0, ref UnityEngine.Vector3 __1, bool __2)
            {
                if (CanRun())
                {
                    __1 = Refs_Manager.player_actor.position(); //Move to Player before drop
                    //Find a way to pickup here and don't drop
                }
            }
            [HarmonyPostfix]
            static void Postfix(GroundItemManager __instance, Actor __0, ref UnityEngine.Vector3 __1, bool __2)
            {
                if (CanRun())
                {
                    System.UInt32 pot_id = __instance.nextPotionId - 1;
                    foreach (PotionPickupInteraction pick_pot_interaction in __instance.activePotions)
                    {
                        if (pick_pot_interaction.id == pot_id)
                        {
                            __instance.pickupPotion(__0, pot_id, pick_pot_interaction);
                            break;
                        }
                    }
                }
            }
        }
    }
}

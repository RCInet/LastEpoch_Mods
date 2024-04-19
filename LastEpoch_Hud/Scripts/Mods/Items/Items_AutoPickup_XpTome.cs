using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_AutoPickup_XpTome
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()) &&
                (!Refs_Manager.player_actor.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_XpTome;
                }
                else { return false; }
            }
            else { return false; }
        }
        [HarmonyPatch(typeof(GroundItemManager), "dropXPTomeForPlayer")]
        public class dropXPTomeForPlayer
        {
            [HarmonyPostfix]
            static void Postfix(ref GroundItemManager __instance, ref Actor __0, ref int __1, ref UnityEngine.Vector3 __2, ref bool __3)
            {
                if (CanRun())
                {
                    System.UInt32 tome_id = __instance.nextXpTomeId - 1;
                    foreach (PickupExperiencePotionInteraction pick_exp_pot_interaction in __instance.activeXPTomes)
                    {
                        if (pick_exp_pot_interaction.id == tome_id)
                        {
                            __2 = Refs_Manager.player_actor.position();
                            __instance.pickupXPTome(__0, tome_id, pick_exp_pot_interaction);
                            break;
                        }
                    }
                }
            }
        }
    }
}

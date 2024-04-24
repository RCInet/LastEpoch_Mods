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
            static bool Prefix()
            {
                bool result = true;
                if (CanRun())
                {
                    if (!Refs_Manager.health_potion.IsNullOrDestroyed())
                    {
                        if (Refs_Manager.health_potion.maxCharges > Refs_Manager.health_potion.currentCharges)
                        {
                            Refs_Manager.health_potion.currentCharges++;
                            result = false;
                        }
                    }
                }

                return result;
            }            
        }
    }
}

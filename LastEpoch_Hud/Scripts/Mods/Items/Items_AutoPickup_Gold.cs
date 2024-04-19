using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_AutoPickup_Gold
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()) &&
                (!Refs_Manager.player_gold_tracker.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed()) { return Save_Manager.instance.data.Items.Pickup.Enable_AutoPickup_Gold; }
                else { return false; }
            }
            else { return false; }
        }
        
        [HarmonyPatch(typeof(GroundItemManager), "dropGoldForPlayer")]
        public class dropGoldForPlayer
        {
            [HarmonyPrefix]
            static bool Prefix(ref GroundItemManager __instance, ref Actor __0, ref int __1, ref UnityEngine.Vector3 __2, ref bool __3)
            {
                if (CanRun())
                {
                    Refs_Manager.player_gold_tracker.modifyGold(__1);
                    return false;
                }
                else { return true; }
            }
        }
    }
}

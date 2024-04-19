using HarmonyLib;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_AutoStore_OnInventoryOpen
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Items.Pickup.Enable_AutoStore_OnInventoryOpen;
                }
                else { return false; }
            }
            else { return false; }
        }

        [HarmonyPatch(typeof(InventoryPanelUI), "OnEnable")]
        public class InventoryPanelUI_OnEnable
        {
            [HarmonyPostfix]
            static void Postfix(ref InventoryPanelUI __instance)
            {
                if (CanRun()) { __instance.StoreMaterialsButtonPress(); }
            }
        }
    }
}

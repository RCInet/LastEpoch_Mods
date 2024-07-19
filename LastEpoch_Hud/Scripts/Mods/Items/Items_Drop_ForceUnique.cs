using HarmonyLib;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_Drop_ForceUnique
    {
        /*public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if ((!Save_Manager.instance.data.IsNullOrDestroyed()) &&
                    (!Save_Manager.instance.data.Items.Drop.Enable_ForceSet) &&
                    (!Save_Manager.instance.data.Items.Drop.Enable_ForceLegendary))
                {
                    return Save_Manager.instance.data.Items.Drop.Enable_ForceUnique;
                }
                else { return false; }
            }
            else { return false; }
        }
        [HarmonyPatch(typeof(GenerateItems), "RollRarity")]
        public class GenerateItems_RollRarity
        {
            [HarmonyPrefix]
            static bool Prefix(ref byte __result, int __0)
            {
                if (CanRun()) { __result = 7; return false; }
                else { return true; }
            }
        }*/
    }
}

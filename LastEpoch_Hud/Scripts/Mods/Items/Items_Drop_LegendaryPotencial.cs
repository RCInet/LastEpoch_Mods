using HarmonyLib;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_Drop_LegendaryPotencial
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Items.Drop.Enable_LegendaryPotencial;
                }
                else { return false; }
            }
            else { return false; }
        }

        [HarmonyPatch(typeof(ItemData), "rollLegendaryPotential")]
        public class rollLegendaryPotential
        {
            [HarmonyPrefix]
            static bool Prefix(ref ItemData __instance, ref int __result, UniqueList.Entry __0, int __1, int __2)
            {
                if (CanRun())
                {
                    int roll = 0;
                    if (Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Min == Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Max) { roll = (int)Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Max; }
                    else { roll = (int)Random.RandomRange(Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Min, Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Max); }
                    __result = roll;
                    return false;
                }
                else { return true; };
            }
        }
    }
}

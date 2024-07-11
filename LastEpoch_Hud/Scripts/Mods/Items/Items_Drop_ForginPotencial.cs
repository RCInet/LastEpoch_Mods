using HarmonyLib;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_Drop_ForginPotencial
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Items.Drop.Enable_ForginPotencial;
                }
                else { return false; }
            }
            else { return false; }
        }              

        [HarmonyPatch(typeof(GenerateItems), "rollForgingPotential")]
        public class GenerateItems_rollForgingPotential
        {
            [HarmonyPrefix]
            static bool Prefix(ref int __result, ref ItemDataUnpacked __0, int __1, bool __2, GenerateItems.GenerationContext __3)
            {
                if (CanRun())
                {
                    int roll = 0;
                    if (Save_Manager.instance.data.Items.Drop.ForginPotencial_Min == Save_Manager.instance.data.Items.Drop.ForginPotencial_Max) { roll = (int)Save_Manager.instance.data.Items.Drop.ForginPotencial_Max; }
                    else { roll = (int)Random.RandomRange(Save_Manager.instance.data.Items.Drop.ForginPotencial_Min, Save_Manager.instance.data.Items.Drop.ForginPotencial_Max); }
                    __result = roll;
                    return false;
                }
                else { return true; }
            }
        }
    }
}

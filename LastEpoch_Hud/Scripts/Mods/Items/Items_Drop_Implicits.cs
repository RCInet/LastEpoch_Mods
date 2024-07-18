using HarmonyLib;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_Drop_Implicits
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Items.Drop.Enable_Implicits;
                }
                else { return false; }
            }
            else { return false; }
        }

        [HarmonyPatch(typeof(ItemData), "randomiseImplicitRolls")]
        public class ItemData_randomiseImplicitRolls
        {
            [HarmonyPrefix]
            static bool Prefix(ref ItemData __instance)
            {
                if (CanRun())
                {
                    for (int z = 0; z < __instance.implicitRolls.Count; z++)
                    {
                        byte roll = 0;
                        if (Save_Manager.instance.data.Items.Drop.Implicits_Min == Save_Manager.instance.data.Items.Drop.Implicits_Max) { roll = (byte)Save_Manager.instance.data.Items.Drop.Implicits_Max; }
                        else { roll = (byte)Random.RandomRange(Save_Manager.instance.data.Items.Drop.Implicits_Min, Save_Manager.instance.data.Items.Drop.Implicits_Max); }
                        __instance.implicitRolls[z] = roll;
                    }
                    __instance.RefreshIDAndValues();
                    
                    return false;
                }
                else { return true; }
            }
        }
    }
}

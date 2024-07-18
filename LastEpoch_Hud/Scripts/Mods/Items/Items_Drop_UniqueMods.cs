using HarmonyLib;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_Drop_UniqueMods
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Items.Drop.Enable_UniqueMods;
                }
                else { return false; }
            }
            else { return false; }
        }

        [HarmonyPatch(typeof(ItemData), "randomiseUniqueRolls")]
        public class randomiseUniqueRolls
        {
            [HarmonyPrefix]
            static bool Prefix(ref ItemData __instance)
            {
                if (CanRun())
                {
                    for (int k = 0; k < __instance.uniqueRolls.Count; k++)
                    {
                        byte roll = 0;
                        if (Save_Manager.instance.data.Items.Drop.UniqueMods_Min == Save_Manager.instance.data.Items.Drop.UniqueMods_Max) { roll = (byte)Save_Manager.instance.data.Items.Drop.UniqueMods_Max; }
                        else { roll = (byte)Random.RandomRange(Save_Manager.instance.data.Items.Drop.UniqueMods_Min, Save_Manager.instance.data.Items.Drop.UniqueMods_Max); }
                        __instance.uniqueRolls[k] = roll;
                    }
                    __instance.RefreshIDAndValues();
                    
                    return false;
                }
                else { return true; };
            }
        }
    }
}

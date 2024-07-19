using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_Drop_Rarity
    {
        public static bool CanRun()
        {
            bool res = false;
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed()) { res = true; }
            }

            return res;
        }
        [HarmonyPatch(typeof(GenerateItems), "RollRarity")]
        public class GenerateItems_RollRarity
        {
            [HarmonyPrefix]
            static bool Prefix(ref byte __result, int __0)
            {
                if (CanRun())
                {
                    if (Save_Manager.instance.data.Items.Drop.Enable_ForceUnique) { __result = 7; }
                    else if (Save_Manager.instance.data.Items.Drop.Enable_ForceSet) { __result = 8; }
                    else if (Save_Manager.instance.data.Items.Drop.Enable_ForceLegendary) { __result = 9; }
                    else
                    {
                        if (Save_Manager.instance.data.modsNotInHud.Items_MaxSockets < 0) { Save_Manager.instance.data.modsNotInHud.Items_MaxSockets = 0; }
                        if (Save_Manager.instance.data.modsNotInHud.Items_MaxSockets > 6) { Save_Manager.instance.data.modsNotInHud.Items_MaxSockets = 6; }
                        __result = (byte)UnityEngine.Random.RandomRangeInt(0, (Save_Manager.instance.data.modsNotInHud.Items_MaxSockets + 1));
                    }
                    return false;
                }
                else { return true; }
            }
        }
    }
}

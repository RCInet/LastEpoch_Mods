using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_Req_Level
    {
        public static bool need_update = false;
        public static void Enable()
        {
            if (CanRun()) { need_update = true; }
        }
        private static bool CanRun()
        {
            if ((!Save_Manager.instance.IsNullOrDestroyed()) && (!Refs_Manager.item_list.IsNullOrDestroyed()))
            {
                if (Save_Manager.instance.initialized)
                {
                    return Save_Manager.instance.data.Items.Req.level;
                }
                else { return false; }
            }
            else { return false; }
        }
        
        [HarmonyPatch(typeof(ItemData), "CalculateLevelAndClassRequirement")]
        public class CalculateLevelAndClassRequirement
        {
            [HarmonyPostfix]
            static void Postfix(ref int __result)
            {
                if (CanRun()) { __result = 0; }
            }
        }
    }
}

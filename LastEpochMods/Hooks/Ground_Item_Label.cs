using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Ground_Item_Label
    {
        [HarmonyPatch(typeof(GroundItemLabel), "ClickedItem")]
        public class ClickedItem
        {
            [HarmonyPrefix]
            static bool Prefix(GroundItemLabel __instance)
            {
                if (Config.Data.mods_config.items.Enable_pickup_range)
                {
                    __instance.requestPickup();
                    return false;
                }
                else { return true; }
            }
        }
    }
}

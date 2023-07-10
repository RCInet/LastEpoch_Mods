using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class UI_Waypoint_Standard
    {
        [HarmonyPatch(typeof(UIWaypointStandard), "OnPointerEnter")]
        public class OnPointerEnter
        {
            [HarmonyPrefix]
            static void Prefix(UIWaypointStandard __instance, UnityEngine.EventSystems.PointerEventData __0)
            {
                if (Config.Data.mods_config.scene.Enable_Waypoint_Unlock)
                {
                    __instance.isActive = true;
                }
            }
        }
    }
}

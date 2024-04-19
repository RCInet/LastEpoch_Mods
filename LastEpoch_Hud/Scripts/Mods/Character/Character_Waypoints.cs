using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Character
{
    public class Character_Waypoints
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Character.Cheats.Enable_WaypointsUnlock;
                }
                else { return false; }
            }
            else { return false; }
        }
        [HarmonyPatch(typeof(UIWaypointStandard), "OnPointerEnter")]
        public class UIWaypointStandard_OnPointerEnter
        {
            [HarmonyPrefix]
            static void Prefix(UIWaypointStandard __instance, UnityEngine.EventSystems.PointerEventData __0)
            {
                if (CanRun()) { __instance.isActive = true; }
            }
        }
    }
}

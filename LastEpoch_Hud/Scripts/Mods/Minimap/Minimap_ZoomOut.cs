using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Minimap
{
    internal class Minimap_ZoomOut
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Scenes.Minimap.Enable_MaxZoomOut;
                }
                else { return false; }
            }
            else { return false; }
        }
        [HarmonyPatch(typeof(DMMapZoom), "ZoomOutMinimap")]
        public class DMMapZoom_ZoomOutMinimap
        {
            public static void Prefix(ref DMMapZoom __instance)
            {
                if (CanRun()) { __instance.maxMinimapZoom = float.MaxValue; }
                else { __instance.maxMinimapZoom = 37.5f; } //Default
            }
        }
    }
}

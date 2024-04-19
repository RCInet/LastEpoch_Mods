using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Minimap
{
    internal class Minimap_FogOfWar
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Scenes.Minimap.Enable_RemoveFogOfWar;
                }
                else { return false; }
            }
            else { return false; }
        }
        [HarmonyPatch(typeof(MinimapFogOfWar), "Initialize")]
        public class MinimapFogOfWar_Initialize
        {
            [HarmonyPrefix]
            static bool Prefix(MinimapFogOfWar __instance, MinimapFogOfWar.QuadScale __0, UnityEngine.Vector3 __1)
            {
                if (CanRun()) { __instance.discoveryDistance = float.MaxValue; }
                else { __instance.discoveryDistance = 20f; } //Default Value
                return true;
            }
        }
    }
}

using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Minimap_Fog_Of_War
    {        
        [HarmonyPatch(typeof(MinimapFogOfWar), "Initialize")]
        public class Initialize
        {
            [HarmonyPrefix]
            static bool Prefix(MinimapFogOfWar __instance, MinimapFogOfWar.QuadScale __0, UnityEngine.Vector3 __1)
            {
                if (Config.Data.mods_config.scene.Remove_Fog_Of_War) { __instance.discoveryDistance = float.MaxValue; }
                else { __instance.discoveryDistance = 20f; }
                return true;
            }
        }
    }
}

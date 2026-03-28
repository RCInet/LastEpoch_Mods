using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Minimap
{
    public class Minimap_FogOfWar
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

        [HarmonyPatch(typeof(Il2CppLE.UI.Minimap.Minimap), "OnInitializeFoW")]
        public class Minimap_OnInitializeFoW
        {
            [HarmonyPostfix]
            static void Postfix(Il2CppLE.UI.Minimap.Minimap __instance)
            {
                if (CanRun()) { __instance.RevealRadius = 255f; }
                else if (__instance.RevealRadius == 255f) { __instance.RevealRadius = 40f; } //Default value
            }
        }
    }
}

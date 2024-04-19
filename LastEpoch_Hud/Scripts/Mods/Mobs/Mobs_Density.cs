using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Mobs
{
    public class Mobs_Density
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Character.Cheats.Enable_DensityMultiplier;
                }
                else { return false; }
            }
            else { return false; }
        }

        [HarmonyPatch(typeof(SpawnerPlacementManager), "Start")]
        public class SpawnerPlacementManager_Start
        {
            [HarmonyPrefix]
            public static void Prefix(ref SpawnerPlacementManager __instance)
            {
                if (CanRun())
                {
                    __instance.defaultSpawnerDensity = Save_Manager.instance.data.Character.Cheats.DensityMultiplier;
                    __instance.alwaysRollSpawnerDensity = false;
                }
            }
        }
    }
}

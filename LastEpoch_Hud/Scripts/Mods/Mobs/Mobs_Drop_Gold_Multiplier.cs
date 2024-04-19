using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Mobs
{
    public class Mobs_Drop_Gold_Multiplier
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Character.Cheats.Enable_GoldDropMultiplier;
                }
                else { return false; }
            }
            else { return false; }
        }

        [HarmonyPatch(typeof(DeathItemDrop), "Start")]
        public class DeathItemDrop_Start
        {
            [HarmonyPrefix]
            static void Prefix(ref DeathItemDrop __instance)
            {
                if (CanRun())
                {
                    __instance.goldMultiplier = Save_Manager.instance.data.Character.Cheats.GoldDropMultiplier;
                }
            }
        }
    }
}

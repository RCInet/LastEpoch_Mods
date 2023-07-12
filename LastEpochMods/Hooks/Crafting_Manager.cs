using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Crafting_Manager
    {
        public static bool NoCost = true;

        [HarmonyPatch(typeof(CraftingManager), "Forge")]
        public class Forge
        {
            [HarmonyPrefix]
            static bool Prefix(ref CraftingManager __instance)
            {
                if (Config.Data.mods_config.craft.no_cost) { __instance.debugNoForgingPotentialCost = true; }

                return true;
            }
        }
    }
}

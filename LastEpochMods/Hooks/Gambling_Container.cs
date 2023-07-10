using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Gambling_Container
    {
        [HarmonyPatch(typeof(GamblingContainer), "getItemLevelForPopulating")]
        public class getItemLevelForPopulating
        {
            [HarmonyPostfix]
            static void Postfix(GamblingContainer __instance, ref int __result)
            {
                __result = 100;
            }
        }
    }
}

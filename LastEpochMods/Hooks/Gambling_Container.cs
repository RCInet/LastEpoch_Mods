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

        /*[HarmonyPatch(typeof(GamblingContainer), "MoveItemTo")]
        public class MoveItemTo
        {
            [HarmonyPostfix]
            static void Postfix(GamblingContainer __instance, bool __result, ref ItemContainerEntry __0, int __1, IItemContainer __2, Il2CppSystem.Nullable<UnityEngine.Vector2Int> __3, Context __4)
            {
                
            }
        }*/
    }
}

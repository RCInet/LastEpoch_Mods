using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Moving_Player
    {
        [HarmonyPatch(typeof(MovingPlayer), "MouseClickMoveCommand")]
        public class ShardAdded
        {
            [HarmonyPrefix]
            static bool Prefix(MovingPlayer __instance, UnityEngine.Ray __0, bool __1, float __2, bool __3, UnityEngine.Vector3 __4, bool __5)
            {
                if (Ui.Menu.lock_movement) { return false; }
                else { return true; }
            }
        }
    }
}

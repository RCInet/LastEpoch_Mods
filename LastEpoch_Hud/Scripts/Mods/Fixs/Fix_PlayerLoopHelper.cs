using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Fixs
{
    public class Fix_PlayerLoopHelper
    {
        //Fix exception when player isn't set
        [HarmonyPatch(typeof(Il2CppCysharp.Threading.Tasks.PlayerLoopHelper), "AddAction")]
        public class Il2CppCysharp_Threading_Tasks_PlayerLoopHelper_AddAction
        {
            [HarmonyPrefix]
            static bool Prefix(
                Il2CppCysharp.Threading.Tasks.PlayerLoopTiming __0,
                Il2CppCysharp.Threading.Tasks.IPlayerLoopItem __1
            )
            {
                if (!Hud_Manager.instance.IsNullOrDestroyed() && Hud_Manager.instance.enabled)
                {
                    return true;
                }
                else
                {
                    Main.logger_instance?.Warning(
                        "Fix : PlayerLoopHelper.AddAction(); Wait all Initialize"
                    );
                    return false;
                }
            }
        }
    }
}

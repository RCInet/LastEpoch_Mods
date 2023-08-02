using HarmonyLib;
using LastEpochMods.Config;

namespace LastEpochMods.Hooks
{
    public class Ability_Panel_Icon
    {
        [HarmonyPatch(typeof(AbilityPanelIcon), "Init")]
        public class Init
        {
            [HarmonyPostfix]
            static void Postfix(ref AbilityPanelIcon __instance, Ability __0, AbilityPanelIcon.AbilityUnlockType __1, bool __2, string __3)
            {
                if (Config.Data.mods_config_duplicate.character.skills.Enable_All)
                {
                    __instance.locked = false;
                    __instance.unlockReqHolder.gameObject.active = false;
                    __instance.dragHandler.disabled = false;
                    __instance.lockedIconFrame.active = false;
                }
            }
        }
    }
}

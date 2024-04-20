using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Character
{
    public class Character_UnlockAllIdols
    {
        public static bool CanRun()
        {
            if ((Scenes.IsGameScene()) && (!Save_Manager.instance.IsNullOrDestroyed()))
            {
                if (!Save_Manager.instance.data.IsNullOrDestroyed())
                {
                    return Save_Manager.instance.data.Character.Cheats.Enable_UnlockAllIdolsSlots;
                }
                else { return false; }
            }
            else { return false; }
        }

        [HarmonyPatch(typeof(IdolsItemContainer), "CheckSlotUnlocked")]
        public class IdolsItemContainer_CheckSlotUnlocked
        {
            [HarmonyPostfix]
            static void Postfix(ref bool __result)
            {
                if (CanRun()) { __result = true; }
            }
        }
    }
}

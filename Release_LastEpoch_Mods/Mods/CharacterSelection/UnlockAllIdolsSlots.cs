using HarmonyLib;

namespace LastEpochMods.Mods.CharacterSelection
{
    public class UnlockAllIdolsSlots
    {
        [HarmonyPatch(typeof(IdolsItemContainer), "CheckSlotUnlocked")]
        public class IdolsItemContainer_CheckSlotUnlocked
        {
            [HarmonyPostfix]
            static void Postfix(IdolsItemContainer __instance, ref bool __result, int __0, int __1)
            {
                if (Managers.Save_Manager.Data.UserData.CharacterSelectectionMenu.Enable_UnlockAllIdols)
                {
                    __result = true;
                }
            }
        }
    }
}

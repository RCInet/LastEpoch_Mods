using HarmonyLib;
using LastEpochMods.Managers;

namespace LastEpochMods.Mods.Items
{
    public class AffixsInAllSlots
    {
        [HarmonyPatch(typeof(AffixList.Affix), "CanRollOnItemType")]
        public class AffixList_Affix_CanRollOnItemType
        {
            [HarmonyPrefix]
            static bool Prefix(AffixList.Affix __instance, ref bool __result, int __0, ItemList.ClassRequirement __1)
            {
                if (Save_Manager.Data.UserData.Items.RemoveReq.Enable_AllAffixsInAllSlots)
                {
                    __result = true;
                    return false;
                }
                else { return true; }
            }
        }
    }
}

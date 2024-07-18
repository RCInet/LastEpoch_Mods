using HarmonyLib;
using LastEpochMods.Managers;

namespace LastEpochMods.Mods.Items
{
    public class RemoveReq
    {
        public static void Update()
        {
            if (Save_Manager.Data.UserData.Items.RemoveReq.Remove_set_req) { RemoveSetReq(); }
            else { RestoreToDefault(); }
        }

        private static Il2CppSystem.Collections.Generic.List<SetBonusesList.Entry> backup = null;
        private static void RestoreToDefault()
        {
            if (backup != null) { SetBonusesList.get().entries = backup; }
        }
        private static void RemoveSetReq()
        {
            if (backup == null) { backup = SetBonusesList.get().entries; }
            if ((backup != null) && (Save_Manager.Data.UserData.Items.RemoveReq.Remove_set_req))
            {
                foreach (SetBonusesList.Entry set_bonuses in SetBonusesList.get().entries)
                {
                    foreach (SetBonus set_bonus in set_bonuses.mods) { set_bonus.setRequirement = 0; }
                }
            }
            else if (backup == null) { Main.logger_instance.Error("SetBonusesList:Set -> Error Backup = null"); }
        }

        /*        
        static bool UniqueList::entryFitsSetRequirement(UniqueList+Entry entry, UniqueList+SetRequirement setRequirement)");
        static void Postfix(bool __result, UniqueList.Entry __0, UniqueList.SetRequirement __1)
        __0 'entry'
        __1 'setRequirement':
        */

        [HarmonyPatch(typeof(ItemData), "CalculateLevelAndClassRequirement")]
        public class CalculateLevelAndClassRequirement
        {
            [HarmonyPostfix]
            static void Postfix(ItemData __instance, ref int __result, ref ItemList.ClassRequirement __0, ref ItemList.SubClassRequirement __1)
            {
                if (Save_Manager.Data.UserData.Items.RemoveReq.Remove_LevelReq) { __result = 0; }
                if (Save_Manager.Data.UserData.Items.RemoveReq.Remove_ClassReq) { __0 = ItemList.ClassRequirement.None; }
                if (Save_Manager.Data.UserData.Items.RemoveReq.Remove_SubClassReq) { __1 = ItemList.SubClassRequirement.None; }
            }
        }
        
        [HarmonyPatch(typeof(ItemContainersManager), "getGearCountForSetID")]
        public class getGearCountForSetID
        {
            [HarmonyPostfix]
            static void Postfix(ItemContainersManager __instance, ref int __result, byte __0)
            {
                if (Save_Manager.Data.UserData.Items.RemoveReq.Remove_set_req) { __result = 10; }
            }
        }
    }
}

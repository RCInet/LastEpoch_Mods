using HarmonyLib;
using LastEpochMods.Managers;

namespace LastEpochMods.Mods.CharacterSelection
{
    public class UniqueSubType
    {
        //SubType
        [HarmonyPatch(typeof(UniqueList.Entry), "GetSubType")]
        public class UniqueList_Entry_GetSubType
        {
            [HarmonyPostfix]
            static void Postfix(UniqueList.Entry __instance, ref ushort __result, UniqueList.SubTypePreference __0, byte __1)
            {
                try
                {
                    if (Save_Manager.Data.UserData.CharacterSelectectionMenu.UniqueSubTypeFromSave)
                    {
                        ushort subTypeFromFile_ushort = (ushort)__1;
                        __result = (ushort)__1;
                    }
                }
                catch { Main.logger_instance.Error("Error trying to use subType from file"); }
            }
        }
    }
}

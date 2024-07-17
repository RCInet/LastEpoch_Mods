using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_UniqueList_SubType
    {
        [HarmonyPatch(typeof(UniqueList.Entry), "GetSubType")]
        public class UniqueList_Entry_GetSubType
        {
            [HarmonyPrefix]
            static bool Prefix(UniqueList.Entry __instance, ref ushort __result/*, UniqueList.SubTypePreference __0, byte __1*/)
            {
                bool ret = true;
                //if (Scenes.IsGameScene())     //Uncomment this part if you don't want items to be repair when loading your character
                //{
                    //int random = UnityEngine.Random.RandomRangeInt(1, (__instance.subTypes.Count + 1));   //random in case there are more than one subtype for this unique
                    //byte unique_subtype = __instance.subTypes[random];                                    //Random subtype from unique
                    byte unique_subtype = __instance.subTypes[0]; //Subtype from unique

                    if (__result != unique_subtype)
                    {
                        Main.logger_instance.Msg("UniqueList.Entry.GetSubType(), result = " + __result + " set to " + unique_subtype + " (Unique.Subtype from list)"); //Comment if you don't want to see debug log
                        __result = unique_subtype;
                        ret = false;            //Don't load UniqueList.Entry.GetSubType(), we already have the result we need
                    }
                //}
                return ret;
            }
        }
    }
}

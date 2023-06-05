using LastEpochMods.Db.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class UniquesDrop
    {
        public static void UnlockForAllUniques(Main main)
        {
            UnityEngine.Object obj2 = Functions.GetObject("UniqueList");
            System.Type type2 = obj2.GetActualType();
            if (type2 == typeof(UniqueList))
            {
                UniqueList unique_list = obj2.TryCast<UniqueList>();
                Il2CppSystem.Collections.Generic.List<UniqueList.Entry> Uniques_List_Entry = unique_list.uniques;
                foreach (UniqueList.Entry ul_entry in Uniques_List_Entry)
                {
                    if (!ul_entry.canDropRandomly)
                    {
                        main.LoggerInstance.Msg("Item : " + ul_entry.name + " : Can Drop Now");
                        ul_entry.canDropRandomly = true;
                    }
                }
            }
        }
    }
}

using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Stat_Buff
    {
        [HarmonyPatch(typeof(StatBuffs), "Update")]
        public class Update
        {
            [HarmonyPostfix]
            static void Postfix(ref StatBuffs __instance)
            {
                if (__instance.ToString().Contains("MainPlayer(Clone)"))
                {
                    Il2CppSystem.Collections.Generic.List<int> Remove_buff_list = new Il2CppSystem.Collections.Generic.List<int>();
                    int remove_count = 0;
                    if (__instance.buffs.Count > 0)
                    {
                        Il2CppSystem.Collections.Generic.List<Buff> buff_list = new Il2CppSystem.Collections.Generic.List<Buff>();
                        for (int i = 0; i < __instance.buffs.Count; i++)
                        {
                            if (__instance.buffs[i].remainingDuration > 0.05f) { buff_list.Add(__instance.buffs[i]); }
                            else //if (__instance.buffs[i].remainingDuration < 0.05f)
                            {
                                remove_count++;
                                Remove_buff_list.Add(i);
                            }
                        }
                        if (remove_count > 0)
                        {
                            Main.logger_instance.Msg("Remove " + remove_count + " Buff(s)");
                            __instance.buffs = buff_list;
                        }
                    }
                    if (remove_count > 0)
                    {
                        Il2CppSystem.Collections.Generic.Dictionary<string, Buff> buff_pairs = new Il2CppSystem.Collections.Generic.Dictionary<string, Buff>();
                        int j = 0;
                        bool values_changed = false;
                        int nb_remove = 0;
                        foreach (Il2CppSystem.Collections.Generic.KeyValuePair<string, Buff> value_pair in __instance.activeBuffNames)
                        {
                            if (!Remove_buff_list.Contains(j))
                            {
                                buff_pairs.Add(value_pair.key, value_pair.value);                                
                            }
                            else
                            {
                                values_changed = true;
                                nb_remove++;
                            }
                            j++;
                        }
                        if (values_changed)
                        {
                            Main.logger_instance.Msg("Remove " + nb_remove + " / " + Remove_buff_list.Count + " Buff Name(s)");
                            __instance.activeBuffNames = buff_pairs;
                        }
                    }
                }
            }
        }
    }
}

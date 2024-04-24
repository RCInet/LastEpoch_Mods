using HarmonyLib;

namespace LastEpoch_Hud.Scripts.Mods.Items
{
    public class Items_Req_Set
    {
        /*public struct req_structure
        {
            public byte set_id;
            public System.Collections.Generic.List<int> set_bonuses;
            public System.Collections.Generic.List<byte> set_description;
        }
        public static bool need_update = false;
        public static void Enable()
        {
            if (((CanRun()) && (!req_removed)) || ((!CanRun()) && (req_removed)))
            {
                need_update = true;
            }
        }*/

        //private static bool req_removed = false;
        //private static System.Collections.Generic.List<req_structure> backup = null;

        /*private static void Backup()
        {
            if (CanRun())
            {
                backup = new System.Collections.Generic.List<req_structure>();
                foreach (SetBonusesList.Entry set_bonuses in Refs_Manager.set_bonuses_list.entries)
                {
                    System.Collections.Generic.List<int> bonuses = new System.Collections.Generic.List<int>();
                    if (!set_bonuses.mods.IsNullOrDestroyed())
                    {
                        foreach (SetBonus set_bonus in set_bonuses.mods) { bonuses.Add(set_bonus.setRequirement); }
                    }

                    System.Collections.Generic.List<byte> descriptions = new System.Collections.Generic.List<byte>();
                    if (!set_bonuses.tooltipDescriptions.IsNullOrDestroyed())
                    {
                        foreach (ItemTooltipDescription item_tooltip_description in set_bonuses.tooltipDescriptions)
                        {
                            descriptions.Add(item_tooltip_description.setRequirement);
                        }
                    }

                    req_structure req = new req_structure
                    {
                        set_id = set_bonuses.setID,
                        set_description = descriptions,
                        set_bonuses = bonuses
                    };

                    descriptions.Clear();
                    bonuses.Clear();

                    backup.Add(req);
                }
            }
        }
        private static void Remove()
        {
            if ((CanRun()) && (!backup.IsNullOrDestroyed()))
            {
                Main.logger_instance.Msg("Remove set req");
                foreach (SetBonusesList.Entry set_bonuses in Refs_Manager.set_bonuses_list.entries)
                {
                    foreach (SetBonus set_bonus in set_bonuses.mods)
                    {
                        set_bonus.setRequirement = 0;
                    }
                    foreach (ItemTooltipDescription item_tooltip_description in set_bonuses.tooltipDescriptions)
                    {
                        item_tooltip_description.setRequirement = 0;
                    }                    
                }
                req_removed = true;
            }
        }
        private static void Reset()
        {
            if ((CanRun()) && (!backup.IsNullOrDestroyed()))
            {
                Main.logger_instance.Msg("Reset set req");
                foreach (SetBonusesList.Entry set_bonuses in Refs_Manager.set_bonuses_list.entries)
                {
                    bool backup_found = false;
                    int i = 0;
                    foreach (req_structure backup_req in backup)
                    {
                        if (backup_req.set_id == set_bonuses.setID)
                        {
                            backup_found = true;
                            break;
                        }
                        i++;
                    }
                    if (backup_found)
                    {
                        int k = 0;
                        if (!backup[i].set_bonuses.IsNullOrDestroyed())
                        {
                            foreach (SetBonus set_bonus in set_bonuses.mods)
                            {
                                if (k < backup[i].set_bonuses.Count)
                                {
                                    set_bonus.setRequirement = backup[i].set_bonuses[k];
                                }
                                k++;
                            }
                        }
                        k = 0;
                        if (!backup[i].set_description.IsNullOrDestroyed())
                        {
                            foreach (ItemTooltipDescription item_tooltip_description in set_bonuses.tooltipDescriptions)
                            {
                                if (k < backup[i].set_description.Count)
                                {
                                    item_tooltip_description.setRequirement = backup[i].set_description[k];
                                }
                                k++;
                            }
                        }
                    }
                }
                req_removed = false;
            }
        }*/
        private static bool CanRun()
        {
            if (!Save_Manager.instance.IsNullOrDestroyed()) //&& (!Refs_Manager.set_bonuses_list.IsNullOrDestroyed()))
            {
                if (Save_Manager.instance.initialized) // && (!Refs_Manager.set_bonuses_list.entries.IsNullOrDestroyed()))
                { return Save_Manager.instance.data.Items.Req.set; }
                else { return false; }
            }
            else { return false; }
        }

        [HarmonyPatch(typeof(ItemContainersManager), "getGearCountForSetID")]
        public class getGearCountForSetID
        {
            [HarmonyPostfix]
            static void Postfix(ref int __result)
            {
                if (CanRun()) { __result = 10; }
            }
        }
    }
}

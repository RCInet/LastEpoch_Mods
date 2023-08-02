using HarmonyLib;

namespace LastEpochMods.Hooks
{
    public class Item_Equip_Manager
    {
        /*[HarmonyPatch(typeof(ItemEquipManager), "UpdateStats")]
        public class UpdateStats
        {
            [HarmonyPostfix]
            static void Postfix(ref ItemEquipManager __instance, ref ItemDataUnpacked __0, ref bool __1)
            {
                if (__1 == true) //OnEquip
                {
                    if (__0.rarity == 8)
                    {
                        Main.logger_instance.Msg("ItemEquipManager : UpdateStats");
                        Main.logger_instance.Msg("Set Id = " + __0.SetId);
                        byte set_id = (byte)__0.SetId;

                        Il2CppSystem.Collections.Generic.List<ushort> new_values = new Il2CppSystem.Collections.Generic.List<ushort>();
                        foreach (UniqueList.Entry item in UniqueList.get().uniques)
                        {
                            if ((item.isSetItem) && (item.setID == set_id))
                            {
                                Main.logger_instance.Msg("Set item found : " + item.name + ", Unique Id = " + item.uniqueID);
                                new_values.Add(item.uniqueID);
                            }
                        }
                        int index = 0;                        
                        bool id_found = false;
                        foreach (byte id in __instance.setsPresent)
                        {
                            Main.logger_instance.Msg("Id = " + id + ", Set Id = " + set_id);
                            if (id == set_id)
                            {
                                Main.logger_instance.Msg("Id Found in setsPresent : Index = " + index + ", Id = " + id);
                                id_found = true;
                                break;
                            }
                            if (!id_found)
                            index++;
                        }
                        //Add item to present

                        //Add item to Completionlist
                        if ((id_found) && (new_values.Count > 0))
                        {
                            if (index < __instance.setCompletion.lists.Count)
                            {
                                Main.logger_instance.Msg("Edit Set Completion Values");
                                __instance.setCompletion.lists[index] = new_values;
                            }
                        }
                    }
                }
            }
        }*/
    }
}

using Newtonsoft.Json;
using UnhollowerBaseLib;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class LastEpochSaveEditorDatabase
    {
        public static void GenerateDatabase(Main main)
        {
            main.LoggerInstance.Msg("Get All Items");
            UnityEngine.Object obj = Functions.GetObject("MasterItemsList");
            System.Type type = obj.GetActualType();
            if (type == typeof(ItemList))
            {
                ItemList item_list = obj.TryCast<ItemList>();
                int index = 0;

                foreach (Db.Get.Type.Type_Structure type_struct in Db.Get.Type.TypesArray)
                {
                    if (type_struct.Id < 34)
                    {
                        if ((type_struct.Id != 11) && (type_struct.Id != 24)) //Fist and Crosbow
                        {
                            Db.Json.Items new_list_item = new Db.Json.Items();
                            string base_type = type_struct.Name;
                            main.LoggerInstance.Msg("");
                            main.LoggerInstance.Msg("Get All " + type_struct.Name);
                            new_list_item.Basic = new System.Collections.Generic.List<Db.Json.Basic>();
                            Il2CppSystem.Collections.Generic.List<ItemList.EquipmentItem> items = item_list.GetEquipmentSubItems(type_struct.Id);
                            foreach (var item in items)
                            {
                                main.LoggerInstance.Msg(base_type + " : Basic : " + item.name + ", Id : " + item.subTypeID);
                                new_list_item.Basic.Add(new Db.Json.Basic
                                {
                                    BaseName = item.name,
                                    DisplayName = item.displayName,
                                    BaseId = item.subTypeID,
                                    Level = item.levelRequirement,
                                    Implicit = Items.GetItemImplicits(item.implicits)
                                });
                                //UnityEngine.Texture2D icon = GetTexture2D(item.name);
                                //Save Icon
                            }
                            new_list_item.Unique = new System.Collections.Generic.List<Db.Json.Unique>();
                            new_list_item.Set = new System.Collections.Generic.List<Db.Json.Set>();
                            UnityEngine.Object obj2 = Functions.GetObject("UniqueList");
                            System.Type type2 = obj2.GetActualType();
                            if (type2 == typeof(UniqueList))
                            {
                                UniqueList unique_list = obj2.TryCast<UniqueList>();
                                Il2CppSystem.Collections.Generic.List<UniqueList.Entry> Uniques_List_Entry = unique_list.uniques;
                                foreach (UniqueList.Entry ul_entry in Uniques_List_Entry)
                                {
                                    if (ul_entry.baseType == type_struct.Id)
                                    {
                                        string base_name = "";
                                        System.Collections.Generic.List<string> prefixs = new System.Collections.Generic.List<string>();
                                        foreach (Db.Json.Basic basic in new_list_item.Basic)
                                        {
                                            if (basic.BaseId == ul_entry.baseType)
                                            {
                                                base_name = basic.BaseName;
                                                prefixs = basic.Implicit;
                                                break;
                                            }
                                        }
                                        System.Collections.Generic.List<string> unique_mod_list = Items.GetUniqueMods(ul_entry.mods);
                                        if (!ul_entry.isSetItem)
                                        {
                                            main.LoggerInstance.Msg(base_type + " : Unique : " + ul_entry.name + ", Id = " + ul_entry.uniqueID);
                                            new_list_item.Unique.Add(new Db.Json.Unique
                                            {
                                                BaseId = ul_entry.baseType,
                                                BaseName = base_name,
                                                Implicit = prefixs,
                                                UniqueId = ul_entry.uniqueID,
                                                UniqueName = ul_entry.name,
                                                Unique_Affixs = unique_mod_list,
                                                LoreText = ul_entry.loreText,
                                                Level = ul_entry.levelRequirement
                                            });
                                        }
                                        else
                                        {
                                            main.LoggerInstance.Msg(base_type + " : Set : " + ul_entry.name + ", Id = " + ul_entry.uniqueID);
                                            new_list_item.Set.Add(new Db.Json.Set
                                            {
                                                BaseId = ul_entry.baseType,
                                                BaseName = base_name,
                                                Implicit = prefixs,
                                                SetId = ul_entry.uniqueID,
                                                SetName = ul_entry.name,
                                                Set_Refs = new System.Collections.Generic.List<Db.Json.Set_Ref>(),
                                                Unique_Affixs = unique_mod_list,
                                                LoreText = ul_entry.loreText,
                                                Level = ul_entry.levelRequirement
                                            });
                                        }
                                        //UnityEngine.Texture2D icon = GetTexture2D(ul_entry.name);
                                        //Save Icon
                                    }
                                }
                            }
                            main.LoggerInstance.Msg(type_struct.Name + " : Basic = " + new_list_item.Basic.Count +
                                ", Unique = " + new_list_item.Unique.Count +
                                ", Set = " + new_list_item.Set.Count);
                            string jsonString = JsonConvert.SerializeObject(new_list_item);
                            string path = System.IO.Directory.GetCurrentDirectory() + @"\Mods\Out_LastEpoch" + type_struct.Path;
                            string filename = type_struct.Name.Replace(' ', '_');
                            main.LoggerInstance.Msg("Save : Path = " + path + ", Filename = " + filename);
                            if (System.IO.File.Exists(path + filename)) { System.IO.File.Delete(path + filename); }
                            if (!System.IO.Directory.Exists(path)) { System.IO.Directory.CreateDirectory(path); }
                            System.IO.File.WriteAllText(path + filename, jsonString);
                        }
                    }
                    else if (type_struct.Id == 101) //Affixs
                    {
                        main.LoggerInstance.Msg("");
                        main.LoggerInstance.Msg("Get All Affixs");
                        UnityEngine.Object obj5 = Functions.GetObject("MasterAffixesList");
                        System.Type type5 = obj5.GetActualType();
                        if (type5 == typeof(AffixList))
                        {
                            if (!Db.Data.Data_init) { Db.Data.InitData(); }
                            AffixList affix_list = obj5.TryCast<AffixList>();
                            Il2CppReferenceArray<AffixList.SingleAffix> single_affix = affix_list.singleAffixes;
                            Il2CppReferenceArray<AffixList.MultiAffix> multi_affix = affix_list.multiAffixes;
                            int i = 0;
                            foreach (AffixList.SingleAffix a in single_affix)
                            {
                                Db.Json.Affixs.Affix shard = Db.Data.SingleAffixToShard(a);
                                Db.Data.DB_Single_Affixs.List.Add(shard);
                                i++;
                            }
                            main.LoggerInstance.Msg(Db.Data.DB_Single_Affixs.List.Count + " / " +
                                single_affix.Count + " Single Affixs Add in Data.Prefixs");
                            i = 0;
                            foreach (AffixList.MultiAffix a in multi_affix)
                            {
                                Db.Json.Affixs.Affix shard = Db.Data.MultiAffixToShard(a);
                                Db.Data.DB_Multi_Affixs.List.Add(shard);
                                i++;
                            }
                            main.LoggerInstance.Msg(Db.Data.DB_Multi_Affixs.List.Count + " / " +
                                multi_affix.Count + " Multi Affixs Add in Data.Suffixs");
                            main.LoggerInstance.Msg("Save " + (Db.Data.DB_Single_Affixs.List.Count + Db.Data.DB_Multi_Affixs.List.Count) + " affixs in Affixs.json");
                            Db.Save.AffixsJson(main);
                        }
                    }
                    index++;
                }
            }
        }
    }
}

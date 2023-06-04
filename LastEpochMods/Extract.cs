using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using LastEpochMods.Db;
using LastEpochMods.Db.Json;
using MelonLoader;
using Newtonsoft.Json;
using System.Drawing;
using System.Linq;
using UnhollowerBaseLib;
using UnityEngine;
using UniverseLib;
using static MelonLoader.MelonLogger;

namespace LastEpochMods
{
    public class Extract : MelonMod
    {
        #region UnityEngine         
        private string scene_name = "";
        private string[] menu_scene_names = { "PersistentUI", "Login", "CharacterSelectScene" };
        private UnityEngine.Object GetObject(string name)
        {
            UnityEngine.Object objet = new UnityEngine.Object();
            foreach (UnityEngine.Object obj in RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                if ((name != "") && (obj.name.Contains(name)))
                {
                    System.Type type = obj.GetActualType();
                    if (type != typeof(TextAsset))
                    {
                        objet = obj;
                        break;
                    }                                 
                }
            }
            return objet;
        }
        private UnityEngine.Texture2D GetTexture2D(string name)
        {
            UnityEngine.Texture2D picture = null;
            foreach (UnityEngine.Object obj in RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                if ((name != "") && (obj.name.Contains(name)))
                {
                    System.Type type = obj.GetActualType();
                    if (type == typeof(UnityEngine.Texture2D))
                    {
                        picture = obj.TryCast<UnityEngine.Texture2D>();
                        break;
                    }
                }
            }

            return picture;
        }
        #endregion
        #region Mods
        #region UniquesMods
        private List<UniqueItemMod> CustomMods_0()
        {
            List<UniqueItemMod> mods = new List<UniqueItemMod>();
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.INCREASED,
                value = 999,
                maxValue = 999,
                property = SP.AttackSpeed,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.ADDED,
                value = 999,
                maxValue = 999,
                property = SP.CriticalChance,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.INCREASED,
                value = 999,
                maxValue = 999,
                property = SP.CriticalMultiplier,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.INCREASED,
                value = 999,
                maxValue = 999,
                property = SP.Damage,
                tags = AT.Physical
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.ADDED,
                value = 999,
                maxValue = 999,
                property = SP.Strength,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.ADDED,
                value = 999,
                maxValue = 999,
                property = SP.Intelligence,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.ADDED,
                value = 999,
                maxValue = 999,
                property = SP.Dexterity,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.INCREASED,
                value = 999,
                maxValue = 999,
                property = SP.IncreasedDropRate,
                tags = AT.None
            });
            mods.Add(new UniqueItemMod
            {
                type = BaseStats.ModType.INCREASED,
                value = 999,
                maxValue = 999,
                property = SP.IncreasedExperience,
                tags = AT.None
            });

            return mods;
        }
        private void EditUniqueMods(int unique_id, List<UniqueItemMod> mods)
        {
            UnityEngine.Object obj = GetObject("UniqueList");
            System.Type type = obj.GetActualType();
            if (type == typeof(UniqueList))
            {
                UniqueList unique_list = obj.TryCast<UniqueList>();
                List<UniqueList.Entry> Uniques_List_Entry = unique_list.uniques;
                foreach (UniqueList.Entry ul_entry in Uniques_List_Entry)
                {
                    if (ul_entry.uniqueID == unique_id)
                    {
                        ul_entry.mods = mods;
                        LoggerInstance.Msg(ul_entry.name + ": Unique Mods Edited");
                        break;
                    }
                }
            }
        }
        private void AddUniqueModToAnother(int item_id, int item2_id)
        {
            UnityEngine.Object obj = GetObject("UniqueList");
            System.Type type = obj.GetActualType();
            if (type == typeof(UniqueList))
            {
                UniqueList unique_list = obj.TryCast<UniqueList>();
                List<UniqueItemMod> mods = new List<UniqueItemMod>();
                //Copy
                int index = -1;
                int i = 0;
                foreach (UniqueList.Entry ul_entry in unique_list.uniques)
                {
                    if ((ul_entry.baseType == item_id) | (ul_entry.baseType == item2_id))
                    {
                        if (ul_entry.baseType == item2_id) { index = i; }
                        foreach (UniqueItemMod m in ul_entry.mods) { mods.Add(m); }
                    }
                    i++;
                }
                //Paste
                if (index > -1) { unique_list.uniques[index].mods = mods; }

                foreach (UniqueList.Entry ul_entry in unique_list.uniques)
                {
                    if (ul_entry.baseType == item2_id) { ul_entry.mods = mods; }
                }
            }
        }
        #endregion
        #endregion
        #region Database
        private void GenerateDatabase()
        {
            LoggerInstance.Msg("Get All Items");
            UnityEngine.Object obj = GetObject("MasterItemsList");
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
                            LoggerInstance.Msg("");
                            LoggerInstance.Msg("Get All " + type_struct.Name);
                            new_list_item.Basic = new System.Collections.Generic.List<Db.Json.Basic>();
                            List<ItemList.EquipmentItem> items = item_list.GetEquipmentSubItems(type_struct.Id);
                            foreach (var item in items)
                            {                                
                                LoggerInstance.Msg(base_type + " : Basic : " + item.name + ", Id : " + item.subTypeID);
                                new_list_item.Basic.Add(new Db.Json.Basic
                                {
                                    BaseName = item.name,
                                    DisplayName = item.displayName,
                                    BaseId = item.subTypeID,
                                    Level = item.levelRequirement,
                                    Implicit = GetItemImplicits(item.implicits)
                                });
                                //UnityEngine.Texture2D icon = GetTexture2D(item.name);
                                //Save Icon
                            }
                            new_list_item.Unique = new System.Collections.Generic.List<Db.Json.Unique>();
                            new_list_item.Set = new System.Collections.Generic.List<Db.Json.Set>();
                            UnityEngine.Object obj2 = GetObject("UniqueList");
                            System.Type type2 = obj2.GetActualType();
                            if (type2 == typeof(UniqueList))
                            {
                                UniqueList unique_list = obj2.TryCast<UniqueList>();
                                List<UniqueList.Entry> Uniques_List_Entry = unique_list.uniques;
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
                                        System.Collections.Generic.List<string> unique_mod_list = GetUniqueMods(ul_entry.mods);
                                        if (!ul_entry.isSetItem)
                                        {
                                            LoggerInstance.Msg(base_type + " : Unique : " + ul_entry.name + ", Id = " + ul_entry.uniqueID);
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
                                            LoggerInstance.Msg(base_type + " : Set : " + ul_entry.name + ", Id = " + ul_entry.uniqueID);
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
                            LoggerInstance.Msg(type_struct.Name + " : Basic = " + new_list_item.Basic.Count +
                                ", Unique = " + new_list_item.Unique.Count +
                                ", Set = " + new_list_item.Set.Count);
                            string jsonString = JsonConvert.SerializeObject(new_list_item);
                            string path = System.IO.Directory.GetCurrentDirectory() + @"\Mods\Out_LastEpoch" + type_struct.Path;
                            string filename = type_struct.Name.Replace(' ', '_');
                            LoggerInstance.Msg("Save : Path = " + path + ", Filename = " + filename);
                            if (System.IO.File.Exists(path + filename)) { System.IO.File.Delete(path + filename); }
                            if (!System.IO.Directory.Exists(path)) { System.IO.Directory.CreateDirectory(path); }
                            System.IO.File.WriteAllText(path + filename, jsonString);
                        }
                    }
                    else if (type_struct.Id == 101) //Affixs
                    {
                        LoggerInstance.Msg("");
                        LoggerInstance.Msg("Get All Affixs");
                        UnityEngine.Object obj5 = GetObject("MasterAffixesList");
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
                                Db.Json.Affixs.Affix shard = Data.SingleAffixToShard(a);
                                Db.Data.DB_Single_Affixs.List.Add(shard);
                                i++;
                            }
                            LoggerInstance.Msg(Db.Data.DB_Single_Affixs.List.Count + " / " +
                                single_affix.Count + " Single Affixs Add in Data.Prefixs");
                            i = 0;
                            foreach (AffixList.MultiAffix a in multi_affix)
                            {
                                Db.Json.Affixs.Affix shard = Data.MultiAffixToShard(a);
                                Db.Data.DB_Multi_Affixs.List.Add(shard);
                                i++;
                            }
                            LoggerInstance.Msg(Db.Data.DB_Multi_Affixs.List.Count + " / " +
                                multi_affix.Count + " Multi Affixs Add in Data.Suffixs");
                            LoggerInstance.Msg("Save " + (Db.Data.DB_Single_Affixs.List.Count + Db.Data.DB_Multi_Affixs.List.Count) + " affixs in Affixs.json");
                            Db.Save.AffixsJson(this);
                        }
                    }
                    index++;
                }
            }
        }
        #region Items
        private System.Collections.Generic.List<string> GetItemImplicits(List<ItemList.EquipmentImplicit> item_implicits)
        {
            System.Collections.Generic.List<string> list_implicits = new System.Collections.Generic.List<string>();
            foreach (ItemList.EquipmentImplicit item_implicit in item_implicits)
            {
                string min_value = Convert.ToString(item_implicit.implicitValue);
                string max_value = Convert.ToString(item_implicit.implicitMaxValue);
                if ((item_implicit.implicitValue >= 0) && (item_implicit.implicitValue <= 1) &&
                    (item_implicit.implicitMaxValue >= 0) && (item_implicit.implicitMaxValue <= 1))
                {
                    min_value = Convert.ToString(item_implicit.implicitValue * 100) + " %";
                    max_value = Convert.ToString(item_implicit.implicitMaxValue * 100) + " %";
                }
                string value = min_value;
                if (item_implicit.implicitValue != item_implicit.implicitMaxValue)
                {
                    value = "(" + min_value + " to " + max_value + ")";
                }
                string implicit_type = item_implicit.type.ToString().ToLower();
                string implicit_tags = "";
                if (item_implicit.tags.ToString() != "None") { implicit_tags = " " + item_implicit.tags.ToString(); }
                string implicit_string = implicit_type + " " + value + implicit_tags + " " + item_implicit.property.ToString().ToLower();
                if (implicit_type == "added")
                {
                    implicit_string = "+" + value + implicit_tags + " " + item_implicit.property.ToString();
                }
                else if (implicit_type == "increased")
                {
                    implicit_string = value + " " + implicit_type + implicit_tags + " " + item_implicit.property.ToString();
                }
                list_implicits.Add(implicit_string);
            }

            return list_implicits;
        }
        private System.Collections.Generic.List<string> GetUniqueMods(List<UniqueItemMod> unique_mods)
        {
            System.Collections.Generic.List<string> list_unique_affixs = new System.Collections.Generic.List<string>();
            foreach (UniqueItemMod mod in unique_mods)
            {
                string min_value = Convert.ToString(mod.value);
                string max_value = Convert.ToString(mod.maxValue);
                if ((mod.value >= 0) && (mod.value <= 1) &&
                    (mod.maxValue >= 0) && (mod.maxValue <= 1))
                {
                    min_value = Convert.ToString(mod.value * 100) + " %";
                    max_value = Convert.ToString(mod.maxValue * 100) + " %";
                }
                string value = min_value;
                if (mod.value != mod.maxValue)
                {
                    value = "(" + min_value + " to " + max_value + ")";
                }
                string mod_type = mod.type.ToString().ToLower();
                string mod_tag = "";
                if (mod.tags.ToString() != "None") { mod_tag = " " + mod.tags.ToString(); }
                string mod_string = mod_type + " " + value + mod_tag + " " + mod.property.ToString().ToLower();
                if (mod_type == "added")
                {
                    mod_string = "+" + value + mod_tag + " " + mod.property.ToString();
                }
                else if (mod_type == "increased")
                {
                    mod_string = value + " " + mod_type + mod_tag + " " + mod.property.ToString();
                }
                list_unique_affixs.Add(mod_string);
            }
            
            return list_unique_affixs;
        }
        #endregion
        #endregion
        #region Events       
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            scene_name = sceneName;
        }
        public override void OnLateUpdate()
        {
            if (scene_name != "")
            {
                if (Input.GetKeyDown(KeyCode.F10)) //Wait UnityEngine Initialized
                {
                    EditUniqueMods(111, CustomMods_0()); //Wover Flesh
                    if (!menu_scene_names.Contains(scene_name))
                    { LoggerInstance.Msg("Go Back to Menu for actualize items Mods"); }
                }
                else if (Input.GetKeyDown(KeyCode.F11))
                {
                    if (!menu_scene_names.Contains(scene_name)) { GenerateDatabase(); }
                    else { LoggerInstance.Msg("Lauch a character before doing this"); }
                }
            }
        }
        #endregion
    }
}

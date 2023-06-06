using LastEpochMods.Db.Json;
using MelonLoader;
using System.Drawing;
using System.Xml.Linq;
using UMA.AssetBundles;
using UnityEngine;
using UniverseLib;

namespace LastEpochMods.Mods
{
    public class LastEpochSaveEditor
    {
        public static void GenerateDatabase(Main main)
        {
            //GetAllItemsSprite();
            //GetAllTexture2D();
            GetAllItems(main);
            GetAllCharactersSkills(main);
        }
        //Texture2D
        public static void GetAllTexture2D()
        {
            UnityEngine.Texture2D Icon = null;
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                string path = System.IO.Directory.GetCurrentDirectory() + @"\Mods\Out_LastEpoch\Texture2D\";
                if (obj.name != "")
                {
                    System.Type type = obj.GetActualType();
                    if (type == typeof(UnityEngine.Texture2D))
                    {
                        Icon = obj.TryCast<UnityEngine.Texture2D>();
                        string filename = Icon.name + ".png";
                        if ((Icon.width >= 64) && (Icon.width <= 512) && (Icon.height >= 64) && (Icon.height <= 512))
                        {
                            if ((Icon.width == 256) && (Icon.height == 247)) { path += @"Glyphs\"; }
                            else if ((Icon.width == 256) && (Icon.height == 244)) { path += @"Runes\"; }
                            else if ((Icon.width == 200) && (Icon.height == 200)) { path += @"1X1\"; }
                            else if ((Icon.width == 128) && (Icon.height == 256)) { path += @"1X3\"; } //2x4
                            else if ((Icon.width == 85) && (Icon.height == 256)) { path += @"1X4\"; }                            
                            else if ((Icon.width == 256) && (Icon.height == 256)) //2x2
                            {
                                try
                                {
                                    int i = System.Convert.ToInt32(Icon.name);
                                    path += @"Blessings\";
                                }
                                catch { path += @"2X2\"; }
                            }
                            else if ((Icon.width == 171) && (Icon.height == 256)) { path += @"2X3\"; }
                            else if ((Icon.width == 256) && (Icon.height == 128)) { path += @"3X1\"; }
                            else if ((Icon.width == 512) && (Icon.height == 128)) { path += @"4X1\"; }
                            else { path += @"Unknow\"; }
                            
                            UniverseLib.Runtime.TextureHelper.SaveTextureAsPNG(Icon, path + filename);
                        }
                    }
                }
            }
        }
        public static void GetAllItemsSprite()
        {
            UnityEngine.Sprite sprite = null;
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
            {
                string path = System.IO.Directory.GetCurrentDirectory() + @"\Mods\Out_LastEpoch\Sprite\";
                if (obj.name != "")
                {
                    System.Type type = obj.GetActualType();
                    if (type == typeof(UnityEngine.Sprite))
                    {
                        sprite = obj.TryCast<UnityEngine.Sprite>();
                        if (sprite.name != "")
                        {
                            string filename = sprite.name + ".png";
                            UniverseLib.Runtime.TextureHelper.SaveTextureAsPNG(sprite.texture, path + filename);
                        }
                    }
                }
            }
        }
        //Items
        public static void GetAllItems(Main main)
        {
            main.LoggerInstance.Msg("Get All Items");
            UnityEngine.Object obj = Functions.GetObject("MasterItemsList");
            System.Type type = obj.GetActualType();
            if (type == typeof(ItemList))
            {
                ItemList item_list = obj.TryCast<ItemList>();
                int index = 0;

                foreach (Type.Type_Structure type_struct in Type.TypesArray)
                {
                    if (type_struct.Id < 34)
                    {
                        if ((type_struct.Id != 11) && (type_struct.Id != 24)) //Fist and Crosbow
                        {
                            string path = System.IO.Directory.GetCurrentDirectory() + @"\Mods\Out_LastEpoch" + type_struct.Path;
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
                                    Implicit = GetItemImplicits(item.implicits)
                                });
                                //SaveItemIcon(item.name, path, main);                                
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
                                    path = System.IO.Directory.GetCurrentDirectory() + @"\Mods\Out_LastEpoch" + type_struct.Path;
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
                                            path += @"Unique\";
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
                                            path += @"Set\";
                                        }
                                        //SaveItemIcon(ul_entry.name, path, main);
                                    }
                                }
                            }
                            main.LoggerInstance.Msg(type_struct.Name + " : Basic = " + new_list_item.Basic.Count +
                                ", Unique = " + new_list_item.Unique.Count +
                                ", Set = " + new_list_item.Set.Count);
                            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(new_list_item);
                            
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
                            if (!Data_init) { InitData(); }
                            AffixList affix_list = obj5.TryCast<AffixList>();
                            UnhollowerBaseLib.Il2CppReferenceArray<AffixList.SingleAffix> single_affix = affix_list.singleAffixes;
                            UnhollowerBaseLib.Il2CppReferenceArray<AffixList.MultiAffix> multi_affix = affix_list.multiAffixes;
                            int i = 0;
                            foreach (AffixList.SingleAffix a in single_affix)
                            {
                                Db.Json.Affixs.Affix shard = SingleAffixToShard(a);
                                DB_Single_Affixs.List.Add(shard);
                                i++;
                            }
                            main.LoggerInstance.Msg(DB_Single_Affixs.List.Count + " / " +
                                single_affix.Count + " Single Affixs Add in Data.Prefixs");
                            i = 0;
                            foreach (AffixList.MultiAffix a in multi_affix)
                            {
                                Db.Json.Affixs.Affix shard = MultiAffixToShard(a);
                                DB_Multi_Affixs.List.Add(shard);
                                i++;
                            }
                            main.LoggerInstance.Msg(DB_Multi_Affixs.List.Count + " / " +
                                multi_affix.Count + " Multi Affixs Add in Data.Suffixs");
                            main.LoggerInstance.Msg("Save " + (DB_Single_Affixs.List.Count + DB_Multi_Affixs.List.Count) + " affixs in Affixs.json");
                            SaveAffixsJson(main);
                        }
                    }
                    index++;
                }
            }
            GetAllCharactersSkills(main);
        }        
        public static System.Collections.Generic.List<string> GetItemImplicits(Il2CppSystem.Collections.Generic.List<ItemList.EquipmentImplicit> item_implicits)
        {
            System.Collections.Generic.List<string> list_implicits = new System.Collections.Generic.List<string>();
            foreach (ItemList.EquipmentImplicit item_implicit in item_implicits)
            {
                string min_value = System.Convert.ToString(item_implicit.implicitValue);
                string max_value = System.Convert.ToString(item_implicit.implicitMaxValue);
                if ((item_implicit.implicitValue >= 0) && (item_implicit.implicitValue <= 1) &&
                    (item_implicit.implicitMaxValue >= 0) && (item_implicit.implicitMaxValue <= 1))
                {
                    min_value = System.Convert.ToString(item_implicit.implicitValue * 100) + " %";
                    max_value = System.Convert.ToString(item_implicit.implicitMaxValue * 100) + " %";
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
        public static System.Collections.Generic.List<string> GetUniqueMods(Il2CppSystem.Collections.Generic.List<UniqueItemMod> unique_mods)
        {
            System.Collections.Generic.List<string> list_unique_affixs = new System.Collections.Generic.List<string>();
            foreach (UniqueItemMod mod in unique_mods)
            {
                string min_value = System.Convert.ToString(mod.value);
                string max_value = System.Convert.ToString(mod.maxValue);
                if ((mod.value >= 0) && (mod.value <= 1) &&
                    (mod.maxValue >= 0) && (mod.maxValue <= 1))
                {
                    min_value = System.Convert.ToString(mod.value * 100) + " %";
                    max_value = System.Convert.ToString(mod.maxValue * 100) + " %";
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
        public static void SaveItemIcon(string item_name, string path, Main main)
        {
            UnityEngine.Sprite sprite = null;
            bool found = false;
            string test_string = item_name.Replace(' ', '_');
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))            
            {
                System.Type type = obj.GetActualType();
                if (type == typeof(UnityEngine.Sprite))
                {
                    sprite = obj.TryCast<UnityEngine.Sprite>();
                    if (sprite.name.Contains(test_string))
                    {
                        string filename = test_string + ".png";
                        UniverseLib.Runtime.TextureHelper.SaveTextureAsPNG(sprite.texture, path + filename);
                        break;
                    }
                }
            }
            if (!found) { main.LoggerInstance.Msg(test_string + " Icon Not Found"); }
        }
        //Affixs
        public static bool Data_init = false;
        public static void InitData()
        {
            DB_Single_Affixs = new Db.Json.Affixs.Shards();
            DB_Single_Affixs.List = new System.Collections.Generic.List<Db.Json.Affixs.Affix>();
            DB_Multi_Affixs = new Db.Json.Affixs.Shards();
            DB_Multi_Affixs.List = new System.Collections.Generic.List<Db.Json.Affixs.Affix>();
            Data_init = true;
        }
        public static Db.Json.Affixs.Shards DB_Single_Affixs = new Db.Json.Affixs.Shards();
        public static Db.Json.Affixs.Shards DB_Multi_Affixs = new Db.Json.Affixs.Shards();
        public static Db.Json.Affixs.Affix SingleAffixToShard(AffixList.SingleAffix affix)
        {
            int x = affix.affixId / 256;
            int id = affix.affixId - (x * 256);
            string modifier = affix.modifierType.ToString();
            if (affix.tags.ToString() != "None") { modifier += " " + affix.tags.ToString(); }
            modifier += " " + affix.property.ToString();
            Db.Json.Affixs.Affix shard = new Db.Json.Affixs.Affix
            {
                Single = true,
                Modifier = modifier,
                ModifierList = null,
                DisplayName = affix.affixDisplayName,
                Id = id,
                X = x,
                Name = affix.affixName,
                Title = affix.affixTitle,
                Class = affix.classSpecificity.ToString(),
                DisplayCategory = affix.displayCategory.ToString(),
                Group = affix.group.ToString(),
                Level = affix.levelRequirement,
                RollOn = affix.rollsOn.ToString(),
                Tiers = affix.tiers.Capacity,
                Type = affix.type.ToString().ToLower()
            };

            return shard;
        }
        public static Db.Json.Affixs.Affix MultiAffixToShard(AffixList.MultiAffix affix)
        {
            int x = affix.affixId / 256;
            int id = affix.affixId - (x * 256);
            System.Collections.Generic.List<string> properties_list = new System.Collections.Generic.List<string>();
            Il2CppSystem.Collections.Generic.List<AffixList.AffixProperty> affix_properties_list = affix.affixProperties;
            foreach (AffixList.AffixProperty alp in affix_properties_list)
            {
                string modifier = alp.modifierType.ToString();
                if (alp.tags.ToString() != "None") { modifier += " " + alp.tags.ToString(); }
                modifier += " " + alp.property.ToString();
                properties_list.Add(modifier);
            }
            Db.Json.Affixs.Affix shard = new Db.Json.Affixs.Affix
            {
                Single = false,
                Modifier = "",
                ModifierList = properties_list,
                DisplayName = affix.affixDisplayName,
                Id = id,
                X = x,
                Name = affix.affixName,
                Title = affix.affixTitle,
                Class = affix.classSpecificity.ToString(),
                DisplayCategory = affix.displayCategory.ToString(),
                Group = affix.group.ToString(),
                Level = affix.levelRequirement,
                RollOn = affix.rollsOn.ToString(),
                Tiers = affix.tiers.Capacity,
                Type = affix.type.ToString().ToLower()
            };

            return shard;
        }
        public static void SaveAffixsJson(Main instance)
        {
            System.Collections.Generic.List<Db.Json.Affixs.Affix> affixslist = new System.Collections.Generic.List<Db.Json.Affixs.Affix>();
            string path = "";
            string filename = "";
            foreach (Type.Type_Structure a in Type.TypesArray)
            {
                if (a.Id == 101)
                {
                    path = System.IO.Directory.GetCurrentDirectory() + @"\Mods\Out_LastEpoch" + a.Path;
                    filename = a.Name + ".json";
                    break;
                }
            }
            if ((path != "") && (filename != ""))
            {
                string icon_path = path + @"\Icons\";
                for (int i = 0; i < 999; i++)
                {
                    bool found = false;
                    int x = i / 256;
                    int id = i - (x * 256);
                    string shard_name = "";
                    bool idol = true;
                    foreach (Db.Json.Affixs.Affix shard in DB_Single_Affixs.List)
                    {
                        if ((shard.Id == id) && (shard.X == x))
                        {
                            affixslist.Add(shard);
                            found = true;
                            shard_name = shard.Name;
                            if (shard.RollOn == "Equipment") { idol = false; }
                            break;
                        }
                    }
                    if (!found)
                    {
                        foreach (Db.Json.Affixs.Affix shard in DB_Multi_Affixs.List)
                        {
                            if ((shard.Id == id) && (shard.X == x))
                            {
                                affixslist.Add(shard);
                                found = true;
                                shard_name = shard.Name;
                                if (shard.RollOn == "Equipment") { idol = false; }
                                break;
                            }
                        }
                    }
                    if ((found) && (!idol))
                    {
                        //remove space for shard name
                        string[] names = shard_name.Split(' ');
                        string new_name = "";
                        foreach (string name in names)
                        {
                            if ((name != "") && (name != " "))
                            {
                                if (new_name == "") { new_name = name; }
                                else { new_name += " " + name; }
                            }
                        }

                        if (!Src.Icons.Load.Shards_init) { Src.Icons.Load.Shards(); }
                        bool Iconfound = false;
                        foreach (Src.Icons.Get.shard_struct shard in Src.Icons.Data.Db_Shards)
                        {
                            if (new_name == shard.name)
                            {
                                string src_icon_path = Src.Icons.Get.ShardsIcon_Path;
                                string src_icon_name = shard.icon_name;
                                string src_image_path = src_icon_path + src_icon_name;
                                if (System.IO.File.Exists(src_image_path))
                                {
                                    string icon_filename = shard_name.Replace(' ', '_') + ".png";
                                    string image_path = icon_path + icon_filename;
                                    if (System.IO.File.Exists(image_path)) { System.IO.File.Delete(image_path); }
                                    if (!System.IO.Directory.Exists(icon_path)) { System.IO.Directory.CreateDirectory(icon_path); }
                                    System.IO.File.Copy(src_image_path, image_path);
                                    Iconfound = true;
                                }
                                else { instance.LoggerInstance.Msg("Src Icon not found : " + src_image_path); }
                                break;
                            }
                        }
                        if (!Iconfound) { instance.LoggerInstance.Msg("Not in Shards.json : " + shard_name); }
                    }
                }
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(affixslist);
                string final_string = "{ \"DbShards\": " + jsonString + " }";
                if (System.IO.File.Exists(path + filename)) { System.IO.File.Delete(path + filename); }
                if (!System.IO.Directory.Exists(path)) { System.IO.Directory.CreateDirectory(path); }
                System.IO.File.WriteAllText(path + filename, final_string);
            }
        }
        //Skills
        public static void GetAllCharactersSkills(Main main)
        {
            main.LoggerInstance.Msg("");
            main.LoggerInstance.Msg("Get All Skills");
            UnityEngine.Object obj = Functions.GetObject("Character Class List");
            System.Type type = obj.GetActualType();            
            Il2CppSystem.Collections.Generic.List<CharacterClass> characters_class = new Il2CppSystem.Collections.Generic.List<CharacterClass>();
            string skill_path = System.IO.Directory.GetCurrentDirectory() + @"\Mods\Out_LastEpoch\Database\Skills\";
            string spritesheet_fullpath = skill_path + "Skills" + "_SpriteSheet.png";
            if (type == typeof(CharacterClassList))
            {
                CharacterClassList characters = obj.TryCast<CharacterClassList>();
                characters_class = characters.classes;
                main.LoggerInstance.Msg("Get Skills SpriteSheet");
                //SaveSkillsSpriteSheet(characters_class[0].knownAbilities[0].abilitySprite.texture, skill_path, "Skills");
            }
            UnityEngine.Texture2D SpriteSheet = characters_class[0].knownAbilities[0].abilitySprite.texture;
            System.Collections.Generic.List<Class_Skills_structure> Class_Skills_List = new System.Collections.Generic.List<Class_Skills_structure>();
            foreach (CharacterClass c_class in characters_class)
            {
                string class_path = skill_path + c_class.name + @"\";
                if (!System.IO.Directory.Exists(class_path)) { System.IO.Directory.CreateDirectory(class_path); }
                int nb_skills = 0;
                System.Collections.Generic.List<Skill_structure> Default_Skills = new System.Collections.Generic.List<Skill_structure>();                
                foreach (Ability ability in c_class.knownAbilities)
                {
                    Default_Skills.Add(AbilityToSkill(ability));
                    string icon_path = class_path + ability.name + @".png";
                    SaveSkillIcon(ability, icon_path);
                    nb_skills++;
                }
                foreach (AbilityAndLevel ability_level in c_class.unlockableAbilities)
                {
                    Default_Skills.Add(AbilityToSkill(ability_level.ability));
                    string icon_path = class_path + ability_level.ability.name + @".png";
                    SaveSkillIcon(ability_level.ability, icon_path);
                    nb_skills++;
                }
                System.Collections.Generic.List<Mastery_structure> Masteries_Skills = new System.Collections.Generic.List<Mastery_structure>();
                foreach (Mastery masterie in c_class.masteries)
                {
                    string mastery_path = class_path + @"\";
                    string masterie_final_name = masterie_final_name = "All";
                    if (masterie.name != c_class.name)
                    {
                        mastery_path = class_path + @"\" + masterie.name + @"\";
                        masterie_final_name = masterie.name;
                    }
                    if (!System.IO.Directory.Exists(mastery_path)) { System.IO.Directory.CreateDirectory(mastery_path); }
                    System.Collections.Generic.List<Skill_structure> MasterySkills = new System.Collections.Generic.List<Skill_structure>();
                    foreach (AbilityAndLevel ability_level in masterie.abilities)
                    {
                        MasterySkills.Add(AbilityToSkill(ability_level.ability));
                        string icon_path = mastery_path + ability_level.ability.name + @".png";
                        SaveSkillIcon(ability_level.ability, icon_path);
                        nb_skills++;
                    }
                    Masteries_Skills.Add(new Mastery_structure
                    {
                        MasteryName = masterie_final_name,
                        MasterySkills = MasterySkills
                    });
                }
                Class_Skills_List.Add(new Class_Skills_structure
                {
                    ClassName = c_class.name,
                    DefaultSkills = Default_Skills,
                    Masteries = Masteries_Skills
                });
                main.LoggerInstance.Msg(c_class.name + " Class Found : Contain " + nb_skills + " Skills");
            }
            SaveSkillJson(Class_Skills_List, main);
        }
        public struct Class_Skills_structure
        {
            public string ClassName;
            public System.Collections.Generic.List<Skill_structure> DefaultSkills;
            public System.Collections.Generic.List<Mastery_structure> Masteries;
        }
        public struct Mastery_structure
        {
            public string MasteryName;
            public System.Collections.Generic.List<Skill_structure> MasterySkills;
        }
        public struct Skill_structure
        {
            public string Name;
            public string Id;
            public Skill_Icon_Structure Icon;
        }
        public struct Skill_Icon_Structure
        {
            public int PositionX;
            public int PositionY;
            public int SizeX;
            public int SizeY;
        }
        public static Skill_structure AbilityToSkill(Ability ability)
        {
            Skill_structure skill = new Skill_structure
            {
                Name = ability.abilityName,
                Id = ability.playerAbilityID,
                Icon = new Skill_Icon_Structure
                {
                    PositionX = System.Convert.ToInt32(ability.abilitySprite.textureRect.x),
                    PositionY = System.Convert.ToInt32(ability.abilitySprite.textureRect.y),
                    SizeX = System.Convert.ToInt32(ability.abilitySprite.textureRect.width),
                    SizeY = System.Convert.ToInt32(ability.abilitySprite.textureRect.height)
                }
            };

            return skill;
        }
        public static void SaveSkillIcon(Ability ability, string icon_fullpath)
        {
            //int spritesheet_w = ability.abilitySprite.texture.width;
            int spritesheet_h = ability.abilitySprite.texture.height;
            //int position_x = System.Convert.ToInt32(ability.abilitySprite.textureRect.position.x);
            int position_y = System.Convert.ToInt32(ability.abilitySprite.textureRect.position.y);
            int icon_size_x = System.Convert.ToInt32(ability.abilitySprite.textureRect.width);
            int icon_size_y = System.Convert.ToInt32(ability.abilitySprite.textureRect.height);
            int x = System.Convert.ToInt32(ability.abilitySprite.textureRect.position.x);
            int y = spritesheet_h - position_y - icon_size_y;
            UnityEngine.Texture2D SpriteSheet = ability.abilitySprite.texture;
            Rect rectangle = new Rect
            {
                position = new Vector2(x, y),
                width = icon_size_x,
                height = icon_size_y
            };
            UnityEngine.Texture2D Icon = UniverseLib.Runtime.TextureHelper.CopyTexture(ability.abilitySprite.texture, rectangle);
            UniverseLib.Runtime.TextureHelper.SaveTextureAsPNG(Icon, icon_fullpath);
        }
        public static void SaveSkillsSpriteSheet(Texture2D texture, string path, string name)
        {
            UnityEngine.Texture2D SpriteSheet = texture;
            if (!System.IO.Directory.Exists(path)) { System.IO.Directory.CreateDirectory(path); }
            if (!System.IO.File.Exists(path + name + "_SpriteSheet.png"))
            { UniverseLib.Runtime.TextureHelper.SaveTextureAsPNG(SpriteSheet, path + name + "_SpriteSheet.png"); }
        }
        public static void SaveSkillJson(System.Collections.Generic.List<Class_Skills_structure> skills, Main main)
        {
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(skills);
            string path = System.IO.Directory.GetCurrentDirectory() + @"\Mods\Out_LastEpoch\Database\Skills\";
            string filename = "Skills.json";
            main.LoggerInstance.Msg("Save Skills : " + path + filename);
            if (System.IO.File.Exists(path + filename)) { System.IO.File.Delete(path + filename); }
            if (!System.IO.Directory.Exists(path)) { System.IO.Directory.CreateDirectory(path); }
            System.IO.File.WriteAllText(path + filename, jsonString);
        }
        //Types
        public class Type
        {
            public struct Type_Structure
            {
                public int Id;
                public string Name;
                public string Path;
            }
            public static Type_Structure[] TypesArray = new Type_Structure[]
            {
                new Type_Structure { Id = 0, Name = "Helm", Path = @"\Database\Items\Armors\Helms\" },
                new Type_Structure { Id = 1, Name = "Body", Path =  @"\Database\Items\Armors\Bodys\" },
                new Type_Structure { Id = 2, Name = "Belt", Path =  @"\Database\Items\Armors\Belts\" },
            new Type_Structure { Id = 3, Name = "Boot", Path = @"\Database\Items\Armors\Boots\" },
            new Type_Structure { Id = 4, Name = "Glove", Path = @"\Database\Items\Armors\Gloves\" },
            new Type_Structure { Id = 5, Name = "Axe", Path = @"\Database\Items\Weapons\Axes\1H\" },
            new Type_Structure { Id = 6, Name = "Dagger", Path = @"\Database\Items\Weapons\Daggers\" },
            new Type_Structure { Id = 7, Name = "Blunt", Path = @"\Database\Items\Weapons\Blunts\1H\" },
            new Type_Structure { Id = 8, Name = "Scepter", Path = @"\Database\Items\Weapons\Scepters\" },
            new Type_Structure { Id = 9, Name = "Sword", Path = @"\Database\Items\Weapons\Swords\1H\" },
            new Type_Structure { Id = 10, Name = "Wand", Path = @"\Database\Items\Weapons\Wands\" },
            new Type_Structure { Id = 11, Name = "Fist", Path = @"\Database\Items\Weapons\Fist\" },
            new Type_Structure { Id = 12, Name = "Two-handed Axe", Path = @"\Database\Items\Weapons\Axes\2H\" },
            new Type_Structure { Id = 13, Name = "Two-handed Blunt", Path = @"\Database\Items\Weapons\Blunts\2H\" },
            new Type_Structure { Id = 14, Name = "Polearm", Path = @"\Database\Items\Weapons\Polearms\" },
            new Type_Structure { Id = 15, Name = "Staff", Path = @"\Database\Items\Weapons\Staffs\" },
            new Type_Structure { Id = 16, Name = "Two-handed Sword", Path = @"\Database\Items\Weapons\words\2H\" },
            new Type_Structure { Id = 17, Name = "Quiver", Path = @"\Database\Items\Weapons\Quivers\" },
            new Type_Structure { Id = 18, Name = "Shield", Path = @"\Database\Items\Armors\Shields\" },
            new Type_Structure { Id = 19, Name = "Catalyst", Path = @"\Database\Items\Accesories\Catalysts\" },
            new Type_Structure { Id = 20, Name = "Amulet", Path = @"\Database\Items\Accesories\Amulets\" },
            new Type_Structure { Id = 21, Name = "Ring", Path = @"\Database\Items\Accesories\Rings\" },
            new Type_Structure { Id = 22, Name = "Relic", Path = @"\Database\Items\Accesories\Relics\" },
            new Type_Structure { Id = 23, Name = "Bow", Path = @"\Database\Items\Weapons\Bows\" },
            new Type_Structure { Id = 24, Name = "CrossBow", Path = @"\Database\Items\Weapons\CrossBows\" },
            new Type_Structure { Id = 25, Name = "Small", Path = @"\Database\Items\Idols\Smalls\" },
            new Type_Structure { Id = 26, Name = "Small Lagonian", Path = @"\Database\Items\dols\Small_Lagonians\" },
            new Type_Structure { Id = 27, Name = "Humble Eterran", Path = @"\Database\Items\Idols\Humble_Eterrans\" },
            new Type_Structure { Id = 28, Name = "Stout", Path = @"\Database\Items\Idols\Stouts\" },
            new Type_Structure { Id = 29, Name = "Grand", Path = @"\Database\Items\Idols\Grands\" },
            new Type_Structure { Id = 30, Name = "Large", Path = @"\Database\Items\Idols\Larges\" },
            new Type_Structure { Id = 31, Name = "Ornate", Path = @"\Database\Items\Idols\Ornates\" },
            new Type_Structure { Id = 32, Name = "Huge", Path = @"\Database\Items\Idols\Huges\" },
            new Type_Structure { Id = 33, Name = "Adorned", Path = @"\Database\Items\Idols\Adorneds\" },
            new Type_Structure { Id = 34, Name = "Blessings", Path = @"\Database\Blessings\" },
            new Type_Structure { Id = 101, Name = "Affixs", Path = @"\Database\Affixs\" },
            new Type_Structure { Id = 102, Name = "Runes", Path = @"\Database\Materials\Runes\" },
            new Type_Structure { Id = 103, Name = "Glyphs", Path = @"\Database\Materials\Glyphs\" },
            new Type_Structure { Id = 104, Name = "Key", Path = @"\Database\Items\Keys\" }
            };
        }
    }
}

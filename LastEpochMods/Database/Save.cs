using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using static AffixList;

namespace LastEpochMods.Db
{
    public class Save
    {
        public static void AffixsJson(Extract instance)
        {
            List<Db.Json.Affixs.Affix> affixslist = new List<Db.Json.Affixs.Affix>();
            
            for (int i = 0; i < 999; i++)
            {
                bool found = false;
                int x = i / 256;
                int id = i - (x * 256);
                string shard_name = "";
                bool idol = true;
                foreach (Db.Json.Affixs.Affix shard in Data.DB_Single_Affixs.List)
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
                    foreach (Db.Json.Affixs.Affix shard in Data.DB_Multi_Affixs.List)
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
                            if (File.Exists(src_image_path))
                            {
                                string icon_path = Database.Get.Affixs_IconPath;
                                string icon_filename = shard_name.Replace(' ', '_') + ".png";
                                string image_path = icon_path + icon_filename;
                                if (File.Exists(image_path)) { File.Delete(image_path); }
                                if (!Directory.Exists(icon_path)) { Directory.CreateDirectory(icon_path); }
                                File.Copy(src_image_path, image_path);
                                Iconfound = true;
                            }
                            else { instance.LoggerInstance.Msg("Src Icon not found : " + src_image_path); }                            
                            break;
                        }
                    }
                    if(!Iconfound) { instance.LoggerInstance.Msg("Not in Shards.json : " + shard_name); }
                }
            }
            string path = Database.Get.Affixs_Path;
            string filename = Database.Get.Affixs_Filename;
            string jsonString = JsonConvert.SerializeObject(affixslist);
            string final_string = "{ \"DbShards\": " + jsonString + " }";
            if (File.Exists(path + filename)) { File.Delete(path + filename); }
            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            File.WriteAllText(path + filename, final_string);
        }
    }
}

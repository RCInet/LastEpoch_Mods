using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class Save
    {
        private static string AppPath = System.IO.Directory.GetCurrentDirectory();
        public static void ItemsJson(int type)
        {
            if (type < Database.Items.Get.TypesArray.Length)
            {
                string path = AppPath + Database.Items.Get.TypesArray[type].Path;
                if (path != "")
                {
                    string filename = Database.Items.Get.TypesArray[type].Name.Replace(' ', '_');
                    Database.Items.Json.Items items = Database.Data.Items[type];

                    string jsonString = JsonConvert.SerializeObject(items);
                    if (File.Exists(path + filename)) { File.Delete(path + filename); }
                    File.WriteAllText(path + filename, jsonString);
                }                
            }
        }
        public static void BlessingsJson()
        {
            string path = AppPath + Blessings.Get.Path;
            string filename = Blessings.Get.Filename;
            string jsonString = JsonConvert.SerializeObject(Data.Blessings);
            if (File.Exists(path + filename)) { File.Delete(path + filename); }
            File.WriteAllText(path + filename, jsonString);
        }
        public static void SkillsJson()
        {
            string path = AppPath + Skills.Get.Path;
            string filename = "";
            string jsonString = JsonConvert.SerializeObject(Data.skills);
            if (File.Exists(path + filename)) { File.Delete(path + filename); }
            File.WriteAllText(path + filename, jsonString);
        }
        public static void RunesJson()
        {
            string path = AppPath + Materials.Runes.Get.Path;
            string filename = Materials.Runes.Get.Filename;
            string jsonString = JsonConvert.SerializeObject(Data.Runes);
            if (File.Exists(path + filename)) { File.Delete(path + filename); }
            File.WriteAllText(path + filename, jsonString);
        }
        public static void GlyphsJson()
        {
            string path = AppPath + Materials.Glyphs.Get.Path;
            string filename = Materials.Glyphs.Get.Filename;
            string jsonString = JsonConvert.SerializeObject(Data.Glyphs);
            if (File.Exists(path + filename)) { File.Delete(path + filename); }
            File.WriteAllText(path + filename, jsonString);
        }
    }
}

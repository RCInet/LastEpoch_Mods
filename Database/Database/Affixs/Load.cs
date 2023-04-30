using Newtonsoft.Json;
using System.IO;

namespace Database.Affix
{
    public class Load
    {
        public static bool Prefixs()
        {
            bool result = false;
            string path = System.IO.Directory.GetCurrentDirectory() + Get.Path + Get.PrefixesFilename;
            if (File.Exists(path))
            {
                string JsonString = File.ReadAllText(path);
                Database.Data.Prefixs = JsonConvert.DeserializeObject<Json.Shards>(JsonString);
                result = true;
            }

            return result;
        }
        public static bool Suffixs()
        {
            bool result = false;
            string path = System.IO.Directory.GetCurrentDirectory() + Get.Path + Get.SuffixesFilename;
            if (File.Exists(path))
            {
                string JsonString = File.ReadAllText(path);
                Database.Data.Suffixs = JsonConvert.DeserializeObject<Json.Shards>(JsonString);
                result = true;
            }

            return result;
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LastEpochMods.Src.Icons.Get;

namespace LastEpochMods.Src.Icons
{
    public class Load
    {
        public static bool Shards_init = false;
        private static string Shards_path = System.IO.Directory.GetCurrentDirectory() + @"\Mods\Src\Icons\Json\";
        private static string Shards_filename = "Shards.json";
        public static void Shards()
        {
            Data.Db_Shards = new List<Get.shard_struct>();
            if (File.Exists(Shards_path + Shards_filename))
            {
                string JsonString = File.ReadAllText(Shards_path + Shards_filename);
                Data.Db_Shards = JsonConvert.DeserializeObject<List<shard_struct>>(JsonString);
            }
            Shards_init = true;
        }
    }
}

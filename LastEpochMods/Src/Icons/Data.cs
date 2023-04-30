using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastEpochMods.Src.Icons
{
    public class Data
    {
        public class Shard
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("icon_name")]
            public string IconName { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }
        }
        public static List<Get.shard_struct> Db_Shards = new List<Get.shard_struct>();
    }
}

using Newtonsoft.Json;
using System.Collections.Generic;
using UnhollowerBaseLib;

namespace LastEpochMods.Db.Json
{
    public class Affixs
    {
        public class Shards
        {
            [JsonProperty("DbShards")]
            public List<Affix> List { get; set; }
        }
        public class Affix
        {
            [JsonProperty("single")]
            public bool Single { get; set; }

            [JsonProperty("modifier")]
            public string Modifier { get; set; }

            [JsonProperty("modifier_list")]
            public List<string> ModifierList { get; set; }

            [JsonProperty("display_name")]
            public string DisplayName { get; set; }

            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("x")]
            public int X { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("class")]
            public string Class { get; set; }

            [JsonProperty("display_category")]
            public string DisplayCategory { get; set; }

            [JsonProperty("group")]
            public string Group { get; set; }

            [JsonProperty("level")]
            public int Level { get; set; }

            [JsonProperty("roll_on")] //Equipment or Idol
            public string RollOn { get; set; }

            [JsonProperty("tiers")]
            public int Tiers { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
        }        
    }
}

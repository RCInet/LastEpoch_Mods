using Newtonsoft.Json;
using System.Collections.Generic;

namespace Database.Affix.Json
{
    public class Json
    {

    }
    public class Shards
    {
        [JsonProperty("DbShards")]
        public List<DbShard> List { get; set; }
    }
    public class DbShard
    {
        [JsonProperty("modifier")]
        public string Modifier { get; set; }

        [JsonProperty("property")]
        public string Property { get; set; }

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

        [JsonProperty("can_roll_on")]
        public string CanRollOn { get; set; }

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
        public List<Tier> Tiers { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
    public class Tier
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("range")]
        public string Range { get; set; }
    }    
}

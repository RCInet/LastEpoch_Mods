using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastEpochMods.Db.Json
{
    public class Items
    {
        [JsonProperty("basic")]
        public List<Basic> Basic { get; set; }

        [JsonProperty("unique")]
        public List<Unique> Unique { get; set; }

        [JsonProperty("set")]
        public List<Set> Set { get; set; }
    }
    public class Basic
    {
        [JsonProperty("base_id")]
        public int BaseId { get; set; }

        [JsonProperty("base_name")]
        public string BaseName { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("implicit")]
        public List<string> Implicit { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }
    }
    public class Unique
    {
        [JsonProperty("base_id")]
        public int BaseId { get; set; }

        [JsonProperty("base_name")]
        public string BaseName { get; set; }

        [JsonProperty("implicit")]
        public List<string> Implicit { get; set; }

        [JsonProperty("unique_id")]
        public int UniqueId { get; set; }

        [JsonProperty("unique_name")]
        public string UniqueName { get; set; }

        [JsonProperty("unique_affix")]
        public List<string> Unique_Affixs { get; set; }

        [JsonProperty("lore_text")]
        public string LoreText { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }
    }
    public class Set
    {
        [JsonProperty("base_id")]
        public int BaseId { get; set; }

        [JsonProperty("base_name")]
        public string BaseName { get; set; }

        [JsonProperty("implicit")]
        public List<string> Implicit { get; set; }

        [JsonProperty("set_id")]
        public int SetId { get; set; }

        [JsonProperty("lore_text")]
        public string LoreText { get; set; }

        [JsonProperty("set_name")]
        public string SetName { get; set; }

        [JsonProperty("unique_affix")]
        public List<string> Unique_Affixs { get; set; }

        [JsonProperty("set_refs")]
        public List<Set_Ref> Set_Refs { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }
    }
    public class Set_Ref
    {
        [JsonProperty("ref_requirement")]
        public int Ref_Requirement { get; set; }

        [JsonProperty("ref_text")]
        public string Ref_Text { get; set; }
    }
}

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Database.Skills.Json
{
    public class Json
    {
        public static string Path = @"\Database\Skills\Skills.json";
    }
    public class Root
    {
        [JsonProperty("All")]
        public All All { get; set; }

        [JsonProperty("Primalist")]
        public Primalist Primalist { get; set; }

        [JsonProperty("Mage")]
        public Mage Mage { get; set; }

        [JsonProperty("Sentinel")]
        public Sentinel Sentinel { get; set; }

        [JsonProperty("Acolyte")]
        public Acolyte Acolyte { get; set; }

        [JsonProperty("Rogue")]
        public Rogue Rogue { get; set; }
    }
    public class Base
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
    public class All
    {
        [JsonProperty("Path")]
        public string Path { get; set; }

        [JsonProperty("Base")]
        public List<Base> Base { get; set; }
    }
    public class Acolyte
    {
        [JsonProperty("Path")]
        public string Path { get; set; }

        [JsonProperty("Base")]
        public List<Base> Base { get; set; }

        [JsonProperty("Necromancer")]
        public List<Base> Necromancer { get; set; }

        [JsonProperty("Lich")]
        public List<Base> Lich { get; set; }
    }
    public class Mage
    {
        [JsonProperty("Path")]
        public string Path { get; set; }

        [JsonProperty("Base")]
        public List<Base> Base { get; set; }

        [JsonProperty("Sorcerer")]
        public List<Base> Sorcerer { get; set; }

        [JsonProperty("SpellBlade")]
        public List<Base> SpellBlade { get; set; }
    }
    public class Primalist
    {
        [JsonProperty("Path")]
        public string Path { get; set; }

        [JsonProperty("Base")]
        public List<Base> Base { get; set; }

        [JsonProperty("Druid")]
        public List<Base> Druid { get; set; }

        [JsonProperty("Shaman")]
        public List<Base> Shaman { get; set; }

        [JsonProperty("BeastMaster")]
        public List<Base> BeastMaster { get; set; }
    }
    public class Rogue
    {
        [JsonProperty("Path")]
        public string Path { get; set; }

        [JsonProperty("Base")]
        public List<Base> Base { get; set; }

        [JsonProperty("Marksman")]
        public List<Base> Marksman { get; set; }

        [JsonProperty("BladeDancer")]
        public List<Base> BladeDancer { get; set; }
    }
    public class Sentinel
    {
        [JsonProperty("Path")]
        public string Path { get; set; }

        [JsonProperty("Base")]
        public List<Base> Base { get; set; }

        [JsonProperty("ForgeGuard")]
        public List<Base> ForgeGuard { get; set; }

        [JsonProperty("VoidKnight")]
        public List<Base> VoidKnight { get; set; }

        [JsonProperty("Paladin")]
        public List<Base> Paladin { get; set; }
    }
}

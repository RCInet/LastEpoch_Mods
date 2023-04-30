using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Materials.Runes.Json
{
    public class Json
    {

    }
    public class Runes
    {
        [JsonProperty("rune")]
        public List<Rune> List { get; set; }
    }
    public class Rune
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}

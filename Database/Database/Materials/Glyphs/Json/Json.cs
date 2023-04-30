using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Materials.Glyphs.Json
{
    public class Json
    {
        
    }
    public class Glyphs
    {
        [JsonProperty("glyph")]
        public List<Glyph> List { get; set; }
    }
    public class Glyph
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}

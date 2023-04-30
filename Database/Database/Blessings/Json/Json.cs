using Newtonsoft.Json;
using System.Collections.Generic;

namespace Database.Blessings.Json
{
    public class Json
    {

    }
    public class Blessings
    {
        [JsonProperty("blessing")]
        public List<Blessing> List { get; set; }
    }
    public class Blessing
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("implicit")]
        public string Implicit { get; set; }

        [JsonProperty("timeline")]
        public int Timeline { get; set; }

        [JsonProperty("difficulty")]
        public int Difficulty { get; set; }
    }
}

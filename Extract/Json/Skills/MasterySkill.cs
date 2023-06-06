using Newtonsoft.Json;
using System.Drawing;

namespace Extract.Json.Skills
{
    public class MasterySkill
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Icon")]
        public Icon Icon { get; set; }
    }
}

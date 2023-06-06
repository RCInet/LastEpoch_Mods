using Newtonsoft.Json;

namespace Extract.Json.Skills
{
    public class DefaultSkill
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Icon")]
        public Icon Icon { get; set; }
    }
}

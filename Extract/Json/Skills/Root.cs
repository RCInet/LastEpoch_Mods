using Newtonsoft.Json;
using System.Collections.Generic;

namespace Extract.Json.Skills
{
    public class Root
    {
        [JsonProperty("ClassName")]
        public string ClassName { get; set; }

        [JsonProperty("DefaultSkills")]
        public List<DefaultSkill> DefaultSkills { get; set; }

        [JsonProperty("Masteries")]
        public List<Mastery> Masteries { get; set; }
    }
}

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Extract.Json.Skills
{
    public class Mastery
    {
        [JsonProperty("MasteryName")]
        public string MasteryName { get; set; }

        [JsonProperty("MasterySkills")]
        public List<MasterySkill> MasterySkills { get; set; }
    }
}

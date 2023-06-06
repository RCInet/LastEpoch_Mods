using Newtonsoft.Json;

namespace Extract.Json.Skills
{
    public class Icon
    {
        [JsonProperty("PositionX")]
        public int PositionX { get; set; }

        [JsonProperty("PositionY")]
        public int PositionY { get; set; }

        [JsonProperty("SizeX")]
        public int SizeX { get; set; }

        [JsonProperty("SizeY")]
        public int SizeY { get; set; }
    }
}

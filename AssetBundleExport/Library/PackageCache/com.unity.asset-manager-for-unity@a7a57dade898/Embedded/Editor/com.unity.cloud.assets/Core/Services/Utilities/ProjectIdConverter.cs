using System;
using Newtonsoft.Json;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class ProjectIdConverter : JsonConverter<ProjectId>
    {
        public override void WriteJson(JsonWriter writer, ProjectId value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override ProjectId ReadJson(JsonReader reader, Type objectType, ProjectId existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.TokenType == JsonToken.String ? new ProjectId(reader.Value?.ToString()) : existingValue;
        }
    }
}

using System;
using Newtonsoft.Json;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class OrganizationIdConverter : JsonConverter<OrganizationId>
    {
        public override void WriteJson(JsonWriter writer, OrganizationId value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override OrganizationId ReadJson(JsonReader reader, Type objectType, OrganizationId existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.TokenType == JsonToken.String ? new OrganizationId(reader.Value?.ToString()) : existingValue;
        }
    }
}

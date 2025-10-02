using System;
using Newtonsoft.Json;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class TransformationIdConverter : JsonConverter<TransformationId>
    {
        public override void WriteJson(JsonWriter writer, TransformationId value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override TransformationId ReadJson(JsonReader reader, Type objectType, TransformationId existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.TokenType == JsonToken.String ? new TransformationId(reader.Value?.ToString()) : existingValue;
        }
    }
}

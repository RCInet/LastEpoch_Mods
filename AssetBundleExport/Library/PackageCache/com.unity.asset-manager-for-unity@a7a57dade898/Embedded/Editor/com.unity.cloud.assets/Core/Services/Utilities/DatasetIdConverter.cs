using System;
using Newtonsoft.Json;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class DatasetIdConverter : JsonConverter<DatasetId>
    {
        public override void WriteJson(JsonWriter writer, DatasetId value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override DatasetId ReadJson(JsonReader reader, Type objectType, DatasetId existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.TokenType == JsonToken.String ? new DatasetId(reader.Value?.ToString()) : existingValue;
        }
    }
}

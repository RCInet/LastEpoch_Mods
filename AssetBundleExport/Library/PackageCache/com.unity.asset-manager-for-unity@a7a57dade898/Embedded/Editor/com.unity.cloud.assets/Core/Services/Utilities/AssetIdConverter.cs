using System;
using Newtonsoft.Json;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class AssetIdConverter : JsonConverter<AssetId>
    {
        public override void WriteJson(JsonWriter writer, AssetId value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override AssetId ReadJson(JsonReader reader, Type objectType, AssetId existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.TokenType == JsonToken.String ? new AssetId(reader.Value?.ToString()) : existingValue;
        }
    }
}

using System;
using Newtonsoft.Json;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class AssetVersionConverter : JsonConverter<AssetVersion>
    {
        public override void WriteJson(JsonWriter writer, AssetVersion value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override AssetVersion ReadJson(JsonReader reader, Type objectType, AssetVersion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.TokenType is JsonToken.String or JsonToken.Integer ? new AssetVersion(reader.Value?.ToString()) : existingValue;
        }
    }
}

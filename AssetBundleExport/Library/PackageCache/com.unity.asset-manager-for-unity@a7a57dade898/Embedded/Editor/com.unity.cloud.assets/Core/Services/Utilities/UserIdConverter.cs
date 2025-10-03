using System;
using Newtonsoft.Json;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class UserIdConverter : JsonConverter<UserId>
    {
        public override void WriteJson(JsonWriter writer, UserId value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override UserId ReadJson(JsonReader reader, Type objectType, UserId existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.TokenType is JsonToken.String or JsonToken.Integer ? new UserId(reader.Value?.ToString()) : existingValue;
        }
    }
}

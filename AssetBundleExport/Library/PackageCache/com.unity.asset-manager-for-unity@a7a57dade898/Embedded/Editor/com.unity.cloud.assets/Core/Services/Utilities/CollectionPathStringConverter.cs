using System;
using Newtonsoft.Json;

namespace Unity.Cloud.AssetsEmbedded
{
    class CollectionPathStringConverter : JsonConverter<CollectionPath>
    {
        public override void WriteJson(JsonWriter writer, CollectionPath value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override CollectionPath ReadJson(JsonReader reader, Type objectType, CollectionPath existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.TokenType == JsonToken.String ? new CollectionPath(reader.Value?.ToString()) : existingValue;
        }
    }
}

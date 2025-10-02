using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

#nullable enable
namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// A class to serialize and deserialize objects to JSON.
    /// </summary>
    static class JsonSerialization
    {
        /// <summary>
        /// Deserialize a JSON string to a specified type.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <returns>The deserialized type if successful, or null if unsuccessful.</returns>
        public static T? Deserialize<T>(string json)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings()
                {
                    Error = delegate(object sender, ErrorEventArgs args)
                    {
                        args.ErrorContext.Handled = true;
                    }
                });

                return result;
            }
            catch (Exception e)
            {
                // Trapping everything since we're returning default(T) if we cannot deserialize, but we are still logging the exception
                var logger = LoggerProvider.GetLogger(typeof(JsonSerialization).FullName);
                logger.LogError(e);
            }

            return default;
        }

        /// <summary>
        /// Serialize a JSON string from a specified type.
        /// </summary>
        /// <typeparam name="T">The type to serialize.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <returns>The serialized JSON string.</returns>
        public static string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value, new JsonSerializerSettings());
        }
    }
}

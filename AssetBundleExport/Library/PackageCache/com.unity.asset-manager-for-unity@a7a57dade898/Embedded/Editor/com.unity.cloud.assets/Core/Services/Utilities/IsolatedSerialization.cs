using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    static class IsolatedSerialization
    {
        public static readonly JsonSerializerSettings defaultSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Populate,
            ObjectCreationHandling = ObjectCreationHandling.Auto,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            TypeNameHandling = TypeNameHandling.None,
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            Formatting = Formatting.None
        };

        public static JsonConverter CollectionPathConverter => new CollectionPathStringConverter();
        public static JsonConverter DatasetIdConverter => new DatasetIdConverter();
        public static JsonConverter TransformationIdConverter => new TransformationIdConverter();
        public static JsonConverter StringEnumConverter => new StringEnumConverter();

        static readonly JsonConverter[] Converters =
        {
            new OrganizationIdConverter(),
            new ProjectIdConverter(),
            new AssetIdConverter(),
            new AssetVersionConverter(),
            new UserIdConverter(),
            DatasetIdConverter,
            TransformationIdConverter,
            CollectionPathConverter,
        };

        /// <summary>
        /// Deserialize a JSON string to a specified type.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <returns>The deserialized type if successful, or null if unsuccessful.</returns>
        public static T DeserializeWithDefaultConverters<T>(string json)
        {
            return DeserializeWithConverters<T>(json, Converters);
        }

        /// <summary>
        /// Deserialize a JSON string to a specified type.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <param name="converters">Custom converters to use during deserialization.</param>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <returns>The deserialized type if successful, or null if unsuccessful.</returns>
        public static T DeserializeWithConverters<T>(string json, params JsonConverter[] converters)
        {
            var settings = defaultSettings.Clone(converters);
            settings.Error = delegate(object sender, ErrorEventArgs args)
            {
                args.ErrorContext.Handled = true;
            };
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        /// <summary>
        /// Deserialize a JSON string to a specified type.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <param name="settings">Custom settings to use during deserialization.</param>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <returns>The deserialized type if successful, or null if unsuccessful.</returns>
        public static T Deserialize<T>(string json, JsonSerializerSettings settings)
        {
            var settingsCopy = settings.Clone();
            settings.Error = delegate(object sender, ErrorEventArgs args)
            {
                args.ErrorContext.Handled = true;
            };
            return JsonConvert.DeserializeObject<T>(json, settingsCopy);
        }

        /// <summary>
        /// Serialize a JSON string from a specified type.
        /// </summary>
        /// <typeparam name="T">The type to serialize.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <returns>The serialized JSON string.</returns>
        public static string SerializeWithDefaultConverters<T>(T value)
        {
            return SerializeWithConverters(value, Converters);
        }

        /// <summary>
        /// Serialize a JSON string from a specified type.
        /// </summary>
        /// <typeparam name="T">The type to serialize.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <param name="converters">Custom converters to use during serialization.</param>
        /// <returns>The serialized JSON string.</returns>
        public static string SerializeWithConverters<T>(T value, params JsonConverter[] converters)
        {
            var settings = defaultSettings.Clone(converters);
            return Serialize(value, settings);
        }

        /// <summary>
        /// Serialize a JSON string from a specified type.
        /// </summary>
        /// <typeparam name="T">The type to serialize.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <param name="settings">Custom settings to use during serialization.</param>
        /// <returns>The serialized JSON string.</returns>
        public static string Serialize<T>(T value, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(value, settings);
        }

        static JsonSerializerSettings Clone(this JsonSerializerSettings settingsSource, params JsonConverter[] converters)
        {
            var mergedConverters = settingsSource.Converters.ToList();
            if (converters != null)
                mergedConverters.AddRange(converters);
            mergedConverters.Add(StringEnumConverter);

#if UC_NUGET
            var cloneSettings = new JsonSerializerSettings
            {
                FloatParseHandling = settingsSource.FloatParseHandling,
                FloatFormatHandling = settingsSource.FloatFormatHandling,
                DateParseHandling = settingsSource.DateParseHandling,
                DateTimeZoneHandling = settingsSource.DateTimeZoneHandling,
                DateFormatHandling = settingsSource.DateFormatHandling,
                Formatting = settingsSource.Formatting,
                MaxDepth = settingsSource.MaxDepth,
                DateFormatString = settingsSource.DateFormatString,
                Context = settingsSource.Context,
                Error = settingsSource.Error,
                SerializationBinder = settingsSource.SerializationBinder,
                TraceWriter = settingsSource.TraceWriter,
                Culture = settingsSource.Culture,
                ReferenceResolverProvider = settingsSource.ReferenceResolverProvider,
                EqualityComparer = settingsSource.EqualityComparer,
                ContractResolver = settingsSource.ContractResolver,
                ConstructorHandling = settingsSource.ConstructorHandling,
                TypeNameAssemblyFormatHandling = settingsSource.TypeNameAssemblyFormatHandling,
                MetadataPropertyHandling = settingsSource.MetadataPropertyHandling,
                TypeNameHandling = settingsSource.TypeNameHandling,
                PreserveReferencesHandling = settingsSource.PreserveReferencesHandling,
                Converters = mergedConverters,
                DefaultValueHandling = settingsSource.DefaultValueHandling,
                NullValueHandling = settingsSource.NullValueHandling,
                ObjectCreationHandling = settingsSource.ObjectCreationHandling,
                MissingMemberHandling = settingsSource.MissingMemberHandling,
                ReferenceLoopHandling = settingsSource.ReferenceLoopHandling,
                CheckAdditionalContent = settingsSource.CheckAdditionalContent,
                StringEscapeHandling = settingsSource.StringEscapeHandling
            };
#else
            var cloneSettings = new JsonSerializerSettings(settingsSource)
            {
                Converters = mergedConverters
            };
#endif
            return cloneSettings;
        }

        internal static Dictionary<string, object> ToObjectDictionary(object jsonObject)
        {
            if (jsonObject is not JObject jObject)
                return new Dictionary<string, object>();

            var properties = new Dictionary<string, object>();

            foreach (var property in jObject.Properties())
            {
                properties[property.Name] = ToObject(property.Value);
            }

            return properties;
        }

        static object[] ToObjectArray(object jToken)
        {
            if (jToken is not JArray jArray)
                return Array.Empty<object>();

            var values = new object[jArray.Count];

            for (var i = 0; i < values.Length; ++i)
            {
                values[i] = ToObject(jArray[i]);
            }

            return values;
        }

        internal static object ToObject(object jToken)
        {
            if (jToken is not JToken)
                return jToken;

            return jToken switch
            {
                JObject => ToObjectDictionary(jToken),
                JValue jValue => jValue.Value,
                JArray => ToObjectArray(jToken),
                _ => throw new InvalidOperationException($"Cannot convert value for {jToken.GetType()}")
            };
        }
    }
}

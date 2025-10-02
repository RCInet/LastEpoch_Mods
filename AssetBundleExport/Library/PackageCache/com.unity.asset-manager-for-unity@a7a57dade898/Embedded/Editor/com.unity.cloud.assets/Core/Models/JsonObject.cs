using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// JsonObject class for encapsulating generic object types. We use this to
    /// hide internal Json implementation details.
    /// </summary>
    class JsonObject : IDeserializable
    {
        /// <summary>
        /// Constructor sets object as the internal object.
        /// </summary>
        internal JsonObject(object obj)
        {
            this.obj = obj;
        }

        /// <summary>The encapsulated object.</summary>
        internal object obj;

        public override string ToString()
        {
            return obj.ToString();
        }

        /// <summary>
        /// Returns the internal object as a string.
        /// </summary>
        /// <returns>The internal object as a string.</returns>
        public string GetAsString()
        {
            try
            {
                return obj switch
                {
                    null => "",
                    string => obj.ToString(),
                    _ => IsolatedSerialization.SerializeWithDefaultConverters(obj)
                };
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Failed to convert object to string.", e);
            }
        }

        /// <summary>
        /// Returns the object as a defined type.
        /// Previously this function restricted use of `object` or `dynamic`
        /// types but validation for these has been removed. As such, be
        /// careful when passing or exposing objects of these types.
        /// </summary>
        /// <typeparam name="T">The type to cast internal object to.</typeparam>
        /// <returns>The internal object cast to type T.</returns>
        public T GetAs<T>()
        {
            try
            {
                var returnObject = IsolatedSerialization.DeserializeWithDefaultConverters<T>(GetAsString());
                return returnObject;
            }
            catch (Exception e)
            {
                throw new InvalidCastException($"Unable to deserialize object as {typeof(T)}.", e);
            }
        }

        /// <summary>
        /// Convert object to jsonobject.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <returns>The jsonobject.</returns>
        public static IDeserializable GetNewJsonObjectResponse(object o)
        {
            return new JsonObject(o);
        }

        /// <summary>
        /// Convert list of object to list of jsonobject.
        /// </summary>
        /// <param name="o">The list of objects.</param>
        /// <returns>The list of jsonobjects.</returns>
        public static List<IDeserializable> GetNewJsonObjectResponse(IEnumerable<object> o)
        {
            return o?.Select(v => (IDeserializable) new JsonObject(v)).ToList();
        }

        /// <summary>
        /// Convert list of list of object to list of list of jsonobject.
        /// </summary>
        /// <param name="o">The list of list of objects.</param>
        /// <returns>The list of list of jsonobjects.</returns>
        public static List<List<IDeserializable>> GetNewJsonObjectResponse(IEnumerable<List<object>> o)
        {
            return o?.Select(l => l.Select(v => v == null ? null : (IDeserializable) new JsonObject(v)).ToList()).ToList();
        }

        /// <summary>
        /// Convert dictionary of string, object to dictionary of string, jsonobject.
        /// </summary>
        /// <param name="o">The dictionary of string, objects.</param>
        /// <returns>The dictionary of string, jsonobjects.</returns>
        public static Dictionary<string, IDeserializable> GetNewJsonObjectResponse(Dictionary<string, object> o)
        {
            return o?.ToDictionary(kv => kv.Key, kv => (IDeserializable) new JsonObject(kv.Value));
        }

        /// <summary>
        /// Convert dictionary of string, list of object to dictionary of string, list of jsonobject.
        /// </summary>
        /// <param name="o">The dictionary of string to list of objects.</param>
        /// <returns>The dictionary of string, list of jsonobjects.</returns>
        public static Dictionary<string, List<IDeserializable>> GetNewJsonObjectResponse(Dictionary<string, List<object>> o)
        {
            return o?.ToDictionary(kv => kv.Key, kv => GetNewJsonObjectResponse(kv.Value));
        }
    }
}

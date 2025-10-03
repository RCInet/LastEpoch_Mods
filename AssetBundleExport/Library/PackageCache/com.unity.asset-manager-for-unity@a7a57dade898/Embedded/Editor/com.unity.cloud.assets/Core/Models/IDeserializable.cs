using System;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// IDeserializable is an interface for wrapping generic objects that might
    /// be returned as part of HTTP requests.
    /// </summary>
    interface IDeserializable
    {
        /// <summary>
        /// Returns the internal object as a string.
        /// </summary>
        /// <returns>The internal object as a string.</returns>
        string GetAsString();

        /// <summary>
        /// Gets this object as the given type.
        /// </summary>
        /// <typeparam name="T">The type you want to convert this object to.</typeparam>
        /// <returns>This object as the given type.</returns>
        T GetAs<T>();
    }

}

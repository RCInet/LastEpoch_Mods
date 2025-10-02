using UnityEngine;

namespace Unity.Cloud.CommonEmbedded.Runtime
{
    /// <summary>
    /// A static factory that handles instantiation of a platform-specific <see cref="IClipboard"/>.
    /// </summary>
    static class ClipboardFactory
    {
        /// <summary>
        /// Instantiates and returns a <see cref="IClipboard"/> for the current platform.
        /// </summary>
        /// <returns>An <see cref="IClipboard"/> implementation for the current platform.</returns>
        public static IClipboard Create()
        {
#if UNITY_WEBGL
            return new BrowserClipboard();
#else
            return new UnityClipboard();
#endif
        }
    }
}

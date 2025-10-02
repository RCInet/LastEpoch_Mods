using System;
using UnityEngine;

namespace Unity.Cloud.CommonEmbedded.Runtime
{
    /// <inheritdoc/>
    /// <remarks>
    /// This clipboard implementation is specifically intended for use in WebGL.
    /// </remarks>
    class BrowserClipboard : IClipboard
    {
        /// <summary>
        /// Creates a BrowserClipboard instance.
        /// </summary>
        /// <exception cref="PlatformNotSupportedException">Thrown if used outside a Unity WebGL execution context.</exception>
        internal BrowserClipboard()
        {
#if !UNITY_WEBGL
            throw new PlatformNotSupportedException();
#endif
        }

        /// <inheritdoc/>
        public bool CopyText(string textContent)
        {
            return CommonBrowserInterop.CopyToClipboard(textContent);
        }

    }
}

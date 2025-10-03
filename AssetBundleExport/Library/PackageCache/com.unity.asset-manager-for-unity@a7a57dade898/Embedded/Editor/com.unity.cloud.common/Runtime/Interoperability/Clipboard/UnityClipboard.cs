using System;
using UnityEngine;

namespace Unity.Cloud.CommonEmbedded.Runtime
{
    /// <inheritdoc/>
    class UnityClipboard : IClipboard
    {
        /// <summary>
        /// Creates a UnityClipboard instance.
        /// </summary>
        /// <exception cref="PlatformNotSupportedException">Thrown if used in a Unity WebGL execution context.</exception>
        internal UnityClipboard()
        {
#if UNITY_WEBGL
            throw new PlatformNotSupportedException();
#endif
        }

        /// <inheritdoc/>
        public bool CopyText(string textContent)
        {
            GUIUtility.systemCopyBuffer = textContent;
            return true;
        }
    }
}

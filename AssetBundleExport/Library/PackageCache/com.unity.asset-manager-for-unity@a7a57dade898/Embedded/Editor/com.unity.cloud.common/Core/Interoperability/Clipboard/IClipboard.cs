using System;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// An interface for clipboard features.
    /// </summary>
    interface IClipboard
    {
        /// <summary>
        /// Copy a string value to the operating system clipboard.
        /// </summary>
        /// <param name="textContent">The text to copy.</param>
        /// <returns>True if operation was allowed, false otherwise.</returns>
        bool CopyText(string textContent);
    }
}

using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// File properties for updating.
    /// </summary>
    interface IFileUpdate
    {
        /// <inheritdoc cref="IFile.Description"/>
        string Description { get; }

        /// <inheritdoc cref="IFile.Tags"/>
        IEnumerable<string> Tags { get; }
    }
}

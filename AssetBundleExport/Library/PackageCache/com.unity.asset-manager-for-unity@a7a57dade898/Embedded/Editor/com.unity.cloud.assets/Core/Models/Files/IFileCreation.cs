using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// File properties for creation.
    /// </summary>
    interface IFileCreation
    {
        /// <summary>
        /// The path to the file.
        /// </summary>
        string Path { get; }

        /// <inheritdoc cref="IFile.Description"/>
        string Description { get; }

        /// <inheritdoc cref="IFile.Tags"/>
        IEnumerable<string> Tags { get; }

        /// <inheritdoc cref="IFile.Metadata"/>
        Dictionary<string, MetadataValue> Metadata { get; }

        /// <summary>
        /// Whether to disable automatic transformations for the new file.
        /// If true, automatic transformations, such as preview generation and metadata extraction, will be disabled.
        /// </summary>
        bool DisableAutomaticTransformations => false;
    }
}

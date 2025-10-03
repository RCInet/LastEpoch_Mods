using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    class FileCreation : IFileCreation
    {
        /// <inheritdoc/>
        public string Path { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public IEnumerable<string> Tags { get; set; }

        /// <inheritdoc/>
        public Dictionary<string, MetadataValue> Metadata { get; set; }

        /// <inheritdoc/>
        public bool DisableAutomaticTransformations { get; set; }

        [Obsolete("Use FileCreation(string path) instead.")]
        public FileCreation() { }

        public FileCreation(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path), "The path of the file cannot be null or empty.");
            }

            Path = path;
        }
    }
}

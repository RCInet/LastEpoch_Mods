using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    class FileUpdate : IFileUpdate
    {
        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public IEnumerable<string> Tags { get; set; }

        public FileUpdate() { }

        [Obsolete("Use the default constructor instead.")]
        public FileUpdate(IFile file)
        {
            Description = file.Description;
            Tags = file.Tags;
        }
    }
}

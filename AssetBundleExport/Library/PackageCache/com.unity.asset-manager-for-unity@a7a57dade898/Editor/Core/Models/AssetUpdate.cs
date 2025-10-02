using System.Collections.Generic;

namespace Unity.AssetManager.Core.Editor
{
    /// <summary>
    /// Class that represent an asset update operation
    /// </summary>
    class AssetUpdate
    {
        public string Name { get; set; }
        public AssetType? Type { get; set; }
        public string PreviewFile { get; set; }

        public List<string> Tags { get; set; }
        public List<IMetadata> Metadata { get; set; }
    }
}

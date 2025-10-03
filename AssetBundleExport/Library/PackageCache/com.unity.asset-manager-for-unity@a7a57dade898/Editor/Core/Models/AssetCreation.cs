using System.Collections.Generic;

namespace Unity.AssetManager.Core.Editor
{
    /// <summary>
    /// Class that holds the information needed when creating a new asset
    /// </summary>
    class AssetCreation
    {
        public string Name { get; set; }
        public AssetType Type { get; set; }
        public List<string> Collections { get; set; } = new();
        public List<string> Tags { get; set; } = new();
        public List<IMetadata> Metadata { get; set; } = new();
    }
}

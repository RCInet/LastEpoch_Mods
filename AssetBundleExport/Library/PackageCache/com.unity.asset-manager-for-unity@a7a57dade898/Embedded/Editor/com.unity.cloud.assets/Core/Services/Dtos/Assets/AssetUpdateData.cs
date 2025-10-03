using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// This object contains all the information about an updated asset.
    /// </summary>
    [DataContract]
    class AssetUpdateData : AssetBaseData, IAssetUpdateData
    {
        /// <inheritdoc />
        public string PreviewFile { get; set; }
    }
}

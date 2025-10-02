using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// This object contains all the information about an updated asset.
    /// </summary>
    interface IAssetUpdateData : IAssetBaseData
    {
        /// <summary>
        /// The preview file path of the asset.
        /// </summary>
        [DataMember(Name = "previewFilePath")]
        string PreviewFile { get; }
    }
}

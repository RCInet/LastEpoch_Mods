using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// This object contains all the information about an updated asset.
    /// </summary>
    interface IAssetBaseData : IMetadataInfo
    {
        /// <summary>
        /// The name of the asset.
        /// </summary>
        [DataMember(Name = "name")]
        string Name { get; }

        /// <summary>
        /// The description of the asset.
        /// </summary>
        [DataMember(Name = "description")]
        string Description
        {
            get => default;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The tags of the asset.
        /// </summary>
        [DataMember(Name = "tags")]
        IEnumerable<string> Tags { get; }

        /// <summary>
        /// The type of the asset.
        /// </summary>
        [DataMember(Name = "primaryType")]
        string Type { get; }
    }
}

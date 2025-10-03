using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// This interface contains all the information about a cloud asset.
    /// </summary>
    interface IAssetCreateData : IAssetBaseData
    {
        /// <summary>
        /// The collections the asset belongs to
        /// </summary>
        [DataMember(Name = "collections")]
        IEnumerable<CollectionPath> Collections { get; set; }

        [DataMember(Name = "statusFlowId")]
        string StatusFlowId { get; set; }
    }
}

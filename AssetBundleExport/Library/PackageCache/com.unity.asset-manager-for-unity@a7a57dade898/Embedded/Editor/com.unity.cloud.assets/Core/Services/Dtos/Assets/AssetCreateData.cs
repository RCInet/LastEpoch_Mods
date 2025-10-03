using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class AssetCreateData : AssetBaseData, IAssetCreateData
    {
        /// <inheritdoc />
        public IEnumerable<CollectionPath> Collections { get; set; }

        /// <inheritdoc />
        public string StatusFlowId { get; set; }
    }
}

using System;
using System.Runtime.Serialization;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class AssetIdentifier : IAssetIdentifier
    {
        /// <inheritdoc />
        public OrganizationId OrganizationId { get; set; }

        /// <inheritdoc />
        public ProjectId ProjectId { get; set; }

        /// <inheritdoc />
        public AssetId Id { get; set; }

        /// <inheritdoc />
        public AssetVersion Version { get; set; }
    }
}

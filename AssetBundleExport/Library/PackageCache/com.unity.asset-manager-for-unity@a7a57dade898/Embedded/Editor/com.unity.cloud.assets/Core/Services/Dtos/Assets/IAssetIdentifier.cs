using System.Runtime.Serialization;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    interface IAssetIdentifier
    {
        /// <summary>
        /// The id of the organization.
        /// </summary>
        [DataMember(Name = "organizationId")]
        OrganizationId OrganizationId { get; }

        /// <summary>
        /// The id of the project.
        /// </summary>
        [DataMember(Name = "projectId")]
        ProjectId ProjectId { get; }

        /// <summary>
        /// The id of the asset.
        /// </summary>
        [DataMember(Name = "assetId")]
        AssetId Id { get; }

        /// <summary>
        /// The version of the asset.
        /// </summary>
        [DataMember(Name = "assetVersion")]
        AssetVersion Version { get; }
    }
}

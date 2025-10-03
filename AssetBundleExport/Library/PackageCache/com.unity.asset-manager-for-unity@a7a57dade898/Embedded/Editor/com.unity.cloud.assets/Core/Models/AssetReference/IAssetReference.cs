using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// An interface containing the information about the reference between two assets.
    /// </summary>
    interface IAssetReference
    {
        /// <summary>
        /// The project descriptor.
        /// </summary>
        ProjectDescriptor ProjectDescriptor { get; }

        /// <summary>
        /// The id of the reference.
        /// </summary>
        string ReferenceId { get; }

        /// <summary>
        /// Whether the reference is valid.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// The id of the source asset.
        /// </summary>
        AssetId SourceAssetId { get; }

        /// <summary>
        /// The version of the source asset.
        /// </summary>
        AssetVersion SourceAssetVersion { get; }

        /// <summary>
        /// The id of the asset that is being targeted.
        /// </summary>
        AssetId TargetAssetId { get; }

        /// <summary>
        /// [Optional] The version of the asset that is being targeted.
        /// </summary>
        /// <remarks>A reference target must point to a version; the version can be explicit as defined by <see cref="TargetAssetVersion"/> or by label as defined by <see cref="TargetLabel"/>. </remarks>
        AssetVersion? TargetAssetVersion { get; }

        /// <summary>
        /// [Optional] The label of the version of the asset that is being targeted.
        /// </summary>
        /// <remarks>A reference target must point to a version; the version can be explicit as defined by <see cref="TargetAssetVersion"/> or by label as defined by <see cref="TargetLabel"/>. </remarks>
        string TargetLabel { get; }
    }
}

using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class AssetReference : IAssetReference
    {
        /// <inheritdoc />
        public ProjectDescriptor ProjectDescriptor { get; }

        /// <inheritdoc />
        public string ReferenceId { get; }

        /// <inheritdoc />
        public bool IsValid { get; set; }

        /// <inheritdoc />
        public AssetId SourceAssetId { get; set; }

        /// <inheritdoc />
        public AssetVersion SourceAssetVersion { get; set; }

        /// <inheritdoc />
        public AssetId TargetAssetId { get; set; }

        /// <inheritdoc />
        public AssetVersion? TargetAssetVersion { get; set; }

        /// <inheritdoc />
        public string TargetLabel { get; set; }

        internal AssetReference(ProjectDescriptor projectDescriptor, string referenceId)
        {
            ProjectDescriptor = projectDescriptor;
            ReferenceId = referenceId;
        }
    }
}

using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Represents a get asset collections request.
    /// </summary>
    class GetAssetCollectionsRequest : ProjectRequest
    {
        /// <summary>
        /// Get Asset Collections Request Object.
        /// Get the collections of an Asset.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        public GetAssetCollectionsRequest(ProjectId projectId, AssetId assetId)
            : base(projectId)
        {
            m_RequestUrl += $"/assets/{assetId}/collections";
        }
    }
}

using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Represents a check project is asset source project request.
    /// </summary>
    class CheckAssetBelongsToProjectRequest : ProjectRequest
    {
        /// <summary>
        /// Checks whether a project is an asset source project Request Object.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        public CheckAssetBelongsToProjectRequest(ProjectId projectId, AssetId assetId)
            : base(projectId)
        {
            m_RequestUrl += $"/assets/{assetId}/check";
        }
    }
}

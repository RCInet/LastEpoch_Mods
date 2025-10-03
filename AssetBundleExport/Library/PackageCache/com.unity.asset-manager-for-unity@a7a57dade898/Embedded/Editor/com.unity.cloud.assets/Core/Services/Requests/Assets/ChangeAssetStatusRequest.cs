using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Represents a change asset's status request.
    /// </summary>
    class ChangeAssetStatusRequest : AssetRequest
    {
        /// <summary>
        /// Changes the asset's status Request Object.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="assetVersion">The version of the asset the file is linked to.</param>
        /// <param name="status">The new status. </param>
        public ChangeAssetStatusRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, string status)
            : base(projectId, assetId, assetVersion)
        {
            m_RequestUrl += $"/status/{status}";
        }
    }
}

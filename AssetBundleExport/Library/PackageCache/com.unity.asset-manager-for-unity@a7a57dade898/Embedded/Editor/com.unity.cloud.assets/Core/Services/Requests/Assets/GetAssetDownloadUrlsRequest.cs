using System.Linq;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Represents a get asset download urls request.
    /// </summary>
    class GetAssetDownloadUrlsRequest : AssetRequest
    {
        /// <summary>
        /// Get Asset Download Urls Request Object.
        /// Get a list of url for an Asset.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="assetVersion">The version of the asset the file is linked to.</param>
        /// <param name="datasetIds">An optional collection of datasets with which to limit the search.</param>
        /// <param name="maxDimension">The desired length to resize the larger image dimension to, while maintaining the same aspect ratio. </param>
        public GetAssetDownloadUrlsRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId[] datasetIds, int? maxDimension)
            : base(projectId, assetId, assetVersion)
        {
            m_RequestUrl += $"/download-urls";

            if (datasetIds != null)
                AddParamToQuery("datasets", datasetIds.Select(x => x.ToString()));
            if (maxDimension.HasValue)
                AddParamToQuery("maxDimension", maxDimension.Value.ToString());
        }
    }
}

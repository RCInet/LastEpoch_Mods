using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Represents a get asset file URL request.
    /// </summary>
    class GetFileDownloadUrlRequest : FileRequest
    {
        /// <summary>
        /// Gets an Asset File URL Request Object.
        /// Gets a single asset file URL.
        /// </summary>
        /// <param name="projectId">The id of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="assetVersion">The version of the asset the file is linked to.</param>
        /// <param name="datasetId">The id of the dataset.</param>
        /// <param name="filePath">The path of the file.</param>
        /// <param name="maxDimension">The desired length to resize the larger image dimension to, while maintaining the same aspect ratio. </param>
        public GetFileDownloadUrlRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, string filePath, int? maxDimension)
            : base(projectId, assetId, assetVersion, datasetId, filePath)
        {
            m_RequestUrl += $"/download-url";

            if (maxDimension.HasValue)
                AddParamToQuery("maxDimension", maxDimension.Value.ToString());
        }
    }
}

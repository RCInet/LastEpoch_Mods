using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Represents a get asset file URL request.
    /// </summary>
    class GetFileUploadUrlRequest : FileRequest
    {
        /// <summary>
        /// Gets an Asset File URL Request Object.
        /// Gets a single asset file URL.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="assetVersion">The version of the asset the file is linked to.</param>
        /// <param name="datasetId"></param>
        /// <param name="filePath">The asset file id url to get.</param>
        /// <param name="fileData"></param>
        public GetFileUploadUrlRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, string filePath, IFileData fileData)
            : base(projectId, assetId, assetVersion, datasetId, filePath)
        {
            m_RequestUrl += $"/upload-url";

            if (fileData != null)
            {
                AddParamToQuery("userChecksum", fileData.UserChecksum);
                AddParamToQuery("fileSize", fileData.SizeBytes.ToString());
            }
        }
    }
}

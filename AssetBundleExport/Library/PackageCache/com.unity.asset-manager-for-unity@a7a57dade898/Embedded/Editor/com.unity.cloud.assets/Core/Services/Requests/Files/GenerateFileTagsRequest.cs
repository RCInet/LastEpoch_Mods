using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class GenerateFileTagsRequest : FileRequest
    {
        /// <summary>
        /// Creates an Asset File Request Object.
        /// Creates a single asset file.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file will linked to.</param>
        /// <param name="assetVersion">The version of the asset the file will linked to.</param>
        /// <param name="datasetId"></param>
        /// <param name="filePath">The path to the file for which the upload will be finalized.</param>
        public GenerateFileTagsRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, string filePath)
            : base(projectId, assetId, assetVersion, datasetId, filePath)
        {
            m_RequestUrl += $"/auto-tags";
        }
    }
}

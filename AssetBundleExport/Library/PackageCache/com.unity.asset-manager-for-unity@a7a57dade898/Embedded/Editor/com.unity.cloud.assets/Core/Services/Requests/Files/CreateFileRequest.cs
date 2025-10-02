using System.Net.Http;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Represents a create asset file request.
    /// </summary>
    class CreateFileRequest : DatasetRequest
    {
        /// <summary>
        /// The asset file to create.
        /// </summary>
        IFileBaseData FileData { get; }

        /// <summary>
        /// Creates an Asset File Request Object.
        /// Creates a single asset file.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file will linked to.</param>
        /// <param name="assetVersion">The version of the asset the file will linked to.</param>
        /// <param name="datasetId"></param>
        /// <param name="fileData">The asset file to create.</param>
        public CreateFileRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, IFileBaseData fileData)
            : base(projectId, assetId, assetVersion, datasetId)
        {
            m_RequestUrl += $"/files";

            FileData = fileData;
        }

        /// <summary>
        /// Provides a helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.Serialize(FileData, IsolatedSerialization.defaultSettings);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}

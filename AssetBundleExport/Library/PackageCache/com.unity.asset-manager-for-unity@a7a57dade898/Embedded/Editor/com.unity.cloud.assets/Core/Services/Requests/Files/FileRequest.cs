using System;
using System.Net.Http;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Represents a finalized upload asset file request.
    /// </summary>
    class FileRequest : DatasetRequest
    {
        readonly IFileBaseData m_Data;

        /// <summary>
        /// Gets a single file by path.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file will linked to.</param>
        /// <param name="assetVersion">The version of the asset the file will linked to.</param>
        /// <param name="datasetId">The id of the dataset. </param>
        /// <param name="filePath">The path to the file in the dataset.</param>
        /// <param name="data">The object containing the data of the file.</param>
        public FileRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, string filePath, IFileBaseData data = null)
            : base(projectId, assetId, assetVersion, datasetId)
        {
            m_RequestUrl += $"/files/{Uri.EscapeDataString(filePath)}";

            m_Data = data;
        }

        /// <summary>
        /// Gets a single file by path.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file will linked to.</param>
        /// <param name="assetVersion">The version of the asset the file will linked to.</param>
        /// <param name="datasetId">The id of the dataset. </param>
        /// <param name="filePath">The path to the file in the dataset.</param>
        /// <param name="includedFileFields">Sets the fields to be included in the response.</param>
        public FileRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, string filePath, FileFields includedFileFields)
            : this(projectId, assetId, assetVersion, datasetId, filePath)
        {
            includedFileFields.Parse(AddFieldFilterToQueryParams);
        }

        public FileRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, FileFields includedFieldsFilter, string token = null, int? limit = null)
            : base(projectId, assetId, assetVersion, datasetId)
        {
            m_RequestUrl += $"/files";

            includedFieldsFilter.Parse(AddFieldFilterToQueryParams);

            AddParamToQuery("Limit", limit?.ToString());
            AddParamToQuery("Token", token);
        }

        /// <summary>
        /// Provides a helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            if (m_Data == null)
            {
                return base.ConstructBody();
            }

            var body = IsolatedSerialization.SerializeWithConverters(m_Data);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}

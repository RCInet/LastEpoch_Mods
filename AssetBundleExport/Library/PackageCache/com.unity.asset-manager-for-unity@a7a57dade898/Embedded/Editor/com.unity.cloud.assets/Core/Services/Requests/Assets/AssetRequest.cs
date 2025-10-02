using System;
using System.Net.Http;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Represents a change asset's status request.
    /// </summary>
    class AssetRequest : ProjectRequest
    {
        readonly IAssetBaseData m_Data;

        /// <summary>
        /// Changes the asset's status Request Object.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="assetVersion">The version of the asset the file is linked to.</param>
        /// <param name="data">The data of the asset.</param>
        public AssetRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, IAssetBaseData data = null)
            : base(projectId)
        {
            m_RequestUrl += $"/assets/{assetId}/versions/{assetVersion}";

            m_Data = data;
        }

        /// <summary>
        /// Get a single asset by id and version.
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        /// <param name="assetId">ID of the asset</param>
        /// <param name="assetVersion">Version of the asset</param>
        /// <param name="includedFieldsFilter">Sets the fields to be included in the response.</param>
        public AssetRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, FieldsFilter includedFieldsFilter)
            : this(projectId, assetId, assetVersion)
        {
            includedFieldsFilter?.Parse(AddFieldFilterToQueryParams);
        }

        /// <summary>
        /// Get a single asset by id and version.
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        /// <param name="assetId">ID of the asset</param>
        /// <param name="label">The labelled version of the asset</param>
        /// <param name="includedFieldsFilter">Sets the fields to be included in the response.</param>
        public AssetRequest(ProjectId projectId, AssetId assetId, string label, FieldsFilter includedFieldsFilter)
            : base(projectId)
        {
            m_RequestUrl += $"/assets/{assetId}/labels/{Uri.EscapeDataString(label)}";

            includedFieldsFilter?.Parse(AddFieldFilterToQueryParams);
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            if (m_Data == null)
            {
                return base.ConstructBody();
            }

            var body = IsolatedSerialization.SerializeWithDefaultConverters(m_Data);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }

        protected void AddFieldFilterToQueryParams(string value)
        {
            AddParamToQuery("IncludeFields", value);
        }
    }
}

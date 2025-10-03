using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Represents a change asset's status request.
    /// </summary>
    [DataContract]
    class SearchAssetVersionRequest : ProjectRequest
    {
        readonly SearchRequestParameters m_Parameters;

        /// <summary>
        /// GetAssetByIdAndVersion Request Object.
        /// Get a single asset by id and version.
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        /// <param name="assetId">ID of the asset</param>
        /// <param name="parameters">The search asset request criteria.</param>
        public SearchAssetVersionRequest(ProjectId projectId, AssetId assetId, SearchRequestParameters parameters = default)
            : base(projectId)
        {
            m_RequestUrl += $"/assets/{assetId}/versions/search";

            m_Parameters = parameters;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.SerializeWithDefaultConverters(m_Parameters);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}

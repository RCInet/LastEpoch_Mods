using System;
using System.Net.Http;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// SearchRequest
    /// Search assets based on criteria.
    /// </summary>
    class SearchRequest : ProjectRequest
    {
        readonly SearchRequestParameters m_RequestParameters;

        /// <summary>
        /// Search Request Object.
        /// Search assets based on criteria.
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        /// <param name="parameters">The search asset request criteria.</param>
        public SearchRequest(ProjectId projectId, SearchRequestParameters parameters = default)
            : base(projectId)
        {
            m_RequestUrl += $"/assets/search";

            m_RequestParameters = parameters;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.SerializeWithDefaultConverters(m_RequestParameters);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}

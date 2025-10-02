using System.Net.Http;
using System.Text;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// AcrossProjectsSearchRequest
    /// Across projects search assets based on criteria.
    /// </summary>
    class AcrossProjectsSearchRequest : OrganizationRequest
    {
        readonly SearchRequestParameters m_RequestParameters;

        /// <summary>
        /// Across projects search Request Object.
        /// Across projects search assets based on criteria.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization</param>
        /// <param name="parameters">The search asset request criteria.</param>
        public AcrossProjectsSearchRequest(OrganizationId organizationId,
            SearchRequestParameters parameters = default)
            : base(organizationId)
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

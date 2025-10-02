using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Base class for api requests on assets.
    /// </summary>
    class OrganizationRequest : ApiRequest
    {
        /// <summary>
        /// AssetRequest Request Object.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization</param>
        protected OrganizationRequest(OrganizationId organizationId)
        {
            m_RequestUrl = $"/organizations/{organizationId}";
        }
    }
}

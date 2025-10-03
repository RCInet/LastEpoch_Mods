using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class ListProjectsRequest : OrganizationRequest
    {
        /// <summary>
        /// ApiAssetsUsersV1UserIdOrganizationOrganizationIdProjectsGet Request Object.
        /// Reads a list of projects in an org that a user has access to.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        public ListProjectsRequest(OrganizationId organizationId,
            int? page = default, int? pageSize = default)
            : base(organizationId)
        {
            m_RequestUrl += "/projects";

            AddParamToQuery("IncludeFields", "hasCollection");
            AddParamToQuery("Page", page.ToString());
            AddParamToQuery("Limit", pageSize.ToString());
        }
    }
}

using System;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    internal interface IOrganizationJsonProvider
    {
        public Task<OrganizationJson> GetOrganizationJsonAsync(OrganizationId organizationId);
    }
}

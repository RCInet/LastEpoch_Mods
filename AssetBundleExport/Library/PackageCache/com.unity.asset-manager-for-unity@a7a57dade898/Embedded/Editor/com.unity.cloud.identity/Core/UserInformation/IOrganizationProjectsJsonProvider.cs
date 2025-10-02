using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    internal interface IOrganizationProjectsJsonProvider
    {
        IAsyncEnumerable<ProjectJson> GetOrganizationProjectsJson(OrganizationId organizationId, IEntityRoleProvider entityRoleProvider, Range range, CancellationToken cancellationToken);
    }
}

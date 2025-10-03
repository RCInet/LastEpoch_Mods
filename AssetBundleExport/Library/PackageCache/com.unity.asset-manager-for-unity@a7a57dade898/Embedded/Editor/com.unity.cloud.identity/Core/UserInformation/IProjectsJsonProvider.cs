using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    internal interface IProjectsJsonProvider
    {
        IAsyncEnumerable<ProjectJson> GetOrganizationProjectsJson(OrganizationId organizationId, IEntityRoleProvider entityRoleProvider, Range range, CancellationToken cancellationToken);
    }
}

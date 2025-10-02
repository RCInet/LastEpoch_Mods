using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// The interface for an organization repository.
    /// </summary>
    interface IOrganizationRepository
    {

        /// <summary>
        /// Lists <see cref="IOrganization"/>.
        /// </summary>
        /// <param name="range">A range of <see cref="IOrganization"/> to request.</param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>An <see cref="System.Collections.Generic.IAsyncEnumerable{T}"/> of Type <see cref="IOrganization"/>.</returns>
        IAsyncEnumerable<IOrganization> ListOrganizationsAsync(Range range, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a single <see cref="IOrganization"/>.
        /// </summary>
        /// <param name="organizationId">The <see cref="OrganizationId"/> of the <see cref="IOrganization"/> to get.</param>
        /// <returns>A task that once completed returns an <see cref="IOrganization"/>.</returns>
        /// <exception cref="NotFoundException"></exception>
        async Task<IOrganization> GetOrganizationAsync(OrganizationId organizationId)
        {
            var organizationsAsyncEnumerable = ListOrganizationsAsync(Range.All);
            await foreach (var organization in organizationsAsyncEnumerable)
            {
                if (organization.Id.Equals(organizationId))
                {
                    return organization;
                }
            }
            throw new NotFoundException();
        }
    }

}

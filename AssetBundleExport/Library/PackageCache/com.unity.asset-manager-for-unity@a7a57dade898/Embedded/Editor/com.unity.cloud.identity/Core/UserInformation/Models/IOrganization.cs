using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{

    /// <summary>
    /// The interface for an organization.
    /// </summary>
    interface IOrganization : IRoleProvider, IMemberInfoProvider, ICloudStorageInfoProvider
    {
        /// <summary>
        /// Gets the Genesis id of the organization.
        /// </summary>
        OrganizationId Id { get; }

        /// <summary>
        /// Gets the name of the organization.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the legacy role of the user in the organization.
        /// </summary>
        Role Role { get; }

        /// <summary>
        /// An awaitable Task that returns the list of <see cref="IProject"/> the user can access in the organization.
        /// </summary>
        /// <param name="range">A range of <see cref="IProject"/> to request.</param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task whose result is an async enumeration of <see cref="IProject"/>.</returns>
        public IAsyncEnumerable<IProject> ListProjectsAsync(Range range, CancellationToken cancellationToken = default);
    }
}

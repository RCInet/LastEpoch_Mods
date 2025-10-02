using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// An interface that exposes cloud storage information.
    /// </summary>
    interface ICloudStorageInfoProvider
    {
        /// <summary>
        /// A Task that returns an <see cref="ICloudStorageUsage"/> once completed.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ForbiddenException">Thrown if the role of the user in the <see cref="Organization"/> is Project guest.</exception>
        /// <returns>An <see cref="ICloudStorageUsage"/>.</returns>
        public Task<ICloudStorageUsage> GetCloudStorageUsageAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

        /// <summary>
        /// A Task that returns a <see cref="bool"/> about an <see cref="Organization"/> metered billing status once completed.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <exception cref="ForbiddenException">Thrown if the role of the user in the <see cref="Organization"/> is Project guest.</exception>
        /// <returns>A <see cref="bool"/> about an <see cref="Organization"/> metered billing status.</returns>
        public Task<bool> HasMeteredBillingActivatedAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }
}

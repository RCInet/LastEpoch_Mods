using System;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Resolves the domain and subdomain for the service address based on a given provider and domain target.
    /// </summary>
    interface IServiceDomainResolver
    {
        /// <summary>
        /// Gets the resolved domain for the given provider.
        /// </summary>
        /// <param name="provider">The provider for which to get the resolved domain.</param>
        /// <returns>The resolved domain for the given provider.</returns>
        string GetResolvedDomain(ServiceDomainProvider provider);

        /// <summary>
        /// Gets the resolved subdomain for the given provider and environment.
        /// </summary>
        /// <param name="provider">The provider for which to get the resolved domain.</param>
        /// <param name="environment">The environment for which to get the resolved domain.</param>
        /// <returns>The resolved subdomain for the given provider and environment.</returns>
        string GetResolvedSubdomain(ServiceDomainProvider provider, ServiceEnvironment environment);
    }
}

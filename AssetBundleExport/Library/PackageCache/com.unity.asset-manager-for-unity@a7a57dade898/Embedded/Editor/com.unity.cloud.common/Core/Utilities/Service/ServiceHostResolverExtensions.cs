using System;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Helper methods for <see cref="IServiceHostResolver"/>.
    /// </summary>
    static class ServiceHostResolverExtensions
    {
        /// <summary>
        /// Returns an instance of <see cref="ServiceHost"/> initialized with values resolved from the <paramref name="serviceHostResolver"/>.
        /// </summary>
        /// <param name="serviceHostResolver">The <see cref="IServiceHostResolver"/> from which to create a <see cref="ServiceHost"/>.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceHostResolver"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the <paramref name="serviceHostResolver"/> is not an instance of <see cref="ServiceHostResolver"/>.</exception>
        [Obsolete("Deprecated. Use the extension method specialized for ServiceHostResolver instances.")]
        public static ServiceHost GetResolvedServiceHost(this IServiceHostResolver serviceHostResolver)
        {
            if (serviceHostResolver == null)
                throw new ArgumentNullException(nameof(serviceHostResolver));

            if (serviceHostResolver is ServiceHostResolver unityServiceHostResolver)
            {
                return new ServiceHost
                {
                    EnvironmentValue = unityServiceHostResolver.GetResolvedEnvironment().ToString(),
                    ProviderValue = unityServiceHostResolver.GetResolvedDomainProvider().ToString()
                };
            }
            throw new InvalidOperationException("Provided IServiceHostResolver is not a ServiceHostResolver instance");
        }

        internal static ServiceHost GetResolvedServiceHost(this ServiceHostResolver serviceHostResolver)
        {
            if (serviceHostResolver == null)
                throw new ArgumentNullException(nameof(serviceHostResolver));

            return new ServiceHost
            {
                EnvironmentValue = serviceHostResolver.GetResolvedEnvironment().ToString(),
                ProviderValue = serviceHostResolver.GetResolvedDomainProvider().ToString()
            };
        }

        /// <summary>
        /// Creates a copy of the <paramref name="serviceHostResolver"/> with the given <paramref name="domainResolverOverride"/>.
        /// </summary>
        /// <param name="serviceHostResolver">The <see cref="IServiceHostResolver"/> to copy.</param>
        /// <param name="domainResolverOverride">The <see cref="IServiceDomainResolver"/> to initialize the copy with.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if either <paramref name="serviceHostResolver"/> or <paramref name="domainResolverOverride"/> are null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the <paramref name="serviceHostResolver"/> is not an instance of <see cref="ServiceHostResolver"/> or if the <paramref name="domainResolverOverride"/> is not an instance of <see cref="ServiceDomainResolver"/>.</exception>
        [Obsolete("Deprecated. Use the extension method specialized for ServiceHostResolver instances.")]
        public static IServiceHostResolver CreateCopyWithDomainResolverOverride(this IServiceHostResolver serviceHostResolver, IServiceDomainResolver domainResolverOverride)
        {
            if (serviceHostResolver == null)
                throw new ArgumentNullException(nameof(serviceHostResolver));

            if (domainResolverOverride == null)
                throw new ArgumentNullException(nameof(domainResolverOverride));

            if (serviceHostResolver is ServiceHostResolver unityServiceHostResolver && domainResolverOverride is ServiceDomainResolver unityDomainResolver)
            {
                return new ServiceHostResolver(unityServiceHostResolver.GetResolvedServiceHost(), unityDomainResolver);
            }
            throw new InvalidOperationException(serviceHostResolver is ServiceHostResolver ? "Provided IServiceDomainResolver is not a ServiceDomainResolver instance" : "Provided IServiceHostResolver is not a ServiceHostResolver instance");
        }

        /// <summary>
        /// Creates a copy of the <paramref name="serviceHostResolver"/> with the given <paramref name="domainResolverOverride"/>.
        /// </summary>
        /// <param name="serviceHostResolver">The <see cref="ServiceHostResolver"/> to copy.</param>
        /// <param name="domainResolverOverride">The <see cref="ServiceDomainResolver"/> to initialize the copy with.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if either <paramref name="serviceHostResolver"/> or <paramref name="domainResolverOverride"/> are null.</exception>
        public static IServiceHostResolver CreateCopyWithDomainResolverOverride(this ServiceHostResolver serviceHostResolver, ServiceDomainResolver domainResolverOverride)
        {
            if (serviceHostResolver == null)
                throw new ArgumentNullException(nameof(serviceHostResolver));

            if (domainResolverOverride == null)
                throw new ArgumentNullException(nameof(domainResolverOverride));

            return new ServiceHostResolver(serviceHostResolver.GetResolvedServiceHost(), domainResolverOverride);
        }

        /// <summary>
        /// Gets the resolved host for the <see cref="IServiceHostResolver"/>.
        /// </summary>
        /// <param name="serviceHostResolver">The <see cref="IServiceHostResolver"/> to get the resolved host from.</param>
        /// <returns>The resolved host.</returns>
        public static string GetResolvedHost(this IServiceHostResolver serviceHostResolver)
        {
            return new Uri(serviceHostResolver.GetResolvedAddress()).Host;
        }

    }
}

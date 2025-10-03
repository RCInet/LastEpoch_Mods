using System;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// A factory to create a <see cref="IServiceHostResolver"/> for the Unity services gateway or for a fully qualified domain name.
    /// </summary>
    static class ServiceHostResolverFactory
    {
        /// <summary>
        /// The environment variable key for the service fully qualified domain name override.
        /// </summary>
        internal static string SystemOverrideFullyQualifiedDomainNameVariableName => "UNITY_CLOUD_SERVICES_FQDN";

        /// <summary>
        /// The environment variable key for the service fully qualified domain name prefix override.
        /// </summary>
        internal static string SystemOverrideFullyQualifiedDomainNamePathPrefixVariableName => "UNITY_CLOUD_SERVICES_FQDN_PATH_PREFIX";

        /// <summary>
        /// Create a <see cref="IServiceHostResolver"/>.
        /// Default to the Unity services gateway.
        /// Any system-level overrides for a fully qualified domain name set via environment variables will take priority.
        /// </summary>
        /// <returns>The created <see cref="IServiceHostResolver"/>.</returns>
        public static IServiceHostResolver Create()
        {
            var fullyQualifiedDomainNameOverride = Environment.GetEnvironmentVariable(SystemOverrideFullyQualifiedDomainNameVariableName);
            if (!string.IsNullOrEmpty(fullyQualifiedDomainNameOverride))
            {
                var fullyQualifiedDomainNamePathPrefixOverride = Environment.GetEnvironmentVariable(SystemOverrideFullyQualifiedDomainNamePathPrefixVariableName);
                return !string.IsNullOrEmpty(fullyQualifiedDomainNamePathPrefixOverride)
                    ? CreateForFullyQualifiedDomainName(fullyQualifiedDomainNameOverride, fullyQualifiedDomainNamePathPrefixOverride)
                    : CreateForFullyQualifiedDomainName(fullyQualifiedDomainNameOverride);
            }

            return CreateForUnityServicesGateway();
        }

        /// <summary>
        /// Create a <see cref="IServiceHostResolver"/> that always resolve the domain to the fully qualified name and API route prefix provided in its constructor.
        /// </summary>
        /// <param name="fullyQualifiedDomainName">The fully qualified domain name.</param>
        /// <param name="pathPrefix">The optional path prefix.</param>
        /// <returns>The created <see cref="IServiceHostResolver"/>.</returns>
        public static IServiceHostResolver CreateForFullyQualifiedDomainName(string fullyQualifiedDomainName, string pathPrefix = "/")
        {
            return new FullyQualifiedDomainNameServiceHostResolver(fullyQualifiedDomainName, pathPrefix);
        }

        /// Create a <see cref="ServiceHostResolver"/>.
        /// Any system-level overrides for <see cref="ServiceEnvironment"/> and <see cref="ServiceDomainProvider"/> set via environment variables will take priority.
        /// <returns>The created <see cref="ServiceHostResolver"/>.</returns>
        static IServiceHostResolver CreateForUnityServicesGateway()
        {
            return new ServiceHostResolver();
        }

        /// <summary>
        /// Create a <see cref="IServiceHostResolver"/> with an optional application-level override for service host options.
        /// Any system-level overrides set via environment variables will take priority.
        /// </summary>
        /// <param name="applicationOverride">An application-level override value for service host options.</param>
        /// <returns>The created <see cref="IServiceHostResolver"/>.</returns>
        internal static IServiceHostResolver CreateWithOverride(ServiceHost applicationOverride)
        {
            return new ServiceHostResolver(applicationOverride);
        }
    }
}

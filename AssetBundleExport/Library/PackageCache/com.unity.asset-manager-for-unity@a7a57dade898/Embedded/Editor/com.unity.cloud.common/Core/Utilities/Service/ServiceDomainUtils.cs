using System;
using System.Collections.Generic;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Helper methods for determining service domain by provider.
    /// </summary>
    static class ServiceDomainUtils
    {
        internal static readonly Dictionary<ServiceDomainProvider, string> s_ServerDomainMap = new()
        {
            { ServiceDomainProvider.UnityServices, "services.api.unity.com"},
        };

        internal static readonly Dictionary<ServiceDomainProvider,  Dictionary<ServiceEnvironment, string>> s_SubdomainMap = new()
        {
            { ServiceDomainProvider.UnityServices, new Dictionary<ServiceEnvironment, string>()
                {
                    {ServiceEnvironment.Production, string.Empty},
                    {ServiceEnvironment.Staging, "staging."},
                    {ServiceEnvironment.Test, "staging."}, // On the test environment, we still want to hit the staging subdomain.
                }
            },
        };

        /// <summary>
        /// Returns a Provider based on ISO Region Name
        /// </summary>
        internal static ServiceDomainProvider UserLocaleDomainProvider => ServiceHostResolver.DefaultDomainProvider; // Force default until new regions are deployed

        internal static ServiceDomainProvider? ParseProviderValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            if (Enum.TryParse<ServiceDomainProvider>(value, true, out var provider))
                return provider;

            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Unity.Cloud.CommonEmbedded.Runtime
{
    /// <summary>
    /// A factory class used to create a <see cref="IServiceHostResolver"/>.
    /// </summary>
    [Obsolete("Use Unity.Cloud.Common.ServiceHostResolverFactory instead.")]
static class UnityRuntimeServiceHostResolverFactory
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        /// <summary>
        /// Create a <see cref="IServiceHostResolver"/> with default values.
        /// Any system-level overrides set via environment variables will take priority.
        /// </summary>
        /// <returns>The created <see cref="IServiceHostResolver"/></returns>
        public static IServiceHostResolver Create()
        {
            var fullyQualifiedDomaineNameOverride = ReadLocalCacheForFullyQualifiedDomainNameOverride();
            var fullyQualifiedDomaineNamePathPrefixOverride = ReadLocalCacheForFullyQualifiedDomainNamePathPrefixOverride();
            if (!string.IsNullOrEmpty(fullyQualifiedDomaineNameOverride))
            {
                ServiceHostResolverFactory.CreateForFullyQualifiedDomainName(fullyQualifiedDomaineNameOverride, fullyQualifiedDomaineNamePathPrefixOverride);
            }

            var systemEnvironmentOverrideValue = ReadLocalCacheForSystemEnvironmentOverride();
            var systemProviderOverrideValue = ReadLocalCacheForSystemProviderOverride();

            systemEnvironmentOverrideValue ??= ParseUrlForSystemEnvironmentOverrideInHost(Application.absoluteURL);
            systemProviderOverrideValue ??= ParseUrlForSystemProviderOverrideInHost(Application.absoluteURL);

            var hostOverride = new ServiceHost()
            {
                EnvironmentValue = systemEnvironmentOverrideValue ,
                ProviderValue = systemProviderOverrideValue
            };

            return new ServiceHostResolver(hostOverride);
        }

        /// <summary>
        /// Create a <see cref="ServiceHostResolver"/> with an optional application-level override for service host options.
        /// Any system-level overrides set via environment variables will take priority.
        /// </summary>
        /// <param name="applicationOverride">An application-level override value for for service host options.</param>
        /// <returns>The created <see cref="ServiceHostResolver"/></returns>
        public static ServiceHostResolver CreateWithOverride(ServiceHost applicationOverride)
        {
            var systemEnvironmentOverrideValue = ReadLocalCacheForSystemEnvironmentOverride();
            var systemProviderOverrideValue = ReadLocalCacheForSystemProviderOverride();

            systemEnvironmentOverrideValue ??= ParseUrlForSystemEnvironmentOverrideInHost(Application.absoluteURL);
            systemProviderOverrideValue ??= ParseUrlForSystemProviderOverrideInHost(Application.absoluteURL);

            var hostOverride = new ServiceHost()
            {
                EnvironmentValue = systemEnvironmentOverrideValue ?? applicationOverride.EnvironmentValue,
                ProviderValue = systemProviderOverrideValue ?? applicationOverride.ProviderValue
            };

            return new ServiceHostResolver(hostOverride);
        }
#else
        /// <summary>
        /// Create a <see cref="IServiceHostResolver"/> with default values.
        /// Any system-level overrides set via environment variables will take priority.
        /// </summary>
        /// <returns>The created <see cref="IServiceHostResolver"/></returns>
        public static IServiceHostResolver Create()
        {
            return ServiceHostResolverFactory.Create();
        }

        /// <summary>
        /// Create a <see cref="IServiceHostResolver"/> with an optional application-level for service host options.
        /// </summary>
        /// <param name="applicationOverride">An application-level override value for service host options.</param>
        /// <returns>The created <see cref="IServiceHostResolver"/>.</returns>
        internal static IServiceHostResolver CreateWithOverride(ServiceHost applicationOverride)
        {
            return ServiceHostResolverFactory.CreateWithOverride(applicationOverride);
        }
#endif

        internal static string ParseUrlForSystemEnvironmentOverrideInHost(string uriString)
        {
            if (Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
            {
                if (Regex.IsMatch(uri.Host, @"^.*staging\..*\.unity\.com$", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1)))
                    return ServiceEnvironment.Staging.ToString().ToLower();
                if (Regex.IsMatch(uri.Host, @"^.*test\..*\.unity\.com$", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1)))
                    return ServiceEnvironment.Test.ToString().ToLower();
                if (Regex.IsMatch(uri.Host, @"^.*\.unity\.com$", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1)))
                    return ServiceEnvironment.Production.ToString().ToLower();
            }
            return null;
        }

        internal static string ParseUrlForSystemProviderOverrideInHost(string uriString)
        {
            if (Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
            {
                if (uri.Host.EndsWith("services.unity.com") || uri.Host.EndsWith("services.api.unity.com"))
                    return ServiceDomainProvider.UnityServices.ToString().ToLower();
            }

            return null;
        }

        internal static string ReadLocalCacheForSystemEnvironmentOverride()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            foreach (var environmentVariableName in EnvironmentVariableNames())
            {
                var cacheValue = CommonBrowserInterop.RetrieveCachedValue(environmentVariableName);
                if (!string.IsNullOrEmpty(cacheValue))
                    return cacheValue;
            }
#endif
            return null;
        }

        internal static string ReadLocalCacheForSystemProviderOverride()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            foreach (var environmentVariableName in ProviderVariableNames())
            {
                var cacheValue = CommonBrowserInterop.RetrieveCachedValue(environmentVariableName);
                if (!string.IsNullOrEmpty(cacheValue))
                    return cacheValue;
            }
#endif
            return null;
        }

        internal static string ReadLocalCacheForFullyQualifiedDomainNameOverride()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            foreach (var environmentVariableName in FullyQualifiedDomainNameVariableNames())
            {
                var cacheValue = CommonBrowserInterop.RetrieveCachedValue(environmentVariableName);
                if (!string.IsNullOrEmpty(cacheValue))
                    return cacheValue;
            }
#endif
            return null;
        }

        internal static string ReadLocalCacheForFullyQualifiedDomainNamePathPrefixOverride()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            foreach (var environmentVariableName in FullyQualifiedDomainNamePathPrefixVariableNames())
            {
                var cacheValue = CommonBrowserInterop.RetrieveCachedValue(environmentVariableName);
                if (!string.IsNullOrEmpty(cacheValue))
                    return cacheValue;
            }
#endif
            return null;
        }

        internal static IEnumerable<string> EnvironmentVariableNames()
        {
            yield return ServiceHostResolver.SystemOverrideEnvironmentVariableName;
            yield return ServiceHostResolver.SystemOverrideEnvironmentVariableName.ToLower();
        }

        internal static IEnumerable<string> ProviderVariableNames()
        {
            yield return ServiceHostResolver.SystemOverrideDomainProviderVariableName;
            yield return ServiceHostResolver.SystemOverrideDomainProviderVariableName.ToLower();
        }

        internal static IEnumerable<string> FullyQualifiedDomainNameVariableNames()
        {
            yield return ServiceHostResolverFactory.SystemOverrideFullyQualifiedDomainNameVariableName;
            yield return ServiceHostResolverFactory.SystemOverrideFullyQualifiedDomainNameVariableName.ToLower();
        }

        internal static IEnumerable<string> FullyQualifiedDomainNamePathPrefixVariableNames()
        {
            yield return ServiceHostResolverFactory.SystemOverrideFullyQualifiedDomainNamePathPrefixVariableName;
            yield return ServiceHostResolverFactory.SystemOverrideFullyQualifiedDomainNamePathPrefixVariableName.ToLower();
        }
    }
}

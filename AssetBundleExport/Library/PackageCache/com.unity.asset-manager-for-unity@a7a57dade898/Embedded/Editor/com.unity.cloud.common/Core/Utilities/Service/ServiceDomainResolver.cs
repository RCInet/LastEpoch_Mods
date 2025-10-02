using System;
using System.Collections.Generic;

namespace Unity.Cloud.CommonEmbedded
{
    /// <inheritdoc/>
    class ServiceDomainResolver : IServiceDomainResolver
    {
        /// <inheritdoc/>
        public virtual string GetResolvedDomain(ServiceDomainProvider provider)
        {
            try
            {
                return ServiceDomainUtils.s_ServerDomainMap[provider];
            }
            catch (KeyNotFoundException)
            {
                throw new NotSupportedException($"The service domain provider is not supported: {provider}");
            }
        }

        /// <inheritdoc/>
        public virtual string GetResolvedSubdomain(ServiceDomainProvider provider, ServiceEnvironment environment)
        {
            try
            {
                return ServiceDomainUtils.s_SubdomainMap[provider][environment];
            }
            catch (KeyNotFoundException)
            {
                throw new NotSupportedException($"The service domain provider is not supported for the chosen environment: {provider}:{environment}");
            }
        }
    }
}

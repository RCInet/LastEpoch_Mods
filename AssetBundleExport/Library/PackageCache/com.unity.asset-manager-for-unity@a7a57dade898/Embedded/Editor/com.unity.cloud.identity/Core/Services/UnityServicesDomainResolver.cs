using System;
using System.Collections.Generic;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <inheritdoc/>
    class UnityServicesDomainResolver : ServiceDomainResolver
    {
        enum ServiceDomainAccessibility
        {
            Public,
            Internal,
        }

        static readonly Dictionary<ServiceDomainProvider, Dictionary<ServiceDomainAccessibility, string>> s_ServerDomainMap = new()
        {
            { ServiceDomainProvider.UnityServices,
                new Dictionary<ServiceDomainAccessibility, string>
                {
                    {ServiceDomainAccessibility.Public, "services.api.unity.com"},
                    {ServiceDomainAccessibility.Internal, "services.unity.com"},
                } },
        };

        readonly ServiceDomainAccessibility m_DomainAccessibility;

        internal UnityServicesDomainResolver(bool useInternal = false)
        {
            m_DomainAccessibility = useInternal ? ServiceDomainAccessibility.Internal : ServiceDomainAccessibility.Public;
        }

        public override string GetResolvedDomain(ServiceDomainProvider provider)
        {
            return s_ServerDomainMap.TryGetValue(provider, out var providerMap) ? providerMap[m_DomainAccessibility] : base.GetResolvedDomain(provider);
        }
    }
}

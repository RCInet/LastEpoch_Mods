using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// A class implementing <see cref="IOrganization"/>.
    /// </summary>
    [Serializable]
    internal class Organization : IOrganization
    {
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<Organization>();

        readonly IServiceHostResolver m_ServiceHostResolver;
        readonly IServiceHostResolver m_InternalServiceHostResolver;
        readonly IServiceHttpClient m_ServiceHttpClient;

        readonly IOrganizationProjectsJsonProvider m_OrganizationProjectsJsonProvider;
        readonly IEntityRoleProvider m_EntityRoleProvider;
        readonly IGuestProjectJsonProvider m_GuestProjectJsonProvider;
        readonly IAssetProjectsJsonProvider m_AssetProjectsJsonProvider;
        readonly IMemberInfoJsonProvider m_MemberInfoJsonProvider;
        readonly ICloudStorageJsonProvider m_CloudStorageJsonProvider;

        readonly GetRequestResponseCache<RangeResultsJson<MemberInfoJson>> m_GetRequestResponseCache;

        readonly GetRequestResponseCache<AssetProjectPageResultsJson<AssetProjectJson>> m_GetAssetProjectRequestResponseCache;

        internal Organization(OrganizationJson organizationJson, IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver, IOrganizationProjectsJsonProvider organizationProjectsJsonProvider, IEntityRoleProvider entityRoleProvider, IGuestProjectJsonProvider guestProjectJsonProvider, IAssetProjectsJsonProvider assetProjectsJsonProvider = null, IMemberInfoJsonProvider memberInfoJsonProvider = null, ICloudStorageJsonProvider cloudStorageUsageJsonProvider = null)
        {
            Id = new OrganizationId(organizationJson.GenesisId);

            // If service host is the public unity services gateway
            if (serviceHostResolver is ServiceHostResolver unityServiceHostResolver && unityServiceHostResolver.GetResolvedHost().EndsWith("services.api.unity.com"))
            {
                // Create a copy for internal unity services gateway calls
                m_InternalServiceHostResolver = unityServiceHostResolver.CreateCopyWithDomainResolverOverride(new UnityServicesDomainResolver(true));
            }
            else
            {
                // Otherwise use injected host resolver
                m_InternalServiceHostResolver = serviceHostResolver;
            }
            m_ServiceHostResolver = serviceHostResolver;
            m_ServiceHttpClient = serviceHttpClient;
            m_GuestProjectJsonProvider = guestProjectJsonProvider;
            m_AssetProjectsJsonProvider = assetProjectsJsonProvider;
            m_MemberInfoJsonProvider = memberInfoJsonProvider;
            m_CloudStorageJsonProvider = cloudStorageUsageJsonProvider;

            EntityId = organizationJson.Id;
            Name = organizationJson.Name;
            Role = organizationJson.Role != null ? new Role(organizationJson.Role) : Role.ProjectGuest;

            m_OrganizationProjectsJsonProvider = organizationProjectsJsonProvider;
            m_EntityRoleProvider = entityRoleProvider;

            m_GetRequestResponseCache = new GetRequestResponseCache<RangeResultsJson<MemberInfoJson>>(60);
            m_GetAssetProjectRequestResponseCache = new GetRequestResponseCache<AssetProjectPageResultsJson<AssetProjectJson>>(60);
        }

        /// <inheritdoc />
        public OrganizationId Id { get; }

        string EntityId { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public Role Role { get; }

        /// <inheritdoc />
        public async IAsyncEnumerable<IProject> ListProjectsAsync(Range range, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            IAsyncEnumerable<AssetProjectJson> assetProjectJsonAsyncEnumerable;
            if (m_AssetProjectsJsonProvider != null)
            {
                assetProjectJsonAsyncEnumerable = m_AssetProjectsJsonProvider.GetAssetProjectsJsonAsync(cancellationToken);
            }
            else
            {
                assetProjectJsonAsyncEnumerable = GetAssetProjectsJsonAsync(cancellationToken);
            }

            var assetProjectRegistry = new HashSet<string>();
            await foreach (var assetProjectJson in assetProjectJsonAsyncEnumerable.WithCancellation(cancellationToken))
            {
                assetProjectRegistry.Add(assetProjectJson.Id);
            }

            if (Role.Equals(Role.ProjectGuest))
            {
                var orgId = Id.ToString();
                var guestProjectJsonAsyncEnumerable = m_GuestProjectJsonProvider.GetGuestProjectsAsync(range, cancellationToken);
                await foreach (var guestProjectJson in guestProjectJsonAsyncEnumerable)
                {
                    if (guestProjectJson.OrganizationGenesisId.Equals(orgId))
                    {
                        yield return new Project(guestProjectJson, m_ServiceHttpClient, m_ServiceHostResolver, m_EntityRoleProvider, m_MemberInfoJsonProvider);
                    }
                }
            }
            else
            {
                var projectsJsonAsyncEnumerable = m_OrganizationProjectsJsonProvider.GetOrganizationProjectsJson(Id, m_EntityRoleProvider, range, cancellationToken);
                await foreach (var projectJson in projectsJsonAsyncEnumerable)
                {
                    if (assetProjectRegistry.Contains(projectJson.Id))
                    {
                        projectJson.EnabledInAssetManager = true;
                    }
                    yield return new Project(projectJson, m_ServiceHttpClient, m_ServiceHostResolver, m_EntityRoleProvider, m_MemberInfoJsonProvider);
                }
            }
        }

        public async IAsyncEnumerable<AssetProjectJson> GetAssetProjectsJsonAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var pageRequest = new PageRequest<AssetProjectJson>(GetAssetProjects, 50);
            var requestBasePath = $"/assets/v1/organizations/{Id}/projects";
            var results = pageRequest.Execute(requestBasePath, cancellationToken);
            await foreach (var projectJson in results)
            {
                yield return projectJson;
            }
        }

        async Task<AssetProjectPageResultsJson<AssetProjectJson>> GetAssetProjects(string pageRequestPath, CancellationToken cancellationToken)
        {
            var url = m_ServiceHostResolver.GetResolvedRequestUri(pageRequestPath);
            if (m_GetAssetProjectRequestResponseCache.TryGetRequestResponseFromCache(url, out AssetProjectPageResultsJson<AssetProjectJson> value))
            {
                return value;
            }
            var response = await m_ServiceHttpClient.GetAsync(url, cancellationToken: cancellationToken);
            var deserializedResponse = await response.JsonDeserializeAsync<AssetProjectPageResultsJson<AssetProjectJson>>();
            return m_GetAssetProjectRequestResponseCache.AddGetRequestResponseToCache(url, deserializedResponse);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IMemberInfo> ListMembersAsync(Range range, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            IAsyncEnumerable<MemberInfoJson> asyncEnumerableMemberInfoJson;
            if (m_MemberInfoJsonProvider != null)
            {
                asyncEnumerableMemberInfoJson = m_MemberInfoJsonProvider.GetMemberInfoJsonAsync(range, cancellationToken);
            }
            else
            {
                var rangeRequest = new RangeRequest<MemberInfoJson>(GetOrganizationMembers, 1000);
                var requestBasePath = $"/api/access/legacy/v1/organizations/{Id}/members";
                asyncEnumerableMemberInfoJson = rangeRequest.Execute(requestBasePath, range, cancellationToken);
            }
            await foreach (var memberInfoJson in asyncEnumerableMemberInfoJson.WithCancellation(cancellationToken))
            {
                yield return new MemberInfo(memberInfoJson);
            }
        }

        async Task<RangeResultsJson<MemberInfoJson>> GetOrganizationMembers(string rangeRequestPath, CancellationToken cancellationToken)
        {
            var url = m_InternalServiceHostResolver.GetResolvedRequestUri(rangeRequestPath);
            if (m_GetRequestResponseCache.TryGetRequestResponseFromCache(url, out RangeResultsJson<MemberInfoJson> value))
            {
                return value;
            }

            var response = await m_ServiceHttpClient.GetAsync(url, cancellationToken: cancellationToken);
            var deserializedResponse = await response.JsonDeserializeAsync<RangeResultsJson<MemberInfoJson>>();
            return m_GetRequestResponseCache.AddGetRequestResponseToCache(url, deserializedResponse);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Role>> ListRolesAsync()
        {
            return await m_EntityRoleProvider.ListEntityRolesAsync(Id.ToString(), EntityType.Organization);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Permission>> ListPermissionsAsync()
        {
            return await m_EntityRoleProvider.ListEntityPermissionsAsync(Id.ToString(), EntityType.Organization);
        }

        /// <inheritdoc/>
        public async Task<ICloudStorageUsage> GetCloudStorageUsageAsync(CancellationToken cancellationToken = default)
        {
            CloudStorageUsageJson cloudStorageUsageJson;
            if (m_CloudStorageJsonProvider != null)
            {
                cloudStorageUsageJson = await m_CloudStorageJsonProvider.GetCloudStorageUsageAsync(cancellationToken);
            }
            else
            {
                cloudStorageUsageJson = await GetCloudStorageUsageJsonAsync(cancellationToken);
            }

            return new CloudStorageUsage(cloudStorageUsageJson);
        }

        async Task<CloudStorageUsageJson> GetCloudStorageUsageJsonAsync(CancellationToken cancellationToken)
        {
            var url = m_InternalServiceHostResolver.GetResolvedRequestUri($"/api/cloud-storage/v1/organizations/{Id}/usage");
            var response = await m_ServiceHttpClient.GetAsync(url, cancellationToken:cancellationToken);
            return await response.JsonDeserializeAsync<CloudStorageUsageJson>();
        }

        internal async Task<ICloudStorageEntitlements> GetCloudStorageEntitlementsAsync(CancellationToken cancellationToken)
        {
            CloudStorageEntitlementsJson cloudStorageEntitlementsJson;
            if (m_CloudStorageJsonProvider != null)
            {
                cloudStorageEntitlementsJson = await m_CloudStorageJsonProvider.GetCloudStorageEntitlementsAsync(cancellationToken);
            }
            else
            {
                cloudStorageEntitlementsJson = await GetCloudStorageEntitlementsJsonAsync(cancellationToken);
            }

            return new CloudStorageEntitlements(cloudStorageEntitlementsJson);
        }

        /// <inheritdoc/>
        public async Task<bool> HasMeteredBillingActivatedAsync(CancellationToken cancellationToken = default)
        {
            var cloudStorageEntitlements = await GetCloudStorageEntitlementsAsync(cancellationToken);
            return cloudStorageEntitlements.MeteredOptInEnabled;
        }

        async Task<CloudStorageEntitlementsJson> GetCloudStorageEntitlementsJsonAsync(CancellationToken cancellationToken)
        {
            var url = m_InternalServiceHostResolver.GetResolvedRequestUri($"/api/cloud-storage/v1/organizations/{Id}/entitlements");
            var response = await m_ServiceHttpClient.GetAsync(url, cancellationToken:cancellationToken);
            return await response.JsonDeserializeAsync<CloudStorageEntitlementsJson>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// A class to hold all Entity type string values.
    /// </summary>
    internal static class EntityType
    {
        public static readonly string Organization = "organization";
        public static readonly string Project = "project";
    }

    /// <summary>
    /// The interface to validate and list roles assigned to a user.
    /// </summary>
    interface IRoleProvider
    {
        /// <summary>
        /// Lists roles assigned to a user.
        /// </summary>
        /// <returns>A task that once completed returns the list of roles assigned to a user.</returns>
        Task<IEnumerable<Role>> ListRolesAsync();

        /// <summary>
        /// Lists permissions assigned to a user.
        /// </summary>
        /// <returns>A task that once completed returns the list of permissions assigned to a user.</returns>
        Task<IEnumerable<Permission>> ListPermissionsAsync();
    }

    /// <summary>
    /// The interface to validate a role assigned to a user on an entity.
    /// </summary>
    internal interface IEntityRoleProvider
    {
        /// <summary>
        /// A Task to return the list of roles assigned to a user on an entity.
        /// </summary>
        /// <param name="entityId">The string id of the entity.</param>
        /// <param name="entityType">The type of the entity.</param>
        /// <returns>The list of roles assigned to a user on an entity.</returns>
        Task<IEnumerable<Role>> ListEntityRolesAsync(string entityId, string entityType);

        /// <summary>
        /// A Task to return the list of permissions assigned to a user on an entity.
        /// </summary>
        /// <param name="entityId">The string id of the entity.</param>
        /// <param name="entityType">The type of the entity.</param>
        /// <returns>The list of permissions assigned to a user on an entity.</returns>
        Task<IEnumerable<Permission>> ListEntityPermissionsAsync(string entityId, string entityType);
    }

    [Serializable]
    internal class EntityJson
    {
        public string EntityId  { get; set; }

        public string EntityType  { get; set; }

        public int OriginId  { get; set; }

        public IEnumerable<PolicyJson> Policies  { get; set; }
    }

    [Serializable]
    internal class PolicyJson
    {
        public string Id  { get; set; }

        public string RoleId  { get; set; }

        public string RoleName  { get; set; }

        public string[] RolePermissions  { get; set; }
    }

    internal class AuthenticatorRoleProvider : IEntityRoleProvider
    {
        readonly IServiceHostResolver m_ServiceHostResolver;
        readonly IServiceHttpClient m_ServiceHttpClient;
        readonly IEntityJsonProvider m_EntityJsonProvider;

        readonly string m_UserId;

        readonly GetRequestResponseCache<IEnumerable<EntityJson>> m_GetRequestResponseCache;

        public AuthenticatorRoleProvider(string userId, IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver, IEntityJsonProvider entityJsonProvider = null)
        {
            // If service host is the public unity services gateway
            if (serviceHostResolver is ServiceHostResolver unityServiceHostResolver && unityServiceHostResolver.GetResolvedHost().EndsWith("services.api.unity.com"))
            {
                // Switch to using the internal unity services gateway host
                serviceHostResolver = unityServiceHostResolver.CreateCopyWithDomainResolverOverride(new UnityServicesDomainResolver(true));
            }

            m_ServiceHostResolver = serviceHostResolver;
            m_ServiceHttpClient = serviceHttpClient;
            m_EntityJsonProvider = entityJsonProvider;
            m_UserId = userId;

            m_GetRequestResponseCache = new GetRequestResponseCache<IEnumerable<EntityJson>>(60);
        }

        async Task<IEnumerable<EntityJson>> GetUserEntityRoles(string entityId, string entityType)
        {
            if (m_EntityJsonProvider != null)
            {
                return m_EntityJsonProvider.GetEntityJsonAsync(entityId, entityType);
            }
            var url = m_ServiceHostResolver.GetResolvedRequestUri($"/api/access/legacy/v1/users/{m_UserId}/entities?entityType={entityType}&entityId={entityId}&filterByEntityType[]={entityType}");
            if (m_GetRequestResponseCache.TryGetRequestResponseFromCache(url, out IEnumerable<EntityJson> value))
            {
                return value;
            }

            // First time, or time to refresh if cached result has expired
            var response = await m_ServiceHttpClient.GetAsync(url);
            var entityJsons = await response.JsonDeserializeAsync<IEnumerable<EntityJson>>();
            return m_GetRequestResponseCache.AddGetRequestResponseToCache(url, entityJsons);
        }

        public async Task<IEnumerable<Role>> ListEntityRolesAsync(string entityId, string entityType)
        {
            var entityListJson = await GetUserEntityRoles(entityId, entityType);
            var roles = new List<Role>();
            foreach (var entity in entityListJson)
                roles.AddRange(entity.Policies.Select(policy => new Role(policy.RoleName)));

            return roles;
        }

        public async Task<IEnumerable<Permission>> ListEntityPermissionsAsync(string entityId, string entityType)
        {
            var entityListJson = await GetUserEntityRoles(entityId, entityType);
            var permissions = new List<Permission>();
            foreach (var entity in entityListJson)
            {
                permissions.AddRange(entity.Policies.SelectMany(policy => policy.RolePermissions).ToList().ConvertAll(rolePermission => new Permission(rolePermission)));
            }

            return permissions;
        }
    }
}

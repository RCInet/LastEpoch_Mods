using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// An <see cref="IAuthenticator"/> implementation that supports service account.
    /// </summary>
    class ServiceAccountAuthenticator : IAuthenticator
    {
        internal static readonly string s_ServiceAccountKeyName = "UNITY_SERVICE_ACCOUNT_CREDENTIALS";
        internal static readonly string s_SystemOverrideServiceAccountOrganizationIdVariableName = "UNITY_CLOUD_SERVICE_ACCOUNT_ORGANIZATION_ID";
        internal static readonly string s_SystemOverrideServiceAccountProjectIdVariableName = "UNITY_CLOUD_SERVICE_ACCOUNT_PROJECT_ID";
        internal static readonly string s_SystemOverrideServiceAccountOrganizationNameVariableName = "UNITY_CLOUD_SERVICE_ACCOUNT_ORGANIZATION_NAME";
        internal static readonly string s_SystemOverrideServiceAccountProjectNameVariableName = "UNITY_CLOUD_SERVICE_ACCOUNT_PROJECT_NAME";

        class ServiceAccountCloudStorageJsonProvider : ICloudStorageJsonProvider
        {
            public async Task<CloudStorageUsageJson> GetCloudStorageUsageAsync(CancellationToken cancellationToken)
            {
                return await Task.FromResult(new CloudStorageUsageJson());
            }

            public async Task<CloudStorageEntitlementsJson> GetCloudStorageEntitlementsAsync(CancellationToken cancellationToken)
            {
                return await Task.FromResult(new CloudStorageEntitlementsJson());
            }
        }

        class ServiceAccountOrganizationProjectsJsonProvider : IOrganizationProjectsJsonProvider
        {
            readonly ProjectDescriptor m_ProjectDescriptor;
            readonly string m_ProjectName;

            public ServiceAccountOrganizationProjectsJsonProvider(ProjectDescriptor projectDescriptor, string projectName)
            {
                m_ProjectDescriptor = projectDescriptor;
                m_ProjectName = projectName;
            }

            public async IAsyncEnumerable<ProjectJson> GetOrganizationProjectsJson(OrganizationId organizationId, IEntityRoleProvider entityRoleProvider,
                Range range, [EnumeratorCancellation] CancellationToken cancellationToken)
            {
                var projectJson = new ProjectJson()
                {
                    OrganizationGenesisId = organizationId.ToString(),
                    GenesisId = m_ProjectDescriptor.ProjectId.ToString(),
                    Id = m_ProjectDescriptor.ProjectId.ToString(),
                    Name = m_ProjectName,
                    EnabledInAssetManager = true
                };
                yield return await Task.FromResult(projectJson);
            }
        }

        class ServiceAccountEntityRoleProvider : IEntityRoleProvider
        {
            public async Task<IEnumerable<Role>> ListEntityRolesAsync(string entityId, string entityType)
            {
                return await Task.FromResult(new List<Role>());
            }

            public async Task<IEnumerable<Permission>> ListEntityPermissionsAsync(string entityId, string entityType)
            {
                return await Task.FromResult(new List<Permission>());
            }
        }

        class ServiceAccountMemberJsonProvider : IMemberInfoJsonProvider
        {
            public async IAsyncEnumerable<MemberInfoJson> GetMemberInfoJsonAsync(Range range, [EnumeratorCancellation] CancellationToken cancellationToken)
            {
                await Task.Yield();
                yield break;
            }
        }

        class ServiceAccountGuestProjectJsonProvider : IGuestProjectJsonProvider
        {
            public async IAsyncEnumerable<ProjectJson> GetGuestProjectsAsync(Range range, [EnumeratorCancellation] CancellationToken cancellationToken)
            {
                await Task.Yield();
                yield break;
            }
        }

        class ServiceAccountAssetProjectsJsonProvider : IAssetProjectsJsonProvider
        {
            public async IAsyncEnumerable<AssetProjectJson> GetAssetProjectsJsonAsync([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                await Task.Yield();
                yield break;
            }
        }

        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<ServiceAccountAuthenticator>();

        /// <inheritdoc/>
        public event Action<AuthenticationState> AuthenticationStateChanged;

        AuthenticationState m_AuthenticationState = AuthenticationState.AwaitingInitialization;

        readonly IAccessTokenExchanger<ServiceAccountCredentials, UnityServicesToken> m_UnityServicesTokenExchanger;
        UnityServicesToken m_UnityServicesToken;

        /// <inheritdoc/>
        public AuthenticationState AuthenticationState
        {
            get => m_AuthenticationState;
            private set
            {
                if (value == m_AuthenticationState)
                {
                    return;
                }

                if (value == AuthenticationState.LoggedOut)
                {
                    m_UnityServicesToken = null;
                }

                m_AuthenticationState = value;
                AuthenticationStateChanged?.Invoke(m_AuthenticationState);
            }
        }

        readonly IAuthenticationPlatformSupport m_AuthenticationPlatformSupport;
        readonly ServiceAccountAuthenticatorSettings m_ServiceAccountAuthenticatorSettings;
        readonly IServiceHostResolver m_ServiceHostResolver;
        readonly IJwtDecoder m_JwtDecoder;
        readonly string m_AuthorizationScheme;
        readonly OrganizationId m_OrganizationId;
        readonly ProjectDescriptor m_ProjectDescriptor;
        readonly SemaphoreSlim m_RefreshAccessTokenSemaphore;
        readonly IOrganizationRepository m_OrganizationRepository;
        readonly IUserInfoProvider m_UserInfoProvider;

        IServiceHttpClient m_ServiceHttpClient;

        string m_ServiceAccountOrganizationId { get; } = Environment.GetEnvironmentVariable(s_SystemOverrideServiceAccountOrganizationIdVariableName, EnvironmentVariableTarget.Process);
        string m_ServiceAccountProjectId { get; } = Environment.GetEnvironmentVariable(s_SystemOverrideServiceAccountProjectIdVariableName, EnvironmentVariableTarget.Process);
        string m_ServiceAccountOrganizationName { get; set;  } = Environment.GetEnvironmentVariable(s_SystemOverrideServiceAccountOrganizationNameVariableName, EnvironmentVariableTarget.Process);
        string m_ServiceAccountProjectName { get; set; } = Environment.GetEnvironmentVariable(s_SystemOverrideServiceAccountProjectNameVariableName, EnvironmentVariableTarget.Process);

        // supports ServiceAccountAuthorizer key:secret formatted string as fallback
        ServiceAccountCredentials m_ServiceAccountCredentials { get; set; } = new (Environment.GetEnvironmentVariable(s_ServiceAccountKeyName, EnvironmentVariableTarget.Process));

        DateTime? m_TokenExpiry;

        /// <summary>
        /// Provides a <see cref="ServiceAccountAuthenticator"/> that accepts a <see cref="ServiceAccountAuthenticatorSettings"/> to handle authentication with service account credentials.
        /// </summary>
        /// <param name="serviceAccountAuthenticatorSettings">A <see cref="ServiceAccountAuthenticatorSettings"/> that contains the parameters required for constructing the authenticator.</param>
        public ServiceAccountAuthenticator(ServiceAccountAuthenticatorSettings serviceAccountAuthenticatorSettings) :
            this(serviceAccountAuthenticatorSettings, null, null)
        {
        }

        internal ServiceAccountAuthenticator(ServiceAccountAuthenticatorSettings serviceAccountAuthenticatorSettings, IOrganizationRepository organizationRepository, IUserInfoProvider userInfoProvider)
        {
            // Use environment variables override for the service account available entities if defined
            if (HasServiceAccountEntityOverrideDefined())
            {
                m_OrganizationId = new OrganizationId(m_ServiceAccountOrganizationId);
                m_ProjectDescriptor = new ProjectDescriptor(new OrganizationId(m_ServiceAccountOrganizationId), new ProjectId(m_ServiceAccountProjectId));
                m_ServiceAccountOrganizationName ??= "Undefined";
                m_ServiceAccountProjectName ??= "Undefined";
            }

            m_OrganizationRepository = organizationRepository;
            m_UserInfoProvider = userInfoProvider;

            m_ServiceAccountAuthenticatorSettings = serviceAccountAuthenticatorSettings;
            m_AuthenticationPlatformSupport = serviceAccountAuthenticatorSettings.AuthenticationPlatformSupport;
            m_UnityServicesTokenExchanger = serviceAccountAuthenticatorSettings.AccessTokenExchanger;
            m_ServiceHostResolver = serviceAccountAuthenticatorSettings.ServiceHostResolver;
            m_JwtDecoder = serviceAccountAuthenticatorSettings.JwtDecoder;
            // If no exchange, use basic auth scheme
            m_AuthorizationScheme = m_UnityServicesTokenExchanger == null
                ? ServiceHeaderUtils.k_BasicScheme
                : ServiceHeaderUtils.k_BearerScheme;

            // Like ServiceAccountAuthorizer, it is possible to set the service account credentials set via a launch argument called -UNITY_SERVICE_ACCOUNT_CREDENTIALS
            if (m_AuthenticationPlatformSupport != null && m_AuthenticationPlatformSupport.ActivationKeyValue.Count > 0 && m_AuthenticationPlatformSupport.ActivationKeyValue.TryGetValue(s_ServiceAccountKeyName, out var value))
            {
                s_Logger.LogDebug($"Service account credentials provided from CLI -{s_ServiceAccountKeyName} key value pair");
                m_ServiceAccountCredentials = new (value);
                // Set the environment variable value for the process
                Environment.SetEnvironmentVariable(s_ServiceAccountKeyName, m_ServiceAccountCredentials.ToString(), EnvironmentVariableTarget.Process);
            }

            m_RefreshAccessTokenSemaphore = new SemaphoreSlim(1, 1);
        }

        /// <inheritdoc/>
        public async Task<bool> HasValidPreconditionsAsync()
        {
            return await Task.FromResult(m_ServiceAccountCredentials != ServiceAccountCredentials.None);
        }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            AuthenticationState = AuthenticationState.AwaitingInitialization;

            if (!await HasValidPreconditionsAsync())
            {
                throw new InvalidOperationException("Missing environment variables credentials values for the service account.");
            }

            if (m_UnityServicesTokenExchanger != null)
            {
                await RefreshUnityServicesTokenAsync();
            }
            else
            {
                m_UnityServicesToken = new UnityServicesToken()
                {
                    AccessToken = m_ServiceAccountCredentials.ToBase64String(),
                };
            }

            m_ServiceHttpClient = new ServiceHttpClient(m_ServiceAccountAuthenticatorSettings.HttpClient, this,
                m_ServiceAccountAuthenticatorSettings.AppIdProvider);

            if (!string.IsNullOrEmpty(m_AuthenticationPlatformSupport.ActivationUrl))
            {
                s_Logger.LogInformation($"intercepting URL: {m_AuthenticationPlatformSupport.ActivationUrl}");
                m_AuthenticationPlatformSupport.UrlRedirectionInterceptor.InterceptAwaitedUrl(m_AuthenticationPlatformSupport.ActivationUrl);
            }

            if (m_UnityServicesToken != null)
            {
                s_Logger.LogInformation($"Logged In");

                AuthenticationState = AuthenticationState.LoggedIn;
            }

            if (AuthenticationState == AuthenticationState.AwaitingInitialization)
            {
                AuthenticationState = AuthenticationState.LoggedOut;
            }
        }

        DateTime? ConvertTimestamp(int timestamp)
        {
            if (timestamp == 0) return null;
            var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
            return dateTimeOffset.UtcDateTime;
        }

        async Task RefreshUnityServicesTokenAsync()
        {
            if (m_UnityServicesTokenExchanger != null)
            {
                if (m_TokenExpiry == null)
                {
                    await ExchangeCredentialsForToken();
                }
                else
                {
                    await m_RefreshAccessTokenSemaphore.WaitAsync();
                    var dif = (DateTime)m_TokenExpiry - DateTime.UtcNow;
                    if (dif.TotalSeconds < 30)
                    {
                        try
                        {
                            await ExchangeCredentialsForToken();
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                    m_RefreshAccessTokenSemaphore.Release();
                }
            }
        }

        async Task ExchangeCredentialsForToken()
        {
            m_UnityServicesToken = await m_UnityServicesTokenExchanger.ExchangeAsync(m_ServiceAccountCredentials);
            var decodedToken = m_JwtDecoder.Decode(m_UnityServicesToken.AccessToken);
            var tokenExpiry = decodedToken.exp;
            m_TokenExpiry = ConvertTimestamp(tokenExpiry);
        }

        bool HasServiceAccountEntityOverrideDefined()
        {
            return !string.IsNullOrEmpty(m_ServiceAccountOrganizationId) && !string.IsNullOrEmpty(m_ServiceAccountProjectId);
        }

        /// <inheritdoc cref="Unity.Cloud.CommonEmbedded.IServiceAuthorizer.AddAuthorization"/>
        public async Task AddAuthorization(HttpHeaders headers)
        {
            if (m_UnityServicesTokenExchanger != null)
            {
                await RefreshUnityServicesTokenAsync();
            }

            headers.AddAuthorization(m_UnityServicesToken.AccessToken, m_AuthorizationScheme);
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<IOrganization> ListOrganizationsAsync(Range range, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (HasServiceAccountEntityOverrideDefined())
            {
                yield return await InternalGetOrganizationAsync(m_OrganizationId);
            }
            else
            {
                if (m_OrganizationRepository == null) yield break;
                await foreach (var organization in m_OrganizationRepository.ListOrganizationsAsync(range, cancellationToken)) yield return organization;
            }
        }

        /// <inheritdoc/>
        public async Task<IOrganization> GetOrganizationAsync(OrganizationId organizationId)
        {
            if (HasServiceAccountEntityOverrideDefined())
                return await InternalGetOrganizationAsync(m_OrganizationId);

            return await m_OrganizationRepository.GetOrganizationAsync(organizationId);
        }

        async Task<IOrganization> InternalGetOrganizationAsync(OrganizationId organizationId)
        {
            var organizationJson = new OrganizationJson()
            {
                GenesisId = organizationId.ToString(),
                Id = organizationId.ToString(),
                Name = m_ServiceAccountOrganizationName,
                Role = Role.User.ToString()
            };

            return await Task.FromResult(new Organization(organizationJson, m_ServiceHttpClient, m_ServiceHostResolver,
                new ServiceAccountOrganizationProjectsJsonProvider(m_ProjectDescriptor, m_ServiceAccountProjectName),
                new ServiceAccountEntityRoleProvider(),
                new ServiceAccountGuestProjectJsonProvider(),
                new ServiceAccountAssetProjectsJsonProvider(),
                new ServiceAccountMemberJsonProvider(),
                new ServiceAccountCloudStorageJsonProvider()));
        }

        /// <inheritdoc/>
        public async Task<IUserInfo> GetUserInfoAsync()
        {
            if (m_UserInfoProvider != null)
            {
                return await m_UserInfoProvider.GetUserInfoAsync();
            }

            throw new NotImplementedException("IUserInfo provider not yet implemented for Service Account.");
        }
    }
}

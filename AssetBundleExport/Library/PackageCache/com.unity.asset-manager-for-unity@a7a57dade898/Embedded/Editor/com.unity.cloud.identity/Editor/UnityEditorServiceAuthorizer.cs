using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.AppLinkingEmbedded.Runtime;
using Unity.Cloud.CommonEmbedded;
using Unity.Cloud.CommonEmbedded.Runtime;
using UnityEditor;
using UnityEngine;

namespace Unity.Cloud.IdentityEmbedded.Editor
{
    /// <summary>
    /// An <see cref="Unity.Cloud.CommonEmbedded.IServiceAuthorizer"/> implementation that supports domain reload in the Unity Editor.
    /// </summary>
    class UnityEditorServiceAuthorizer : ScriptableSingleton<UnityEditorServiceAuthorizer>, IServiceAuthorizer,
        IAuthenticationStateProvider, IUserInfoProvider, IOrganizationRepository, ISerializationCallbackReceiver
    {
        /// <inheritdoc/>
        public AuthenticationState AuthenticationState
        {
            get => m_AuthenticationState;
            private set
            {
                if (m_AuthenticationState == value)
                    return;
                m_AuthenticationState = value;
                AuthenticationStateChanged?.Invoke(m_AuthenticationState);
            }
        }

        [SerializeField] AuthenticationState m_AuthenticationState;

        /// <inheritdoc/>
        public event Action<AuthenticationState> AuthenticationStateChanged;

        string AccessToken
        {
            get => m_AccessToken;
            set => m_AccessToken = value;
        }

        [SerializeField] string m_AccessToken;

        string UnityServicesToken
        {
            get => m_UnityServicesToken;
            set => m_UnityServicesToken = value;
        }

        [SerializeField] string m_UnityServicesToken;

        double m_LastExchangeRequestCheck;

        const double k_ExchangeRequestRetryDelayInSeconds = 0.5;

        const string k_UnityHubUriScheme = "unityhub://";
        const string k_UnityHubLoginDomain = "login";

        IAccessTokenExchanger<TargetClientIdToken, UnityServicesToken>
            m_TargetClientIdTokenToUnityServicesTokenExchanger;

        AuthenticatedUserSession m_AuthenticatedUserSession;

        IUnityEditorAccessTokenProvider m_UnityEditorAccessTokenProvider;

        IUnityUserInfoJsonProvider m_UnityUserInfoJsonProvider;
        IGuestProjectJsonProvider m_GuestProjectJsonProvider;
        IOrganizationJsonProvider m_OrganizationJsonProvider;
        IJwtDecoder m_JwtDecoder;

        bool m_UseOverride = false;
        Task<string> m_GetAccessTokenTask;

        DateTime? m_TokenExpiry;
        [NonSerialized] bool m_AwaitingExchangeOperation;

        internal void OverrideUnityEditorServiceAuthorizer(
            IAccessTokenExchanger<TargetClientIdToken, UnityServicesToken> accessTokenExchanger,
            IUnityEditorAccessTokenProvider unityEditorAccessTokenProvider,
            IUnityUserInfoJsonProvider unityUserInfoJsonProvider = null,
            IGuestProjectJsonProvider guestProjectJsonProvider = null,
            IOrganizationJsonProvider organizationJsonProvider = null,
            IJwtDecoder jwtDecoder = null)
        {
            m_TargetClientIdTokenToUnityServicesTokenExchanger = accessTokenExchanger;
            m_UnityEditorAccessTokenProvider = unityEditorAccessTokenProvider;
            m_UnityUserInfoJsonProvider = unityUserInfoJsonProvider;
            m_GuestProjectJsonProvider = guestProjectJsonProvider;
            m_OrganizationJsonProvider = organizationJsonProvider;
            m_JwtDecoder = jwtDecoder;

            m_UseOverride = true;
            InitAuthenticatedUserSession();
        }

        void OnRefreshAccessToken(bool isRefreshed)
        {
            Debug.Log($"Cloud project settings access token refreshed.");
        }

        void OnEnable()
        {
            m_LastExchangeRequestCheck = EditorApplication.timeSinceStartup;

            if (AuthenticationState.Equals(AuthenticationState.AwaitingInitialization))
            {
                AuthenticationState = AuthenticationState.LoggedOut;
            }

            EditorApplication.update += Update;
        }

        void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        void InitAuthenticatedUserSession()
        {
            var playerSettings = UnityCloudPlayerSettings.Instance;
            var httpClient = new UnityHttpClient();
            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();

            m_JwtDecoder ??= new JwtDecoder();
            if (string.IsNullOrEmpty(UnityServicesToken)) return;
            var jwt = m_JwtDecoder.Decode(UnityServicesToken);
            var tokenExpiry = jwt.exp;
            var userId = jwt.sub;
            m_TokenExpiry = ConvertTimestamp(tokenExpiry);

            if (m_UseOverride)
            {
                m_AuthenticatedUserSession = new AuthenticatedUserSession(userId,
                    new ServiceHttpClient(httpClient, this, playerSettings),
                    serviceHostResolver,
                    m_UnityUserInfoJsonProvider,
                    m_GuestProjectJsonProvider,
                    m_OrganizationJsonProvider
                );
            }
            else
            {
                m_AuthenticatedUserSession = new AuthenticatedUserSession(userId, new ServiceHttpClient(httpClient, this, playerSettings), serviceHostResolver);
            }
        }

        async void Update()
        {
            if (!AccessTokenAvailable(out var accessToken))
                return;

            var editorHasToken = !string.IsNullOrEmpty(accessToken);

            // If editor token was refreshed
            if (editorHasToken && AccessToken != accessToken)
            {
                AccessToken = accessToken;
                AuthenticationState = AuthenticationState.AwaitingLogin;
            }
            if (AuthenticationState.Equals(AuthenticationState.LoggedIn) && !editorHasToken)
            {
                UnityServicesToken = string.Empty;
                m_TokenExpiry = null;
                AuthenticationState = AuthenticationState.LoggedOut;
            }
            else if (AuthenticationState.Equals(AuthenticationState.LoggedOut) && editorHasToken)
            {
                AuthenticationState = AuthenticationState.AwaitingLogin;
            }
            else if (AuthenticationState.Equals(AuthenticationState.AwaitingLogin) && editorHasToken)
            {
                await RefreshUnityTokenFromAccessTokenAsync();
            }
        }

        async Task RefreshUnityTokenFromAccessTokenAsync()
        {
            // Throttle request to retry token exchange until connectivity is restored
            if (EditorApplication.timeSinceStartup - m_LastExchangeRequestCheck < k_ExchangeRequestRetryDelayInSeconds)
                return;

            m_LastExchangeRequestCheck = EditorApplication.timeSinceStartup;
            if (!m_AwaitingExchangeOperation)
            {
                m_AwaitingExchangeOperation = true;
                try
                {
                    Debug.Log("Try refresh the unity services token.");
                    await RefreshUnityTokenAsync(AccessToken);
                    if (!string.IsNullOrEmpty(UnityServicesToken))
                    {
                        if (!m_UseOverride)
                        {
                            var jwt = new JwtDecoder().Decode(UnityServicesToken);
                            var userId = jwt.sub;
                            var tokenExpiry = jwt.exp;
                            m_TokenExpiry = ConvertTimestamp(tokenExpiry);

                            var playerSettings = UnityCloudPlayerSettings.Instance;
                            var httpClient = new UnityHttpClient();
                            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();

                            Debug.Log($"Unity services token refreshed.");

                            m_AuthenticatedUserSession =
                                new AuthenticatedUserSession(userId,
                                    new ServiceHttpClient(httpClient, this, playerSettings),
                                    serviceHostResolver);
                        }
                        AuthenticationState = AuthenticationState.LoggedIn;
                    }

                }
                catch (HttpRequestException)
                {
                    /* silent fail */
                }
                finally
                {
                    m_AwaitingExchangeOperation = false;
                }
            }
        }

        bool AccessTokenAvailable(out string accessToken)
        {
            if (m_UseOverride)
            {
                accessToken = string.Empty;
                if (m_UnityEditorAccessTokenProvider == null)
                    return false;

                if (m_GetAccessTokenTask == null)
                {
                    m_GetAccessTokenTask = m_UnityEditorAccessTokenProvider.GetAccessTokenAsync();
                    return false;
                }

                if (!m_GetAccessTokenTask.IsCompleted)
                    return false;

                accessToken = m_GetAccessTokenTask.Result;
                m_GetAccessTokenTask = null;
                return true;
            }
            accessToken = CloudProjectSettings.accessToken;
            return true;
        }

        async Task RefreshUnityTokenAsync(string accessToken)
        {
            if (m_TargetClientIdTokenToUnityServicesTokenExchanger == null)
            {
                if (!m_UseOverride)
                {
                    var httpClient = new UnityHttpClient();
                    var serviceHostResolver = ServiceHostResolverFactory.Create();
                    m_TargetClientIdTokenToUnityServicesTokenExchanger = new TargetClientIdTokenToUnityServicesTokenExchanger(httpClient, serviceHostResolver);
                }
                InitAuthenticatedUserSession();
            }

            var targetClientIdToken = new TargetClientIdToken { token = accessToken};
            var exchangedToken = await m_TargetClientIdTokenToUnityServicesTokenExchanger.ExchangeAsync(targetClientIdToken);

            UnityServicesToken = exchangedToken.AccessToken;
        }

        /// <inheritdoc cref="Unity.Cloud.CommonEmbedded.IServiceAuthorizer.AddAuthorization"/>
        public async Task AddAuthorization(HttpHeaders headers)
        {
#if UNITY_EDITOR
            ValidateOrRevokeUnityServicesToken();

            // yield until UnityServicesToken is reset if undefined
            while (string.IsNullOrEmpty(UnityServicesToken))
            {
                await Task.Yield();
            }

            headers.AddAuthorization(UnityServicesToken, ServiceHeaderUtils.k_BearerScheme);
            await Task.CompletedTask;
#else
            throw new InvalidOperationException(k_InvalidOperationMessage);
#endif
        }

        void ValidateOrRevokeUnityServicesToken()
        {
            // Revoke any existing unity services token if expiry time is less than 30 seconds
            if (string.IsNullOrEmpty(UnityServicesToken) || m_TokenExpiry == null) return;
            var dif = (DateTime)m_TokenExpiry - DateTime.UtcNow;
            // When Unity Hub is running in background, it refreshes the access token 60 seconds before its expiry time
            // If the unity token is still not refreshed 30 seconds before its expiry time, we assume he Unity Hub is not running
            // We manually revoke the unity token and activate the Unity Hub to initiate the access token refresh.
            if ((dif.TotalSeconds > 30)) return;
            Debug.Log("Revoking Unity services token. Awaiting the Unity Hub to refresh the Unity Editor cloud project settings.");
            RevokeAndForceRefreshUnityServicesToken();
        }

        void RevokeAndForceRefreshUnityServicesToken()
        {
            UnityServicesToken = string.Empty;
            m_TokenExpiry = null;

            // Activation of the Hub to initiate the token refresh
            CloudProjectSettings.RefreshAccessToken(OnRefreshAccessToken);
            Application.OpenURL($"{k_UnityHubUriScheme}{k_UnityHubLoginDomain}");
        }

        DateTime? ConvertTimestamp(int timestamp)
        {
            if (timestamp == 0) return null;
            var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
            return dateTimeOffset.UtcDateTime;
        }

        /// <inheritdoc/>
        public async Task<IUserInfo> GetUserInfoAsync()
        {
            ThrowIfInvalidAuthenticatedUserSessionState();
            return await m_AuthenticatedUserSession.GetUserInfoAsync();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<IOrganization> ListOrganizationsAsync(Range range, CancellationToken cancellationToken = default)
        {
            ThrowIfInvalidAuthenticatedUserSessionState();
            return m_AuthenticatedUserSession.ListOrganizationsAsync(range, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IOrganization> GetOrganizationAsync(OrganizationId organizationId)
        {
            ThrowIfInvalidAuthenticatedUserSessionState();
            return await m_AuthenticatedUserSession.GetOrganizationAsync(organizationId);
        }

        void ThrowIfInvalidAuthenticatedUserSessionState()
        {
            if (!m_AuthenticationState.Equals(AuthenticationState.LoggedIn))
            {
                throw new InvalidOperationException("Cannot reach Cloud service, awaiting user Access Token.");
            }

            if (m_AuthenticatedUserSession == null)
            {
                InitAuthenticatedUserSession();
            }
        }

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            // Nothing to do before serialization occurs
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
            if (AuthenticationState.Equals(AuthenticationState.AwaitingInitialization))
            {
                AuthenticationState = AuthenticationState.LoggedOut;
            }
        }
    }
}

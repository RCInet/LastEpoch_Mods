using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.AppLinkingEmbedded.Runtime;
using Unity.Cloud.CommonEmbedded;
using Unity.Cloud.CommonEmbedded.Runtime;
using Unity.Cloud.IdentityEmbedded.Runtime;
using UnityEditor;
using UnityEngine;

namespace Unity.Cloud.IdentityEmbedded.Editor
{
    /// <summary>
    /// An <see cref="IAuthenticator"/> implementation to access the user authenticated session in the Unity Editor.
    /// </summary>
    [Obsolete("Deprecated in favor of the UnityEditorServiceAuthorizer.")]
class UnityEditorAuthenticator : IAuthenticator, IDisposable
    {
#if !UNITY_EDITOR
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<UnityEditorAuthenticator>();
        const string k_InvalidOperationMessage = "This class can only be used in the Unity Editor execution context.";
#endif
        AuthenticationState m_AuthenticationState = AuthenticationState.AwaitingInitialization;

        string m_EditorAccessToken = string.Empty;

        /// <inheritdoc/>
        public event Action<AuthenticationState> AuthenticationStateChanged;

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

        readonly IAccessTokenExchanger<TargetClientIdToken, UnityServicesToken> m_TargetClientIdTokenToUnityServicesTokenExchanger;
        UnityServicesToken m_UnityServicesToken;

        AuthenticatedUserSession m_AuthenticatedUserSession;

        readonly IUnityEditorAccessTokenProvider m_UnityEditorAccessTokenProvider;

        /// <summary>
        /// Returns an <see cref="IAuthenticator"/> implementation that expects an access token from a Unity Editor environment.
        /// </summary>
        public UnityEditorAuthenticator()
            : this(new TargetClientIdTokenToUnityServicesTokenExchanger(new UnityHttpClient(), UnityRuntimeServiceHostResolverFactory.Create()), new CloudProjectSettingsUnityEditorAccessTokenProvider())
        {
        }

        /// <summary>
        /// Returns an <see cref="IAuthenticator"/> implementation that expects an access token from a Unity Editor environment.
        /// </summary>
        /// <param name="accessTokenExchanger">An <see cref="IAccessTokenExchanger{T1, T2}"/> where the T1 input is a <see cref="TargetClientIdToken"/> and T2 output is a <see cref="UnityServicesToken"/></param>
        public UnityEditorAuthenticator(IAccessTokenExchanger<TargetClientIdToken, UnityServicesToken> accessTokenExchanger)
        : this(accessTokenExchanger, new CloudProjectSettingsUnityEditorAccessTokenProvider())
        {
        }

        /// <summary>
        /// Returns an <see cref="IAuthenticator"/> implementation that expects an access token from a Unity Editor environment.
        /// </summary>
        /// <param name="accessTokenExchanger">An <see cref="IAccessTokenExchanger{T1, T2}"/> where the T1 input is a <see cref="TargetClientIdToken"/> and T2 output is a <see cref="UnityServicesToken"/></param>
        /// <param name="unityEditorAccessTokenProvider">An <see cref="IUnityEditorAccessTokenProvider"/> reference.</param>
        public UnityEditorAuthenticator(IAccessTokenExchanger<TargetClientIdToken, UnityServicesToken> accessTokenExchanger, IUnityEditorAccessTokenProvider unityEditorAccessTokenProvider)
        {
            m_TargetClientIdTokenToUnityServicesTokenExchanger = accessTokenExchanger;
            m_UnityEditorAccessTokenProvider = unityEditorAccessTokenProvider;

            m_AuthenticatedUserSession = RefreshAuthenticatedUserSession();

#if !UNITY_EDITOR
            s_Logger.LogWarning(k_InvalidOperationMessage);
#endif
        }

        AuthenticatedUserSession RefreshAuthenticatedUserSession(string userId = null)
        {
            var httpClient = new UnityHttpClient();
            var playerSettings = UnityCloudPlayerSettings.Instance;
            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();

            return new AuthenticatedUserSession(userId,
                new ServiceHttpClient(httpClient, this,
                    playerSettings), serviceHostResolver);
        }

        async void Update()
        {
            var accessToken = await m_UnityEditorAccessTokenProvider.GetAccessTokenAsync();
            var isLoggedIn = !string.IsNullOrEmpty(accessToken);
            if (m_AuthenticationState.Equals(IdentityEmbedded.AuthenticationState.LoggedIn) && !isLoggedIn)
            {
                m_UnityServicesToken = null;
                AuthenticationState = AuthenticationState.LoggedOut;
            }
            else if (m_AuthenticationState.Equals(IdentityEmbedded.AuthenticationState.LoggedOut) && isLoggedIn)
            {
                AuthenticationState = AuthenticationState.AwaitingLogin;

                m_EditorAccessToken = accessToken;

                if (m_TargetClientIdTokenToUnityServicesTokenExchanger != null)
                {
                    var targetClientIdToken = new TargetClientIdToken() { token = m_EditorAccessToken};
                    m_UnityServicesToken =
                        await m_TargetClientIdTokenToUnityServicesTokenExchanger.ExchangeAsync(targetClientIdToken);

                    var userId = new JwtDecoder().Decode(m_UnityServicesToken.AccessToken).sub;

                    m_AuthenticatedUserSession = RefreshAuthenticatedUserSession(userId);
                }

                AuthenticationState = AuthenticationState.LoggedIn;
            }
        }

        /// <summary>
        /// Disposes of any `IDisposable` references.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of any `IDisposable` references.
        /// </summary>
        /// <param name="disposing">Dispose pattern boolean value received from public Dispose method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
#if UNITY_EDITOR
                EditorApplication.update -= Update;
#endif
            }
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<IOrganization> ListOrganizationsAsync(Range range, CancellationToken cancellationToken = default)
        {
            return m_AuthenticatedUserSession.ListOrganizationsAsync(range, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IOrganization> GetOrganizationAsync(OrganizationId organizationId)
        {
            return await m_AuthenticatedUserSession.GetOrganizationAsync(organizationId);
        }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
#if UNITY_EDITOR

#pragma warning disable S2696 // Instance members should not write to static fields
            EditorApplication.update += Update;
#pragma warning restore S2696

            var accessToken = await m_UnityEditorAccessTokenProvider.GetAccessTokenAsync();
            if (!string.IsNullOrEmpty(accessToken))
            {
                m_EditorAccessToken = accessToken;

                if (m_TargetClientIdTokenToUnityServicesTokenExchanger != null)
                {
                    var targetClientIdToken = new TargetClientIdToken() { token = accessToken};
                    m_UnityServicesToken =
                        await m_TargetClientIdTokenToUnityServicesTokenExchanger.ExchangeAsync(targetClientIdToken);
                }

                AuthenticationState = AuthenticationState.LoggedIn;
            }
            else
            {
                AuthenticationState = AuthenticationState.LoggedOut;
            }
#else
            throw new InvalidOperationException(k_InvalidOperationMessage);
#endif
            await Task.CompletedTask;
        }

        /// <summary>
        /// Indicates if the <see cref="UnityEditorAuthenticator"/> running instance has access to an access token from the Unity Editor environment.
        /// </summary>
        /// <returns>If the <see cref="UnityEditorAuthenticator"/> running instance has access to an access token from the Unity Editor environment.</returns>
        public Task<bool> HasValidPreconditionsAsync()
        {
#if UNITY_EDITOR
            return Task.FromResult(true);
#else
            return Task.FromResult(false);
#endif
        }

        /// <inheritdoc cref="IServiceAuthorizer.AddAuthorization"/>
        public async Task AddAuthorization(HttpHeaders headers)
        {
#if UNITY_EDITOR
            var accessToken = await m_UnityEditorAccessTokenProvider.GetAccessTokenAsync();
            if (!m_EditorAccessToken.Equals(accessToken))
            {
                m_EditorAccessToken = accessToken;
                if (m_TargetClientIdTokenToUnityServicesTokenExchanger != null)
                {
                    var targetClientIdToken = new TargetClientIdToken() { token = m_EditorAccessToken};
                    m_UnityServicesToken =
                        await m_TargetClientIdTokenToUnityServicesTokenExchanger.ExchangeAsync(targetClientIdToken);
                }
            }

            headers.AddAuthorization(
                m_TargetClientIdTokenToUnityServicesTokenExchanger != null
                    ? m_UnityServicesToken.AccessToken
                    : accessToken, ServiceHeaderUtils.k_BearerScheme);
#else
            throw new InvalidOperationException(k_InvalidOperationMessage);
#endif
        }

        /// <inheritdoc/>
        public async Task<IUserInfo> GetUserInfoAsync()
        {
            return await m_AuthenticatedUserSession.GetUserInfoAsync();
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Unity.Cloud.AppLinkingEmbedded;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{

    /// <summary>
    /// Provides authentication through PKCE (Proof Key Code Exchange) standards.
    /// </summary>
    /// <code source="../../Samples/Documentation/Scripting/PkceAuthenticatorExample.cs" region="PkceAuthenticator"/>
    class PkceAuthenticator : IUrlRedirectionAuthenticator, IDisposable
    {
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<PkceAuthenticator>();

        static readonly string s_BaseDeviceTokenFileName = "devicetoken.data";
        static readonly string s_CodeVerifierFileName = "codeVerifier";
        static readonly string s_CachedActivationUrl = "cached_activation_url";

        static readonly string s_InvalidQueryArgumentsMessage = "The redirect query arguments are invalid; they must include a state and code.";
        static readonly string s_StateMismatchMessage = "Request returned state does not match original request state.";
        static readonly string s_AuthorizationFailedMessage = "Authorization failed with exception: ";

        static readonly List<string> s_AwaitedQueryArguments = new List<string>() { "code", "state" };
        static readonly string s_StateCancelled = "cancelled";

        /// <inheritdoc/>
        public event Action<AuthenticationState> AuthenticationStateChanged;

        LazyPkceAccessTokenRefresher m_AccessTokenRefresher = null;

        AuthenticationState m_AuthenticationState = AuthenticationState.AwaitingInitialization;

        readonly string m_DeviceTokenFileName;

        readonly IAccessTokenExchanger<string, UnityServicesToken> m_UnityServicesTokenExchanger;
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
                    if (m_AuthenticationPlatformSupport.SecretCacheStore != null)
                        _ = m_AuthenticationPlatformSupport.SecretCacheStore.DeleteCacheAsync(m_DeviceTokenFileName);

                    m_AccessTokenRefresher?.Dispose();
                    m_AccessTokenRefresher = null;

                    m_UnityServicesToken = null;
                    m_AuthenticatedUserSession = null;
                }

                m_AuthenticationState = value;
                AuthenticationStateChanged?.Invoke(m_AuthenticationState);
            }
        }

        readonly PkceAuthenticatorSettings m_PkceAuthenticatorSettings;
        readonly IPkceConfigurationProvider m_PkceConfigurationProvider;
        readonly IAuthenticationPlatformSupport m_AuthenticationPlatformSupport;
        readonly IPkceRequestHandler m_PkceRequestHandler;
        readonly IAppNamespaceProvider m_AppNamespaceProvider = new DefaultAppNamespaceProvider();

        AuthenticatedUserSession m_AuthenticatedUserSession;

        readonly IOrganizationRepository m_OrganizationRepository;
        readonly IUserInfoProvider m_UserInfoProvider;
        readonly IJwtDecoder m_JwtDecoder;

        /// <summary>
        /// Provides a <see cref="PkceAuthenticator"/> that accepts a <see cref="PkceAuthenticatorSettings"/> to handle Proof Key Code Exchange (PKCE) authentication contexts.
        /// </summary>
        /// <param name="pkceAuthenticatorSettings">A <see cref="PkceAuthenticatorSettings"/> that contains the parameters required for constructing the authenticator.</param>
        /// <param name="organizationRepository">An optional <see cref="IOrganizationRepository"/>.</param>
        /// <param name="userInfoProvider">An optional <see cref="IUserInfoProvider"/>.</param>
        public PkceAuthenticator(PkceAuthenticatorSettings pkceAuthenticatorSettings, IOrganizationRepository organizationRepository = null, IUserInfoProvider userInfoProvider = null)
        {
            m_PkceAuthenticatorSettings = pkceAuthenticatorSettings;
            m_JwtDecoder = pkceAuthenticatorSettings.JwtDecoder;
            m_AuthenticationPlatformSupport = pkceAuthenticatorSettings.AuthenticationPlatformSupport;
            m_PkceConfigurationProvider = pkceAuthenticatorSettings.PkceConfigurationProvider;
            m_PkceRequestHandler = pkceAuthenticatorSettings.PkceRequestHandler;
            m_UnityServicesTokenExchanger = pkceAuthenticatorSettings.AccessTokenExchanger;
            m_AppNamespaceProvider = pkceAuthenticatorSettings.AppNamespaceProvider;
            m_OrganizationRepository = organizationRepository;
            m_UserInfoProvider = userInfoProvider;

            m_DeviceTokenFileName = BuildDeviceTokenFileNameFromHostConfiguration(pkceAuthenticatorSettings.ServiceHostResolver);
        }

        internal PkceAuthenticator(IAuthenticationPlatformSupport authenticationPlatformSupport, IHttpClient httpClient, IPkceRequestHandler pkceRequestHandler, IServiceHostResolver serviceHostResolver)
        {
            var pkceConfigurationProvider = new PkceConfigurationProvider(serviceHostResolver);
            pkceRequestHandler ??= new HttpPkceRequestHandler(httpClient, pkceConfigurationProvider);

            m_UnityServicesTokenExchanger = new AccessTokenToUnityServicesTokenExchanger(httpClient, serviceHostResolver);
            m_PkceConfigurationProvider = pkceConfigurationProvider;
            m_AuthenticationPlatformSupport = authenticationPlatformSupport;
            m_PkceRequestHandler = pkceRequestHandler;

            m_DeviceTokenFileName = BuildDeviceTokenFileNameFromHostConfiguration(serviceHostResolver);
        }

        static string BuildDeviceTokenFileNameFromHostConfiguration(IServiceHostResolver serviceHostResolver)
        {
            return string.Concat($"{serviceHostResolver?.GetResolvedHost()}.", s_BaseDeviceTokenFileName);
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
            if (disposing && m_AccessTokenRefresher != null)
            {
                m_AccessTokenRefresher?.Dispose();
            }
        }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            AuthenticationState = AuthenticationState.AwaitingInitialization;

            var pkceConfiguration = await m_PkceConfigurationProvider.GetPkceConfigurationAsync();

            // From fresh recovered appConfiguration we should now decide if reviving a session is allowed
            if (pkceConfiguration.CacheRefreshToken)
            {
                try
                {
                    await TryReviveSessionAsync(pkceConfiguration);
                }
                catch (Exception ex)
                {
                    s_Logger.LogInformation($"Failed to revive session. User will need to login manually. {ex.Message}");
                    await m_AuthenticationPlatformSupport.SecretCacheStore.DeleteCacheAsync(m_DeviceTokenFileName);
                }
            }

            // Process ActivationUrl, if any
            var activationUrl = m_AuthenticationPlatformSupport.ActivationUrl;
            if (!string.IsNullOrEmpty(activationUrl))
            {
                s_Logger.LogDebug($"ActivationUrl detected: {activationUrl}");

                // If no user and url is a login response (WebGL context)
                if (m_AccessTokenRefresher == null && ActivationUrlHasCodeAndStateParams(activationUrl))
                {
                    if (await CompleteLoginFromActivationUrlAsync(activationUrl, pkceConfiguration))
                    {
                        // After login, look at awaiting cached activation url
                        try
                        {
                            var cachedActivationUrl = await m_AuthenticationPlatformSupport.SecretCacheStore.ReadCacheAsync(s_CachedActivationUrl);

                            if (!string.IsNullOrEmpty(cachedActivationUrl))
                            {
                                // Remove as soon as detected
                                await m_AuthenticationPlatformSupport.SecretCacheStore.DeleteCacheAsync(s_CachedActivationUrl);
                                s_Logger.LogDebug($"ActivationUrl detected from cache: {cachedActivationUrl}");

                                m_AuthenticationPlatformSupport.UrlRedirectionInterceptor.InterceptAwaitedUrl(cachedActivationUrl);
                            }
                        }
                        catch (FileNotFoundException)
                        {
                            s_Logger.LogDebug("ActivationUrl could not be found in cache.");
                        }
                    }
                }
                else
                {
                    m_AuthenticationPlatformSupport.UrlRedirectionInterceptor.InterceptAwaitedUrl(activationUrl);
                }
            }

            if (AuthenticationState == AuthenticationState.AwaitingInitialization)
            {
                AuthenticationState = AuthenticationState.LoggedOut;
            }
        }

        async Task<bool> CompleteLoginFromActivationUrlAsync(string activationUrl, PkceConfiguration pkceConfiguration)
        {
            s_Logger.LogDebug($"Completing login...");

            if (m_AuthenticationPlatformSupport.CodeVerifierCacheStore == null)
                return false;

            string codeVerifier = null;
            try
            {
                codeVerifier = await m_AuthenticationPlatformSupport.CodeVerifierCacheStore.ReadCacheAsync(s_CodeVerifierFileName);
                // Delete as soon as we read it back
                await m_AuthenticationPlatformSupport.CodeVerifierCacheStore.DeleteCacheAsync(s_CodeVerifierFileName);
            }
            catch (FileNotFoundException)
            {
                s_Logger.LogDebug("CodeVerifier could not be found in cache.");
            }

            if (string.IsNullOrEmpty(codeVerifier))
                return false;

            AuthenticationState = AuthenticationState.AwaitingLogin;

            if (Uri.TryCreate(activationUrl, UriKind.Absolute, out Uri activationUri))
            {
                var queryArguments = QueryArgumentsParser.GetDictionaryFromString(activationUri.Query.Substring(1));

                var redirectResultCode = string.Empty;
                queryArguments?.TryGetValue("code", out redirectResultCode);

                var redirectUri = await m_AuthenticationPlatformSupport.GetRedirectUriAsync("login");

                var appNamespace = m_AppNamespaceProvider.GetAppNamespace();
                var requestStringParam = PkceHelper.CreateTokenUrlRequestStringContent(appNamespace, redirectResultCode, codeVerifier, redirectUri, pkceConfiguration);

                await OnReceivedNonceCodeAsync(pkceConfiguration, requestStringParam);
            }

            return true;
        }

        static bool ActivationUrlHasCodeAndStateParams(string activationUrl)
        {
            var uriQuery = new Uri(activationUrl).Query;
            if (string.IsNullOrEmpty(uriQuery))
            {
                return false;
            }
            var queryArgs = QueryArgumentsParser.GetDictionaryFromString(uriQuery.Substring(1));
            return queryArgs.ContainsKey("state") && queryArgs.ContainsKey("code");
        }

        async Task TryReviveSessionAsync(PkceConfiguration pkceConfiguration)
        {
            if (m_AuthenticationPlatformSupport.SecretCacheStore == null)
                return;

            string refreshToken = null;
            try
            {
                refreshToken = await m_AuthenticationPlatformSupport.SecretCacheStore.ReadCacheAsync(m_DeviceTokenFileName);
            }
            catch (FileNotFoundException e)
            {
                s_Logger.LogDebug($"Token could not be found in cache: {e.Message}");
            }

            if (string.IsNullOrEmpty(refreshToken))
                return;

            DeviceToken newDeviceToken = null;
            newDeviceToken = await m_PkceRequestHandler.RefreshTokenAsync(PkceHelper.CreateRefreshTokenUrlRequestStringContent(refreshToken, pkceConfiguration), refreshToken);

            if (newDeviceToken != null && !string.IsNullOrEmpty(newDeviceToken.AccessToken))
            {
                s_Logger.LogDebug("Revived access token from cached refresh token.");
                await RegisterNewDeviceTokenAsync(pkceConfiguration, newDeviceToken);
            }
            else
            {
                s_Logger.LogDebug("Invalid refresh token from cache. Awaiting manual logging.");
                await m_AuthenticationPlatformSupport.SecretCacheStore.DeleteCacheAsync(m_DeviceTokenFileName);
            }
        }

        /// <inheritdoc/>
        public async Task LoginAsync()
        {
            if (AuthenticationState == AuthenticationState.AwaitingInitialization)
                throw new InvalidOperationException("PkceAuthenticator was not properly initialized");

            if (AuthenticationState != AuthenticationState.LoggedOut)
                throw new InvalidOperationException("Attempting login when already logged in.");

            AuthenticationState = AuthenticationState.AwaitingLogin;

            var pkceConfiguration = await m_PkceConfigurationProvider.GetPkceConfigurationAsync();

            PkceHelper.GenerateChallengeVerifier(out var codeVerifier, out var codeChallenge);

            if (m_AuthenticationPlatformSupport.CodeVerifierCacheStore != null)
                await m_AuthenticationPlatformSupport.CodeVerifierCacheStore.WriteToCacheAsync(s_CodeVerifierFileName, codeVerifier);

            var stateOverride = m_AuthenticationPlatformSupport.GetAppStateOverride();
            var state = string.IsNullOrEmpty(stateOverride)
                ? PkceHelper.GenerateState()
                : stateOverride;

            var redirectUri = await m_AuthenticationPlatformSupport.GetRedirectUriAsync("login");
            var appNamespace = m_AppNamespaceProvider.GetAppNamespace();
            var url = PkceHelper.CreateAuthenticateUrl(appNamespace, state, codeChallenge, redirectUri, pkceConfiguration, stateOverride);

            UrlRedirectResult urlRedirectResult;

            try
            {
                urlRedirectResult = await m_AuthenticationPlatformSupport.OpenUrlAndWaitForRedirectAsync(url, s_AwaitedQueryArguments);
            }
            catch (TimeoutException e)
            {
                AuthenticationState = AuthenticationState.LoggedOut;
                throw new AuthenticationFailedException(s_AuthorizationFailedMessage, e);
            }
            catch (Exception)
            {
                // If any exception occurs, the user should be considered logged out
                AuthenticationState = AuthenticationState.LoggedOut;
                throw;
            }

            switch (urlRedirectResult.Status)
            {
                case UrlRedirectStatus.NotApplicable:
                    break;
                case UrlRedirectStatus.Success:
                    ValidateQueryArguments(urlRedirectResult.QueryArguments, s_AwaitedQueryArguments);
                    var requestStringParam = PkceHelper.CreateTokenUrlRequestStringContent(appNamespace, urlRedirectResult.QueryArguments["code"], codeVerifier, redirectUri, pkceConfiguration, stateOverride);
                    await OnUrlRedirectSuccess(pkceConfiguration, urlRedirectResult, state, requestStringParam);
                    break;
            }
        }

        void ValidateQueryArguments(Dictionary<string, string> queryArguments,  List<string> awaitedQueryArguments)
        {
            if ( queryArguments == null || !ValidateAllKeysExistInDictionary(queryArguments, awaitedQueryArguments))
            {
                AuthenticationState = AuthenticationState.LoggedOut;
                throw new AuthenticationFailedException(s_InvalidQueryArgumentsMessage);
            }
        }

        static bool ValidateAllKeysExistInDictionary(Dictionary<string, string> queryArguments, List<string> awaitedQueryArguments)
        {
            foreach (var awaitedQueryArgument in awaitedQueryArguments)
            {
                if (!queryArguments.ContainsKey(awaitedQueryArgument))
                {
                    return false;
                }
            }
            return true;
        }

        async Task OnUrlRedirectSuccess(PkceConfiguration pkceConfiguration, UrlRedirectResult urlRedirectResult, string state, string requestStringParam)
        {
            if (!urlRedirectResult.QueryArguments["state"].Equals(state))
            {
                AuthenticationState = AuthenticationState.LoggedOut;
                if (urlRedirectResult.QueryArguments["state"].Equals(s_StateCancelled))
                {
                    s_Logger.LogDebug($"User manually cancelled the login operation.");
                }
                else
                {
                    throw new AuthenticationFailedException(s_StateMismatchMessage);
                }
            }
            else
            {
                await OnReceivedNonceCodeAsync(pkceConfiguration, requestStringParam);
            }
        }

        /// <inheritdoc/>
        public void CancelLogin()
        {
            if (AuthenticationState == AuthenticationState.AwaitingInitialization)
                throw new InvalidOperationException("PkceAuthenticator was not properly initialized");

            if (AuthenticationState != AuthenticationState.AwaitingLogin)
                throw new InvalidOperationException("Attempting cancellation when not awaiting login.");

            m_AuthenticationPlatformSupport.UrlRedirectionInterceptor.InterceptAwaitedUrl(m_AuthenticationPlatformSupport.GetCancellationUri(), s_AwaitedQueryArguments);
        }

        /// <inheritdoc/>
        public async Task LogoutAsync(bool clearBrowserCache = false)
        {
            if (AuthenticationState == AuthenticationState.AwaitingInitialization)
                throw new InvalidOperationException("PkceAuthenticator was not properly initialized");

            if (AuthenticationState != AuthenticationState.LoggedIn)
                throw new InvalidOperationException("Attempting logout when already logged out.");

            AuthenticationState = AuthenticationState.AwaitingLogout;

            var pkceConfiguration = await m_PkceConfigurationProvider.GetPkceConfigurationAsync();

            try
            {
                await RevokeRefreshTokenAsync(pkceConfiguration, m_AccessTokenRefresher?.DeviceToken.RefreshToken);
            }
            catch (Exception ex)
            {
                // Silent fail, token was not revoked, but we still want to log the user out
                s_Logger.LogDebug($"EX: {ex}");
            }

            if (clearBrowserCache)
            {
                await SignOut(pkceConfiguration);
            }

            AuthenticationState = AuthenticationState.LoggedOut;
        }

        async Task SignOut(PkceConfiguration pkceConfiguration)
        {
            if (string.IsNullOrEmpty(pkceConfiguration.SignOutUrl))
            {
                AuthenticationState = AuthenticationState.LoggedOut;
                throw new InvalidOperationException("Missing SignOut URL definition, cannot clear browser cache.");
            }

            var stateOverride = m_AuthenticationPlatformSupport.GetAppStateOverride();
            var state = string.IsNullOrEmpty(stateOverride)
                ? PkceHelper.GenerateState()
                : stateOverride;

            var redirectUri = await m_AuthenticationPlatformSupport.GetRedirectUriAsync("signout");
            var appNamespace = m_AppNamespaceProvider.GetAppNamespace();
            var url = PkceHelper.CreateSignOutUrl(appNamespace, state, redirectUri, pkceConfiguration, stateOverride);

            UrlRedirectResult urlRedirectResult;

            var awaitedQueryArgument = new List<string> { "state" };
            try
            {
                urlRedirectResult = await m_AuthenticationPlatformSupport.OpenUrlAndWaitForRedirectAsync(url, awaitedQueryArgument);
            }
            catch (TimeoutException e)
            {
                AuthenticationState = AuthenticationState.LoggedOut;
                throw new AuthenticationFailedException(s_AuthorizationFailedMessage, e);
            }
            catch (Exception)
            {
                // If any exception occurs, the user should be considered logged out
                AuthenticationState = AuthenticationState.LoggedOut;
                throw;
            }

            switch (urlRedirectResult.Status)
            {
                case UrlRedirectStatus.NotApplicable:
                    break;
                case UrlRedirectStatus.Success:
                    ValidateQueryArguments(urlRedirectResult.QueryArguments, awaitedQueryArgument);
                    if (!urlRedirectResult.QueryArguments["state"].Equals(state))
                    {
                        throw new AuthenticationFailedException(s_StateMismatchMessage);
                    }
                    break;
            }
        }

        async Task OnReceivedNonceCodeAsync(PkceConfiguration pkceConfiguration, string exchangeCodeForDeviceTokenParams)
        {
            DeviceToken newDeviceToken;
            try
            {
                newDeviceToken = await m_PkceRequestHandler.ExchangeCodeForDeviceTokenAsync(exchangeCodeForDeviceTokenParams);
            }
            catch (Exception)
            {
                AuthenticationState = AuthenticationState.LoggedOut;
                throw;
            }

            s_Logger.LogDebug($"Access Token provided from successful PKCE authentication flow.");
            await RegisterNewDeviceTokenAsync(pkceConfiguration, newDeviceToken);
        }

        async Task RegisterNewDeviceTokenAsync(PkceConfiguration pkceConfiguration, DeviceToken deviceToken)
        {
            if (pkceConfiguration.CacheRefreshToken && m_AuthenticationPlatformSupport.SecretCacheStore != null)
                await m_AuthenticationPlatformSupport.SecretCacheStore?.WriteToCacheAsync(m_DeviceTokenFileName, deviceToken.RefreshToken);

            m_UnityServicesToken = await m_UnityServicesTokenExchanger.ExchangeAsync(deviceToken.AccessToken);

            var userId = m_JwtDecoder.Decode(m_UnityServicesToken.AccessToken).sub;

            m_AuthenticatedUserSession = new AuthenticatedUserSession(userId,
                new ServiceHttpClient(m_PkceAuthenticatorSettings.HttpClient, this,
                    m_PkceAuthenticatorSettings.AppIdProvider), m_PkceAuthenticatorSettings.ServiceHostResolver);

            m_AccessTokenRefresher = new LazyPkceAccessTokenRefresher(deviceToken, m_PkceRequestHandler, pkceConfiguration);

            AuthenticationState = AuthenticationState.LoggedIn;
        }

        async Task RevokeRefreshTokenAsync(PkceConfiguration pkceConfiguration, string refreshToken)
        {
            await m_PkceRequestHandler.RevokeRefreshTokenAsync(PkceHelper.CreateRevokeRefreshTokenUrlRequestStringContent(refreshToken, pkceConfiguration));
        }

        /// <inheritdoc cref="Unity.Cloud.CommonEmbedded.IServiceAuthorizer.AddAuthorization"/>
        public async Task AddAuthorization(HttpHeaders headers)
        {
            if (m_AccessTokenRefresher != null && await m_AccessTokenRefresher.ShouldRefreshAccessToken())
            {
                var newDeviceToken = await m_AccessTokenRefresher.RefreshAccessTokenAsync();

                m_UnityServicesToken = await m_UnityServicesTokenExchanger.ExchangeAsync(newDeviceToken.AccessToken);
            }

            headers.AddAuthorization(m_UnityServicesToken.AccessToken, ServiceHeaderUtils.k_BearerScheme);
        }

        /// <inheritdoc/>
        public Task<bool> HasValidPreconditionsAsync()
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<IOrganization> ListOrganizationsAsync(Range range, CancellationToken cancellationToken = default)
        {
            if (m_OrganizationRepository != null)
            {
                return m_OrganizationRepository.ListOrganizationsAsync(range, cancellationToken);
            }
            return m_AuthenticatedUserSession.ListOrganizationsAsync(range, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IOrganization> GetOrganizationAsync(OrganizationId organizationId)
        {
            if (m_OrganizationRepository != null)
            {
                return await m_OrganizationRepository.GetOrganizationAsync(organizationId);
            }
            return await m_AuthenticatedUserSession.GetOrganizationAsync(organizationId);
        }

        /// <inheritdoc/>
        public async Task<IUserInfo> GetUserInfoAsync()
        {
            if (m_UserInfoProvider != null)
            {
                return await m_UserInfoProvider.GetUserInfoAsync();
            }
            return await m_AuthenticatedUserSession.GetUserInfoAsync();
        }
    }

    class LazyPkceAccessTokenRefresher : IDisposable
    {
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<LazyPkceAccessTokenRefresher>();

        public DeviceToken DeviceToken { get; private set; }
        DateTime m_DeviceTokenRetrievalTime;

        readonly IPkceRequestHandler m_PkceRequestHandler;
        readonly PkceConfiguration m_PkceConfiguration;
        readonly SemaphoreSlim m_GetAccessTokenSemaphore;

        public event Action<DeviceToken> DeviceTokenRefreshed;

        public LazyPkceAccessTokenRefresher(DeviceToken deviceToken, IPkceRequestHandler pkceRequestHandler, PkceConfiguration pkceConfiguration)
        {
            m_PkceRequestHandler = pkceRequestHandler;
            m_PkceConfiguration = pkceConfiguration;

            DeviceToken = deviceToken;
            m_DeviceTokenRetrievalTime = DateTime.Now;

            m_GetAccessTokenSemaphore = new SemaphoreSlim(1, 1);
        }

        public async Task<bool> ShouldRefreshAccessToken()
        {
            // The token should be refreshed within 1 minute of its expiration time
            await m_GetAccessTokenSemaphore.WaitAsync();
            var isExpired = DateTime.Now > m_DeviceTokenRetrievalTime + DeviceToken.AccessTokenExpiresIn - TimeSpan.FromSeconds(60);
            if (!isExpired)
            {
                m_GetAccessTokenSemaphore.Release();
            }
            return isExpired;
        }

        public async Task<DeviceToken> RefreshAccessTokenAsync()
        {
            try
            {
                DeviceToken = await RefreshDeviceTokenAsync(DeviceToken.RefreshToken);
                m_DeviceTokenRetrievalTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                s_Logger.LogDebug(ex.Message);
            }
            finally
            {
                m_GetAccessTokenSemaphore.Release();
            }
            return DeviceToken;
        }

        async Task<DeviceToken> RefreshDeviceTokenAsync(string refreshToken)
        {
            var deviceToken = await m_PkceRequestHandler.RefreshTokenAsync(PkceHelper.CreateRefreshTokenUrlRequestStringContent(refreshToken, m_PkceConfiguration), refreshToken);

            DeviceTokenRefreshed?.Invoke(deviceToken);

            return deviceToken;
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
                m_GetAccessTokenSemaphore?.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{

    /// <summary>
    /// An <see cref="IAuthenticator"/> implementation that expects an access token from a browser environment.
    /// </summary>
    /// <example>
    /// <code source="../../Samples/Documentation/Scripting/BrowserAuthenticatedAccessTokenProviderExample.cs" region="BrowserAuthenticatedAccessTokenProvider"/>
    /// </example>
    class BrowserAuthenticatedAccessTokenProvider : IAuthenticator
    {
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<BrowserAuthenticatedAccessTokenProvider>();

        /// <summary>
        /// Default list of supported domains with corresponding localStorage key names.
        /// </summary>
        public static Dictionary<string, string> DefaultLocalStorageKeyNames =>
            new()
            {
                { "dev.staging.cloud.unity.com", "genesis-access-token-staging" },
                { "staging.cloud.unity.com", "genesis-access-token-staging" },
                { "cloud.unity.com", "genesis-access-token" }
            };

        readonly string m_LocalStorageKeyName;

        AuthenticationState m_AuthenticationState = AuthenticationState.AwaitingInitialization;

        /// <inheritdoc/>
        public event Action<AuthenticationState> AuthenticationStateChanged;

        /// <inheritdoc/>
        public AuthenticationState AuthenticationState
        {
            get => m_AuthenticationState;
            private set
            {
                m_AuthenticationState = value;
                AuthenticationStateChanged?.Invoke(m_AuthenticationState);
            }
        }

        readonly IAuthenticationPlatformSupport m_AuthenticationPlatformSupport;

        readonly IAccessTokenExchanger<string, UnityServicesToken> m_UnityServicesTokenExchanger;
        UnityServicesToken m_UnityServicesToken;

        string m_SessionBrowserAccessTokenValue;

        readonly IPkceRequestHandler m_PkceRequestHandler;

        AuthenticatedUserSession m_AuthenticatedUserSession;

        readonly PkceAuthenticatorSettings m_PkceAuthenticatorSettings;
        readonly IOrganizationRepository m_OrganizationRepository;
        readonly IUserInfoProvider m_UserInfoProvider;
        readonly IJwtDecoder m_JwtDecoder;

        /// <summary>
        /// Returns an <see cref="IAuthenticator"/> implementation that expects an access token from a browser environment.
        /// </summary>
        /// <remarks>The `BrowserAuthenticatedAccessTokenProvider` tries to match the running host location with the location provided in the `localStorageKeyNames` dictionary. Use a single wildcard character (*) to match any host location.</remarks>
        /// <param name="pkceAuthenticatorSettings">The <see cref="PkceAuthenticatorSettings"/> that contains all PKCE authentication classes</param>
        /// <param name="localStorageKeyNames">A dictionary with browser locations as keys and local storage key names as values.</param>
        /// <param name="organizationRepository">An optional <see cref="IOrganizationRepository"/>.</param>
        /// <param name="userInfoProvider">An optional <see cref="IUserInfoProvider"/>.</param>
        public BrowserAuthenticatedAccessTokenProvider(PkceAuthenticatorSettings pkceAuthenticatorSettings, Dictionary<string, string> localStorageKeyNames = null, IOrganizationRepository organizationRepository = null, IUserInfoProvider userInfoProvider = null)
        {
            m_PkceAuthenticatorSettings = pkceAuthenticatorSettings;
            m_JwtDecoder = pkceAuthenticatorSettings.JwtDecoder;
            localStorageKeyNames ??= DefaultLocalStorageKeyNames;
            m_AuthenticationPlatformSupport = pkceAuthenticatorSettings.AuthenticationPlatformSupport;
            m_PkceRequestHandler = pkceAuthenticatorSettings.PkceRequestHandler;
            m_UnityServicesTokenExchanger = pkceAuthenticatorSettings.AccessTokenExchanger;

            m_LocalStorageKeyName = GetHostAccessTokenFilename(localStorageKeyNames);

            m_OrganizationRepository = organizationRepository;
            m_UserInfoProvider = userInfoProvider;
        }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            try
            {
                m_SessionBrowserAccessTokenValue = await m_AuthenticationPlatformSupport.SecretCacheStore.ReadCacheAsync(m_LocalStorageKeyName);

                if (!string.IsNullOrEmpty(m_SessionBrowserAccessTokenValue))
                {
                    s_Logger.LogDebug("genesis Access Token provided from a browser environment.");
                   await RefreshUnityServicesToken();
                }

                AuthenticationState = string.IsNullOrEmpty(m_SessionBrowserAccessTokenValue) ? AuthenticationState.LoggedOut : AuthenticationState.LoggedIn;

                if (!string.IsNullOrEmpty(m_AuthenticationPlatformSupport.ActivationUrl))
                {
                    m_AuthenticationPlatformSupport.UrlRedirectionInterceptor.InterceptAwaitedUrl(m_AuthenticationPlatformSupport.ActivationUrl);
                }
            }
            catch (FileNotFoundException)
            {
                s_Logger.LogDebug("Token could not be found in cache.");
                AuthenticationState = AuthenticationState.LoggedOut;
            }
        }

        async Task RefreshUnityServicesToken()
        {
            m_UnityServicesToken = await m_UnityServicesTokenExchanger.ExchangeAsync(m_SessionBrowserAccessTokenValue);

            var userId = m_JwtDecoder.Decode(m_UnityServicesToken.AccessToken).sub;

            m_AuthenticatedUserSession = new AuthenticatedUserSession(userId,
                new ServiceHttpClient(m_PkceAuthenticatorSettings.HttpClient, this,
                    m_PkceAuthenticatorSettings.AppIdProvider), m_PkceAuthenticatorSettings.ServiceHostResolver);

        }

        /// <inheritdoc cref="Unity.Cloud.CommonEmbedded.IServiceAuthorizer.AddAuthorization"/>
        public async Task AddAuthorization(HttpHeaders headers)
        {
            try
            {
                var browserAccessTokenValue = await m_AuthenticationPlatformSupport.SecretCacheStore.ReadCacheAsync(m_LocalStorageKeyName);
                if (string.IsNullOrEmpty(browserAccessTokenValue))
                {
                    throw new InvalidOperationException($"Missing '{m_LocalStorageKeyName}' value from browser local storage.");
                }

                if (!browserAccessTokenValue.Equals(m_SessionBrowserAccessTokenValue))
                {
                    m_SessionBrowserAccessTokenValue = browserAccessTokenValue;
                    await RefreshUnityServicesToken();
                }

                headers.AddAuthorization(m_UnityServicesToken.AccessToken, ServiceHeaderUtils.k_BearerScheme);
            }
            catch (FileNotFoundException)
            {
                throw new InvalidOperationException($"Missing '{m_LocalStorageKeyName}' value from browser local storage.");
            }
        }

        /// <summary>
        /// Indicates if the <see cref="BrowserAuthenticatedAccessTokenProvider"/> running instance has access to an access token from the browser environment.
        /// </summary>
        /// <returns>If the <see cref="BrowserAuthenticatedAccessTokenProvider"/> running instance has access to an access token.</returns>
        public Task<bool> HasValidPreconditionsAsync()
        {
            if (!string.IsNullOrEmpty(m_LocalStorageKeyName))
            {
                return m_AuthenticationPlatformSupport.SecretCacheStore.ValidateFilenameExistsAsync(m_LocalStorageKeyName);
            }
            return Task.FromResult(false);
        }

        string GetHostAccessTokenFilename(Dictionary<string, string> keyNameDictionary)
        {
            var aboutBlankIframeSrcValue = "about:blank";
            if (!string.IsNullOrEmpty(m_AuthenticationPlatformSupport.ActivationUrl) && m_AuthenticationPlatformSupport.ActivationUrl.Equals(aboutBlankIframeSrcValue))
            {
                return keyNameDictionary.TryGetValue(aboutBlankIframeSrcValue, out var keyValue) ? keyValue : null;
            }
            return ValidateUrlSrc(keyNameDictionary);
        }

        string ValidateUrlSrc(Dictionary<string, string> keyNameDictionary)
        {
            if (Uri.TryCreate(m_AuthenticationPlatformSupport.ActivationUrl, UriKind.Absolute, out Uri browserUri))
            {
                foreach (var kvp in keyNameDictionary)
                {
                    var prefixKey = $"https://{kvp.Key}";
                    if (Uri.TryCreate(prefixKey, UriKind.Absolute, out Uri uriExpected))
                    {
                        var expectedAbsolutePath = $"{uriExpected.Scheme}://{uriExpected.Host}{uriExpected.AbsolutePath}";
                        var browserAbsolutePath = $"{browserUri.Scheme}://{browserUri.Host}{browserUri.AbsolutePath}";
                        if (browserAbsolutePath.StartsWith(expectedAbsolutePath))
                        {
                            return kvp.Value;
                        }
                    }
                }
            }
            return null;
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
}

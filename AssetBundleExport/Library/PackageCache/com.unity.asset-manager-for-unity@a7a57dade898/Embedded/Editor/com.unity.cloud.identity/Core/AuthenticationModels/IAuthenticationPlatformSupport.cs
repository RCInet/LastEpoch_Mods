using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Cloud.AppLinkingEmbedded;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// This interface abstracts common handling of platform-specific features in URL redirection flows.
    /// </summary>
    interface IAuthenticationPlatformSupport : IActivatePlatformSupport
    {
        /// <summary>
        /// The <see cref="IUrlRedirectionInterceptor"/> monitoring URL redirection requests.
        /// </summary>
        IUrlRedirectionInterceptor UrlRedirectionInterceptor { get; }

        /// <summary>
        /// Gets a string override for the default random state parameter used in authenticated redirection flows.
        /// </summary>
        /// <returns>
        /// This method returns a string override of the default random state or null if no override is defined.
        /// </returns>
        string GetAppStateOverride();

        /// <summary>
        /// Creates a pending task that starts by opening a URL in a browser and is completed when response is intercepted, validated and returns a <see cref="UrlRedirectResult"/>.
        /// </summary>
        /// <param name="url">The URL to open. It must trigger a redirection to the URI referenced by <see cref="GetRedirectUri"/>.</param>
        /// <param name="awaitedQueryArguments">The list of query arguments to validate when receiving the awaited callback url.</param>
        /// <returns>
        /// A task that results in a <see cref="UrlRedirectResult"/> when completed.
        /// </returns>
        /// <exception cref="TimeoutException">Thrown if no redirect occurred within the allotted time limit.</exception>
        Task<UrlRedirectResult> OpenUrlAndWaitForRedirectAsync(string url, List<string> awaitedQueryArguments = null);

        /// <summary>
        /// Exports the token and its token type used to authorize Service endpoints call.
        /// </summary>
        /// <param name="type">Authorization type for the token (Bearer or Basic).</param>
        /// <param name="token">The token string value.</param>
        void ExportServiceAuthorizerToken(string type, string token);

        /// <summary>
        /// Gets the redirection URI expected from the browser when calling <see cref="OpenUrlAndWaitForRedirectAsync"/>.
        /// </summary>
        /// <param name="operation">Optional string to append to the path of the redirect URI</param>
        /// <returns>
        /// The redirection URI expected from the browser when calling <see cref="OpenUrlAndWaitForRedirectAsync"/>.
        /// </returns>
        string GetRedirectUri(string operation = null);

        /// <summary>
        /// Gets the redirection URI expected from the browser when calling <see cref="OpenUrlAndWaitForRedirectAsync"/>.
        /// </summary>
        /// <param name="operation">Optional string to append to the path of the redirect URI</param>
        /// <returns>
        /// The redirection URI expected from the browser when calling <see cref="OpenUrlAndWaitForRedirectAsync"/>.
        /// </returns>
        Task<string> GetRedirectUriAsync(string operation = null);

        /// <summary>
        /// Gets the cancellation URI expected from the browser when calling <see cref="OpenUrlAndWaitForRedirectAsync"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the app isn't awaiting a URL redirection response.</exception>
        /// <returns>
        /// The cancellation URI expected from the browser when calling <see cref="OpenUrlAndWaitForRedirectAsync"/>.
        /// </returns>
        string GetCancellationUri();

        /// <summary>
        /// The secret cache store used in authenticated redirection flow.
        /// </summary>
        IKeyValueStore SecretCacheStore { get; }

        /// <summary>
        /// The code verifier cache store used in authenticated redirection flow.
        /// </summary>
        IKeyValueStore CodeVerifierCacheStore { get; }

        /// <summary>
        /// Processes activation URL to either complete a URL redirection flow or consume an authenticated app resource.
        /// </summary>
        /// <param name="awaitedQueryArguments">The list of query arguments to validate when processing the activation URL.</param>
        void ProcessActivationUrl(List<string> awaitedQueryArguments = null);

        /// <summary>
        /// This method returns the <see cref="UrlRedirectResult"/> captured at app initializing time, or null if none available.
        /// </summary>
        /// <returns>
        /// The <see cref="UrlRedirectResult"/> captured at app initializing time, or null if none available on the specific platform.
        /// </returns>
        UrlRedirectResult? GetRedirectionResult();
    }
}

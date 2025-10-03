using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Unity.Cloud.AppLinkingEmbedded
{
    /// <summary>
    /// This interface abstracts url redirection interception and validation of awaited query arguments.
    /// </summary>
    interface IUrlRedirectionInterceptor : IDisposable
    {
        /// <summary>
        /// Call this method with any incoming activation url and a list of awaited query arguments required for interception.
        /// </summary>
        /// <param name="url">The url received.</param>
        /// <param name="awaitedQueryArguments">The list of query arguments required to validate url.</param>
        /// <exception cref="ArgumentException">
        /// - Thrown when <paramref name="url"/> is null or empty.
        /// - Thrown when <paramref name="url"/> not a valid Uri.
        /// </exception>
        void InterceptAwaitedUrl(string url, List<string> awaitedQueryArguments = null);

        /// <summary>
        /// Triggered when an intercepted uri can be forwarded to a deep link processor.
        /// </summary>
        event Action<Uri> DeepLinkForwarded;

        /// <summary>
        /// Gets a string representation of the current process id or main window id.
        /// </summary>
        /// <returns>
        /// A string representation of the current process id or main window id.
        /// </returns>
        ProcessId GetRedirectProcessId();

        /// <summary>
        /// Gets a nullable <see cref="UrlRedirectResult"/> value.
        /// </summary>
        /// <returns>
        /// A nullable <see cref="UrlRedirectResult"/> value.
        /// </returns>
        UrlRedirectResult? GetRedirectionResult();

        /// <summary>
        /// Returns a Task awaiting a <see cref="UrlRedirectResult"/>.
        /// </summary>
        /// <param name="awaitedQueryArguments">The list of query arguments to validate when receiving the awaited callback url.</param>
        /// <returns>
        /// A Task that results in a <see cref="UrlRedirectResult"/> when completed.
        /// </returns>
        /// <exception cref="TimeoutException">Thrown if no redirect occured within the allotted time limit.</exception>
        Task<UrlRedirectResult> AwaitRedirectAsync(List<string> awaitedQueryArguments = null);

        /// <summary>
        /// The list of query arguments to validate when receiving the awaited callback url.
        /// </summary>
        List<string> AwaitedQueryArguments { get; }
    }
}

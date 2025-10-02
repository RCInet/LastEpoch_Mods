using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AppLinkingEmbedded
{
    /// <summary>
    /// An interface that provides utilities for web app resources.
    /// </summary>
    interface IWebAppUrlComposer
    {
        /// <summary>
        /// An awaitable task that returns an absolute URL to a web app resource.
        /// </summary>
        /// <param name="webAppName">The web app name.</param>
        /// <param name="pathAndQuery">The optional path and query to append.</param>
        /// <exception cref="InvalidArgumentException">Thrown when the web app name is not supported on the server.</exception>
        /// <returns>A task that returns an absolute URL to a web app resource.</returns>
        Task<string> ComposeUrlAsync(string webAppName, string pathAndQuery = null);

        /// <summary>
        /// An awaitable task that returns if a web app name is supported on the server.
        /// </summary>
        /// <param name="webAppName">The web app name.</param>
        /// <returns>A task that returns if a web app name is supported on the server.</returns>
        Task<bool> IsWebAppSupportedAsync(string webAppName);
    }
}

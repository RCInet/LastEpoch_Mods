using System;
using System.Reflection;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Helper methods for <see cref="IServiceMessagingClient"/>.
    /// </summary>
    static class IServiceMessagingClientExtensions
    {
        /// <summary>
        /// Modifies an instance of <see cref="IServiceMessagingClient"/> which adds the API source version info to the websocket connection request.
        /// The source values are retrieved from the <see cref="ApiSourceVersionAttribute"/> which must be defined in the calling <see cref="Assembly"/>.
        /// </summary>
        /// <param name="messagingClient">The client to modify.</param>
        /// <param name="assembly">The target assembly.</param>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="ApiSourceVersionAttribute"/> does not exist or is not initialized in the calling assembly.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null or the name or version defined in the retrieved <see cref="ApiSourceVersionAttribute"/> are null or white space.</exception>
        public static void AddApiSourceVersionFromAssembly(this IServiceMessagingClient messagingClient, Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var apiSourceVersion = ApiSourceVersion.GetApiSourceVersionForAssembly(assembly);
            messagingClient.AddApiSourceVersion(apiSourceVersion);
        }
    }
}

using System;
using System.Threading.Tasks;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// This interface abstracts access to a <see cref="PkceConfiguration"/> instance.
    /// </summary>
    interface IPkceConfigurationProvider
    {
        /// <summary>
        /// Create a Task that results in a <see cref="PkceConfiguration"/> when completed.
        /// </summary>
        /// <returns>
        /// A task that results in a <see cref="PkceConfiguration"/> when completed.
        /// </returns>
        Task<PkceConfiguration> GetPkceConfigurationAsync();
    }
}

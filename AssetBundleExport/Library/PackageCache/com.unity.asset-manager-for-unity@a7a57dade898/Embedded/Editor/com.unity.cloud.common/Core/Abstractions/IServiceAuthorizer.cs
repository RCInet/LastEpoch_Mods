using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// This interface abstracts the task of applying authorization information to a given resource.
    /// </summary>
    interface IServiceAuthorizer
    {
        /// <summary>
        /// Applies authorization information to a given set of <see cref="HttpHeaders"/>.
        /// </summary>
        /// <param name="headers">The <see cref="HttpHeaders"/> to add authorization information to.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="headers"/> is null.</exception>
        /// <returns>A <see cref="Task"/> for the operation.</returns>
        Task AddAuthorization(HttpHeaders headers);
    }
}

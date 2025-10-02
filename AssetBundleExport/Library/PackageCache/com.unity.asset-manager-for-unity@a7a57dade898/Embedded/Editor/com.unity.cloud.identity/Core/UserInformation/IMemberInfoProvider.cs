using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// An interface that provides <see cref="IMemberInfo"/>.
    /// </summary>
    interface IMemberInfoProvider
    {
        /// <summary>
        /// An awaitable Task that returns the list of <see cref="IMemberInfo"/>.
        /// </summary>
        /// <param name="range">A range of <see cref="IMemberInfo"/> to request.</param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task whose result is an async enumeration of <see cref="IMemberInfo"/>.</returns>
        public IAsyncEnumerable<IMemberInfo> ListMembersAsync(Range range, CancellationToken cancellationToken = default);

        /// <summary>
        /// An awaitable Task that returns a <see cref="IMemberInfo"/>.
        /// </summary>
        /// <param name="userId">The <see cref="UserId"/> of the member of the <see cref="IOrganization"/> to get.</param>
        /// <returns>A task whose result is a <see cref="IMemberInfo"/>.</returns>
        /// <exception cref="NotFoundException"></exception>
        async Task<IMemberInfo> GetMemberAsync(UserId userId)
        {
            var membersAsyncEnumerable = ListMembersAsync(Range.All);
            await foreach (var member in membersAsyncEnumerable)
            {
                if (member.UserId.Equals(userId))
                {
                    return member;
                }
            }
            throw new NotFoundException();
        }
    }
}

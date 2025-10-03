using System;
using System.Collections.Generic;
using System.Threading;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// An interface containing information about a status flow for an asset.
    /// </summary>
    interface IStatusFlow
    {
        /// <summary>
        /// The descriptor of the status flow.
        /// </summary>
        StatusFlowDescriptor Descriptor { get; }

        /// <summary>
        /// The name of the status flow.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Whether the status flow is a default flow.
        /// </summary>
        bool IsDefault { get; }

        /// <summary>
        /// The id of the starting status.
        /// </summary>
        string StartStatusId { get; }

        /// <summary>
        /// Returns the statuses in the flow.
        /// </summary>
        /// <param name="range">The range of results to return. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>An async enumeration of <see cref="IStatus"/>. </returns>
        IAsyncEnumerable<IStatus> ListStatusesAsync(Range range, CancellationToken cancellationToken);

        /// <summary>
        /// Returns the transitions between statuses.
        /// </summary>
        /// <param name="range">The range of results to return. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>An async enumeration of <see cref="IStatusTransition"/>. </returns>
        IAsyncEnumerable<IStatusTransition> ListTransitionsAsync(Range range, CancellationToken cancellationToken);
    }
}

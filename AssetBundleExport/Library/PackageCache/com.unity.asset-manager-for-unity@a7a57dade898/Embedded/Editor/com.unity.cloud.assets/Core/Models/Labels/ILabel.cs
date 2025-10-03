using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.AssetsEmbedded
{
    interface ILabel
    {
        /// <summary>
        /// The descriptor for the label.
        /// </summary>
        LabelDescriptor Descriptor { get; }

        /// <inheritdoc cref="LabelDescriptor.LabelName"/>
        [Obsolete("Use Descriptor.LabelName instead.")]
        string Name => Descriptor.LabelName;

        /// <summary>
        /// The description of the label.
        /// </summary>
        [Obsolete("Use LabelProperties.Description instead.")]
        string Description { get; }

        /// <summary>
        /// Whether the label is a system label.
        /// </summary>
        [Obsolete("Use LabelProperties.IsSystemLabel instead.")]
        bool IsSystemLabel { get; }

        /// <summary>
        /// Whether the label can be manually assigned to an asset.
        /// </summary>
        [Obsolete("Use LabelProperties.IsAssignable instead.")]
        bool IsAssignable { get; }

        /// <summary>
        /// The authoring information for the label.
        /// </summary>
        [Obsolete("Use LabelProperties.AuthoringInfo instead.")]
        AuthoringInfo AuthoringInfo { get; }

        /// <summary>
        /// The color of the label.
        /// </summary>
        [Obsolete("Use LabelProperties.DisplayColor instead.")]
        Color DisplayColor { get; }

        /// <summary>
        /// The caching configuration for the label.
        /// </summary>
        LabelCacheConfiguration CacheConfiguration => throw new NotImplementedException();

        /// <summary>
        /// Returns a label configured with the specified caching configuration.
        /// </summary>
        /// <param name="labelCacheConfiguration">The caching configuration for the label. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="ILabel"/> with cached values specified by the caching configurations. </returns>
        Task<ILabel> WithCacheConfigurationAsync(LabelCacheConfiguration labelCacheConfiguration, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Fetches the latest changes.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task RefreshAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Returns the properties of the label.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the <see cref="LabelProperties"/> of the label. </returns>
        Task<LabelProperties> GetPropertiesAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Updates the label.
        /// </summary>
        /// <param name="labelUpdate">The object containing information to update the label. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task UpdateAsync(ILabelUpdate labelUpdate, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the label name.
        /// </summary>
        /// <param name="labelName">A new unique name for the label. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task RenameAsync(string labelName, CancellationToken cancellationToken);

        /// <summary>
        /// Archives the label.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task ArchiveAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Unarchives the label.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task UnarchiveAsync(CancellationToken cancellationToken);
    }
}

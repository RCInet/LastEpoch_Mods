using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    partial interface IAssetDataSource
    {
        /// <summary>
        /// Lists the labels for the specified organization.
        /// </summary>
        /// <param name="organizationId">The id of the organization. </param>
        /// <param name="pagination">The range and order of results. </param>
        /// <param name="archived">Whether the results include archived labels. </param>
        /// <param name="systemLabels">Whether the results will include system labels. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>An async enumeration of <see cref="ILabelData"/>. </returns>
        IAsyncEnumerable<ILabelData> ListLabelsAsync(OrganizationId organizationId, PaginationData pagination, bool? archived, bool? systemLabels, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <param name="labelDescriptor">The object containing the necessary information to identify the label. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is an <see cref="ILabelData"/>. </returns>
        Task<ILabelData> GetLabelAsync(LabelDescriptor labelDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new label.
        /// </summary>
        /// <param name="organizationId">The id of the organization. </param>
        /// <param name="labelCreation">The object containing the necessary information to create a label. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is an <see cref="ILabelData"/>. </returns>
        Task<LabelDescriptor> CreateLabelAsync(OrganizationId organizationId, ILabelBaseData labelCreation, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a label.
        /// </summary>
        /// <param name="labelDescriptor">The object containing the necessary information to identify the label. </param>
        /// <param name="labelUpdate">The object containing the information to update the label. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result. </returns>
        Task UpdateLabelAsync(LabelDescriptor labelDescriptor, ILabelBaseData labelUpdate, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the status of a label.
        /// </summary>
        /// <param name="labelDescriptor">The object containing the necessary information to identify the label. </param>
        /// <param name="archive">The status to update to. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result. </returns>
        Task UpdateLabelStatusAsync(LabelDescriptor labelDescriptor, bool archive, CancellationToken cancellationToken);

        /// <summary>
        /// Lists the labels for the asset by asset version.
        /// </summary>
        /// <param name="projectDescriptor">The object containing the necessary information to identify the project. </param>
        /// <param name="assetId">The id of the asset. </param>
        /// <param name="pagination">The range and order of results. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>An async enumeration of <see cref="AssetLabelsDto"/>. </returns>
        IAsyncEnumerable<AssetLabelsDto> ListLabelsAcrossAssetVersions(ProjectDescriptor projectDescriptor, AssetId assetId, PaginationData pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Assigns labels to an asset.
        /// </summary>
        /// <param name="assetDescriptor">The object containing the necessary information to identify the asset.</param>
        /// <param name="labels">The collection of labels to assign. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result. </returns>
        Task AssignLabelsAsync(AssetDescriptor assetDescriptor, IEnumerable<string> labels, CancellationToken cancellationToken);

        /// <summary>
        /// Unassigns labels from an asset.
        /// </summary>
        /// <param name="assetDescriptor">The object containing the necessary information to identify the asset.</param>
        /// <param name="labels">The collection of labels to remove. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result. </returns>
        Task UnassignLabelsAsync(AssetDescriptor assetDescriptor, IEnumerable<string> labels, CancellationToken cancellationToken);
    }
}

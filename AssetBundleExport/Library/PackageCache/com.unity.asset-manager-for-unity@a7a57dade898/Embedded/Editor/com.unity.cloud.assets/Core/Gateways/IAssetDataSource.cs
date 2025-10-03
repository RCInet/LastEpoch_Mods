using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Implement this interface to transform user facing data like <see cref="IAssetData"/> into service DTOs
    /// </summary>
    partial interface IAssetDataSource
    {
        /// <summary>
        /// Retrieves an <see cref="IAssetData"/>.
        /// </summary>
        /// <param name="assetDescriptor">The object containing the necessary information to identify the asset.</param>
        /// <param name="includedFieldsFilter">The fields that should be included in the response. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is a <see cref="IAssetData"/>. </returns>
        Task<IAssetData> GetAssetAsync(AssetDescriptor assetDescriptor, FieldsFilter includedFieldsFilter, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves an <see cref="IAssetData"/>.
        /// </summary>
        /// <param name="projectDescriptor">The object containing the necessary information to identify the project. </param>
        /// <param name="assetId">The id of the asset. </param>
        /// <param name="label">The label associated to the asset version. </param>
        /// <param name="includedFieldsFilter">The fields that should be included in the response. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is a <see cref="IAssetData"/>. </returns>
        Task<IAssetData> GetAssetAsync(ProjectDescriptor projectDescriptor, AssetId assetId, string label, FieldsFilter includedFieldsFilter, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a subset of <see cref="IAssetData"/>.
        /// </summary>
        /// <param name="projectDescriptor">The object containing the necessary information to identify the project. </param>
        /// <param name="parameters">An object containing the parameters of a search. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is a cancellationToken for the next page and a collection of <see cref="IAssetData"/>. </returns>
        IAsyncEnumerable<IAssetData> ListAssetsAsync(ProjectDescriptor projectDescriptor, SearchRequestParameters parameters, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a subset of <see cref="IAssetData"/> across specified projects.
        /// </summary>
        /// <param name="organizationId">The organization id. </param>
        /// <param name="projectIds">The ids of the projects in which to search. </param>
        /// <param name="parameters">An object containing the parameters of a search. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is a cancellationToken for the next page and a collection of <see cref="IAssetData"/>. </returns>
        IAsyncEnumerable<IAssetData> ListAssetsAsync(OrganizationId organizationId, IEnumerable<ProjectId> projectIds, SearchRequestParameters parameters, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the aggregate of assets that meet the search criteria.
        /// </summary>
        /// <param name="projectDescriptor">The object containing the necessary information to identify the project. </param>
        /// <param name="parameters">An object defining the search criteria. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is the aggregated count of assets. </returns>
        Task<AggregateDto[]> GetAssetAggregateAsync(ProjectDescriptor projectDescriptor, SearchAndAggregateRequestParameters parameters, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the aggregate of assets across specified projects that meet the search criteria.
        /// </summary>
        /// <param name="organizationId">The id of the organization. </param>
        /// <param name="parameters">An object defining the search criteria. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is the aggregated count of assets. </returns>
        Task<AggregateDto[]> GetAssetAggregateAsync(OrganizationId organizationId, AcrossProjectsSearchAndAggregateRequestParameters parameters, CancellationToken cancellationToken);

        /// <summary>
        /// Creates an asset.
        /// </summary>
        /// <param name="projectDescriptor">The object containing the necessary information to identify the project. </param>
        /// <param name="assetCreation">The object containing the necessary information to create an <see cref="IAssetData"/>. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is an <see cref="IAssetData"/>. </returns>
        Task<AssetDescriptor> CreateAssetAsync(ProjectDescriptor projectDescriptor, IAssetCreateData assetCreation, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an asset.
        /// </summary>
        /// <param name="assetDescriptor">The object containing the necessary information to identify the asset.</param>
        /// <param name="data">The object containing the updated asset data. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result. </returns>
        Task UpdateAssetAsync(AssetDescriptor assetDescriptor, IAssetUpdateData data, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the asset download URLs.
        /// </summary>
        /// <param name="assetDescriptor">The object containing the necessary information to identify the asset.</param>
        /// <param name="datasetIds">An optional collection of ids to limit the fetched urls. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.. </param>
        /// <returns>An enumeration of download urls. </returns>
        Task<IEnumerable<AssetDownloadUrl>> GetAssetDownloadUrlsAsync(AssetDescriptor assetDescriptor, DatasetId[] datasetIds, CancellationToken cancellationToken);

        /// <summary>
        /// Add an asset link to the project.
        /// </summary>
        /// <param name="assetDescriptor">The object containing the necessary information to identify the asset.</param>
        /// <param name="destinationProject">The object containing the necessary information to identify the destination project.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result. </returns>
        Task LinkAssetToProjectAsync(AssetDescriptor assetDescriptor, ProjectDescriptor destinationProject, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the asset link from the project.
        /// </summary>
        /// <param name="assetDescriptor">The object containing the necessary information to identify the asset.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result. </returns>
        Task UnlinkAssetFromProjectAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Checks whether the project is an asset source project.
        /// </summary>
        /// <param name="assetDescriptor">The object containing the necessary information to identify the asset.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose is result is true if the project is the asset's source, false otherwise. </returns>
        Task<bool> CheckIsProjectAssetSourceAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Checks whether the asset is linked to the project.
        /// </summary>
        /// <param name="assetDescriptor">The object containing the necessary information to identify the asset.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose is result is true if the project is the asset's source, false otherwise. </returns>
        Task<bool> CheckAssetBelongsToProjectAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the status of an asset.
        /// </summary>
        /// <param name="assetDescriptor">The object containing the necessary information to identify the asset.</param>
        /// <param name="statusName">The new status of the asset.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result. </returns>
        Task UpdateAssetStatusAsync(AssetDescriptor assetDescriptor, string statusName, CancellationToken cancellationToken);

        /// <summary>
        /// Uploads content.
        /// </summary>
        /// <param name="uploadUri">The url to upload the content stream to. </param>
        /// <param name="sourceStream">The stream to the file content</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result. </returns>
        Task UploadContentAsync(Uri uploadUri, Stream sourceStream, IProgress<HttpProgress> progress, CancellationToken cancellationToken);

        /// <summary>
        /// Downloads content.
        /// </summary>
        /// <param name="downloadUri">The url from which to download the content stream. </param>
        /// <param name="destinationStream">The destination stream for the file content</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task with no result.</returns>
        Task DownloadContentAsync(Uri downloadUri, Stream destinationStream, IProgress<HttpProgress> progress, CancellationToken cancellationToken);

        /// <summary>
        /// Removes metadata from an asset.
        /// </summary>
        /// <param name="assetDescriptor">The object containing the necessary information to identify the asset. </param>
        /// <param name="metadataType">The type of metadata to remove. </param>
        /// <param name="keys">The metadata fields to remove. </param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task with no result.</returns>
        Task RemoveAssetMetadataAsync(AssetDescriptor assetDescriptor, string metadataType, IEnumerable<string> keys, CancellationToken cancellationToken);

        /// <summary>
        /// Implement this method to get the service request url for a relative path.
        /// </summary>
        /// <param name="relativePath">The relative path of the requested resource.</param>
        /// <returns>A <see cref="Uri"/>.</returns>
        Uri GetServiceRequestUrl(string relativePath);
    }
}

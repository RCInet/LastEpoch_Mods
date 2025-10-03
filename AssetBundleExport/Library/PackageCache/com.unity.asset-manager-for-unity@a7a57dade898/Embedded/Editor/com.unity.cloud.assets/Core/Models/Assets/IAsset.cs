using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// An interface containing the information about an asset.
    /// </summary>
    interface IAsset
    {
        /// <summary>
        /// The descriptor of the asset.
        /// </summary>
        AssetDescriptor Descriptor { get; }

        /// <summary>
        /// The state of the asset.
        /// </summary>
        [Obsolete("Use AssetProperties.State instead.")]
        AssetState State => throw new NotImplementedException();

        /// <summary>
        /// Whether the asset version is frozen.
        /// </summary>
        [Obsolete("Use AssetProperties.State instead.")]
        bool IsFrozen => State == AssetState.Frozen;

        /// <summary>
        /// The sequence number of the asset. This will only be populated if the version is frozen.
        /// </summary>
        [Obsolete("Use AssetProperties.FrozenSequenceNumber instead.")]
        int FrozenSequenceNumber => -1;

        /// <summary>
        /// The change log of the asset version.
        /// </summary>
        [Obsolete("Use AssetProperties.Changelog instead.")]
        string Changelog => string.Empty;

        /// <summary>
        /// The version id from which this version was branched.
        /// </summary>
        [Obsolete("Use AssetProperties.ParentVersion instead.")]
        AssetVersion ParentVersion => AssetVersion.None;

        /// <summary>
        /// The sequence number from which this version was branched.
        /// </summary>
        [Obsolete("Use AssetProperties.ParentFrozenSequenceNumber instead.")]
        int ParentFrozenSequenceNumber => -1;

        /// <summary>
        /// The source project of the asset.
        /// </summary>
        [Obsolete("Use AssetProperties.SourceProject instead.")]
        ProjectDescriptor SourceProject { get; }

        /// <summary>
        /// The list of projects the asset is linked to.
        /// </summary>
        [Obsolete("Use AssetProperties.LinkedProjects instead.")]
        IEnumerable<ProjectDescriptor> LinkedProjects { get; }

        /// <summary>
        /// The name of the asset.
        /// </summary>
        [Obsolete("Use AssetProperties.Name instead.")]
        string Name { get; }

        /// <summary>
        /// The description of the asset.
        /// </summary>
        [Obsolete("Use AssetProperties.Description instead.")]
        string Description { get; }

        /// <summary>
        /// The tags of the asset.
        /// </summary>
        [Obsolete("Use AssetProperties.Tags instead.")]
        IEnumerable<string> Tags { get; }

        /// <summary>
        /// The tags of the asset.
        /// </summary>
        [Obsolete("Use AssetProperties.SystemTags instead.")]
        IEnumerable<string> SystemTags { get; }

        /// <summary>
        /// The labels associated to the asset version.
        /// </summary>
        [Obsolete("Use AssetProperties.Labels instead.")]
        IEnumerable<LabelDescriptor> Labels => throw new NotImplementedException();

        /// <summary>
        /// The labels no longer associated to the asset version.
        /// </summary>
        [Obsolete("Use AssetProperties.ArchivedLabel instead.")]
        IEnumerable<LabelDescriptor> ArchivedLabels => throw new NotImplementedException();

        /// <summary>
        /// The type of the asset.
        /// </summary>
        [Obsolete("Use AssetProperties.Type instead.")]
        AssetType Type { get; }

        /// <summary>
        /// The preview file ID of the asset.
        /// </summary>
        [Obsolete("Use AssetProperties.PreviewFileDescriptor instead.")]
        string PreviewFile { get; }

        /// <summary>
        /// The descriptor for the preview file of the asset.
        /// </summary>
        [Obsolete("Use AssetProperties.PreviewFileDescriptor instead.")]
        FileDescriptor PreviewFileDescriptor => default;

        /// <summary>
        /// The status of the asset.
        /// </summary>
        [Obsolete("Use AssetProperties.StatusName instead.")]
        string Status { get; }

        /// <summary>
        /// The status of the asset.
        /// </summary>
        [Obsolete("Use AssetProperties.StatusName instead.")]
        string StatusName => string.Empty;

        /// <summary>
        /// The descriptor for the status flow of the asset.
        /// </summary>
        [Obsolete("Use AssetProperties.StatusFlowDescriptor instead.")]
        StatusFlowDescriptor StatusFlowDescriptor => default;

        /// <summary>
        /// The creation and update information of the asset.
        /// </summary>
        [Obsolete("Use AssetProperties.AuthoringInfo instead.")]
        AuthoringInfo AuthoringInfo { get; }

        /// <summary>
        /// The searchable metadata of the asset.
        /// </summary>
        IMetadataContainer Metadata { get; }

        /// <summary>
        /// The system metadata of the asset.
        /// </summary>
        IReadOnlyMetadataContainer SystemMetadata => throw new NotImplementedException();

        /// <summary>
        /// The caching configuration for the asset.
        /// </summary>
        AssetCacheConfiguration CacheConfiguration => throw new NotImplementedException();

        /// <summary>
        /// Returns an asset configured with the specified caching configurations.
        /// </summary>
        /// <param name="assetConfiguration">The caching configuration for the asset. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="IAsset"/> with cached values specified by the caching configurations. </returns>
        Task<IAsset> WithCacheConfigurationAsync(AssetCacheConfiguration assetConfiguration, CancellationToken cancellationToken)
            => throw new NotImplementedException();

        /// <summary>
        /// Returns an asset in the context of the specified project.
        /// </summary>
        /// <param name="projectDescriptor">The descriptor of the project. </param>
        /// <returns>A copy of the asset with a different parent project. </returns>
        [Obsolete("Use WithProjectAsync instead.")]
        IAsset WithProject(ProjectDescriptor projectDescriptor);

        /// <summary>
        /// Changes the path of the asset to the specified project.
        /// </summary>
        /// <param name="projectDescriptor">The descriptor of the project. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the <see cref="IAsset"/> with the specified project path. </returns>
        Task<IAsset> WithProjectAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Switches the asset to the specified version.
        /// </summary>
        /// <param name="assetVersion">The version of the asset to fetch. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the <see cref="IAsset"/> with the specified version. </returns>
        Task<IAsset> WithVersionAsync(AssetVersion assetVersion, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Switches the asset to the specified version.
        /// </summary>
        /// <param name="label">The label associated to the version of the asset. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the <see cref="IAsset"/> with the version attributed to the specified label. </returns>
        Task<IAsset> WithVersionAsync(string label, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Fetches the latest changes.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task RefreshAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Returns the properties of the asset.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the <see cref="AssetProperties"/> of the asset. </returns>
        Task<AssetProperties> GetPropertiesAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Updates the asset.
        /// </summary>
        /// <param name="assetUpdate">The object containing information to update this version of the asset. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        /// <exception cref="InvalidArgumentException">If the asset is frozen, because it cannot be modified. </exception>
        /// <remarks>Can only be called on a version that is unfrozen. </remarks>
        Task UpdateAsync(IAssetUpdate assetUpdate, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the asset's status.
        /// </summary>
        /// <param name="statusAction">The new status of the asset. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        [Obsolete("Use UpdateStatus(string, CancellationToken) instead.")]
        Task UpdateStatusAsync(AssetStatusAction statusAction, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the asset's status.
        /// </summary>
        /// <param name="statusName">The name of the status. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        /// <remarks>
        /// The status of an asset can be updated whether an asset is frozen or unfrozen.
        /// However, note that the reachable statuses may vary based on the state of the asset.
        /// </remarks>
        Task UpdateStatusAsync(string statusName, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Creates a new unfrozen version of the asset.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is a new version of the asset. </returns>
        /// <exception cref="InvalidArgumentException">If the asset is unfrozen, because it cannot be used as a parent for a new version. </exception>
        /// <remarks>Can only be called if the version is frozen; this version will be the parent of the new version. </remarks>
        Task<IAsset> CreateUnfrozenVersionAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Creates a new unfrozen version of the asset.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is a new version of the asset. </returns>
        /// <exception cref="InvalidArgumentException">If the asset is unfrozen, because it cannot be used as a parent for a new version. </exception>
        /// <remarks>Can only be called if the version is frozen; this version will be the parent of the new version. </remarks>
        Task<AssetDescriptor> CreateUnfrozenVersionLiteAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Freezes the current version.
        /// </summary>
        /// <param name="changeLog">The change log for the new version. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is a frozen sequence number. </returns>
        /// <exception cref="InvalidArgumentException">If the asset is frozen, because it cannot be re-submitted. </exception>
        /// <exception cref="InvalidArgumentException">If there are on-going transformations. </exception>
        /// <remarks>Can only be called on a version that is unfrozen. </remarks>
        [Obsolete("Use FreezeAsync(IAssetFreeze, CancellationToken) instead.")]
        Task<int> FreezeAsync(string changeLog, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Freezes the current version.
        /// </summary>
        /// <param name="assetFreeze">The object containing information to freeze the current version. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result.</returns>
        /// <exception cref="InvalidArgumentException">If the asset is frozen, because it cannot be re-submitted. </exception>
        /// <remarks>Can only be called on a version that is unfrozen. </remarks>
        Task FreezeAsync(IAssetFreeze assetFreeze, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// If the asset is pending freeze, cancels the freeze.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result.</returns>
        Task CancelPendingFreezeAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Returns an object that can be used to query the asset's versions.
        /// </summary>
        /// <returns>A <see cref="VersionQueryBuilder"/>. </returns>
        [Obsolete("Use ListVersionsAsync or IAssetProject.QueryAssetVersions instead.")]
        VersionQueryBuilder QueryVersions() => throw new NotImplementedException();

        /// <summary>
        /// Returns the versions of the asset.
        /// </summary>
        /// <param name="range">The range of results to return. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>An async enumeration of <see cref="IAsset"/> in descending order of <see cref="FrozenSequenceNumber"/>. Unfrozen assets will be listed last. </returns>
        IAsyncEnumerable<IAsset> ListVersionsAsync(Range range, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Returns an enumeration of the asset's linked <see cref="IAssetProject"/>.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an async enumeration of <see cref="IAssetProject"/>. </returns>
        IAsyncEnumerable<IAssetProject> GetLinkedProjectsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Creates a reference between an asset and the project.
        /// </summary>
        /// <param name="projectDescriptor">The descriptor of the project to link to. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        [Obsolete("Use IAssetProject.LinkAssetsAsync instead.")]
        Task LinkToProjectAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the reference between an asset and the project.
        /// </summary>
        /// <param name="projectDescriptor">The descriptor of the project to unlink from. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        [Obsolete("Use IAssetProject.UnlinkAssetsAsync instead.")]
        Task UnlinkFromProjectAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Returns the url to the preview image of the asset.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is downloadble url pointing to the preview image. </returns>
        Task<Uri> GetPreviewUrlAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Returns the download URLs for the asset's files.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the download URLs for all the asset's files and attachments. </returns>
        Task<IDictionary<string, Uri>> GetAssetDownloadUrlsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Returns the <see cref="IAssetCollection"/> the asset belongs too.
        /// </summary>
        /// <param name="range">The range of collections to return. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>An async enumeration of <see cref="IAssetCollection"/>. </returns>
        IAsyncEnumerable<CollectionDescriptor> ListLinkedAssetCollectionsAsync(Range range, CancellationToken cancellationToken);

        /// <summary>
        /// Returns a <see cref="IDataset"/> with the specified creation information.
        /// </summary>
        /// <param name="datasetCreation">The object containing the necessary information to create a dataset. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the newly created dataset. </returns>
        /// <exception cref="InvalidArgumentException">If the asset is frozen, because it cannot be modified. </exception>
        /// <remarks>Can only be called on a version that is unfrozen. </remarks>
        [Obsolete("Use CreateDatasetAsync(IDatasetCreation, CancellationToken) instead.")]
        Task<IDataset> CreateDatasetAsync(DatasetCreation datasetCreation, CancellationToken cancellationToken);

        /// <summary>
        /// Returns a <see cref="IDataset"/> with the specified creation information.
        /// </summary>
        /// <param name="datasetCreation">The object containing the necessary information to create a dataset. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the newly created dataset. </returns>
        /// <exception cref="InvalidArgumentException">If the asset is frozen, because it cannot be modified. </exception>
        /// <remarks>Can only be called on a version that is unfrozen. </remarks>
        Task<IDataset> CreateDatasetAsync(IDatasetCreation datasetCreation, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Returns a <see cref="IDataset"/> with the specified creation information.
        /// </summary>
        /// <param name="datasetCreation">The object containing the necessary information to create a dataset. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the newly created dataset. </returns>
        /// <exception cref="InvalidArgumentException">If the asset is frozen, because it cannot be modified. </exception>
        /// <remarks>Can only be called on a version that is unfrozen. </remarks>
        Task<DatasetDescriptor> CreateDatasetLiteAsync(IDatasetCreation datasetCreation, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Retrieves the specified <see cref="IDataset"/>.
        /// </summary>
        /// <param name="datasetId">The id of the dataset. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the requested dataset. </returns>
        Task<IDataset> GetDatasetAsync(DatasetId datasetId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves all the <see cref="IDataset"/>.
        /// </summary>
        /// <param name="range">The range of datasets to return. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an async enumeration of datasets. </returns>
        IAsyncEnumerable<IDataset> ListDatasetsAsync(Range range, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the specified <see cref="IFile"/>.
        /// </summary>
        /// <param name="filePath">The id of the file</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="IFile"/>. </returns>
        [Obsolete("Use IDataset.GetFileAsync instead.")]
        Task<IFile> GetFileAsync(string filePath, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves all the <see cref="IFile"/>s for the asset.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an async enumeration of <see cref="IFile"/> referenced by the asset. </returns>
        [Obsolete("Use IDataset.ListFilesAsync instead.")]
        IAsyncEnumerable<IFile> ListFilesAsync(Range range, CancellationToken cancellationToken);

        /// <summary>
        /// Returns an object that can be used to query asset labels across versions.
        /// </summary>
        /// <returns>A <see cref="AssetLabelQueryBuilder"/>. </returns>
        AssetLabelQueryBuilder QueryLabels() => throw new NotImplementedException();

        /// <summary>
        /// Adds labels to the asset.
        /// </summary>
        /// <param name="labels">The collection of labels to add. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task AssignLabelsAsync(IEnumerable<string> labels, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Remove labels from the asset.
        /// </summary>
        /// <param name="labels">The collection of labels to remove. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task UnassignLabelsAsync(IEnumerable<string> labels, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Returns a set of reachable statuses from the current status.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an array of status names. </returns>
        Task<string[]> GetReachableStatusNamesAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Returns references to the asset.
        /// </summary>
        /// <param name="range">The range of results to return. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>An async enumeration of <see cref="IAssetReference"/>. </returns>
        IAsyncEnumerable<IAssetReference> ListReferencesAsync(Range range, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Creates a reference between the asset and another asset, where the asset is the source of the reference.
        /// </summary>
        /// <param name="targetAssetId">The id of the asset. </param>
        /// <param name="targetAssetVersion">The version of the asset. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the reference between the assets. </returns>
        Task<IAssetReference> AddReferenceAsync(AssetId targetAssetId, AssetVersion targetAssetVersion, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Creates a reference between the asset and another asset, where the asset is the source of the reference.
        /// </summary>
        /// <param name="targetAssetId">The id of the asset. </param>
        /// <param name="targetLabel">The label associated to the asset version. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the reference between the assets. </returns>
        Task<IAssetReference> AddReferenceAsync(AssetId targetAssetId, string targetLabel, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Removes a reference between the asset and another asset. The asset can be either the source or the target.
        /// </summary>
        /// <param name="referenceId">The id of the reference between the assets. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task RemoveReferenceAsync(string referenceId, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Returns a JSON serialized string of the asset's identifiers.
        /// </summary>
        /// <returns>The serialized identifiers of the asset. </returns>
        [Obsolete("Use Descriptor.ToJson() instead.")]
        string SerializeIdentifiers();

        /// <summary>
        /// Returns a JSON string of the asset.
        /// </summary>
        /// <remarks>
        /// To deserialize the asset use <see cref="IAssetRepository.DeserializeAsset"/>. The <see cref="IAssetRepository"/> is responsible for injecting the necessary dependencies into the asset.
        /// </remarks>
        /// <returns>The asset serialized as a JSON string. </returns>
        [Obsolete("IAsset serialization is no longer supported.")]
        string Serialize();
    }
}

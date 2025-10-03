# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.8.1] - 2025-05-12

### Changed
- Updated samples to use ServiceConnectorFactory API

## [1.8.0] - 2025-04-25

### Added
- Added `IAssetProject.DeleteUnfrozenAssetVersionAsync`.

### Fixed
- Improved WebGL support.

## [1.7.0] - 2025-03-27

### Added
- Added `TransformationStatus.Queued`.
- Added `DatasetProperties.Type` and `DatasetProperties.WorkflowName`.
- Added `DatasetSearchCriteria.Type` and `DatasetSearchCriteria.WorkflowName`.

### Changed
- Exposed new values for `AssetType`.

### Fixed
- Fixed file caching when configuring the dataset to `CacheFileList`.

## [1.6.0] - 2025-02-03

### Changed
- Updated samples.

### Fixed
- Addressed issue where service error is thrown for some search ranges.

## [1.6.0-exp.2] - 2025-01-21

### Added
- Added property structs and `GetPropertiesAsync(CancellationToken)` method to `IFieldDefinition`, `ILabel`, `IAssetProject`, `IAssetCollection`, `IAsset`, `IDataset`, `ITransformation`, and `IFile`.
- Added cache configuration structs, `CacheConfiguration` property, and `WithCacheConfigurationAsync` method to `IFieldDefinition`, `ILabel`, `IAssetProject`, `IAssetCollection`, `IAsset`, `IDataset`, `ITransformation`, and `IFile`.
- Added struct `AssetRepositoryCacheConfiguration` and property `IAssetRepository.CacheConfiguration`.
- Added `GroupAndCountAssetsQueryBuilder.ExecuteAsync(Groupable, CancellationToken)` and `GroupAndCountAssetsQueryBuilder.ExecuteAsync(IEnumerable<Groupable>, CancellationToken)`.
- Added `SystemMetadata` search criteria to `AssetSearchCriteria`, `DatasetSearchCriteria`, and `FileSearchCriteria`.
- Added `FileSearchCriteria.Size` which supports `NumericRangePredicate`.
- Added `NumericRange` to build numeric range predicates.
- Added `WithTextValue(string, StringPredicate)`, `WithNumberValue(string, NumericRangePredicate)`, and `WithTimestampValue(string, DateTime, bool, DateTime, bool)` to `MetadataSearchCriteria`.
- Added `WithValue(string, StringPredicate)` to `StringSearchCriteria`.

### Changed
- Updated internal api.
- Revert rate limitation to normal for endpoints related to file uploading to prevent error when bulk uploading file to the same asset.
- Added additional fields in `GroupableField`.

### Deprecated
- Deprecated properties in `IFieldDefinition`; use properties in `FieldDefinitionProperties` instead.
- Deprecated properties in `ILabel`; use properties in `LabelProperties` instead.
- Deprecated properties in `IAssetProject`; use properties in `AssetProjectProperties` instead.
- Deprecated properties in `IAssetCollection`; use properties in `AssetCollectionProperties` instead.
- Deprecated properties in `IAsset`; use properties in `AssetProperties` instead.
- Deprecated properties in `IDataset`; use properties in `DatasetProperties` instead.
- Deprecated properties in `ITransformation`; use properties in `TransformationProperties` instead.
- Deprecated properties in `IFile`; use properties in `FileProperties` instead.
- Deprecated `IAsset.GetLinkedProjectsAsync`; use `AssetProperties.LinkedProjects` in conjunction with `IAssetRepository.GetAssetProjectAsync` instead.
- Deprecated `IFile.GetLinkedDatasetsAsync`; use `FileProperties.LinkedDatasets` in conjunction with `IAssetRepository.GetDatasetAsync` instead.
- Deprecated `GroupAndCountAssetsQueryBuilder.ExecuteAsync(GroupableField)`; use `GroupAndCountAssetsQueryBuilder.ExecuteAsync(Groupable)` instead.
- Deprecated `GroupAndCountAssetsQueryBuilder.GroupByCollectionAndExecuteAsync`; use `GroupAndCountAssetsQueryBuilder.ExecuteAsync(Groupable)` instead.
- Deprecated `IAsset.SerializeAsset` and `IAssetRepository.DeserializeAsset`; serialization will no longer be supported.
- Deprecated `StringSearchCriteria.SearchOptions`; use `StringSearchOptions` instead.
- Deprecated `WithValue(string, string, StringSearchCriteria.SearchOptions)`, `WithValue(string, Regex)`, and `WithFuzzyValue(string)` in `MetadataSearchCriteria`; use `WithTextValue(string, StringPredicate)` instead.
- Deprecated `WithValue(string, StringSearchCriteria.SearchOptions)`, `WithValue(Regex)`, and `WithFuzzyValue(string)` in `StringSearchCriteria`; use `WithValue(StringPredicate)` instead.
- Deprecated `FileSearchCriteria.SizeBytes`; use `FileSearchCriteria.Size` instead.

## [1.6.0-exp.1] - 2024-12-12

### Added
- Added `IAssetProject.QueryAssetVersions(AssetId)` and `IAsset.ListVersionsAsync(Range, CancellationToken)`.
- Added `IAssetRepository.CreateAssetProjectLiteAsync`.
- Added `IAssetRepository.CreateFieldDefinitionLiteAsync`.
- Added `IAssetRepository.CreateLabelLiteAsync`.
- Added `IAssetProject.CreateAssetLiteAsync`.
- Added `IAssetProject.CreateCollectionLiteAsync`.
- Added `IAsset.CreateDatasetLiteAsync`.
- Added `IDataset.UploadFileLiteAsync`.
- Added `IDataset.StartTransformationLiteAsync`.
- Added `IAsset.CreateUnfrozenVersionLiteAsync`.
- Added `IAsset.CreateUnfrozenVersionLiteAsync`.
- Added `IFile.WithDatasetAsync`.
- Added `ITransformation.WorkflowName`.
- Added `WorkflowType.Optimize_Convert_Free` and custom workflow implementations `OptimizeAndConvertFreeTransformation` for free users.
- Added `WorkflowType.Optimize_Convert_Pro` and custom workflow implementations  `OptimizeAndConvertProTransformation` for pro/enterprise users.
- Added `IAssetRepository.EnableProjectForAssetManagerLiteAsync`.

### Fixed
- Fixed extra parameters of `ITransformationCreation` being passed incorrectly to the service.

### Deprecated
- Deprecated `IAsset.QueryVersions`, use `IAsset.ListVersionsAsync` or `IAssetProject.QueryAssetVersions` instead.
- Deprecated `IAssetProject.GetAssetAsync(AssetId, CancellationToken)`, use `IAssetProject.QueryAssetVersions(AssetId)` instead.
- Deprecated `AssetUpdate(IAsset)` constructor; use `AssetUpdate()` instead.
- Deprecated `DatasetInfo(string)` and `DatasetInfo(IDataset)` constructors; use `DatasetInfo()` instead.
- Deprecated `DatasetUpdate(IDataset)` constructor; use `DatasetUpdate()` instead.
- Deprecated `FileUpdate(IFile)` constructor; use `FileUpdate()` instead.
- Deprecated `FieldDefinitionUpdate(IFieldDefinition)` constructor; use `FieldDefinitionUpdate()` instead.
- Deprecated `IFile.WithDataset`; use `IFile.WithDatasetAsync` instead.
- Deprecated `WorkflowType.Generic_Polygon_Target`; use `WorkflowType.Optimize_Convert_Free` or `WorkflowType.Optimize_Convert_Pro` instead.
- Deprecated `GenericPolygonTargetTransformation`; use custom workflow `OptimizeAndConvertProTransformation` instead.
- Deprecated `ITransformation.InputDatasetId`; use `ITransformation.Descriptor.DatasetId` instead.

## [1.5.1] - 2024-11-15

### Changed
- Updated deprecated documentation.

### Fixed
- Fixed IAsset.Changelog not being populated.

## [1.5.0] - 2024-10-18

### Added
- Added `HasCollections` property to `IAssetProject`.
- Added `CountAssetsAsync` and `CountCollectionsAsync` to `IAssetProject`.
- Added `AssetState` enum and `State` property to `IAsset`.
- Added overloaded `FreezeAsync(IAssetFreeze, CancellationToken)` and `CancelFreezeAsync` to `IAsset`.

### Changed
- Improved sample code snippets.
- Improved documentation code snippets.

### Deprecated
- Deprecated `IAsset.IsFrozen`; use `IAsset.State` instead.
- Deprecated `IAsset.FreezeAsync(string, CancellationToken)`; use `IAsset.FreezeAsync(IAssetFreeze, CancellationToken)` instead.

## [1.4.0] - 2024-09-23

### Added
- Added `IAssetRepository.EnableProjectForAssetManagerAsync` to enable the Asset Manager feature for a project; this allows a project to be retrieved as an `IAssetProject`.
- Added documentation for asset to asset references.
- Added `IAsset.PreviewFileDescriptor`
- Added `IFile.GetResizedImageDownloadUrlAsync` to get the download url of an image file with a specified max dimension.

### Changed
- Improved sample code snippets.

### Fixed
- Fixed bug where organization search returned asset results with missing ProjectId.
- Fixes missing script in Asset Discovery sample

### Removed
- [Experimental][Breaking] Removed `StatusDescriptor` and `StatusTransitionDescriptor`. IStatus and IStatusTransition expose separate properties for `StatusFlowDescriptor` and string ids instead.

### Deprecated
- Deprecated `IAsset.PreviewFile`; use `IAsset.PreviewFileDescriptor` instead.
- Deprecated `IAsset.IsFrozen`; use `IAsset.State` instead.
- Deprecated `IAsset.FreezeAsync(string, CancellationToken)`; use `IAsset.FreezeAsync(IAssetFreeze, CancellationToken)` instead.

## [1.4.0-exp.3] - 2024-08-26

### Added
- Added overload `IAssetRepository.GetAssetAsync(ProjectDescriptor, AssetId, string, CancellationToken)` to get an asset by asset id and label.
- Added `Unity_Editor` in `AssetType` enum.

### Changed
- Changed rate limitation for endpoints related to file uploading to prevent error when bulk uploading file to the same asset.

## [1.4.0-exp.2] - 2024-08-08

### Added
- Added overload `IAsset.CreateDatasetAsync(IDatasetCreation, CancellationToken)` to create a dataset.
- Added use case documentation and example for folder upload.
- Added `IAsset.AddTagsAsync(IEnumerable<string>, CancellationToken)` and `IAsset.RemoveTagsAsync(IEnumerable<string>, CancellationToken)` to respectively add and remove tags from an asset.
- [Experimental] Added `IAsset.StatusName` property to get the current status name of an asset.
- [Experimental] Added `IAsset.GetReachableStatusNamesAsync` to get the names of statuses reachable from the current status of an asset.
- [Experimental] Added `IAsset.UpdateStatusAsync(string, CancellationToken)` to update the status of an asset.
- Added overload `IAssetRepository.QueryAssets(OrganizationId)` which can query assets across all projects in an organization.
- Added overloads `IAssetRepository.GroupAndCountAssets(OrganizationId)` and `IAssetRepository.CountAssetsAsync(OrganizationId, IAssetSearchFilter, CancellationToken)` which can group and count assets across all projects in an organization.
- [Experimental] Added `IAssetReference` to expose dependencies between assets.
- [Experimental] Added `IAsset.ListReferencesAsync` to list the references to an asset.
- [Experimental] Added `IAsset.AddReferenceAsync(AssetId, AssetVersion, CancellationToken)` and `IAsset.AddReferenceAsync(AssetId, string, CancellationToken)` to add a reference to an asset either by version or label.
- [Experimental] Added `IAsset.RemoveReferenceAsync(string, CancellationToken)` to remove a reference to an asset.
- Added use cases to documentation

### Removed
- [Experimental][Breaking] Removed `IAsset.GetStatusAsync`; use `IAsset.StatusName` property instead.
- [Experimental][Breaking] Removed `IAsset.GetReachableStatusesAsync`; use `IAsset.GetReachableStatusNamesAsync` instead.
- [Experimental][Breaking] Removed `IAsset.UpdateStatusAsync(IStatus, CancellationToken)`; use `IAsset.UpdateStatusAsync(string, CancellationToken)` instead.

### Deprecated
- Deprecated `IAsset.CreateDatasetAsync(DatasetCreation, CancellationToken)`; use `IAsset.CreateDatasetAsync(IDatasetCreation, CancellationToken)` instead.

## [1.4.0-exp.1] - 2024-07-26

### Added
- Added `IDataset.GetDownloadUrlsAsync` to get download urls for all files in a dataset.
- [Experimental] Added `IStatusFlow`, `IStatus`, and `IStatusTransition` to expose asset status information.
- [Experimental] Added `StatusFlowQueryBuilder` and `IAssetRepository.QueryStatusFlows` to fetch the status flows available in the organization.
- [Experimental] Added `IAsset.GetStatusAsync` to get the status information of an asset.
- [Experimental] Added `IAsset.GetReachableStatusesAsync` to get the information of statuses reachable from the current status of an asset.
- [Experimental] Added overloaded `IAsset.UpdateStatusAsync` to update the status of an asset with an `IStatus` parameter.
- [Experimental] Added `IAsset.StatusFlowDescriptor` to expose the status flow of an asset.
- [Experimental] Added `StatusFlowDescriptor` property to `IAssetCreation` and `IAssetUpdate` specify the status flow when creating and updating an asset.

### Changed
- Improved documentation code snippets.
- Added status details to Asset Discovery and Asset Manager samples

### Deprecated
- Deprecated `IAsset.Status` property; use `IAsset.GetStatusAsync` instead.
- Deprecated `IAsset.UpdateStatusAsync(AssetStatusAction, CancellationToken)`; use `IAsset.UpdateStatusAsync(IStatus, CancellationToken)` instead.
- Deprecated `AssetStatusAction` enum.

## [1.3.0] - 2024-07-15

### Added
- Added `Terminating` status to `TransformationStatus`.
- Added `Metadata_Extraction`, `Generic_Polygon_Target`, and `Custom` to `WorkflowType`.
- Added `CustomWorkflowName` to `TransformationCreation` for specifying the transformation when `ITransformationCreation.WorkflowType` is set to `WorkflowType.Custom`.

### Changed
- Improved documentation code snippets.

## [1.2.0] - 2024-06-20

### Added
- Added overloads to `IAsset.WithVersionAsync` to get the version of the asset from a sequence number or a label.
- Added a readonly `SystemMetadata` property to `IAsset`, `IDataset`, and `IFile` to expose system metadata.
- `IFieldDefinition.FieldOrigin` property to expose the originator of a field definition.
- `IAssetProject.LinkAssetsAsync` and `IAssetProject.UnlinkAssetsAsync` to link and unlink assets to a project.
- Added overload `IAssetProject.GetAssetAsync` without a version to get the default version of an asset.
- Added versioning details to Asset Discovery and Management samples.
- Added `UserId` and `JobId` properties in `ITransformation`.
- Added `IFileCreation.DisableAutomaticTransformations` property to give the option to skip automatic transformations triggered by a file upload.
- Added `IReadOnlyMetadataContainer` to query system metadata.

### Changed
- [Experimental][Breaking] Renamed `AssetVersionQueryBuilder` to `VersionQueryBuilder`.
- [Experimental][Breaking] Renamed `IAsset.QueryAssetVersion` to `IAsset.QueryVersions`.
- [Experimental][Breaking] Changed `IAsset.WithProjectAsync` to return another instance of the asset at a different project path.
- [Experimental][Breaking] Changed `IAsset.WithVersionAsync` to return another instance of the asset with the specified version.
- Improved test coverage.

### Fixed
- Sanitize file paths on file creation.
- Fixed search criteria for date ranges.
- Fixed dropped calls when rate limiting.
- Fixed incorrectly escaped download urls.

### Removed
- [Experimental][Breaking] Removed `IAssetProject.QueryAssetVersions`.

### Deprecated
- Deprecated `IAsset.LinkToProjectAsync`; use `IAssetProject.LinkAssetsAsync` instead.
- Deprecated `IAsset.UnlinkFromProjectAsync`; use `IAssetProject.UnlinkAssetsAsync` instead.

## [1.2.0-exp.3] - 2024-05-31

### Added
- Added `ITransformation.TerminateAsync` to cancel a transformation.
- Added `IAsset.WithProjectAsync` to change the project path of the asset.
- Added `IAsset.WithVersionAsync` to switch to another version of the asset.
- `IAsset.WithLatestVersionAsync` and `IAssetProject.GetAssetWithLatestVersionAsync` to get the latest version of an asset.

### Changed
- Removed internal caching of files and datasets in `IAsset`.
- [Experimental][Breaking] Renames all instances of `VersionLabel` to `Label`.
- [Experimental][Breaking] Renames `IAsset.VersionNumber` and `IAsset.ParentVersionNumber` to `IAsset.FrozenSequenceNumber` and `IAsset.ParentFrozenSequenceNumber` respectively.

### Deprecated
- Deprecated `IAsset.GetFileAsync` and `IAsset.ListFilesAsync`; use `IDataset.GetFileAsync` and `IDataset.ListFilesAsync` instead.
- Deprecated `IAsset.WithProject`; use `IAsset.WithProjectAsync` instead.

## [1.2.0-exp.2] - 2024-05-02

### Added
- Added Apple Privacy Manifest documentation.

### Changed
- Changed slightly the rate limitation algorithm to completely prevent any rate-limit exceptions while keeping the throughput almost optimal.

### Fixed
- `IsFrozen` property in `IAsset` now returns the correct value.
- Asset versioning related properties in `IAsset` are serialized correctly by `IAsset.Serialize()`
- Fixed overlapping text in the Asset Manager sample.
- Fixed video transformations not starting.

## [1.2.0-exp.1] - 2024-04-19

### Added
- Added a section on code stripping in troubleshooting documentation.
- [Experimental] Adding support for Asset Versioning.

### Removed
- Removed Asset Database Uploader sample.

## [1.1.0] - 2024-04-05

### Added
- `UserId` property in `TransformationSearchFilter`.
- `GreaterThan` and `LessThanOrEqual` to `SearchConditionRange`.
- `WithValueGreaterThan`, `WithValueGreaterThanOrEqualTo`, `WithValueLessThan`, and `WithValueLessThanOrEqualTo` to `ConditionalSearchCriteria`.
- `GenerateTagsAsync` method to `IFile`.
- Default constructor to `DatasetUpdate` class.
- String search queries recognize '?' as wildcard character.
- Memory warning popups in Discovery and Management samples.
- Added rate limiter to API calls to avoid 429 errors when doing multiple operations in parallel. 
- Added Apple Privacy Manifest file to `/Plugins` directory.

### Changed
- Improved Discovery and Management samples search bar.
- Improved samples' UI and cleaned up debug logs.
- Updated use case documentation.
- Modified the `LogLevel` for several log messages to reduce the default amount of logs in the console.
- FieldDefinitions no longer auto-refresh properties on update; use `RefreshAsync`.
- Reviewed manual documentation
- Manual documentation code-snippets set to compile only in editor.
- Documentation updates.
- Updated deprecated endpoints.

### Fixed
- Bubbles up exceptions in `IFile.UploadAsync`.
- Updated deprecated endpoints.
- Upgrade to Common 1.1.0.

### Removed
- Removes `com.unity.editorcoroutines` dependency from package.

### Deprecated
- Deprecated `IAssetCollection.GetFullCollectionPath`; use `IAssetCollection.Descriptor.Path`.
- Deprecated default constructor of `FileCreation` class; use `FileCreation(string path)`.
- Deprecated `IAsset.SerializeIdentifiers` and `IAssetRepository.DeserializeAssetIdentifiers`; use `AssetDescriptor.ToJson` and `AssetDescriptor.FromJson` respectively.

## [1.0.0] - 2024-02-26

### Added
- `IsDeleted` property to `IFieldDefinition`
- `FieldDefinitionQueryBuilder`, `ProjectQueryBuilder`, `CollectionQueryBuilder`, `AssetQueryBuilder`, classes to build and execute queries.
- `BooleanMetadata`, `DateTimeMetadata`, `NumberMetadata`, `StringMetadata`, and `UserMetadata`.
- `IAsset.GetPreviewUrlAsync` and `IFile.GetPreviewUrlAsync`.
- `IDataset.RefreshAsync()` and `IFile.RefreshAsync()`.
- `IAsset.GetPreviewDatasetAsync()` and `IAsset.GetSourceDatasetAsync()` extension methods.
- `UpdateStatusAsync` method to `IAsset`.
- `WithValue(string, SearchOptions)` and `WithValue(Regex)` overloads to `StringSearchCriteria`.
- Re-added `UploadAsync` method to `IFile` ans updated the method of the same name in `FileEntity`.

### Changed
- [Breaking] Renamed `OrganizationGenesisId` property to `OrganizationId` in `CollectionDescriptor` and `TransformationDescriptor` classes.
- [Breaking] Changed `CreatedBy` and `UpdatedBy` property type of `AuthoringInfo` class to `UserId`.
- [Breaking] Changed `CreatedBy` and `UpdatedBy` property type of `AuthoringInfoSearchFilter` class to `IdSearchCriteria<UserId>`.
- [Breaking] `Collections` property of `AssetSearchFilter` changed from `List<CollectionPath>` to `QueryListParameter<CollectionPath>`.
- [Breaking] `SearchAssetsAsync()` replaced by `QueryAssets()`.
- [Breaking] `CountAssetsAsync()` replaced by `GroupAndCountAssets()`; `CountAssetsAsync()` returns `int` instead of `Aggregation`.
- [Breaking] Renames `CollectionDescriptor.CollectionPath` to `CollectionDescriptor.Path`.
- [Breaking] Renamed `AssetSearchFilter` to `AssetSearchCriteria`, `DatasetSearchFilter` to `DatasetSearchCriteria`, `FileSearchFiter` to `FilterSearchCriteria`, and `MetadataSearchFilter` to `MetadataSearchCriteria`.
- [Breaking] `AssetSearchFilter` contains methods to `Include()`, `Exclude()`, and `Any()` on search criteria.
- [Breaking] `Include()`, `Exclude()`, and `Any()` methods of search criteria replaced with `WithValue()` method; use `Include()`, `Exclude()`, and `Any()`, on `AssetSearchFilter`.
- [Breaking] Renamed `IAssetCollection.AddAssetsAsync()` and `IAssetCollection.RemoveAssetsAsync()` to `IAssetCollection.ListAssetsAsync()` and `IAssetCollection.UnlinkAssetsAsync()` respectively.
- [Breaking] Renamed `WorkflowType.TranscodeVideo` to `WorkflowType.Transcode_Video`.
- [Breaking] `AccumulateIncludedCriteria`, `AccumulateExcludedCriteria`, and `AccumulateAnyCriteria` methods of `IAssetSearchFilter` return `IReadOnlyDictionary` instead of `Dictionary`.
- Update com.unity.cloud.common dependency to 1.0.0.

### Fixed
- Fixes listing of files in `IDataset`.

### Removed
- [Breaking] Removed `Status` property from `IFieldDefinition`; use `IsDeleted`.
- [Breaking] Removed `Pagination`; use `LimitTo()` and `OrderBy()` methods of query builders where available.
- [Breaking] Removed `AggregationParameters`; use `GroupAndCountAssetsQueryBuilder`.
- [Breaking] Removed `Aggregation`.
- [Breaking] Internalized `FieldsFilter`.
- [Breaking] Removed `MetadataObject` and `IMetadataValue`; use `MetadataValue`.
- [Breaking] Removed `IAsset.PreviewFileUrl` and `IFile.PreviewUrl`; use `IAsset.GetPreviewUrlAsync` and `IFile.GetPreviewUrlAsync` respectively.
- [Breaking] Removed `IAsset.GetProject()`, `IDataset.GetProject()`, and `IDataset.GetAsset()`.
- [Breaking] Removed `IFile.InvalidateCachedUrls()`; use `IFile.RefreshAsync().
- [Breaking] Removed `IdSearchCriteria`; use `SearchCriteria<string>`.
- [Breaking] Removed inheritance of `IFieldDefinitionUpdate` from `IFieldDefinitionCreation` and `FieldDefinitionUpdate` from `FieldDefinitionCreation`.
- [Breaking] Removed `PublishAsync`, `WithdrawAsync`, `SendToReviewAsync`, `ApproveAsync`, and `RejectAsync` methods from `IAsset`; use `UpdateStatusAsync`.

## [1.0.0-exp.7] - 2024-02-13

### Added
- Added `StartTransformationAsync(ITransformationCreation, CancellationToken)` method to `IDataset`.
- `GetTransformationAsync` method to `IAssetRepository`
- `GetAssetProjectAsync` shortcut method to `IDataset`
- `ListTransformationsAsync` method to `IDataset`
- `TransformationQueryBuilder` class to build transformation queries
- `QueryTransformations` method to `IAssetProject`
- `Progress` property and `RefreshAsync` method to `ITransformation`
- Added `UpdateAsync(IAssetCollectionUpdate, CancellationToken)` method to `IAssetCollection`.
- `RefreshAsync` method to `IAssetCollection` to update properties.
- Added `ReturnCached` and `ForceRefresh` options to `MetadataQueryBuilder`.

### Changed
- Updated Asset Manager sample to display progress bars for workflow operations.
- Updated Asset Manager sample to add a `Remove` button in the asset's context menu to unlink an asset from the current project.
- [Breaking] Renamed `ComplexSearchCriteria<T>` to `CompoundSearchCriteria`.
- [Breaking] Renamed `SearchCriteriaBase` to `BaseSearchCriteria`.
- [Breaking] Renamed `CollectionSearchCriteria<U,T>` to `ListSearchCriteria<T>`.
- [Breaking] Renamed `SearchConditionType` to `SearchConditionRange`.
- Update com.unity.cloud.common dependency to 1.0.0-pre.6.

### Fixed
- Maintenance of internal code.
- `IAsset.LinkedProjects` no longer empty on asset creation.
- Fixes formatting query when parameter can expect a list.
- Fixes pagination of search requests.

### Removed
- [Breaking] Removed `ISearchCriteria`, `ISearchCriteria<T>`, and `HashsetSearchCriteria`
- [Breaking] Removed `SystemMetadata` from `IAsset`, `IDataset`, and `IFile`.
- [Breaking] Removed `StartTransformationAsync(WorkflowType, CancellationToken)` method from `IDataset`.
- [Breaking] Removed `IAsset.StorageId` property.
- [Breaking] Removed `SetName`, `SetDescription`, and `UpdateAsync(CancellationToken)` from `IAssetCollection`.

## [1.0.0-exp.6] - 2024-01-15

### Added
- Added `IMetadataContainer` and `IMetadataValue` to expose metadata operations.
- Added `ISelectionFieldDefinition`, `ISelectionFieldDefinitionCreation`, and `ISelectionFieldDefinitionUpdate` which exposes selection specific field definition properties.
- Added `StartTransformationAsync` and `GetTransformationAsync` methods to `IDataset`.

### Changed
- [Breaking] `IAsset.GetCollectionAsync`, `IAsset.GetDatasetAsync`, `IAsset.GetFileAsync`, and `IDataset.GetFileAsync` throw `NotFoundException` if the requested entity fails to be found.
- [Breaking] Moved `Multiselection` and `AcceptedValues` properties and `AddSelectionValuesAsync` and `RemoveSelectionValuesAsync` methods from `IFieldDefinition` to `ISelectionFieldDefinition`.
- [Breaking] Changed types of  `Metadata` and `SystemMetadata` properties of `IAsset`, `IDataset`, and `IFile` from `IDeserializable` to `IMetadataContainer`.
- Updated `Asset Discovery` and `Asset Management` samples to display metadata values.
- Unified exception logging across samples.
- Error thrown on selection field definition creation when accepted values are not provided.
- Update com.unity.cloud.common dependency to 1.0.0-pre.5.

### Fixed
- Fixed asset searches returning incorrect number of results for large offsets.
- Fixed `AssetType` searching to allow searching for more than one type.
- Added validation to `SearchConditionValue`.
- Fixed issue with metadata DateTime values not being serialized correctly.

### Removed
- [Breaking] Removed `IsMatch`, `Include`, `Exclude`, and `Any` methods from `IAssetSearchFilter`.
- [Breaking] Removed `GetAssetTypeFromString` from `AssetType`.
- [Breaking] Turned `AssetDownloadUrl` internal
- [Breaking] Removed `PortalMetadata` properties from `IAsset`, `IDataset`, and `IFile`.
- [Breaking] Removed `SystemMetadata` properties from `AssetSearchFilter`, `DatasetSearchFilter`, and `FileSearchFilter`; searching by system metadata is not supported.
- [Breaking] Removed `Metadata` and `SystemMetadata` properties from `IAssetUpdate`, `IDatasetUpdate`, and `IFileUpdate`; use `IMetadataContainer.AddOrUpdateAsync` instead.

## [1.0.0-exp.5] - 2023-12-20

### Fixed
- Fixed exception in Asset Management sample on start up.

## [1.0.0-exp.4] - 2023-12-07

### Added
- Added a Metadata Management sample for listing and managing an organization's field definitions.

### Changed
- Update `ListProjectsAsync` to call new public api endpoints.

### Fixed
- Fixed asset and asset list refreshing in Asset Management sample following create, save, and publish actions.
- Fixed `IFile` url fetching when using the `FileFilter.All` flag.

## [1.0.0-exp.3] - 2023-11-23

### Added
- `IFieldDefinition` to expose field definitions of organizations for managing asset metadata.
- `ListFieldDefinitionsAsync`, `GetFieldDefinitionAsync`, `CreateFieldDefinitionAsync`, `DeleteFieldDefinitionAsync` methods added to `IAssetRepository`.
- Added Supported platforms section to the Prerequisites page of the manual documentation.

### Changed
- Clean up of (de)serialization to reduce external dependencies.
- `AuthoringInfo` changed from struct to class to allow null values.
- Switched from multiple asmef files to one asmdef file in Samples/Shared and multiple asmref files in each sample folder.
- Added the !UC_EXCLUDE_SAMPLES constraint to the asmdef file in Samples/Shared and removed the conditional compilation code in the sample scripts.
- [Breaking] Internal `Pagination.Order` enum extracted and renamed `SortingOrder`.

## [1.0.0-exp.2] - 2023-11-07

### Added
- Added `PreviewFileUrl` to `IAsset` to expose the preview file url of the asset.

### Changed
- Updated LICENSE.md file.
- Improved information in README
- Update `ListProjectsAsync`, `GetProjectAsync`and `CreateProjectAsync` to call public api endpoints

### Fixed
- Fix SearchBarController to include Asset fields on search actions.
- Fixed potential null reference exception in in the `ThumbnailController` class used in the `Asset Discovery` sample.
- Fixed missing version header.
- Fix `DatasetEntity.ListFilesAsync` to show all files fields information and included `Status` as a default field.

### Removed
- [Breaking] Removed `UploadAsync` and `GetUploadUrlAsync` methods from `IFile`. Overwriting file content is not supported.
- Removed inapplicable notices in documentation.
- [Breaking] Removed `GetPreviewFileDownloadUrlAsync` from `IAsset`. Use `GetDownloadUrlAsync` of `IFile` instead.
- Removed mocking code for assets.

## [1.0.0-exp.1] - 2023-10-26

### Added
- `RefreshAsync` method added to `IAsset` to refresh the asset data. This is useful for fetching additional data that is not populated by default.
- `PreviewUrl` property added to `IFile` to expose the preview url of the file.
- `ListAssetCollectionsAsync` and `GetAssetCollectionAsync` methods added to `IAssetRepository`.
- Added error popup to Asset Collection sample when creation fails.
- Added error message to Asset Collection sample during creation and edit when a collection with the same name already exists.

### Changed
- [Breaking] `GetDatasetAsync` and `GetFileAsync` methods of `IAssetRepository` now require a `DatasetFields` and `FileFields` parameter respectively.
- [Breaking] `Id` in `IAssetProject` replaced by `Descriptor` of type `ProjectDescriptor`.
- Refactored `AssetDataSource` to match other packages.
- Change minimal Unity version to 2022.3

### Fixed
- Asset Collection sample list selection now allows de-selection of item.
- Remove usage of system.web for encoding urls.
- Remove Cancellation tokens timeout from Asset Management sample to allow big file to be uploaded.
- Fix file size display to correspond correctly on the unit.
- Add checks on UI buttons in the Asset Management sample to prevent multiple clicks
- Add UI message to give feedback after an action like (publish asset, save asset)
- Added filtering of already included assets for the 'Add to Collection' asset list in Asset Collection sample.
- `IDataset` refreshes its data when `UpdateAsync` and `RemoveFileAsync` are called.
- Fix `IAsset.UnlinkFromProjectAsync` unlinking context project instead of the one passed in parameter.

### Removed
- Removing empty and unused directories and scripts.
- Removed `IAssetHttpClient`, its implementations and its tests.

## [0.7.0] - 2023-10-18

### Added
- `IDataset` to expose dataset operations.
- Default thumbnails in asset discovery sample
- `SerializeIdentifiers` method added to `IAsset` to allow for serialization.
- `DeserializeAssetIdentifiers` added to `IAssetRepository` to deserialize identifiers into a usable `AssetDescriptor`.
- `Serialize` method added to `IAsset` to allow for serialization.
- `DeserializeAsset` added to `IAssetRepository` to deserialize an asset from a JSON string.
- [Breaking] `FieldsFilter` added to `GetAssetAsync` operations and to the `IAssetSearchFilter` to define which `IAsset` fields are populated.
- `MockDataSource` class added. `UC_MOCK_ASSETS` symbol must be defined to use the `MockDataSource` instead of `AssetDataSource`.
- `GetFileUrl` added to `IDataset` to get the a file download url.
- `ConditionalSearchCriteria`, `DatasetSearchFilter`, `MetadataSearchFilter`
- `AssetType` enum to get predefined Asset's type supported values.
- `IsVisible` property added to `IDataset`, `IDatasetUpdate`, `IDatasetUpdateData`, `Dataset`, `DatasetUpdate`, `DatasetUpdateData` and `DatasetSearchFilter`.
- `WorkflowName` property added to `DatasetEntity`, `IDatasetData` and `DatasetData`.
- `IFile.Userchecksum` property
- `InvalidateUrls` method added to `IFile` to clean up the cached download and upload urls of the file.
- `LinkedDatasetIds` property added to `IFile`.
- `Descriptor` in `IAsset`.
- `LinkedProjects` property added to `IAsset`.
- `Descriptor` in `IDataset`.
- `Descriptor` in `IFile`.
- `WithProject` method to `IAsset` to switch between projects.
- `WithDataset` method to `IFile` to switch between datasets.

### Changed
- Changed the discovery sample to show smaller thumbnails by using an image resizer service.
- [Breaking] Migration to v1 of the Assets API.
- [Breaking] `IFile` replaces `IAssetFile` for file operations.
- `IAsset` exposes `IDataset` and `IFile`.
- [Breaking] New `AuthoringInfo` struct encapsulates `Created`, `CreatedBy`, `Updated`, and `UpdatedBy` properties.
- Changed `MockDataSource` to return 2 files in mocked `DatasetData.FileOrder`.
- [Breaking] Updated `AssetSearchCriteria` properties for parity with searchable fields of `IAsset`.
- [Breaking] Renamed `IProject` to `IAssetProject` to avoid conflicts with Identity's `IProject`.
- [Breaking] Remove the AssetVersionId and AssetVersionDescriptor structs and replace by AssetVersion in the AssetDescriptor.
- [Breaking] Changed `IAsset.Type` property type from `string` to `AssetType` enum.
- Updated `Asset Discovery`, `Asset Manager`, `Asset database uploader` samples to use the dataset.
- [Breaking] `ListFiles` in `IDataset` renamed to `ListFilesAsync`.
- [Breaking] `GetAssetDownloadUrlsAsync` of `IAsset` returns a mapping of file paths to Uris.
- [Breaking] Renamed `LinkedDatasetIds` in `IFile` to `LinkedDatasets` and enumerable type changed to `DatasetDescriptor`.

### Fixed
- Fixed CryptographicUnexpectedOperationException during Md5 checksum calculation.
- Fixed `AssetSearchFilter.Type` criteria in search and aggregate requests.
- Fixed Shared Samples Search bar to allow search by type.
- Fixed Get collections json parsing.
- Fixed Pagination in assets search
- Fixed GetFileUrl in `Dataset` to start with ServiceUrl.
- Fixed Pagination in project list
- Cross project search with included collections.
- Aggregation search with included collections.

### Removed
- [Breaking] Removed `AssetServiceConfiguration`.
- [Breaking] `AssetTaxonomy`, `AssetAuthor`, `AssetLocation` removed.
- [Breaking] `Metadata` property of `IProject` removed.
- [Breaking] `CatalogId` and `Metadata` properties removed from `IAssetCollection`
- [Breaking] `VersionName`, `Origin`, `ShortId`, `Categories`, `StatusDetails` properties removed from `IAsset`
- Removed mocking code from `AssetDataSource`.
- [Breaking] Removed `IOrganizationProvider` and `IOrganization`. Use Identity's `IOrganizationRepository` and `IOrganization` instead.
- [Breaking] Removed `Id` and `Version` properties from `IAsset`; use `Descriptor.AssetId` and `Descriptor.Version` instead.
- [Breaking] Removed `Id` property from `IDataset`; use `Descriptor.DatasetId`.
- [Breaking] Removed `Path` property from `IFile`; use `Descriptor.Path`.
- [Breaking] Remove `UserCriteria` from `AssetSearchFilter`. For custom fields, extend `AssetSearchFilter` or implement `IAssetSearchFilter`.

## [0.6.0] - 2023-09-15

### Added
- Added single entry point for API calls: `IAssetRepository`.

### Changed
- Turned AssetManager sample visible
- [Breaking] Changed how `AssetSearchFilter` searches collections. Instead of a `SearchCriteria`, populate the `Collections` list with the collection paths to search.

### Removed
- [Breaking] Removed all manager scripts: `IAssetProvider`, `IAssetManager`, `IFileManager`, `ICollectionManager`.
- All previous actions are now available in entities: `IProject`, `IAsset`, `IAssetFile`, `IAssetCollection`.

## [0.5.0] - 2023-08-31

### Added
- [Breaking] Added the `DownloadAssetFileAsync` to `IAssetFileManager`.
- Added UseCaseDownloadFileExample documentation page.
- [Breaking] Added two new methods SendAsync to `IAssetHttpClient` to provide ways to do requests passing HttpCompletionOption argument.
- Added `InvalidDownloadUrlException` to `AssetExceptions`.
- Added official support for the latest LTS Editor 2022.3 while maintaining support for 2021.3.

### Changed
- [Breaking] Changed the `UploadAssetFileAsync` from `IAssetFileManager` to add the progress tracking.
- Changed the DiscoverySample download action to use the new `DownloadAssetFileAsync` method.
- Put `InternalsVisibleTo` attributes under conditional compilation in `Core\AssemblyInfo.cs`

### Fixed
- Fixed issue where search wasn't returning all results.

## [0.4.0] - 2023-08-17

### Added
- Added `IOrganization` property to `IProject`
- Added `AssetServiceConfiguration` parameter to `CloudAssetProvider` and `CloudAssetManager` constructors
- Added `SearchAsync` of `IAssetProvider` allow search across projects
- Updated Asset Database Uploader to keep the extension of source file in the Asset file name.
- Added search across all projects to Discovery sample
- New documentation.

### Changed
- Updated the UI of the Collection Management sample
- Updated Assets Runtime sample to allow create, upload actions.

### Removed
- [Breaking] Removed `IOrganization` parameter from all API methods that also have an `IProject` parameter.

## [0.3.0] - 2023-08-03

### Added
- Added asset collection management sample to allow creation, deletion, and updating of asset collections.

### Changed
- Updated Asset Database Uploader to keep the extension of source file in the Asset file name.
- Added headers to requests.
- [Breaking] `SearchAsync` of `IAssetProvider` now returns an `IAsyncEnumerable<IAsset>`.
- [Breaking] `GetCurrentUserProjectList` of `IProjectProvider` renamed to `ListProjectsAsync` and now returns an `IAsyncEnumerable<IProject>`.

### Removed
- [Breaking] Removed `GetProjectsByOrganizationAndUserIdsAsync` from `IProjectProvider`
- [Breaking] Removed `IPagedResponse<T>`, `IAssetPage`, and `IProjectPage`.
- [Breaking] Removed `Projects` list property from `IOrganization`.

## [0.2.1] - 2023-07-20

### Added
- Added Send asset to review, Approve asset in review and Reject asset in review requests in `IAssetManager`
- Added Check on GetAssetByIdAndVersionRequest.IncludeThumbnailDownloadURLs value before adding it to the query parameters
- Adds search value selection to Discovery sample

### Changed
- Updated miscellaneous existing documentation pages.

### Fixed
- Fixed a bug where the `GetAssetFileUrlAsync` of `CloudFileAssetManager` was failing and no url was returned.

## [0.2.0] - 2023-07-06

### Added
- UnityEditor uploader Sample.
- [Breaking] Added `IAssetSearchFilter` interface to allow for more complex search filters and replaced references to the implemented `AssetSearchFilter` class with the interface.
- Exposed an abstract `AssetPage` for extension.
- Added overload for `SearchAsync` in `ICloudAssetProvider` that takes an `IAsset` type parameter.
- New method to get asset collections in AssetManager
- New documentation.
- Added `IOrganization` and `IProject` properties to `IAssetCollection`.

### Changed
- Updated sample to show thumbnails.
- Added `IAsset` type parameter to `IAssetPage` `GetNextAsync` method.
- Updated documentation for getting started pathways.
- [Breaking] Replaced the `TryGetValue` function in `Aggregation` with the `Values` property.
- Updated sample to show asset collections.
- Updated documentation's Getting started pages.
- [Breaking] Replaced the `IAssetFile` parameter of `CreateAssetFileAsync` in `IAssetFileManager` with an `IAssetFileCreation` object.
- Added an `IAssetFile` return value to `CreateAssetFileAsync` in `IAssetFileManager`.
- [Breaking] Changed the return value of task `UploadAssetFileAsync` from `IAssetFileManager` to `bool`.
- [Breaking] Uses of `ServiceHostConfiguration` have been replaced for `IServiceHostResolver`.
- Updated all references to Common's `IHttpClient.SendAsync` to match its new signature.
- [Breaking] Renamed `IAssetCollectionController` to `IAssetCollectionManager`.
- [Breaking] Removed the return values of `InsertAssetsToCollectionAsync` and `RemoveAssetsFromCollectionAsync` in `IAssetCollectionManager`.
- [Breaking] Removed the `IOrganization` and `IProject` parameters from `UpdateCollectionAsync`, `DeleteCollectionAsync`, and `MoveCollectionAsync` in `IAssetCollectionManager`.

### Removed
- [Breaking] Removed the `AggregationFields` property from `Aggregation`.

## [0.1.0] - 2023-06-22

### Changed
- Upgrade to Moq 2.0.0-pre.2
- Removed default values for `ServiceEnvironment` in documentation.

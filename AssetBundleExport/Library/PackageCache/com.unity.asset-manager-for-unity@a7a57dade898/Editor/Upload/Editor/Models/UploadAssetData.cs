using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEngine;

namespace Unity.AssetManager.Upload.Editor
{
    [Serializable]
    class UploadAssetData : BaseAssetData
    {
        // TODO Ideally we should use a class that contains both the status (pending or new) and actual sequence number
        public static readonly int NewVersionSequenceNumber = -42; // This can be any number as long as it's negative because versions from the server are always positive

        [SerializeField]
        List<AssetIdentifier> m_Dependencies = new();

        [SerializeField]
        AssetIdentifier m_Identifier;

        [SerializeField]
        string m_Name;

        [SerializeField]
        string m_AssetGuid;

        [SerializeField]
        string m_AssetPath;

        [SerializeField]
        bool m_Ignored;

        [SerializeField]
        bool m_IsDependency;

        [SerializeField]
        Core.Editor.AssetType m_AssetType;

        [SerializeField]
        List<string> m_MainFileGuids = new();

        [SerializeField]
        List<string> m_AdditionalFileGuids = new();

        [SerializeField]
        List<string> m_Tags;

        Task<Texture2D> m_GetThumbnailTask;
        Task<UploadAttribute.UploadStatus> m_UploadStatusTask;

        [SerializeField]
        AssetIdentifier m_ExistingAssetIdentifier;

        [SerializeField]
        ProjectIdentifier m_TargetProject;

        [SerializeField]
        UploadAttribute.UploadStatus m_SelfStatus; // This is the status of the asset itself, not the status of the asset in relation to dependencies

        [SerializeField]
        UploadAttribute.UploadStatus m_ResolvedStatus; // This is the status of the asset in relation to dependencies

        public UploadAttribute.UploadStatus? UploadStatus => AssetDataAttributeCollection.GetAttribute<UploadAttribute>()?.Status;

        [SerializeField]
        int m_ExistingSequenceNumber;

        [SerializeField]
        bool m_ExistingAssetIsAccessible;

        [SerializeField]
        UploadFilePathMode m_FilePathMode;

        [SerializeField]
        ComparisonDetails m_ComparisonDetails;

        static bool s_UseAdvancedPreviewer = false;

        public override string Name => m_Name;
        public string Guid => m_AssetGuid;
        public override AssetIdentifier Identifier => m_Identifier;

        // Upload Asset Data Should have its own settings so it can rebuild itself. If settings are changed, it rebuilds itself. again mainly files
        // So you can then change the data from anywhere without going through the staging that lives in the upload page

        public UploadFilePathMode FilePathMode
        {
            get => m_FilePathMode;
            set
            {
                if (m_FilePathMode == value)
                    return;

                m_FilePathMode = value;
                ResolveFilePaths();
            }
        }

        public override int SequenceNumber
        {
            get
            {
                if (!CanBeUploaded && m_ExistingAssetIdentifier != null)
                    return m_ExistingSequenceNumber;

                return NewVersionSequenceNumber;
            }
        }

        public override int ParentSequenceNumber => -1;
        public override string Changelog => "";
        public override Core.Editor.AssetType AssetType => m_AssetType;
        public override string Status => "Local";
        public override DateTime? Updated => null;
        public override DateTime? Created => null;
        public override IEnumerable<string> Tags => m_Tags;
        public override string Description => "";
        public override string CreatedBy => "";
        public override string UpdatedBy => "";
        public override IEnumerable<AssetLabel> Labels => null;
        public override IEnumerable<ProjectIdentifier> LinkedProjects => Enumerable.Empty<ProjectIdentifier>();

        public override IEnumerable<AssetIdentifier> Dependencies
        {
            get => m_Dependencies;
            internal set => m_Dependencies = value?.ToList() ?? new List<AssetIdentifier>();
        }

        public override IEnumerable<BaseAssetData> Versions => Array.Empty<BaseAssetData>();

        public bool IsIgnored
        {
            get => m_Ignored;
            set
            {
                m_Ignored = value;
                InvokeEvent(AssetDataEventType.ToggleValueChanged);
            }
        }

        public void NotifyIgnoredChanged()
        {
            InvokeEvent(AssetDataEventType.ToggleValueChanged);
        }

        public bool IsDependency
        {
            get => m_IsDependency;
            set => m_IsDependency = value;
        }

        public bool CanBeIgnored
        {
            get
            {
                if (m_ResolvedStatus is UploadAttribute.UploadStatus.Skip or UploadAttribute.UploadStatus.SourceControlled or UploadAttribute.UploadStatus.DontUpload)
                    return false;

                return m_IsDependency;
            }
        }

        public bool CanBeRemoved => !m_IsDependency;

        public bool CanBeUploaded
        {
            get
            {
                if (m_IsDependency && m_Ignored)
                {
                    return false;
                }

                return m_ResolvedStatus is UploadAttribute.UploadStatus.Override or UploadAttribute.UploadStatus.Duplicate or UploadAttribute.UploadStatus.Add;
            }
        }

        public ComparisonResults ComparisonDetails => m_ComparisonDetails.Results;

        public AssetIdentifier TargetAssetIdentifier
        {
            get
            {
                if (m_ResolvedStatus is UploadAttribute.UploadStatus.Add or UploadAttribute.UploadStatus.Duplicate)
                {
                    return null;
                }

                return m_ExistingAssetIdentifier;
            }
        }

        public UploadAssetData(AssetIdentifier localIdentifier,
            string assetGuid,
            IEnumerable<string> mainFileAssetGuids,
            IEnumerable<string> additionalFileAssetGuids,
            IEnumerable<UploadAssetData> dependencies,
            BaseAssetData existingAssetData,
            ProjectIdentifier targetProject,
            UploadFilePathMode filePathMode)
        {
            Utilities.DevAssert(assetGuid != null);
            Utilities.DevAssert(!string.IsNullOrEmpty(targetProject.OrganizationId));
            Utilities.DevAssert(!string.IsNullOrEmpty(targetProject.ProjectId));

            Utilities.DevAssert(localIdentifier.IsLocal());
            Utilities.DevAssert(existingAssetData == null || !existingAssetData.Identifier.IsLocal());
            Utilities.DevAssert(existingAssetData == null || existingAssetData.Identifier.OrganizationId == targetProject.OrganizationId);
            Utilities.DevAssert(existingAssetData == null || existingAssetData.Identifier.ProjectId == targetProject.ProjectId);

            m_AssetGuid = assetGuid;
            m_Identifier = localIdentifier;
            m_TargetProject = targetProject;

            var assetDatabaseProxy = ServicesContainer.instance.Resolve<IAssetDatabaseProxy>();
            m_AssetPath = assetDatabaseProxy.GuidToAssetPath(m_AssetGuid);

            Utilities.DevAssert(!string.IsNullOrEmpty(m_AssetPath));

            // Dependencies
            if (dependencies != null)
            {
                m_Dependencies = dependencies.Select(d => d.Identifier).ToList();
            }

            // Files
            m_FilePathMode = filePathMode;
            AddFiles(mainFileAssetGuids, mainFiles: true);
            AddFiles(additionalFileAssetGuids, mainFiles: false);

            // Tags
            var tags = new HashSet<string>();

            if (existingAssetData != null)
            {
                // If the asset is new (i.e. existingAssetIdentifier is null), we need to populate fields from it.
                m_ExistingAssetIdentifier = existingAssetData.Identifier;
                m_ExistingSequenceNumber = existingAssetData.SequenceNumber;

                m_Name = existingAssetData.Name;

                CopyMetadata(existingAssetData.Metadata);

                tags = existingAssetData.Tags.ToHashSet();
            }
            else
            {
                m_ExistingAssetIdentifier = null;
                m_ExistingSequenceNumber = -1;

                m_Name = Path.GetFileNameWithoutExtension(m_AssetPath);
                m_Metadata = new MetadataContainer();
            }

            tags.UnionWith(TagExtractor.ExtractFromAsset(m_AssetPath));
            m_Tags = tags.ToList();
        }

        public IUploadAsset GenerateUploadAsset(string collectionPath)
        {
            Utilities.DevAssert(m_ResolvedStatus != UploadAttribute.UploadStatus.DontUpload
                && m_ResolvedStatus != UploadAttribute.UploadStatus.Skip
                && m_ResolvedStatus != UploadAttribute.UploadStatus.ErrorOutsideProject);

            var uploadAsset = new UploadAsset(m_Name, m_AssetGuid, m_Identifier, m_AssetType,
                GetFiles(x => x.IsSource).Cast<UploadAssetDataFile>().Select(f => f.GenerateUploadFile()),
                m_Tags, ResolveDependencyIdentifiers(), m_Metadata,
                m_ResolvedStatus == UploadAttribute.UploadStatus.Override ? m_ExistingAssetIdentifier : null,
                ComparisonDetails, m_TargetProject, collectionPath);

            return uploadAsset;
        }

        public override async Task GetThumbnailAsync(CancellationToken token = default)
        {
            if (m_GetThumbnailTask == null)
            {
                var asset = ServicesContainer.instance.Resolve<IAssetDatabaseProxy>().LoadAssetAtPath(m_AssetPath);

                if (asset != null)
                {
                    m_GetThumbnailTask = s_UseAdvancedPreviewer
                        ? AssetPreviewer.GenerateAdvancedPreview(asset, m_AssetPath)
                        : AssetPreviewer.GetDefaultPreviewTexture(asset);
                }
            }

            var texture = m_GetThumbnailTask != null ? await m_GetThumbnailTask : null;
            m_GetThumbnailTask = null;

            Thumbnail = texture;
        }

        public async Task ResolveSelfStatus(UploadAssetMode uploadMode, ImportAttribute.ImportStatus? cloudStatus, CancellationToken token)
        {
            ResetAssetDataAttributes();

            m_UploadStatusTask ??= ResolveSelfStatusInternalAsync(uploadMode, cloudStatus, token);

            try
            {
                m_SelfStatus = await m_UploadStatusTask;
            }
            catch (Exception)
            {
                m_UploadStatusTask = null;
                throw;
            }

            m_UploadStatusTask = null;
        }

        public void ResolveFinalStatus(UploadAssetMode uploadMode)
        {
            Utilities.DevAssert(m_SelfStatus != UploadAttribute.UploadStatus.DontUpload, "ResolveSelfStatus must be called before ResolveFinalStatus");

            // If the asset is already modified, no need to check for dependencies since they will not change the fact that the asset is modified
            // Also, if the asset is not re-uploading to a target, we have nothing to compare too
            UploadAttribute.UploadStatus resolvedStatus;

            if (m_SelfStatus != UploadAttribute.UploadStatus.Skip || m_ExistingAssetIdentifier == null)
            {
                resolvedStatus = m_SelfStatus;
            }
            else if (uploadMode == UploadAssetMode.ForceNewAsset)
            {
                resolvedStatus = m_SelfStatus;
            }
            else
            {
                var result = CompareResolvedDependencies();

                if (result.Results == ComparisonResults.None)
                {
                    resolvedStatus = m_SelfStatus;
                    m_ComparisonDetails = Core.Editor.ComparisonDetails.Merge(m_ComparisonDetails, result);
                }
                else
                {
                    resolvedStatus = UploadAttribute.UploadStatus.Override;
                    m_ComparisonDetails = Core.Editor.ComparisonDetails.Merge(m_ComparisonDetails, result);
                }
            }

            m_ResolvedStatus = resolvedStatus;
            AssetDataAttributeCollection = GetResolveStatusAttributes();
        }

        public override void ResetAssetDataAttributes()
        {
            m_SelfStatus = UploadAttribute.UploadStatus.DontUpload;
            m_ResolvedStatus = UploadAttribute.UploadStatus.DontUpload;
            AssetDataAttributeCollection = GetResolveStatusAttributes();
        }

        public override Task RefreshAssetDataAttributesAsync(CancellationToken token = default) => Task.CompletedTask;

        public override Task ResolveDatasetsAsync(CancellationToken token = default) => Task.CompletedTask;

        public override Task RefreshPropertiesAsync(CancellationToken token = default) => Task.CompletedTask;

        public override Task RefreshVersionsAsync(CancellationToken token = default) => Task.CompletedTask;

        public override Task RefreshDependenciesAsync(CancellationToken token = default) => Task.CompletedTask;

        public override Task RefreshLinkedProjectsAsync(CancellationToken token = default) => Task.CompletedTask;

        async Task<UploadAttribute.UploadStatus> ResolveSelfStatusInternalAsync(UploadAssetMode uploadMode, ImportAttribute.ImportStatus? cloudStatus, CancellationToken token)
        {
            if (!DependencyUtils.IsPathInsideAssetsFolder(m_AssetPath))
            {
                return UploadAttribute.UploadStatus.ErrorOutsideProject;
            }

            if (m_ExistingAssetIdentifier == null)
            {
                return UploadAttribute.UploadStatus.Add;
            }

            if (uploadMode == UploadAssetMode.ForceNewAsset)
            {
                return UploadAttribute.UploadStatus.Duplicate;
            }

            var assetDataManager = ServicesContainer.instance.Resolve<IAssetDataManager>();
            var importedAssetInfo = assetDataManager.GetImportedAssetInfo(m_ExistingAssetIdentifier);

            if (importedAssetInfo == null)
            {
                return UploadAttribute.UploadStatus.Add;
            }

            if (cloudStatus.HasValue)
            {
                m_ExistingAssetIsAccessible = cloudStatus != ImportAttribute.ImportStatus.ErrorSync;
            }

            if (!m_ExistingAssetIsAccessible)
            {
                return UploadAttribute.UploadStatus.Add;
            }

            return await ResolveSelfStatusInternalAsync(importedAssetInfo, uploadMode, cloudStatus, token);
        }

        async Task<UploadAttribute.UploadStatus> ResolveSelfStatusInternalAsync(ImportedAssetInfo importedAssetInfo, UploadAssetMode uploadMode, ImportAttribute.ImportStatus? cloudStatus, CancellationToken token)
        {
            // When the source dataset is Source Controlled, it cannot be uploaded to.
            var isSourceControlled = false;

            // When the asset is not in the source dataset, it can only be uploaded as a new asset.
            var isAssetInSourceDataset = false;

            AssetDataset sourceDataset;
            if (cloudStatus.HasValue)
            {
                var assetsProvider = ServicesContainer.instance.Resolve<IAssetsProvider>();
                sourceDataset = await GetSourceDataset(importedAssetInfo.AssetData, assetsProvider, token);
            }
            else
            {
                sourceDataset = importedAssetInfo.AssetData.Datasets.FirstOrDefault(d => d.IsSource);
            }

            isSourceControlled = sourceDataset is {IsSourceControlled: true};

            // Check if the imported file is in the source dataset
            var importedFileInfo = importedAssetInfo.FileInfos.FirstOrDefault(f => f.Guid == m_AssetGuid);
            if (importedFileInfo != null)
            {
                isAssetInSourceDataset = string.IsNullOrEmpty(importedFileInfo.DatasetId) || importedFileInfo.DatasetId == sourceDataset?.Id;
            }

            switch (uploadMode)
            {
                case UploadAssetMode.SkipIdentical:

                    if (isSourceControlled)
                    {
                        return UploadAttribute.UploadStatus.SourceControlled;
                    }

                    if (!isAssetInSourceDataset)
                    {
                        return UploadAttribute.UploadStatus.Add;
                    }

                    if (await IsLocallyModifiedAsync(importedAssetInfo, token))
                    {
                        return UploadAttribute.UploadStatus.Override;
                    }

                    return UploadAttribute.UploadStatus.Skip;

                case UploadAssetMode.ForceNewVersion:
                    if (isSourceControlled)
                    {
                        return UploadAttribute.UploadStatus.SourceControlled;
                    }

                    if (isAssetInSourceDataset)
                    {
                        // Make sure we gather comparison details so that we update the correct data if necessary
                        _ = await IsLocallyModifiedAsync(importedAssetInfo, token);
                        return UploadAttribute.UploadStatus.Override;
                    }

                    return UploadAttribute.UploadStatus.Add;

                default:
                    return UploadAttribute.UploadStatus.DontUpload;
            }
        }

        async Task<bool> IsLocallyModifiedAsync(ImportedAssetInfo importedAssetInfo, CancellationToken token)
        {
            if (importedAssetInfo?.AssetData == null)
            {
                return false;
            }

            // Only compare files in the source dataset as it is the only dataset we will upload to.
            m_ComparisonDetails = Compare(importedAssetInfo.AssetData, dataset => dataset.IsSource);
            m_ComparisonDetails = Core.Editor.ComparisonDetails.Merge(m_ComparisonDetails, PreCompareDependencies(importedAssetInfo.AssetData?.Dependencies));

            // Next, check if there are modifications in the files.
            var fileComparisons = await HasLocallyModifiedFilesAsync(importedAssetInfo, token);
            m_ComparisonDetails = Core.Editor.ComparisonDetails.Merge(m_ComparisonDetails, fileComparisons);

            return m_ComparisonDetails.Results != ComparisonResults.None;
        }

        static async Task<ComparisonDetails> HasLocallyModifiedFilesAsync(ImportedAssetInfo importedAssetInfo, CancellationToken token = default)
        {
            if (importedAssetInfo == null)
            {
                // Un-imported asset cannot have modified files by definition
                return default;
            }

            var assetDatabase = ServicesContainer.instance.Resolve<IAssetDatabaseProxy>();
            var ioProxy = ServicesContainer.instance.Resolve<IIOProxy>();
            var fileUtility = ServicesContainer.instance.Resolve<IFileUtility>();

            var results = new List<ComparisonDetails>();

            // Otherwise, check if the files are identical
            foreach (var importedFileInfo in importedAssetInfo.FileInfos)
            {
                var path = assetDatabase.GuidToAssetPath(importedFileInfo.Guid);

                var result = await fileUtility.FileWasModified(path, importedFileInfo.Timestamp, importedFileInfo.Checksum, token);
                if (result.Results != ComparisonResults.None)
                {
                    results.Add(result);
                    break; // Because checking files is potentially expensive, we only need to know if at least one file was modified.
                }

                // Check if the meta file was modified
                var metaPath = MetafilesHelper.AssetMetaFile(path);
                if (ioProxy.FileExists(metaPath))
                {
                    result = await fileUtility.FileWasModified(metaPath, importedFileInfo.MetalFileTimestamp, importedFileInfo.MetaFileChecksum, token);
                    if (result.Results != ComparisonResults.None)
                    {
                        results.Add(result);
                        break; // Because checking files is potentially expensive, we only need to know if at least one file was modified.
                    }
                }
            }

            return Core.Editor.ComparisonDetails.Merge(results.ToArray());
        }

        static async Task<AssetDataset> GetSourceDataset(BaseAssetData assetData, IAssetsProvider assetsProvider, CancellationToken token)
        {
            var sourceDataset = assetData.Datasets.FirstOrDefault(d => d.IsSource);
            if (sourceDataset != null && sourceDataset.SystemTags.Contains(k_NotSynced))
            {
                // Come from an older version of persistence, we need to update the system tags
                var cloudDataset = await assetsProvider.GetDatasetAsync(assetData as AssetData, new List<string> {k_Source}, token);
                if (cloudDataset != null)
                {
                    sourceDataset.Copy(cloudDataset);
                }
            }

            return sourceDataset;
        }

        void ResolveFilePaths()
        {
            var allFiles = m_MainFileGuids.Union(m_AdditionalFileGuids).Distinct();

            var assetDatabaseProxy = ServicesContainer.instance.Resolve<IAssetDatabaseProxy>();
            var filePaths = allFiles.Select(guid => assetDatabaseProxy.GuidToAssetPath(guid)).ToList();

            var processedPaths = new HashSet<string>();

            var commonPath = m_FilePathMode == UploadFilePathMode.Compact ? Utilities.ExtractCommonFolder(filePaths) : null;

            var files = new List<UploadAssetDataFile>();

            foreach (var filePath in filePaths)
            {
                var sanitizedPath = filePath.Replace('\\', '/').ToLower();

                if (!processedPaths.Add(sanitizedPath))
                    continue;

                if (!AddAssetAndItsMetaFile(files, filePath, commonPath, m_FilePathMode))
                {
                    Debug.LogWarning($"Asset {filePath} is already added to the upload list.");
                }
            }

            // Update Asset Type since it might have changed
            var extensions = files.Select(e => Path.GetExtension(e.Path)).ToHashSet();
            var primaryExtension = AssetDataTypeHelper.GetAssetPrimaryExtension(extensions);

            m_AssetType = AssetDataTypeHelper.GetUnityAssetType(primaryExtension);

            // Source Dataset
            m_Datasets = new List<AssetDataset>
            {
                new(k_Source, new List<string> {k_Source}, files)
            };
            ResolvePrimaryExtension();
        }

        AssetDataAttributeCollection GetResolveStatusAttributes()
        {
            return new AssetDataAttributeCollection(new LinkedDependencyAttribute(m_IsDependency),
                new UploadAttribute(m_ResolvedStatus, $"{ComparisonDetails}"));
        }

        /// <summary>
        /// Pre-checks whether the dependencies have been modified. This is before their self-status has been resolved.
        /// </summary>
        ComparisonDetails PreCompareDependencies(IEnumerable<AssetIdentifier> otherDependencies)
        {
            if (otherDependencies == null)
                return default;

            var dependencies = ResolveDependencyIdentifiers(false);
            if (dependencies == null)
                return default;

            var localHashset = dependencies.Select(x => x.AssetId).ToHashSet();
            var otherHashset = otherDependencies.Select(x => x.AssetId).ToHashSet();

            var results = new List<ComparisonDetails>();

            // Check for added dependencies
            var dependenciesAdded = localHashset.Except(otherHashset);
            if (dependenciesAdded.Any())
            {
                results.Add(new ComparisonDetails(ComparisonResults.DependenciesAdded, $"Dependencies added to {Name}: {string.Join(", ", dependenciesAdded)}"));
            }

            // Check for removed dependencies
            var dependenciesRemoved = otherHashset.Except(localHashset);
            if (dependenciesRemoved.Any())
            {
                results.Add(new ComparisonDetails(ComparisonResults.DependenciesRemoved, $"Dependencies removed from {Name}: {string.Join(", ", dependenciesRemoved)}"));
            }

            foreach (var dependency in dependencies)
            {
                var otherDependency = otherDependencies.FirstOrDefault(d => d.AssetId == dependency.AssetId);
                if (otherDependency != null && otherDependency.Version != dependency.Version)
                {
                    results.Add(new ComparisonDetails(ComparisonResults.DependenciesModified, $"The version of dependency {dependency.AssetId} has changed for {Name}."));
                }
            }

            return Core.Editor.ComparisonDetails.Merge(results.ToArray());
        }

        /// <summary>
        /// Checks whether the dependencies have been modified after their self-status has been resolved.
        /// </summary>
        ComparisonDetails CompareResolvedDependencies()
        {
            var dependencies = ResolveDependencyIdentifiers();
            if (dependencies == null)
                return default;

            var results = new List<ComparisonDetails>();

            foreach (var otherDependency in dependencies.Where(d => d.IsLocal()))
            {
                results.Add(new ComparisonDetails(ComparisonResults.DependenciesModified, $"The dependency for {otherDependency.AssetId} has been modified."));
            }

            return Core.Editor.ComparisonDetails.Merge(results.ToArray());
        }

        IEnumerable<AssetIdentifier> ResolveDependencyIdentifiers(bool evaluateUploadStatus = true)
        {
            var assetDataManager = ServicesContainer.instance.Resolve<IAssetDataManager>();

            var dependencyIdentifiers = new List<AssetIdentifier>();
            foreach (var id in Dependencies)
            {
                var dependency = assetDataManager.GetAssetData(id) as UploadAssetData;
                Utilities.DevAssert(dependency != null, $"Dependency {id.AssetId} for {Name} could not be loaded.");

                if (dependency != null)
                {
                    var identifier = dependency.TargetAssetIdentifier ?? dependency.Identifier;

                    // When evaluating the upload status, we need to check if the dependency can be uploaded, otherwise we use the existing asset identifier.
                    if (evaluateUploadStatus)
                    {
                        identifier = dependency.CanBeUploaded ? identifier : dependency.m_ExistingAssetIdentifier;
                    }

                    Utilities.DevAssert(!string.IsNullOrEmpty(identifier?.AssetId), $"Id is not defined for dependency of {Name}.");
                    if (!string.IsNullOrEmpty(identifier?.AssetId))
                    {
                        dependencyIdentifiers.Add(identifier);
                    }
                }
            }

            return dependencyIdentifiers;
        }

        public void AddFiles(IEnumerable<string> fileAssetGuids, bool mainFiles)
        {
            var allFiles = m_MainFileGuids.Union(m_AdditionalFileGuids).ToHashSet();
            var targetList = mainFiles ? m_MainFileGuids : m_AdditionalFileGuids;

            foreach (var guid in fileAssetGuids)
            {
                if (allFiles.Contains(guid))
                    continue;

                targetList.Add(guid);
            }

            ResolveFilePaths();
        }

        public void RemoveFiles(IEnumerable<string> fileAssetGuids, bool mainFiles)
        {
            var targetList = mainFiles ? m_MainFileGuids : m_AdditionalFileGuids;

            foreach (var guid in fileAssetGuids)
            {
                targetList.Remove(guid);
            }

            ResolveFilePaths();
        }

        public override bool CanRemovedFile(BaseAssetDataFile assetDataFile)
        {
            return m_AdditionalFileGuids.Contains(assetDataFile.Guid);
        }

        public override void RemoveFile(BaseAssetDataFile assetDataFile)
        {
            m_AdditionalFileGuids.Remove(assetDataFile.Guid);
            ResolveFilePaths();
        }

        public void RemoveAllAdditionalFiles()
        {
            m_AdditionalFileGuids.Clear();
            ResolveFilePaths();
        }

        public bool HasAdditionalFiles()
        {
            return m_AdditionalFileGuids.Count > 0;
        }

        static bool AddAssetAndItsMetaFile(IList<UploadAssetDataFile> addedFiles, string assetPath, string commonPath, UploadFilePathMode filePathMode)
        {
            string dst;

            switch (filePathMode)
            {
                case UploadFilePathMode.Compact:
                    if (string.IsNullOrEmpty(commonPath))
                    {
                        dst = Utilities.GetPathRelativeToAssetsFolder(assetPath);
                    }
                    else
                    {
                        var normalizedPath = Utilities.NormalizePathSeparators(assetPath);
                        var commonPathNormalized = Utilities.NormalizePathSeparators(commonPath);

                        Utilities.DevAssert(normalizedPath.StartsWith(commonPathNormalized));
                        dst = normalizedPath[commonPathNormalized.Length..];
                    }

                    break;

                case UploadFilePathMode.Flatten:
                    dst = GetFlattenPath(addedFiles, assetPath);
                    break;

                default:
                    dst = Utilities.GetPathRelativeToAssetsFolder(assetPath);
                    break;
            }

            if (addedFiles.Any(e => Utilities.ComparePaths(e.Path, assetPath)))
            {
                return false;
            }

            addedFiles.Add(new UploadAssetDataFile(assetPath,
                dst, null, new List<string>()));

            var metaFileSourcePath = MetafilesHelper.AssetMetaFile(assetPath);
            if (!string.IsNullOrEmpty(metaFileSourcePath))
            {
                addedFiles.Add(new UploadAssetDataFile(metaFileSourcePath,
                    dst + MetafilesHelper.MetaFileExtension, null, new List<string>()));
            }

            return true;
        }

        static string GetFlattenPath(ICollection<UploadAssetDataFile> files, string assetPath)
        {
            var fileName = Path.GetFileName(assetPath);
            return Utilities.GetUniqueFilename(files.Select(e => e.Path).ToArray(), fileName);
        }

        public void RemoveMetadata(string fieldKey)
        {
            m_Metadata.Remove(fieldKey);
        }

        public void AddMetadata(IMetadata metadata)
        {
            m_Metadata.Add(metadata);
        }
    }
}

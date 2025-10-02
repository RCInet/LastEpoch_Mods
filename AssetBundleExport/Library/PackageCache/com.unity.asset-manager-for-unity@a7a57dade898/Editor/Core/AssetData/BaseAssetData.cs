using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    enum AssetDataEventType
    {
        None,
        ThumbnailChanged,
        AssetDataAttributesChanged,
        FilesChanged,
        PrimaryFileChanged,
        ToggleValueChanged,
        DependenciesChanged,
        PropertiesChanged,
        LinkedProjectsChanged,
    }

    [Serializable]
    abstract class BaseAssetData
    {
        public delegate void AssetDataChangedDelegate(BaseAssetData assetData, AssetDataEventType eventType);

        public event AssetDataChangedDelegate AssetDataChanged;

        public abstract string Name { get; }
        public abstract AssetIdentifier Identifier { get; }
        public abstract int SequenceNumber { get; }
        public abstract int ParentSequenceNumber { get; }
        public abstract string Changelog { get; }
        public abstract AssetType AssetType { get; }
        public abstract string Status { get; }
        public abstract DateTime? Updated { get; }
        public abstract DateTime? Created { get; }
        public abstract IEnumerable<string> Tags { get; }
        public abstract string Description { get; }
        public abstract string CreatedBy { get; }
        public abstract string UpdatedBy { get; }

        public abstract IEnumerable<AssetIdentifier> Dependencies { get; internal set; }
        public abstract IEnumerable<BaseAssetData> Versions { get; }
        public abstract IEnumerable<AssetLabel> Labels { get; }

        public abstract Task GetThumbnailAsync(CancellationToken token = default);
        public abstract Task ResolveDatasetsAsync(CancellationToken token = default);

        public abstract Task RefreshAssetDataAttributesAsync(CancellationToken token = default);
        public abstract Task RefreshPropertiesAsync(CancellationToken token = default);
        public abstract Task RefreshVersionsAsync(CancellationToken token = default);
        public abstract Task RefreshDependenciesAsync(CancellationToken token = default);
        public abstract Task RefreshLinkedProjectsAsync(CancellationToken token = default);

        public string PrimaryExtension => m_PrimarySourceFile?.Extension;

        protected const string k_Source = AssetDataset.k_SourceTag;
        protected const string k_NotSynced = "NotSynced";

        [SerializeField]
        TextureReference m_Thumbnail = new();

        [SerializeReference]
        AssetDataAttributeCollection m_AssetDataAttributeCollection;

        [SerializeReference]
        protected BaseAssetDataFile m_PrimarySourceFile;

        [SerializeReference]
        protected List<AssetDataset> m_Datasets = new();

        [SerializeReference]
        protected MetadataContainer m_Metadata = new();
        [SerializeField]
        protected List<ProjectIdentifier> m_LinkedProjects = new();

        public virtual Texture2D Thumbnail
        {
            get => m_Thumbnail.Value;
            protected set
            {
                if (m_Thumbnail.Value == value)
                    return;

                m_Thumbnail.Value = value;

                if (m_Thumbnail.Value != null)
                {
                    // Unity destroys textures created during runtime when existing play mode.
                    // Make sure they are flagged to stay in memory
                    m_Thumbnail.Value.hideFlags = HideFlags.HideAndDontSave;
                }

                InvokeEvent(AssetDataEventType.ThumbnailChanged);
            }
        }

        public IMetadataContainer Metadata => m_Metadata;

        internal IEnumerable<IMetadata> MetadataList
        {
            get => m_Metadata;
            set => SetMetadata(value);
        }
        public void SetMetadata(IEnumerable<IMetadata> metadata)
        {
            m_Metadata.Set(metadata);
        }

        public void CopyMetadata(IMetadataContainer metadataContainer)
        {
            // Clone the original IMetadata instead of using the reference so that the original is not modified when modifying this UploadAssetData in the UI
            m_Metadata.Set(metadataContainer.Select(m => m.Clone()));
        }

        public virtual AssetDataAttributeCollection AssetDataAttributeCollection
        {
            get => m_AssetDataAttributeCollection;
            set
            {
                m_AssetDataAttributeCollection = value;
                InvokeEvent(AssetDataEventType.AssetDataAttributesChanged);
            }
        }

        // Virtual to allow overriding in test classes
        public virtual IEnumerable<AssetDataset> Datasets
        {
            get => m_Datasets;
            internal set => m_Datasets = value?.ToList();
        }

        public BaseAssetDataFile PrimarySourceFile
        {
            get => m_PrimarySourceFile;
            private set
            {
                m_PrimarySourceFile = value;
                InvokeEvent(AssetDataEventType.PrimaryFileChanged);
            }
        }

        public virtual IEnumerable<ProjectIdentifier> LinkedProjects
        {
            get => m_LinkedProjects;
            protected set
            {
                m_LinkedProjects = value?.ToList();
                InvokeEvent(AssetDataEventType.LinkedProjectsChanged);
            }
        }

        public virtual void ResetAssetDataAttributes()
        {
            AssetDataAttributeCollection = null;
        }

        protected void InvokeEvent(AssetDataEventType eventType)
        {
            AssetDataChanged?.Invoke(this, eventType);
        }

        public IEnumerable<BaseAssetDataFile> GetFiles(Func<AssetDataset, bool> predicate = null)
        {
            if (predicate != null)
                return Datasets?.Where(predicate).SelectMany(d => d.Files) ?? Array.Empty<BaseAssetDataFile>();

            return Datasets?.SelectMany(d => d.Files) ?? Array.Empty<BaseAssetDataFile>();
        }

        public virtual bool CanRemovedFile(BaseAssetDataFile assetDataFile)
        {
            return false;
        }

        public virtual void RemoveFile(BaseAssetDataFile assetDataFile)
        {
            // Do nothing
        }

        public void ResolvePrimaryExtension()
        {
            if (m_Datasets == null || !m_Datasets.Any())
                return;

            // Only consider files in datasets that will be visible to the user
            PrimarySourceFile = GetFiles(d => d.CanBeImported)?
                .FilterUsableFilesAsPrimaryExtensions()
                .OrderBy(x => x, new AssetDataFileComparerByExtension())
                .LastOrDefault();
        }

        private protected ComparisonDetails Compare(BaseAssetData other, Func<AssetDataset, bool> datasetsToComparePredicate = null)
        {
            // Technically, we should compare ALL editable fields, including the Name and Tags.
            // But until the user can edit those fields, we will only compare the files.

            var results = new List<ComparisonDetails>();

            var localFiles = GetFiles(datasetsToComparePredicate).Select(f => f.Path).Select(Utilities.NormalizePathSeparators).ToHashSet();
            var otherFiles = other.GetFiles(datasetsToComparePredicate).Select(f => f.Path).Select(Utilities.NormalizePathSeparators).ToHashSet();

            // Check for added files
            var filesAdded = localFiles.Except(otherFiles);
            if (filesAdded.Any())
            {
                results.Add(new ComparisonDetails(ComparisonResults.FilesAdded, $"Files added to {Name}: {string.Join(", ", filesAdded)}"));
            }

            // Check for removed files
            var filesRemoved = otherFiles.Except(localFiles);
            if (filesRemoved.Any())
            {
                results.Add(new ComparisonDetails(ComparisonResults.FilesRemoved, $"Files removed from {Name}: {string.Join(", ", filesRemoved)}"));
            }

            // Don't check for modified files, this is done by HasLocallyModifiedFilesAsync as an optional, separate call as this is an expensive operation.

            var localMetadata = Metadata.Select(m => m.FieldKey).ToHashSet();
            var otherMetadata = other.Metadata.Select(m => m.FieldKey).ToHashSet();

            // Check for added metadata
            var metadataAdded = localMetadata.Except(otherMetadata);
            if (metadataAdded.Any())
            {
                results.Add(new ComparisonDetails(ComparisonResults.MetadataAdded, $"Metadata added to {Name}: {string.Join(", ", metadataAdded)}"));
            }

            // Check for removed metadata
            var metadataRemoved = otherMetadata.Except(localMetadata);
            if (metadataRemoved.Any())
            {
                results.Add(new ComparisonDetails(ComparisonResults.MetadataRemoved, $"Metadata removed from {Name}: {string.Join(", ", metadataRemoved)}"));
            }

            // Check if the list of metadata has changed
            foreach (var metadata in Metadata)
            {
                if (other.Metadata.ContainsKey(metadata.FieldKey) && !other.Metadata.ContainsMatch(metadata))
                {
                    results.Add(new ComparisonDetails(ComparisonResults.MetadataModified, $"The value of metadata {metadata.Name} has changed for {Name}."));
                }
            }

            return ComparisonDetails.Merge(results.ToArray());
        }
    }

    // Texture in memory are not serializable by the Editor,
    // This class is used to serialize the texture as a byte array
    [Serializable]
    class TextureReference : ISerializationCallbackReceiver
    {
        [SerializeField]
        byte[] m_Bytes;

        [SerializeField]
        HideFlags m_HideFlags;

        Texture2D m_Texture;

        public Texture2D Value
        {
            get
            {
                if (m_Texture != null)
                    return m_Texture;

                if (m_Bytes == null || m_Bytes.Length == 0)
                    return null;

                m_Texture = new Texture2D(1, 1)
                {
                    hideFlags = m_HideFlags
                };
                m_Texture.LoadImage(m_Bytes);

                return m_Texture;
            }
            set => m_Texture = value;
        }

        public void OnBeforeSerialize()
        {
            m_Bytes = null;
            if (m_Texture != null)
            {
                m_Bytes = m_Texture.EncodeToPNG();
                m_HideFlags = m_Texture.hideFlags;
            }
        }

        public void OnAfterDeserialize()
        {
            // m_Texture will be deserialized on demand
        }
    }
}

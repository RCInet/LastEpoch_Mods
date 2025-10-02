using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    /// <summary>
    /// Map the AssetData class as it was when we persisted data using it directly
    /// </summary>
    [Serializable]
    class AssetDataPersistenceLegacy
    {
        [SerializeField]
        public List<DependencyAssetPersistenceLegacy> m_DependencyAssets = new();

        [SerializeField]
        public string m_JsonAssetSerialized;

        [SerializeField]
        public string m_ThumbnailUrl;

        [SerializeReference]
        public List<AssetDataFilePersistenceLegacy> m_SourceFiles = new();

        [SerializeReference]
        public AssetDataFilePersistenceLegacy m_PrimarySourceFile;
    }

    /// <summary>
    /// Map the AssetDataFile class as it was when we persisted data using it directly
    /// </summary>
    [Serializable]
    class AssetDataFilePersistenceLegacy
    {
        [SerializeField]
        public string m_Path;

        [SerializeField]
        public string m_Extension;

        [SerializeField]
        public bool m_Available;

        [SerializeField]
        public string m_Description;

        [SerializeField]
        public List<string> m_Tags = new();

        [SerializeField]
        public long m_FileSize;

        [SerializeField]
        public string m_Guid;
    }

    /// <summary>
    /// Map the DependencyAsset class as it was when we persisted data using it directly
    /// </summary>
    [Serializable]
    class DependencyAssetPersistenceLegacy
    {
        [SerializeField]
        public AssetIdentifier m_Identifier;

        [SerializeReference]
        public AssetDataPersistenceLegacy m_AssetData;
    }

    /// <summary>
    /// Map the ImportedFile class as it was when we persisted data using it directly
    /// </summary>
    [Serializable]
    class ImportedFileInfoPersistenceLegacy
    {
        public string Guid;
        public string OriginalPath;
    }

    /// <summary>
    /// Map the ImportedAssetInfo class as it was when we persisted data using it directly
    /// </summary>
    [Serializable]
    class ImportedAssetInfoPersistenceLegacy
    {
        [SerializeReference]
        public AssetDataPersistenceLegacy AssetData;

        public List<ImportedFileInfoPersistenceLegacy> FileInfos;
    }

    class PersistenceLegacy : IPersistenceVersion
    {
        readonly Dictionary<ImportedAssetInfoPersistenceLegacy, ImportedAssetInfo> m_ImportedAssetInfos = new();
        readonly Dictionary<ImportedFileInfoPersistenceLegacy, ImportedFileInfo> m_ImportedFileInfos = new();
        readonly Dictionary<AssetDataPersistenceLegacy, AssetData> m_AssetDatas = new();
        readonly Dictionary<AssetDataFilePersistenceLegacy, AssetDataFile> m_AssetDataFiles = new();

        public int MajorVersion => 0;
        public int MinorVersion => 0;


        public ImportedAssetInfo ConvertToImportedAssetInfo(string content)
        {
            var fileContentWithTypeMapped = MapTypes(content);
            var importedAssetInfoPersistedLegacy = Parse(fileContentWithTypeMapped);
            return Convert(importedAssetInfoPersistedLegacy);
        }

        public string SerializeEntry(AssetData assetData, IEnumerable<ImportedFileInfo> fileInfos)
        {
            // We should never write to this version
            Utilities.DevLogError("Trying to write to legacy persistence version");
            return null;
        }

        static string MapTypes(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                return null;
            }

            // Maps the types persisted in the files to newly created data-identical types
            return jsonString
                .Replace("\"class\": \"AssetData\"", "\"class\": \"AssetDataPersistenceLegacy\"")
                .Replace("\"class\": \"AssetDataFile\"", "\"class\": \"AssetDataFilePersistenceLegacy\"")
                .Replace("\"ns\": \"Unity.AssetManager.Editor\"", "\"ns\": \"Unity.AssetManager.Core.Editor\"")
                .Replace("\"asm\": \"Unity.AssetManager.Editor\"", "\"asm\": \"Unity.AssetManager.Core.Editor\"");
        }

        static ImportedAssetInfoPersistenceLegacy Parse(string jsonString)
        {
            return JsonUtility.FromJson<ImportedAssetInfoPersistenceLegacy>(jsonString);
        }

        AssetDataFile Convert(AssetDataFilePersistenceLegacy persistedLegacy)
        {
            if (persistedLegacy == null)
            {
                return null;
            }

            if (m_AssetDataFiles.TryGetValue(persistedLegacy, out var assetDataFile))
            {
                return assetDataFile;
            }

            assetDataFile = new AssetDataFile(
                persistedLegacy.m_Path,
                persistedLegacy.m_Extension,
                persistedLegacy.m_Guid,
                persistedLegacy.m_Description,
                persistedLegacy.m_Tags,
                persistedLegacy.m_FileSize,
                persistedLegacy.m_Available);

            m_AssetDataFiles[persistedLegacy] = assetDataFile;

            return assetDataFile;
        }

        AssetData Convert(AssetDataPersistenceLegacy persistedLegacy)
        {
            if (persistedLegacy == null)
            {
                return null;
            }

            if (m_AssetDatas.TryGetValue(persistedLegacy, out var assetData))
            {
                return assetData;
            }

#pragma warning disable 618 // Maintaining for compatibility with legacy data

            // Since AssetData can reference itself in the m_Versions member (not sure why), we need to create
            // the object and put it in cache before de-serializing it. Otherwise, we get stack overflow
            var assetsProvider = ServicesContainer.instance.Resolve<IAssetsProvider>();

            assetData = ((AssetsSdkProvider)assetsProvider).DeserializeAssetData(persistedLegacy.m_JsonAssetSerialized);
#pragma warning restore 618

            m_AssetDatas[persistedLegacy] = assetData;

            assetData.FillFromPersistenceLegacy(
                persistedLegacy.m_DependencyAssets.Select(x => x.m_Identifier),
                persistedLegacy.m_ThumbnailUrl,
                persistedLegacy.m_SourceFiles.Select(Convert),
                Convert(persistedLegacy.m_PrimarySourceFile));

            return assetData;
        }

        ImportedFileInfo Convert(ImportedFileInfoPersistenceLegacy persistedLegacy)
        {
            if (persistedLegacy == null)
            {
                return null;
            }

            if (m_ImportedFileInfos.TryGetValue(persistedLegacy, out var importedFileInfo))
            {
                return importedFileInfo;
            }

            importedFileInfo = new ImportedFileInfo(
                string.Empty,
                persistedLegacy.Guid,
                persistedLegacy.OriginalPath);

            m_ImportedFileInfos[persistedLegacy] = importedFileInfo;

            return importedFileInfo;
        }

        ImportedAssetInfo Convert(ImportedAssetInfoPersistenceLegacy persistedLegacy)
        {
            if (persistedLegacy == null)
            {
                return null;
            }

            if (m_ImportedAssetInfos.TryGetValue(persistedLegacy, out var importedAssetInfo))
            {
                return importedAssetInfo;
            }

            importedAssetInfo = new ImportedAssetInfo(
                Convert(persistedLegacy.AssetData),
                persistedLegacy.FileInfos.Select(Convert));

            m_ImportedAssetInfos[persistedLegacy] = importedAssetInfo;

            return importedAssetInfo;
        }
    }
}

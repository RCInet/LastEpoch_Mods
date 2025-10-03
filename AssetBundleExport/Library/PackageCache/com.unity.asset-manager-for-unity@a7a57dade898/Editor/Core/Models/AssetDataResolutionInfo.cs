using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity.AssetManager.Core.Editor
{
    class AssetDataResolutionInfo
    {
        static readonly List<string> k_IgnoreExtensions = new() {".meta", ".am4u_dep", ".am4u_guid"};

        readonly List<BaseAssetDataFile> m_FileConflicts = new();
        readonly List<Object> m_DirtyObjects = new();

        public BaseAssetData AssetData { get; }

        public bool Existed { get; } // Whether the asset is already in the project

        public bool HasChanges { get; } // Whether the asset data has changed

        public int CurrentVersion { get; } // The index of the version in the asset's version list

        public bool HasConflicts => m_FileConflicts.Any(); // Whether the asset has conflicting files

        public int ConflictCount => m_FileConflicts.Count;

        public IEnumerable<Object> DirtyObjects => m_DirtyObjects;

        public AssetDataResolutionInfo(BaseAssetData assetData, IEnumerable<BaseAssetDataFile> existingFiles, IAssetDataManager assetDataManager)
        {
            AssetData = assetData;
            Existed = assetDataManager.IsInProject(assetData.Identifier);

            if (existingFiles != null)
            {
                m_FileConflicts.AddRange(existingFiles);
            }

            var currentAssetData = assetDataManager.GetAssetData(AssetData.Identifier);
            if (currentAssetData == null)
            {
                CurrentVersion = 0;
                HasChanges = Existed;
            }
            else
            {
                CurrentVersion = currentAssetData.SequenceNumber;

                var isDifferentVersion = currentAssetData.Identifier.Version != AssetData.Identifier.Version;
                var isUpdated = currentAssetData.Updated != AssetData.Updated;
                HasChanges = Existed && (isDifferentVersion || isUpdated);
            }
        }

        public async Task GatherFileConflictsAsync(IAssetDataManager assetDataManager, CancellationToken token)
        {
            var importedAssetInfo = assetDataManager.GetImportedAssetInfo(AssetData.Identifier);

            var modifiedFiles = await GetModifiedFilesAsync(importedAssetInfo, AssetData.GetFiles(), token);

            m_FileConflicts.AddRange(modifiedFiles);
            if (m_FileConflicts.Any())
            {
                var assetDatabase = ServicesContainer.instance.Resolve<IAssetDatabaseProxy>();
                var editorUtility = ServicesContainer.instance.Resolve<IEditorUtilityProxy>();

                foreach (var file in m_FileConflicts)
                {
                    try
                    {
                        var importedFileInfo =
                            importedAssetInfo?.FileInfos.Find(f => Utilities.ComparePaths(f.OriginalPath, file.Path));

                        // Check dirty flag
                        var path = assetDatabase.GuidToAssetPath(importedFileInfo?.Guid);
                        var asset = assetDatabase.LoadAssetAtPath(path);
                        if (asset != null && editorUtility.IsDirty(asset))
                        {
                            m_DirtyObjects.Add(asset);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }
        }

        public bool ExistsConflict(BaseAssetDataFile file)
        {
            return m_FileConflicts.Contains(file);
        }

        static async Task<IEnumerable<BaseAssetDataFile>> GetModifiedFilesAsync(ImportedAssetInfo importedAssetInfo, IEnumerable<BaseAssetDataFile> files, CancellationToken token)
        {
            var modifiedFiles = new List<BaseAssetDataFile>();

            if (importedAssetInfo == null)
            {
                return modifiedFiles;
            }

            var assetDatabase = ServicesContainer.instance.Resolve<IAssetDatabaseProxy>();
            var fileUtility = ServicesContainer.instance.Resolve<IFileUtility>();

            foreach (var file in files.Where(f => !k_IgnoreExtensions.Contains(Path.GetExtension(f.Path))))
            {
                try
                {
                    var importedFileInfo = importedAssetInfo.FileInfos.Find(f => Utilities.ComparePaths(f.OriginalPath, file.Path));
                    if (importedFileInfo == null)
                    {
                        continue;
                    }

                    var filePath = assetDatabase.GuidToAssetPath(importedFileInfo.Guid);

                    // Check if the file exists, the import info may be out of date.
                    // If the file no longer exists, we don't need to check for modifications; treat it as if the file was never imported.
                    if (string.IsNullOrEmpty(filePath))
                    {
                        continue;
                    }

                    var result = await fileUtility.FileWasModified(filePath, importedFileInfo.Timestamp, importedFileInfo.Checksum, token);
                    if (result.Results != ComparisonResults.None)
                    {
                        modifiedFiles.Add(file);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            return modifiedFiles;
        }
    }
}

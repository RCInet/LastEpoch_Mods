using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    interface IPersistenceVersion
    {
        int MajorVersion { get; }
        int MinorVersion { get; }

        ImportedAssetInfo ConvertToImportedAssetInfo(string content);
        string SerializeEntry(AssetData assetData, IEnumerable<ImportedFileInfo> fileInfos);
    }

    static class Persistence
    {
        static Regex s_SerializationVersionRegex =
            new ("\"serializationVersion\"\\s*:\\s*\\[\\s*(\\d+)\\s*,\\s*(\\d+)\\s*\\]",
                RegexOptions.None,
                TimeSpan.FromMilliseconds(100));

        static readonly string s_DefaultTrackedFolder =
            Path.Combine(
                Application.dataPath,
                "..",
                "ProjectSettings",
                "Packages",
                AssetManagerCoreConstants.PackageName,
                "ImportedAssetInfo");
        static string s_OverrideTrackedFolder = null; // for testing purposes
        internal static void OverrideTrackedFolder(string trackedFolder) => s_OverrideTrackedFolder = trackedFolder;

        static string TrackedFolder
        {
            get
            {
                var trackedFolder = s_DefaultTrackedFolder;

                if (!string.IsNullOrEmpty(s_OverrideTrackedFolder))
                {
                    trackedFolder = s_OverrideTrackedFolder;
                }

                return trackedFolder;
            }
        }

        static IPersistenceVersion[] s_PersistenceVersions =
        {
            new PersistenceLegacy(),
            new PersistenceV1(),
            new PersistenceV2(),
            new PersistenceV3()
        };

        internal class ReadCache
        {
            public Dictionary<AssetIdentifier, AssetData> s_AssetDatas = new(); // assetId => AssetData

            public AssetData GetAssetDataFor(AssetIdentifier assetIdentifier)
            {
                if (!s_AssetDatas.TryGetValue(assetIdentifier, out var assetData))
                {
                    assetData = new AssetData();
                    s_AssetDatas[assetIdentifier] = assetData;
                }
                return assetData;
            }
        }

        public static IReadOnlyCollection<ImportedAssetInfo> ReadAllEntries(IIOProxy ioProxy)
        {
            if (ioProxy == null)
            {
                Utilities.DevLogError("Null IIOProxy service");
                return Array.Empty<ImportedAssetInfo>();
            }

            if (!ioProxy.DirectoryExists(TrackedFolder))
            {
                return Array.Empty<ImportedAssetInfo>();
            }

            // Read data as-is into persistence structure data

            List<ImportedAssetInfo> importedAssetInfos = new();
            foreach (var assetPath in ioProxy.EnumerateFiles(TrackedFolder, "*", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    var content = ioProxy.FileReadAllText(assetPath);
                    var (major, minor) = ExtractSerializationVersion(content);
                    var currentVersion =  s_PersistenceVersions[^1].MajorVersion;

                    if (major > currentVersion)
                    {
                        Debug.LogError($"Unsupported serialization version {major}.{minor} in tracking file '{assetPath}'");
                        continue;
                    }

                    for (int versionIndex = 0; versionIndex < currentVersion; versionIndex++)
                    {
                        if(major > versionIndex)
                            continue;

                        content = ConvertFromPreviousVersion(s_PersistenceVersions[versionIndex], s_PersistenceVersions[versionIndex+1], content);
                        major++;
                    }

                    var importedAssetInfo = s_PersistenceVersions[^1].ConvertToImportedAssetInfo(content);
                    if (importedAssetInfo != null)
                    {
                        importedAssetInfos.Add(importedAssetInfo);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Unable to read tracking data. Tracking file might be corrupted '{assetPath}'");
                    Utilities.DevLogException(e);
                }
            }

            return importedAssetInfos;
        }

        static string ConvertFromPreviousVersion(IPersistenceVersion previous, IPersistenceVersion next, string content)
        {
            string persistedNextContent = null;
            var importedAssetInfo = previous.ConvertToImportedAssetInfo(content);
            if (importedAssetInfo != null)
            {
                persistedNextContent = next.SerializeEntry(importedAssetInfo.AssetData as AssetData, importedAssetInfo.FileInfos);
            }

            return persistedNextContent;
        }

        public static void RemoveEntry(IIOProxy ioProxy, string assetId)
        {
            ioProxy.DeleteFile(GetFilenameFor(assetId));
        }

        public static void WriteEntry(IIOProxy ioProxy, AssetData assetData, IEnumerable<ImportedFileInfo> fileInfos)
        {
            var fileContent = s_PersistenceVersions[^1].SerializeEntry(assetData, fileInfos);
            WriteEntry(ioProxy, assetData.Identifier.AssetId, fileContent);
        }

        static void WriteEntry(IIOProxy ioProxy, string assetId, string fileContent)
        {
            var importInfoFilePath = GetFilenameFor(assetId);
            try
            {
                var directoryPath = Path.GetDirectoryName(importInfoFilePath);
                if (!ioProxy.DirectoryExists(directoryPath))
                {
                    ioProxy.CreateDirectory(directoryPath);
                }

                ioProxy.FileWriteAllText(importInfoFilePath, fileContent);
            }
            catch (IOException e)
            {
                Debug.Log($"Couldn't write imported asset info to {importInfoFilePath} :\n{e}.");
            }
        }

        static string GetFilenameFor(string assetId)
        {
            return Path.Combine(TrackedFolder, assetId);
        }

        static (int major, int minor) ExtractSerializationVersion(string content)
        {
            var match = s_SerializationVersionRegex.Match(content);
            if (match.Success)
            {
                return (Int32.Parse(match.Groups[1].Value), Int32.Parse(match.Groups[2].Value));
            }

            return (0, 0); // when no serializationVersion is present, we're in version 0.0
        }
    }
}

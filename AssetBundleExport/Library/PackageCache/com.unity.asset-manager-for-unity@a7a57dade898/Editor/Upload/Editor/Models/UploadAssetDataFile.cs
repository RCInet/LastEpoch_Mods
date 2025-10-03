using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEngine;

namespace Unity.AssetManager.Upload.Editor
{
    [Serializable]
    class UploadAssetDataFile : BaseAssetDataFile
    {
        [SerializeField]
        string m_SourcePath;

        public string SourcePath => m_SourcePath;

        public UploadAssetDataFile(string sourcePath, string destinationPath, string description, IEnumerable<string> tags)
        {
            m_SourcePath = sourcePath;
            Path = destinationPath;

            var guid = ServicesContainer.instance.Resolve<IAssetDatabaseProxy>().AssetPathToGuid(sourcePath);
            Guid = string.IsNullOrEmpty(guid) ? null : guid;

            Extension = System.IO.Path.GetExtension(sourcePath).ToLower();
            Description = description;
            Tags = tags?.ToList();
            FileSize = GetFileSize(sourcePath);
            Available = true;
        }

        static long GetFileSize(string assetPath)
        {
            var application = ServicesContainer.instance.Resolve<IApplicationProxy>();
            var io = ServicesContainer.instance.Resolve<IIOProxy>();
            
            var fullPath = System.IO.Path.Combine(application.DataPath, Utilities.GetPathRelativeToAssetsFolder(assetPath));

            if (io.FileExists(fullPath))
            {
                return io.GetFileLength(fullPath);
            }

            Debug.LogError("Asset does not exist: " + fullPath);
            return 0;
        }

        public IUploadFile GenerateUploadFile()
        {
            return new UploadFile(m_SourcePath, Path);
        }
    }
}

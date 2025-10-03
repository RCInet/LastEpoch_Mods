using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    [Serializable]
    class ImportedAssetInfo
    {
        [SerializeReference]
        public BaseAssetData AssetData;

        public List<ImportedFileInfo> FileInfos;

        public AssetIdentifier Identifier => AssetData?.Identifier;

        public ImportedAssetInfo(BaseAssetData assetData, IEnumerable<ImportedFileInfo> fileInfos)
        {
            AssetData = assetData;
            FileInfos = fileInfos.ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    [Serializable]
    class AssetDataFile : BaseAssetDataFile
    {
        public AssetDataFile(string path, string extension, string guid, string description, IEnumerable<string> tags, long fileSize, bool available)
        {
            Path = path;
            Extension = extension;
            Guid = guid;
            Available = available;
            Description = description;
            Tags = tags?.ToList();
            FileSize = fileSize;
        }
    }
}

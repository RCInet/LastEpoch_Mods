using System.Collections.Generic;

namespace Unity.AssetManager.Core.Editor
{
    class AssetDataFileComparerByExtension : IComparer<BaseAssetDataFile>
    {
        public int Compare(BaseAssetDataFile file1, BaseAssetDataFile file2)
        {
            return AssetDataTypeHelper.GetPriority(file1?.Extension).CompareTo(
                    AssetDataTypeHelper.GetPriority(file2?.Extension));
        }
    }
}

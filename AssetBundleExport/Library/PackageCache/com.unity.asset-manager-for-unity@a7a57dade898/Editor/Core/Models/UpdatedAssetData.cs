using System;
using System.Collections.Generic;

namespace Unity.AssetManager.Core.Editor
{
    class UpdatedAssetData
    {
        readonly List<AssetDataResolutionInfo> m_Assets = new();
        readonly List<AssetDataResolutionInfo> m_Dependants = new();
        readonly List<BaseAssetData> m_UpwardDependencies = new();

        public List<AssetDataResolutionInfo> Assets => m_Assets;
        public List<AssetDataResolutionInfo> Dependants => m_Dependants;
        public List<BaseAssetData> UpwardDependencies => m_UpwardDependencies;
    }
}

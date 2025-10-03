using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    [Serializable]
    class LocalUnityTypeFilter : LocalFilter
    {
        [SerializeReference]
        IAssetDataManager m_AssetDataManager;

        public override string DisplayName => "Type";

        List<FilterSelection> m_CachedSelections;

        public LocalUnityTypeFilter(IPageFilterStrategy pageFilterStrategy, IAssetDataManager assetDataManager)
            : base(pageFilterStrategy)
        {
            m_AssetDataManager = assetDataManager;
        }

        public override Task<List<FilterSelection>> GetSelections(bool _ = false)
        {
            if (m_CachedSelections == null)
            {
                var values = m_AssetDataManager.ImportedAssetInfos.Select(i => i.AssetData.AssetType).Distinct();
                m_CachedSelections = values.Select(x => new FilterSelection(m_PageFilterStrategy.ToString(x), x.GetToolTip())).ToList();
            }

            return Task.FromResult(m_CachedSelections);
        }

        public override Task<bool> Contains(BaseAssetData assetData, CancellationToken token = default)
        {
            return Task.FromResult(SelectedFilters == null || SelectedFilters.Any(selectedFilter => m_PageFilterStrategy.ToString(assetData.AssetType) == selectedFilter));
        }
    }
}

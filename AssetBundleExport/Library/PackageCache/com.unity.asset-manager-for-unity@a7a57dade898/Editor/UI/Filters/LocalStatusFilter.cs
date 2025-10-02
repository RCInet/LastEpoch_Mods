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
    class LocalStatusFilter : LocalFilter
    {
        [SerializeReference]
        IAssetDataManager m_AssetDataManager;

        public override string DisplayName => "Status";

        List<FilterSelection> m_CachedSelections;

        public LocalStatusFilter(IPageFilterStrategy pageFilterStrategy, IAssetDataManager assetDataManager)
            : base(pageFilterStrategy)
        {
            m_AssetDataManager = assetDataManager;
        }

        public override Task<List<FilterSelection>> GetSelections(bool _ = false)
        {
            if (m_CachedSelections == null)
            {
                var values = m_AssetDataManager.ImportedAssetInfos.Select(i => i.AssetData.Status).Distinct();
                m_CachedSelections = values.Select(x => new FilterSelection(x)).ToList();
            }

            return Task.FromResult(m_CachedSelections);
        }

        public override Task<bool> Contains(BaseAssetData assetData, CancellationToken token = default)
        {
            return Task.FromResult(SelectedFilters == null || SelectedFilters.Any(selectedFilter => assetData.Status == selectedFilter));
        }

        public override void Clear()
        {
            base.Clear();

            m_CachedSelections = null;
        }
    }
}

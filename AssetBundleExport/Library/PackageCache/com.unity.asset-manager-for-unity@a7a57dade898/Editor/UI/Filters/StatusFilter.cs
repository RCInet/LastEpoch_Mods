using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;

namespace Unity.AssetManager.UI.Editor
{
    [Serializable]
    class StatusFilter : CloudFilter
    {
        public StatusFilter(IPageFilterStrategy pageFilterStrategy)
            : base(pageFilterStrategy) { }

        public override string DisplayName => "Status";
        protected override AssetSearchGroupBy GroupBy => AssetSearchGroupBy.Status;

        public override bool ApplyFromAssetSearchFilter(AssetSearchFilter searchFilter)
        {
            ClearFilter();

            if (searchFilter.Status == null || searchFilter.Status.Count == 0)
                return false;

            ApplyFilter(searchFilter.Status);
            return true;
        }

        public override void ResetSelectedFilter(AssetSearchFilter assetSearchFilter)
        {
            assetSearchFilter.Status = SelectedFilters?.ToList();
        }

        protected override void IncludeFilter(List<string> selectedFilters)
        {
            m_PageFilterStrategy.AssetSearchFilter.Status = selectedFilters;
        }

        protected override void ClearFilter()
        {
            m_PageFilterStrategy.AssetSearchFilter.Status = null;
        }
    }
}

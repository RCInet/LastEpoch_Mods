using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEditor;

namespace Unity.AssetManager.UI.Editor
{
    class CreatedByFilter : CloudFilter
    {
        public override string DisplayName => L10n.Tr(Constants.CreatedByText);
        protected override AssetSearchGroupBy GroupBy => AssetSearchGroupBy.CreatedBy;

        public CreatedByFilter(IPageFilterStrategy pageFilterStrategy)
            : base(pageFilterStrategy) { }

        public override bool ApplyFromAssetSearchFilter(AssetSearchFilter searchFilter)
        {
            ClearFilter();

            if (searchFilter.CreatedBy == null || searchFilter.CreatedBy.Count == 0)
                return false;

            TaskUtils.TrackException(ApplyFilterAsync(searchFilter.CreatedBy));
            return true;
        }

        async Task ApplyFilterAsync(List<string> userIds)
        {
            var selectionNames = await m_PageFilterStrategy.GetUserNamesAsync(userIds);
            selectionNames.Sort();

            ApplyFilter(selectionNames.Select(s => s.Text).ToList());
        }

        public override void ResetSelectedFilter(AssetSearchFilter assetSearchFilter)
        {
            TaskUtils.TrackException(ResetSelectedFilterAsync(assetSearchFilter));
        }

        protected override void IncludeFilter(List<string> selectedFilters)
        {
            TaskUtils.TrackException(IncludeFilterAsync(selectedFilters));
        }

        protected override void ClearFilter()
        {
            m_PageFilterStrategy.AssetSearchFilter.CreatedBy = null;
        }

        protected override async Task<List<FilterSelection>> GetSelectionsAsync()
        {
            var selections = await base.GetSelectionsAsync();
            return await m_PageFilterStrategy.GetUserNamesAsync(selections);
        }

        async Task ResetSelectedFilterAsync(AssetSearchFilter assetSearchFilter)
        {
            var userIds = new List<string>();
            if (SelectedFilters != null)
            {
                foreach (var selectedFilter in SelectedFilters)
                {
                    userIds.Add(await m_PageFilterStrategy.GetUserIdAsync(selectedFilter));
                }
            }

            assetSearchFilter.CreatedBy = userIds;
        }

        async Task IncludeFilterAsync(List<string> selectedFilters)
        {
            if (selectedFilters == null)
            {
                m_PageFilterStrategy.AssetSearchFilter.CreatedBy = null;
                return;
            }


            var userIds = new List<string>();
            foreach (var selectedFilter in selectedFilters)
            {
                userIds.Add(await m_PageFilterStrategy.GetUserIdAsync(selectedFilter));
            }

            m_PageFilterStrategy.AssetSearchFilter.CreatedBy = userIds;
        }
    }
}

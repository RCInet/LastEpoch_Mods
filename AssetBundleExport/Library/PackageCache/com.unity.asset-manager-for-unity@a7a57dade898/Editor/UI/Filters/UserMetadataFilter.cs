using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    class UserMetadataFilter : CustomMetadataFilter
    {
        [SerializeReference]
        UserMetadata m_UserMetadata;

        public override FilterSelectionType SelectionType => FilterSelectionType.SingleSelection;

        public UserMetadataFilter(IPageFilterStrategy pageFilterStrategy, IMetadata metadata)
            : base(pageFilterStrategy, metadata)
        {
            m_UserMetadata = metadata as UserMetadata;
        }

        public override bool ApplyFromAssetSearchFilter(AssetSearchFilter searchFilter)
        {
            ClearFilter();

            if (searchFilter.CustomMetadata == null || searchFilter.CustomMetadata.Count == 0)
                return false;

            var result = false;

            foreach (var metadata in searchFilter.CustomMetadata)
            {
                if (metadata is UserMetadata userMetadata && userMetadata.FieldKey == m_UserMetadata.FieldKey)
                {
                    TaskUtils.TrackException(ApplyFilterAsync(new List<string> { userMetadata.Value }));
                    result = true;
                }
            }

            return result;
        }

        async Task ApplyFilterAsync(List<string> userIds)
        {
            var selectionNames = await m_PageFilterStrategy.GetUserNamesAsync(userIds);
            selectionNames.Sort();

            ApplyFilter(selectionNames.Select(s => s.Text).ToList());
        }

        protected override void IncludeFilter(List<string> selectedFilters)
        {
            if (selectedFilters == null)
            {
                m_UserMetadata.Value = null;
                base.IncludeFilter(null);
                return;
            }

            TaskUtils.TrackException(IncludeFilterAsync(selectedFilters));
        }

        protected override async Task<List<FilterSelection>> GetFilterSelectionsAsync()
        {
            var selections = await base.GetFilterSelectionsAsync();
            return await m_PageFilterStrategy.GetUserNamesAsync(selections);
        }

        async Task IncludeFilterAsync(List<string> selectedFilters)
        {
            // Could be done with only the first selected filter, but we keep that implementation for future use,
            // when the backend will support multiple selected filters for custom metadata.
            var userIds = new List<string>();
            foreach (var selectedFilter in selectedFilters)
            {
                userIds.Add(await m_PageFilterStrategy.GetUserIdAsync(selectedFilter));
            }

            if (userIds.Count > 0)
                m_UserMetadata.Value = userIds[0];
            base.IncludeFilter(selectedFilters);
        }
    }
}

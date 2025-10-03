using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    abstract class CloudFilter : BaseFilter
    {
        List<FilterSelection> m_CachedSelections = new();

        protected abstract AssetSearchGroupBy GroupBy { get; }

        public abstract bool ApplyFromAssetSearchFilter(AssetSearchFilter searchFilter);
        public abstract void ResetSelectedFilter(AssetSearchFilter assetSearchFilter);
        protected abstract void ClearFilter();
        protected abstract void IncludeFilter(List<string> selectedFilters);

        void ResetSelectedFilter()
        {
            ResetSelectedFilter(m_PageFilterStrategy.AssetSearchFilter);
        }

        protected CloudFilter(IPageFilterStrategy pageFilterStrategy)
            : base(pageFilterStrategy) { }

        public override void Cancel()
        {
            ResetSelectedFilter();
            IsDirty = true;
        }

        public override void Clear()
        {
            ClearFilter();
        }

        public override bool ApplyFilter(List<string> selectedFilters)
        {
            if (selectedFilters == null)
            {
                ClearFilter();
            }
            else
            {
                IncludeFilter(selectedFilters);
            }

            return base.ApplyFilter(selectedFilters);
        }

        public override async Task<List<FilterSelection>> GetSelections(bool includeSelectedFilters = false)
        {
            if (IsDirty)
            {
                m_CachedSelections = await GetSelectionsAsync();
                IsDirty = false;
            }

            // If the include flag is set, return the selected filters as well
            if (includeSelectedFilters && SelectedFilters != null)
            {
                foreach (var selectedFilter in SelectedFilters)
                {
                    if (m_CachedSelections.All(s => s.Text != selectedFilter))
                        m_CachedSelections.Add(new FilterSelection(selectedFilter));
                }
            }

            return m_CachedSelections;
        }

        protected virtual async Task<List<FilterSelection>> GetSelectionsAsync()
        {
            ClearFilter();

            try
            {
                return await GetFilterSelectionsAsync();
            }
            catch (OperationCanceledException)
            {
                // Do nothing
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
            finally
            {
                ResetSelectedFilter();
            }

            return null;
        }

        protected virtual async Task<List<FilterSelection>> GetFilterSelectionsAsync()
        {
            return await m_PageFilterStrategy.GetFilterSelectionsAsync(GroupBy);
        }
    }
}

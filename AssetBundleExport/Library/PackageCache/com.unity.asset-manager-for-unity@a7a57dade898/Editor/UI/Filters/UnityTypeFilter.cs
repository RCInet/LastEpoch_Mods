using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    [Serializable]
    class UnityTypeFilter : CloudFilter, ISerializationCallbackReceiver
    {
        public override string DisplayName => "Type";

        protected override AssetSearchGroupBy GroupBy => AssetSearchGroupBy.Type;

        public UnityTypeFilter(IPageFilterStrategy pageFilterStrategy)
            : base(pageFilterStrategy) { }

        public override bool ApplyFromAssetSearchFilter(AssetSearchFilter searchFilter)
        {
            ClearFilter();

            if (searchFilter.AssetTypeStrings == null || searchFilter.AssetTypeStrings.Count == 0)
                return false;

            ApplyFilter(searchFilter.AssetTypeStrings);
            return true;
        }

        public override void ResetSelectedFilter(AssetSearchFilter assetSearchFilter)
        {
            assetSearchFilter.AssetTypeStrings = SelectedFilters?.ToList();
        }

        protected override void IncludeFilter(List<string> selectedFilters)
        {
            m_PageFilterStrategy.AssetSearchFilter.AssetTypeStrings = selectedFilters;
        }

        protected override void ClearFilter()
        {
            m_PageFilterStrategy.AssetSearchFilter.AssetTypeStrings = null;
        }

        public void OnBeforeSerialize()
        {
            // Do nothing
        }

        public void OnAfterDeserialize()
        {
            ConvertFromLegacy(SelectedFilters);
            IsDirty = true;
        }

        void ConvertFromLegacy(IList<string> assetTypeStrings)
        {
            if (assetTypeStrings != null)
            {
                for (var i = 0; i < assetTypeStrings.Count; ++i)
                {
                    assetTypeStrings[i] = m_PageFilterStrategy.ConvertAssetTypeFromLegacy(assetTypeStrings[i]);
                }
            }
        }
    }
}

using System.Collections.Generic;
using Unity.AssetManager.Core.Editor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    class NumberMetadataFilter : CustomMetadataFilter
    {
        [SerializeReference]
        Core.Editor.NumberMetadata m_NumberMetadata;

        public double Value;

        public override FilterSelectionType SelectionType => FilterSelectionType.Number;

        public NumberMetadataFilter(IPageFilterStrategy pageFilterStrategy, IMetadata metadata)
            : base(pageFilterStrategy, metadata)
        {
            m_NumberMetadata = metadata as Core.Editor.NumberMetadata;
        }

        public override bool ApplyFromAssetSearchFilter(AssetSearchFilter searchFilter)
        {
            ClearFilter();

            if (searchFilter.CustomMetadata == null || searchFilter.CustomMetadata.Count == 0)
                return false;

            var result = false;

            foreach (var metadata in searchFilter.CustomMetadata)
            {
                if (metadata is NumberMetadata numberMetadata && numberMetadata.FieldKey == m_NumberMetadata.FieldKey)
                {
                    ApplyFilter(new List<string> { numberMetadata.Value.ToString() });
                    result = true;
                }
            }

            return result;
        }

        public override void ResetSelectedFilter(AssetSearchFilter assetSearchFilter)
        {
            assetSearchFilter.CustomMetadata ??= new List<IMetadata>();

            assetSearchFilter.CustomMetadata.RemoveAll(m => m.FieldKey == m_NumberMetadata.FieldKey);
            assetSearchFilter.CustomMetadata.Add(m_NumberMetadata);
        }

        protected override void IncludeFilter(List<string> selectedFilters)
        {
            Utilities.DevAssert(selectedFilters == null || selectedFilters.Count == 1, "NumberMetadataFilter: IncludeFilter: selection must be null or have only 1 element");

            m_NumberMetadata.Value = double.Parse(selectedFilters?[0] ?? "0");
            base.IncludeFilter(selectedFilters);
        }

        public override string DisplaySelectedFilters()
        {
            if(SelectedFilters is { Count: 1 })
            {
                return $"{DisplayName} : {m_NumberMetadata.Value}";
            }

            return base.DisplaySelectedFilters();
        }
    }
}

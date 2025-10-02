using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    class MultiSelectionMetadataFilter : CustomMetadataFilter
    {
        [SerializeReference]
        MultiSelectionMetadata m_MultiSelectionMetadata;

        public MultiSelectionMetadataFilter(IPageFilterStrategy pageFilterStrategy, IMetadata metadata)
            : base(pageFilterStrategy, metadata)
        {
            m_MultiSelectionMetadata = metadata as MultiSelectionMetadata;
        }

        public override bool ApplyFromAssetSearchFilter(AssetSearchFilter searchFilter)
        {
            ClearFilter();

            if (searchFilter.CustomMetadata == null || searchFilter.CustomMetadata.Count == 0)
                return false;

            var result = false;

            foreach (var metadata in searchFilter.CustomMetadata)
            {
                if (metadata is MultiSelectionMetadata multiSelectionMetadata && multiSelectionMetadata.FieldKey == m_MultiSelectionMetadata.FieldKey)
                {
                    ApplyFilter(multiSelectionMetadata.Value);
                    result = true;
                }
            }

            return result;
        }

        protected override void IncludeFilter(List<string> selectedFilters)
        {
            m_MultiSelectionMetadata.Value = selectedFilters;
            base.IncludeFilter(selectedFilters);
        }
    }
}

using System.Collections.Generic;
using Unity.AssetManager.Core.Editor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    class TextMetadataFilter : CustomMetadataFilter
    {
        [SerializeReference]
        TextMetadata m_TextMetadata;

        public override FilterSelectionType SelectionType => FilterSelectionType.Text;

        public TextMetadataFilter(IPageFilterStrategy pageFilterStrategy, IMetadata metadata)
            : base(pageFilterStrategy, metadata)
        {
            m_TextMetadata = metadata as TextMetadata;
        }

        public override bool ApplyFromAssetSearchFilter(AssetSearchFilter searchFilter)
        {
            ClearFilter();

            if (searchFilter.CustomMetadata == null || searchFilter.CustomMetadata.Count == 0)
                return false;

            var result = false;

            foreach (var metadata in searchFilter.CustomMetadata)
            {
                if (metadata is TextMetadata textMetadata && textMetadata.FieldKey == m_TextMetadata.FieldKey)
                {
                    ApplyFilter(new List<string> { textMetadata.Value });
                    result = true;
                }
            }

            return result;
        }

        protected override void IncludeFilter(List<string> selectedFilters)
        {
            if (selectedFilters?.Count > 0)
                m_TextMetadata.Value = selectedFilters[0];

            base.IncludeFilter(selectedFilters);
        }
    }
}

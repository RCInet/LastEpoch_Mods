using System.Collections.Generic;
using Unity.AssetManager.Core.Editor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    class UrlMetadataFilter : CustomMetadataFilter
    {
        [SerializeReference]
        UrlMetadata m_UrlMetadata;

        public override FilterSelectionType SelectionType => FilterSelectionType.Url;

        public UrlMetadataFilter(IPageFilterStrategy pageFilterStrategy, IMetadata metadata)
            : base(pageFilterStrategy, metadata)
        {
            m_UrlMetadata = metadata as UrlMetadata;
        }

        public override bool ApplyFromAssetSearchFilter(AssetSearchFilter searchFilter)
        {
            ClearFilter();

            if (searchFilter.CustomMetadata == null || searchFilter.CustomMetadata.Count == 0)
                return false;

            var result = false;

            foreach (var metadata in searchFilter.CustomMetadata)
            {
                if (metadata is UrlMetadata urlMetadata && urlMetadata.FieldKey == m_UrlMetadata.FieldKey)
                {
                    ApplyFilter(new List<string> { urlMetadata.Value.Label });
                    result = true;
                }
            }

            return result;
        }

        protected override void IncludeFilter(List<string> selectedFilters)
        {
            m_UrlMetadata.Value = new UriEntry(m_UrlMetadata.Value.Uri, selectedFilters?[0] ?? string.Empty);
            base.IncludeFilter(selectedFilters);
        }
    }
}

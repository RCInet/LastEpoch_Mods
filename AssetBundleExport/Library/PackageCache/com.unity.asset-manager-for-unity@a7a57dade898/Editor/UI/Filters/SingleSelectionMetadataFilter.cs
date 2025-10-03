using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    class SingleSelectionMetadataFilter : CustomMetadataFilter
    {
        [SerializeReference]
        SingleSelectionMetadata m_SingleSelectionMetadata;

        public override FilterSelectionType SelectionType => FilterSelectionType.SingleSelection;

        public SingleSelectionMetadataFilter(IPageFilterStrategy pageFilterStrategy, IMetadata metadata)
            : base(pageFilterStrategy, metadata)
        {
            m_SingleSelectionMetadata = metadata as SingleSelectionMetadata;
        }

        public override bool ApplyFromAssetSearchFilter(AssetSearchFilter searchFilter)
        {
            ClearFilter();

            if (searchFilter.CustomMetadata == null || searchFilter.CustomMetadata.Count == 0)
                return false;

            var result = false;

            foreach (var metadata in searchFilter.CustomMetadata)
            {
                if (metadata is SingleSelectionMetadata singleSelectionMetadata &&
                    singleSelectionMetadata.FieldKey == m_SingleSelectionMetadata.FieldKey)
                {
                    ApplyFilter(new List<string> { singleSelectionMetadata.Value });
                    result = true;
                }
            }

            return result;
        }

        protected override void IncludeFilter(List<string> selectedFilters)
        {
            if (selectedFilters?.Count > 0)
                m_SingleSelectionMetadata.Value = selectedFilters[0];

            base.IncludeFilter(selectedFilters);
        }
    }
}

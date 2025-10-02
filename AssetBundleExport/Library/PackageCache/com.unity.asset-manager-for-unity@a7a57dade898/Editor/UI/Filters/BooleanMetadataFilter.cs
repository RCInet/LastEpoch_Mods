using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    class BooleanMetadataFilter : CustomMetadataFilter
    {
        static readonly FilterSelection[] k_Selections =
        {
            new(bool.TrueString),
            new(bool.FalseString),
        };

        [SerializeReference]
        Core.Editor.BooleanMetadata m_BooleanMetadata;

        public override FilterSelectionType SelectionType => FilterSelectionType.SingleSelection;

        public BooleanMetadataFilter(IPageFilterStrategy pageFilterStrategy, IMetadata metadata)
            : base(pageFilterStrategy, metadata)
        {
            m_BooleanMetadata = metadata as Core.Editor.BooleanMetadata;
        }

        public override bool ApplyFromAssetSearchFilter(AssetSearchFilter searchFilter)
        {
            ClearFilter();

            if (searchFilter.CustomMetadata == null || searchFilter.CustomMetadata.Count == 0)
                return false;

            var result = false;

            foreach (var metadata in searchFilter.CustomMetadata)
            {
                if (metadata is Core.Editor.BooleanMetadata booleanMetadata && booleanMetadata.FieldKey == m_BooleanMetadata.FieldKey)
                {
                    ApplyFilter(new List<string> { booleanMetadata.Value.ToString() });
                    result = true;
                }
            }

            return result;
        }

        protected override void IncludeFilter(List<string> selectedFilters)
        {
            Utilities.DevAssert(selectedFilters == null || selectedFilters.Count == 1, "BooleanMetadataFilter: IncludeFilter: selection must be null or have 1 element");

            m_BooleanMetadata.Value = bool.Parse(selectedFilters?[0] ?? "false");
            base.IncludeFilter(selectedFilters);
        }

        protected override Task<List<FilterSelection>> GetSelectionsAsync()
        {
            return Task.FromResult(k_Selections.ToList());
        }
    }
}

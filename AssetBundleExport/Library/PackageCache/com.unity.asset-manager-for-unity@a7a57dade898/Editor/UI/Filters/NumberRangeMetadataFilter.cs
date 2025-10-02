using System.Collections.Generic;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    class NumberRangeMetadataFilter : CustomMetadataFilter
    {
        [SerializeReference]
        Core.Editor.NumberMetadata m_NumberMetadata;

        [SerializeField]
        double m_FromValue;

        [SerializeField]
        double m_ToValue;

        public double FromValue => m_FromValue;
        public double ToValue => m_ToValue;

        public override FilterSelectionType SelectionType => FilterSelectionType.NumberRange;

        public NumberRangeMetadataFilter(IPageFilterStrategy pageFilterStrategy, IMetadata metadata)
            : base(pageFilterStrategy, metadata)
        {
            m_NumberMetadata = metadata as Core.Editor.NumberMetadata;
        }

        public override bool ApplyFromAssetSearchFilter(AssetSearchFilter searchFilter)
        {
            ClearFilter();

            if (searchFilter.CustomMetadata == null || searchFilter.CustomMetadata.Count == 0)
                return false;

            // Because the number metadata range is stored as two separate entries in the search filter,
            // we need to check if both entries exist in the search filter, and if so, set the FromValue and ToValue accordingly.

            var numberMetadatas = new List<Core.Editor.NumberMetadata>();
            foreach (var metadata in searchFilter.CustomMetadata)
            {
                if (metadata is Core.Editor.NumberMetadata numberMetadata && numberMetadata.FieldKey == m_NumberMetadata.FieldKey)
                {
                    numberMetadatas.Add(numberMetadata);
                }
            }

            if (numberMetadatas.Count != 2)
                return false;

            var firstEntry = numberMetadatas[0].Value;
            var secondEntry = numberMetadatas[1].Value;

            m_FromValue = firstEntry < secondEntry ? firstEntry : secondEntry;
            m_ToValue = firstEntry > secondEntry ? firstEntry : secondEntry;

            var orderedSelections = new List<string>();
            orderedSelections.Add(m_FromValue.ToString());
            orderedSelections.Add(m_ToValue.ToString());

            ApplyFilter(orderedSelections);

            return true;
        }

        public override void ResetSelectedFilter(AssetSearchFilter assetSearchFilter)
        {
            assetSearchFilter.CustomMetadata ??= new List<IMetadata>();

            assetSearchFilter.CustomMetadata.RemoveAll(m => m.FieldKey == m_NumberMetadata.FieldKey);
            assetSearchFilter.CustomMetadata.Add(new Core.Editor.NumberMetadata(m_NumberMetadata.FieldKey, m_NumberMetadata.Name, m_FromValue));
            assetSearchFilter.CustomMetadata.Add(new Core.Editor.NumberMetadata(m_NumberMetadata.FieldKey, m_NumberMetadata.Name, m_ToValue));
        }

        protected override void IncludeFilter(List<string> selectedFilters)
        {
            Utilities.DevAssert(selectedFilters == null || selectedFilters.Count == 2, "NumberRangeMetadataFilter: IncludeFilter: selection must be null or have 2 elements");

            m_FromValue = double.Parse(selectedFilters?[0] ?? "0");
            m_ToValue = double.Parse(selectedFilters?[1] ?? "0");
            base.IncludeFilter(selectedFilters);
        }

        public override string DisplaySelectedFilters()
        {
            if(SelectedFilters != null && SelectedFilters.Count == 2)
            {
                return $"{DisplayName} : {L10n.Tr(Constants.FromText)} {m_FromValue} {L10n.Tr(Constants.ToText)} {m_ToValue}";
            }

            return base.DisplaySelectedFilters();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    [Serializable]
    class TimestampMetadataFilter : CustomMetadataFilter
    {
        internal const string k_DisplayFormat = "yyyy-MM-dd hh:mm:ss tt";

        [SerializeReference]
        TimestampMetadata m_TimestampMetadata;

        [SerializeField]
        DateTime m_FromValue;

        [SerializeField]
        DateTime m_ToValue;

        public DateTime FromValue => m_FromValue;
        public DateTime ToValue => m_ToValue;

        public override FilterSelectionType SelectionType => FilterSelectionType.Timestamp;

        public TimestampMetadataFilter(IPageFilterStrategy pageFilterStrategy, IMetadata metadata)
            : base(pageFilterStrategy, metadata)
        {
            m_TimestampMetadata = metadata as TimestampMetadata;
            m_FromValue = DateTime.MinValue;
            m_ToValue = DateTime.MinValue;
        }

        public override bool ApplyFromAssetSearchFilter(AssetSearchFilter searchFilter)
        {
            ClearFilter();

            if (searchFilter.CustomMetadata == null || searchFilter.CustomMetadata.Count == 0)
                return false;

            // Because the timestamp metadata range is stored as two separate entries in the search filter,
            // we need to check if both entries exist in the search filter, and if so, set the FromValue and ToValue accordingly.

            var timestampMetadatas = new List<TimestampMetadata>();
            foreach (var metadata in searchFilter.CustomMetadata)
            {
                if (metadata is TimestampMetadata timestampMetadata && timestampMetadata.FieldKey == m_TimestampMetadata.FieldKey)
                {
                    timestampMetadatas.Add(timestampMetadata);
                }
            }

            if (timestampMetadatas.Count != 2)
                return false;

            var firstEntry = timestampMetadatas[0].Value;
            var secondEntry = timestampMetadatas[1].Value;

            var orderedSelections = new List<string>();
            if (firstEntry.DateTime < secondEntry.DateTime)
            {
                orderedSelections.Add(firstEntry.DateTime.ToString(k_DisplayFormat));
                orderedSelections.Add(secondEntry.DateTime.ToString(k_DisplayFormat));
            }
            else
            {
                orderedSelections.Add(secondEntry.DateTime.ToString(k_DisplayFormat));
                orderedSelections.Add(firstEntry.DateTime.ToString(k_DisplayFormat));
            }

            ApplyFilter(orderedSelections);

            return true;
        }

        public override void ResetSelectedFilter(AssetSearchFilter assetSearchFilter)
        {
            assetSearchFilter.CustomMetadata ??= new List<IMetadata>();

            assetSearchFilter.CustomMetadata.RemoveAll(m => m.FieldKey == m_TimestampMetadata.FieldKey);
            assetSearchFilter.CustomMetadata.Add(new TimestampMetadata(m_TimestampMetadata.FieldKey, m_TimestampMetadata.Name,  new DateTimeEntry(m_FromValue)));
            assetSearchFilter.CustomMetadata.Add(new TimestampMetadata(m_TimestampMetadata.FieldKey, m_TimestampMetadata.Name, new DateTimeEntry(m_ToValue)));
        }

        protected override void IncludeFilter(List<string> selectedFilters)
        {
            Utilities.DevAssert(selectedFilters == null || selectedFilters.Count == 2, "TimestampMetadataFilter: IncludeFilter: selection must be null or have 2 elements");

            m_FromValue = DateTime.Parse(selectedFilters?[0] ?? "0", DateTimeFormatInfo.CurrentInfo);
            m_ToValue = DateTime.Parse(selectedFilters?[1] ?? "0", DateTimeFormatInfo.CurrentInfo);
            base.IncludeFilter(selectedFilters);
        }

        public override string DisplaySelectedFilters()
        {
            if (SelectedFilters is { Count: 2 })
            {
                return $"{DisplayName} : {L10n.Tr(Constants.FromText)} {m_FromValue.ToString(k_DisplayFormat)} {L10n.Tr(Constants.ToText)} {m_ToValue.ToString(k_DisplayFormat)}";
            }

            return base.DisplaySelectedFilters();
        }
    }
}

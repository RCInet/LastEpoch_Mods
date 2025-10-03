using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Unity.Cloud.AssetsEmbedded
{
    sealed class MetadataSearchCriteria : BaseSearchCriteria
    {
        readonly Dictionary<string, object> m_Included = new();

        public MetadataSearchCriteria(string propertyName, string searchKey)
            : base(propertyName, searchKey) { }

        /// <inheritdoc/>
        internal override void Include(Dictionary<string, object> includedValues, string prefix = "")
        {
            AddValues(m_Included, includedValues, prefix);
        }

        void AddValues(IDictionary<string, object> from, IDictionary<string, object> to, string prefix)
        {
            var searchKey = SearchKey.BuildSearchKey(prefix);
            foreach (var kvp in from)
            {
                switch (kvp.Value)
                {
                    case null:
                    case ISearchValue searchValue when searchValue.IsEmpty():
                        continue;
                    default:
                        to.Add($"{searchKey}.{kvp.Key}", kvp.Value);
                        break;
                }
            }
        }

        /// <inheritdoc/>
        public override void Clear()
        {
            m_Included.Clear();
        }

        /// <summary>
        /// Sets the search criteria for the metadata field.
        /// </summary>
        /// <param name="metadataFieldKey">The key of the metadata field. </param>
        /// <param name="value">The expected value of the field. </param>
        public void WithValue(string metadataFieldKey, MetadataValue value)
        {
            m_Included[metadataFieldKey] = value.GetValue();
        }

        /// <summary>
        /// Sets the search criteria for the metadata text field.
        /// </summary>
        /// <param name="metadataFieldKey">The key of the metadata field. </param>
        /// <param name="value">The expected value of the text field. </param>
        /// <param name="options">The search options. </param>
        [Obsolete("Use WithTextValue(string, StringPredicate) instead.")]
        public void WithValue(string metadataFieldKey, string value, StringSearchCriteria.SearchOptions options)
        {
            m_Included[metadataFieldKey] = StringSearchCriteria.BuildSearchValue(value, options.HasFlag(StringSearchCriteria.SearchOptions.Prefix) ? StringSearchOption.Prefix : StringSearchOption.Wildcard);
        }

        /// <summary>
        /// Sets the search criteria for the metadata text field.
        /// </summary>
        /// <param name="metadataFieldKey">The key of the metadata field. </param>
        /// <param name="pattern">The expected pattern of the text field. </param>
        [Obsolete("Regex search on text metadata is not supported.")]
        public void WithValue(string metadataFieldKey, Regex pattern)
        {
            m_Included[metadataFieldKey] = SearchStringValue.BuildRegexQuery(pattern);
        }

        /// <summary>
        /// Sets the search criteria for the metadata text field.
        /// </summary>
        /// <param name="metadataFieldKey">The key of the metadata field. </param>
        /// <param name="value">The approximate value of the text field. </param>
        [Obsolete("Fuzzy search on text metadata is not supported.")]
        public void WithFuzzyValue(string metadataFieldKey, string value)
        {
            WithTextValue(metadataFieldKey, new StringPredicate(value, StringSearchOption.Fuzzy));
        }

        /// <summary>
        /// Sets the search criteria for the metadata text field.
        /// </summary>
        /// <param name="metadataFieldKey">The key of the metadata text field. </param>
        /// <param name="stringPredicate">The predicate to match to the metadata text field.</param>
        /// <remarks>
        /// Does not currently support wildcard, regex, or fuzzy search. Only use prefix and exact match.
        /// </remarks>
        public void WithTextValue(string metadataFieldKey, StringPredicate stringPredicate)
        {
            m_Included[metadataFieldKey] = stringPredicate.GetSearchValue();
        }

        /// <summary>
        /// Sets the search criteria for the metadata number field.
        /// </summary>
        /// <param name="metadataFieldKey">The key of the metadata number field. </param>
        /// <param name="numericRangePredicate">The predicate to match to the metadata number field. </param>
        /// <remarks>
        /// Only supported for system metadata.
        /// </remarks>
        public void WithNumberValue(string metadataFieldKey, NumericRangePredicate numericRangePredicate)
        {
            m_Included[metadataFieldKey] = numericRangePredicate.GetSearchValue();
        }

        /// <summary>
        /// Sets the search criteria for the metadata date time field.
        /// </summary>
        /// <param name="metadataFieldKey">The key of the metadata text field. </param>
        /// <param name="min">The minimum value of the date range.</param>
        /// <param name="inclusiveOfMin">Whether the minimum value is inclusive.</param>
        /// <param name="max">The maximum value of the date range.</param>
        /// <param name="inclusiveOfMax">Whether the maximum value is inclusive.</param>
        public void WithTimestampValue(string metadataFieldKey, DateTime min = default, bool inclusiveOfMin = true, DateTime max = default, bool inclusiveOfMax = true)
        {
            var value = new SearchConditionData(SearchConditionData.DateRangeType);

            if (min != default)
            {
                value.AddCondition(SearchUtilities.GetConditionalValueForMin(min, inclusiveOfMin));
            }

            if (max != default)
            {
                value.AddCondition(SearchUtilities.GetConditionalValueForMax(max, inclusiveOfMax));
            }

            value.Validate();

            if (!value.IsEmpty())
            {
                m_Included[metadataFieldKey] = value;
            }
        }
    }
}

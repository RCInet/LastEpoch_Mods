using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Exposes partial string searches.
    /// </summary>
    class StringSearchCriteria : SearchCriteria<string>
    {
        [Flags]
        [Obsolete("Use StringSearchOption instead.")]
        public enum SearchOptions
        {
            None = 0,
            Prefix = 2
        }

        internal static readonly char[] k_WildcardChars = new[] {'*', '?'};

        ISearchValue m_IncludedPartial;

        internal StringSearchCriteria(string propertyName, string searchKey)
            : base(propertyName, searchKey) { }

        /// <inheritdoc/>
        internal override void Include(Dictionary<string, object> includedValues, string prefix = "")
        {
            if (m_IncludedPartial != null)
            {
                includedValues.Add(SearchKey.BuildSearchKey(prefix), m_IncludedPartial);
                return;
            }

            base.Include(includedValues, prefix);
        }

        /// <inheritdoc/>
        public override void Clear()
        {
            base.Clear();

            m_IncludedPartial = default;
        }

        /// <inheritdoc />
        /// <param name="value">The expected value of the field.</param>
        public override void WithValue(string value)
        {
            if (k_WildcardChars.Any(value.Contains))
            {
                m_IncludedPartial = SearchStringValue.BuildWildcardQuery(value);
                m_Included = default;
                return;
            }

            base.WithValue(value);
            m_IncludedPartial = default;
        }

        /// <summary>
        /// Sets the value of the string search term.
        /// </summary>
        /// <param name="value">The string to match. </param>
        /// <param name="options">The additional options. </param>
        [Obsolete("Use WithValue(StringPredicate) instead.")]
        public void WithValue(string value, SearchOptions options)
        {
            m_IncludedPartial = BuildSearchValue(value, options.HasFlag(SearchOptions.Prefix) ? StringSearchOption.Prefix : StringSearchOption.Wildcard);
            m_Included = null;
        }

        /// <summary>
        /// Sets the pattern of the string search term.
        /// </summary>
        /// <param name="pattern">The string pattern to match. </param>
        public void WithValue(Regex pattern)
        {
            m_IncludedPartial = SearchStringValue.BuildRegexQuery(pattern);
            m_Included = null;
        }

        /// <summary>
        /// Sets the predicate criteria for the string search term.
        /// </summary>
        /// <param name="stringPredicate">The string predicate to match.</param>
        public void WithValue(StringPredicate stringPredicate)
        {
            m_IncludedPartial = stringPredicate.GetSearchValue();
            m_Included = null;
        }

        /// <summary>
        /// Sets the fuzzy value of the string search term.
        /// </summary>
        /// <param name="value">The approximate string to match. </param>
        [Obsolete("Use WithValue(StringPredicate) instead.")]
        public void WithFuzzyValue(string value)
        {
            m_IncludedPartial = SearchStringValue.BuildFuzzyQuery(value);
            m_Included = null;
        }

        internal static ISearchValue BuildSearchValue(string value, StringSearchOption option)
        {
            switch (option)
            {
                case StringSearchOption.Prefix:
                    return SearchStringValue.BuildPrefixQuery(value);
                case StringSearchOption.ExactMatch:
                    return SearchStringValue.BuildExactQuery(value);
                case StringSearchOption.Fuzzy:
                    return SearchStringValue.BuildFuzzyQuery(value);
                case StringSearchOption.Wildcard:
                    if (!k_WildcardChars.Any(value.Contains))
                    {
                        value = $"*{value}*";
                    }

                    return SearchStringValue.BuildWildcardQuery(value);
                default:
                    return SearchStringValue.BuildExactQuery(value);
            }
        }
    }
}

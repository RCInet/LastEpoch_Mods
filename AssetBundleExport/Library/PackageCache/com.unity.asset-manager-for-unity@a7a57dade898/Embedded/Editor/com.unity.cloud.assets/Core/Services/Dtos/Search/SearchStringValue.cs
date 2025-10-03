using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class SearchStringValue : ISearchValue
    {
        [DataMember(Name = "type")]
        public string Type { get; private set; }

        [DataMember(Name = "value")]
        public object Value { get; private set; }

        SearchStringValue() { }

        public bool IsEmpty() => string.IsNullOrEmpty(Type) || Value == null;

        internal static SearchStringValue BuildPrefixQuery(string value)
        {
            return new SearchStringValue
            {
                Type = "prefix",
                Value = value,
            };
        }

        internal static SearchStringValue BuildWildcardQuery(string value)
        {
            return new SearchStringValue
            {
                Type = "wildcard",
                Value = value,
            };
        }

        internal static SearchStringValue BuildRegexQuery(Regex regex)
        {
            return new SearchStringValue
            {
                Type = "regex",
                Value = regex.ToString(),
            };
        }

        internal static SearchStringValue BuildFuzzyQuery(string value)
        {
            return new SearchStringValue
            {
                Type = "fuzzy",
                Value = value
            };
        }

        internal static SearchStringValue BuildExactQuery(string value)
        {
            return new SearchStringValue
            {
                Type = "exact-match",
                Value = value
            };
        }
    }
}

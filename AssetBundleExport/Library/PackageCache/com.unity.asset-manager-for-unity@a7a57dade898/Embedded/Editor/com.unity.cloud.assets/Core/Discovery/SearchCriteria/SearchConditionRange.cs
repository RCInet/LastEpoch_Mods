using System;

namespace Unity.Cloud.AssetsEmbedded
{
    readonly struct SearchConditionRange : IEquatable<SearchConditionRange>, IEquatable<string>
    {
        public static readonly SearchConditionRange GreaterThan = new(SearchConditionValue.GreaterThan);
        public static readonly SearchConditionRange GreaterThanOrEqual = new(SearchConditionValue.GreaterThanOrEqual);
        public static readonly SearchConditionRange LessThan = new(SearchConditionValue.LessThan);
        public static readonly SearchConditionRange LessThanOrEqual = new(SearchConditionValue.LessThanOrEqual);

        readonly string m_Value;

        SearchConditionRange(string value)
        {
            m_Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is SearchConditionRange other && Equals(other);
        }

        public bool Equals(string str)
        {
            return m_Value.Equals(str);
        }

        public bool Equals(SearchConditionRange other)
        {
            return Equals(other.m_Value);
        }

        public override int GetHashCode()
        {
            return m_Value != null ? m_Value.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return m_Value;
        }

        public static bool operator ==(SearchConditionRange a, SearchConditionRange b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(SearchConditionRange a, SearchConditionRange b)
        {
            return !(a == b);
        }

        public static implicit operator string(SearchConditionRange a) => a.m_Value;

        public static implicit operator SearchConditionRange(string a) => new(a);
    }
}

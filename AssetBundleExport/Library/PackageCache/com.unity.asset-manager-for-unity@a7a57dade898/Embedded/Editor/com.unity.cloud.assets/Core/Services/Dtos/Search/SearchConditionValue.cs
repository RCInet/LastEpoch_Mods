using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class SearchConditionValue : ISearchValue
    {
        internal const string GreaterThan = "greaterThan";
        internal const string GreaterThanOrEqual = "greaterThanOrEqual";
        internal const string LessThan = "lessThan";
        internal const string LessThanOrEqual = "lessThanOrEqual";

        [DataMember(Name = "value")]
        public object Value => SerializeValue(m_Value);

        [DataMember(Name = "conditionType")]
        public string Type { get; private set; }

        readonly object m_Value;

        public SearchConditionValue(string relationalOperator, object value)
        {
            Type = relationalOperator;
            m_Value = value;
        }

        public bool IsEmpty() => string.IsNullOrEmpty(Type) || m_Value == null;

        public bool Overlaps(ISearchValue other)
        {
            return Type switch
            {
                GreaterThanOrEqual or GreaterThan => other.Type is GreaterThanOrEqual or GreaterThan,
                LessThanOrEqual or LessThan => other.Type is LessThanOrEqual or LessThan,
                _ => false
            };
        }

        static object SerializeValue(object value)
        {
            if (value is DateTime dateTime)
            {
                return dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            }

            return value;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class SearchConditionData : ISearchValue
    {
        internal const string DateRangeType = "date-range";
        internal const string NumericRangeType = "numeric-range";
        internal const string AndType = "and";
        internal const string OrType = "or";

        [DataMember(Name = "type")]
        public string Type { get; private set; }

        [DataMember(Name = "conditions")]
        List<ISearchValue> Conditions = new();

        public ISearchValue this[int index] => Conditions[index];

        public int Count => Conditions.Count;

        public SearchConditionData(string type)
        {
            Type = type;
        }

        public bool IsEmpty() => string.IsNullOrEmpty(Type) || Conditions.Count == 0;

        public bool Any(Func<SearchConditionValue, bool> predicate)
        {
            return Conditions.Select(x => x as SearchConditionValue).Any(predicate);
        }

        public void Clear()
        {
            Conditions.Clear();
        }

        public bool Validate()
        {
            Conditions.RemoveAll(x => x.IsEmpty());
            return Conditions.Count > 0;
        }

        public void AddCondition(ISearchValue searchValue)
        {
            if (searchValue == null) return;

            var index = Conditions.FindIndex(x => x.Overlaps(searchValue));
            if (index >= 0)
            {
                Conditions[index] = searchValue;
            }
            else
            {
                Conditions.Add(searchValue);
            }
        }
    }
}

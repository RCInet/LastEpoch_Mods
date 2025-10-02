#nullable enable
using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    sealed class NullableSearchCriteria<T> : BaseSearchCriteria where T : struct
    {
        T? m_Included;

        internal NullableSearchCriteria(string propertyName, string searchKey)
            : base(propertyName, searchKey) { }

        internal override void Include(Dictionary<string, object> includedValues, string prefix = "")
        {
            if (m_Included.HasValue)
            {
                includedValues.Add(SearchKey.BuildSearchKey(prefix), m_Included.Value);
            }
        }

        /// <inheritdoc/>
        public override void Clear()
        {
            m_Included = null;
        }

        /// <summary>
        /// Sets the value of the search criteria.
        /// </summary>
        public void WithValue(T? value)
        {
            m_Included = value;
        }
    }
}

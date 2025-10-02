using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    class SearchCriteria<T> : BaseSearchCriteria
    {
        private protected T m_Included;

        internal SearchCriteria(string propertyName, string searchKey)
            : base(propertyName, searchKey) { }

        /// <inheritdoc/>
        internal override void Include(Dictionary<string, object> includedValues, string prefix = "")
        {
            if (!IsValueEmpty(m_Included))
            {
                includedValues.Add(SearchKey.BuildSearchKey(prefix), TransformValue(m_Included));
            }
        }

        /// <inheritdoc/>
        public override void Clear()
        {
            m_Included = default;
        }

        /// <summary>
        /// Sets the value of the search criteria.
        /// </summary>
        public virtual void WithValue(T value)
        {
            m_Included = value;
        }

        private protected virtual bool IsValueEmpty(T value)
        {
            if (value is string s)
            {
                return string.IsNullOrWhiteSpace(s);
            }

            return value == null || value.Equals(default(T));
        }

        private protected virtual object TransformValue(T value)
        {
            return value;
        }
    }
}

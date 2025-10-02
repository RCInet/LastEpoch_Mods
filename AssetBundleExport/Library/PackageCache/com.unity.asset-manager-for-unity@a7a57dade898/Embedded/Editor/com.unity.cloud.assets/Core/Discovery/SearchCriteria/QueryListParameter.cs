using System.Collections.Generic;
using System.Linq;

namespace Unity.Cloud.AssetsEmbedded
{
    sealed class QueryListParameter<T>
    {
        IEnumerable<T> m_Value;

        public QueryListParameter(IEnumerable<T> value = default)
        {
            m_Value = value?.ToArray();
        }

        /// <summary>
        /// Adds a filter to the query for the given <typeparamref name="T"/> value.
        /// </summary>
        /// <param name="value">The value to query against. </param>
        public void WhereContains(IEnumerable<T> value)
        {
            m_Value = value?.ToArray();
        }

        public void WhereContains(params T[] value)
        {
            m_Value = value;
        }

        internal IEnumerable<T> GetValue()
        {
            return m_Value;
        }
    }
}

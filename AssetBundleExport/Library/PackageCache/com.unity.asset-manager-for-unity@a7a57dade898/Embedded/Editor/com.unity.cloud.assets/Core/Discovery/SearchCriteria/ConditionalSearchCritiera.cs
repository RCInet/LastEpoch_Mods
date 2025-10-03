using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    sealed class ConditionalSearchCriteria<T> : BaseSearchCriteria
    {
        readonly SearchConditionData m_Included;

        internal ConditionalSearchCriteria(string propertyName, string searchKey, string type)
            : base(propertyName, searchKey)
        {
            m_Included = new SearchConditionData(type);
        }

        internal override void Include(Dictionary<string, object> includedValues, string prefix = "")
        {
            if (m_Included.Validate())
            {
                includedValues.Add(SearchKey.BuildSearchKey(prefix), m_Included);
            }
        }

        /// <inheritdoc/>
        public override void Clear()
        {
            m_Included.Clear();
        }

        /// <summary>
        /// Sets the value of the conditional criteria.
        /// </summary>
        /// <param name="range">The range to consider. </param>
        /// <param name="value">The threshold value. </param>
        public void WithValue(SearchConditionRange range, T value) => WithValue(range.ToString(), value);

        /// <summary>
        /// Sets the value of the conditional criteria.
        /// </summary>
        /// <param name="value">The threshold value. </param>
        public void WithValueGreaterThan(T value)
        {
            WithValue(SearchConditionValue.GreaterThan, value);
        }

        /// <inheritdoc cref="WithValueGreaterThan"/>
        public void WithValueGreaterThanOrEqualTo(T value)
        {
            WithValue(SearchConditionValue.GreaterThanOrEqual, value);
        }

        /// <inheritdoc cref="WithValueGreaterThan"/>
        public void WithValueLessThan(T value)
        {
            WithValue(SearchConditionValue.LessThan, value);
        }

        /// <inheritdoc cref="WithValueGreaterThan"/>
        public void WithValueLessThanOrEqualTo(T value)
        {
            WithValue(SearchConditionValue.LessThanOrEqual, value);
        }

        void WithValue(string relationalOperator, T value)
        {
            m_Included.AddCondition(new SearchConditionValue(relationalOperator, value));
        }
    }
}

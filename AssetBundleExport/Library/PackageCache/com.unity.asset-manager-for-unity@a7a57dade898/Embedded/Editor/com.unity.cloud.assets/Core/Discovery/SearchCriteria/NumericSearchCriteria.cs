using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    sealed class NumericSearchCriteria<T> : SearchCriteria<T>
    {
        ISearchValue m_SearchValue;

        internal NumericSearchCriteria(string propertyName, string searchKey)
            : base(propertyName, searchKey) { }

        /// <inheritdoc/>
        internal override void Include(Dictionary<string, object> includedValues, string prefix = "")
        {
            if (m_SearchValue != null)
            {
                includedValues.Add(SearchKey.BuildSearchKey(prefix), m_SearchValue);
                return;
            }

            base.Include(includedValues, prefix);
        }

        /// <inheritdoc/>
        public override void Clear()
        {
            base.Clear();

            m_SearchValue = default;
        }

        /// <summary>
        /// Sets the predicate criteria for the numeric search term.
        /// </summary>
        /// <param name="numericRangePredicate">The range to match.</param>
        public void WithValue(NumericRangePredicate numericRangePredicate)
        {
            m_SearchValue = numericRangePredicate.GetSearchValue();
            m_Included = default;
        }
    }
}

using System;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Defines a numeric range for searching.
    /// </summary>
    sealed class NumericRange
    {
        readonly SearchConditionData m_SearchConditionData = new(SearchConditionData.NumericRangeType);

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericRange"/> class.
        /// </summary>
        /// <param name="min">The minimum value of the range.</param>
        /// <param name="inclusiveOfMin">Whether the minimum value is inclusive.</param>
        /// <param name="max">The maximum value of the range.</param>
        /// <param name="inclusiveOfMax">Whether the maximum value is inclusive.</param>
        public NumericRange(double min = double.MinValue, bool inclusiveOfMin = true, double max = double.MaxValue, bool inclusiveOfMax = true)
        {
            m_SearchConditionData.AddCondition(SearchUtilities.GetConditionalValueForMin(min, inclusiveOfMin));
            m_SearchConditionData.AddCondition(SearchUtilities.GetConditionalValueForMax(max, inclusiveOfMax));
        }

        NumericRange(ISearchValue searchValue)
        {
            m_SearchConditionData.AddCondition(searchValue);
        }

        internal ISearchValue GetSearchValue()
        {
            return m_SearchConditionData.Validate() ? m_SearchConditionData : null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericRange"/> class.
        /// </summary>
        /// <param name="value">The exclusive minimum value of the range.</param>
        /// <returns>A <see cref="NumericRange"/>. </returns>
        public static NumericRange GreaterThan(double value) => new(SearchUtilities.GetConditionalValueForMin(value, false));

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericRange"/> class.
        /// </summary>
        /// <param name="value">The inclusive minimum value of the range.</param>
        /// <returns>A <see cref="NumericRange"/>. </returns>
        public static NumericRange GreaterThanOrEqual(double value) => new(SearchUtilities.GetConditionalValueForMin(value, true));

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericRange"/> class.
        /// </summary>
        /// <param name="value">The exclusive maximum value of the range.</param>
        /// <returns>A <see cref="NumericRange"/>. </returns>
        public static NumericRange LessThan(double value) => new(SearchUtilities.GetConditionalValueForMax(value, false));

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericRange"/> class.
        /// </summary>
        /// <param name="value">The inclusive maximum value of the range.</param>
        /// <returns>A <see cref="NumericRange"/>. </returns>
        public static NumericRange LessThanOrEqual(double value) => new(SearchUtilities.GetConditionalValueForMax(value, true));
    }
}

using System;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Available options for string searches.
    /// </summary>
    enum StringSearchOption
    {
        /// <summary>
        /// Default search option, accepts wildcard characters '?' and '*'.
        /// </summary>
        Wildcard = 0,
        /// <summary>
        /// Searches for values that start with the given search term.
        /// </summary>
        Prefix = 1,
        /// <summary>
        /// Searches for values that are an exact match to the given search term.
        /// </summary>
        ExactMatch = 2,
        /// <summary>
        /// Searches for values similar to the given search term.
        /// </summary>
        Fuzzy = 3,
    }

    static class SearchUtilities
    {
        /// <summary>
        /// Returns a predicate result of applying the AND operator to a NumericRange and a numeric range predicate.
        /// </summary>
        public static NumericRangePredicate And(this NumericRange lhs, NumericRangePredicate rhs) => new NumericRangePredicate(lhs).And(rhs);

        /// <summary>
        /// Returns a predicate result of applying the OR operator to a NumericRange and a numeric range predicate.
        /// </summary>
        public static NumericRangePredicate Or(this NumericRange lhs, NumericRangePredicate rhs) => new NumericRangePredicate(lhs).Or(rhs);

        /// <summary>
        /// Returns a predicate result of applying the AND operator to a NumericRange and the negated numeric range predicate.
        /// </summary>
        public static NumericRangePredicate AndNot(this NumericRange lhs, NumericRangePredicate rhs) => new NumericRangePredicate(lhs).AndNot(rhs);

        /// <summary>
        /// Returns a predicate result of applying the OR operator to a numeric range and the negated numeric range predicate.
        /// </summary>
        public static NumericRangePredicate OrNot(this NumericRange lhs, NumericRangePredicate rhs) => new NumericRangePredicate(lhs).OrNot(rhs);

        internal static ISearchValue GetConditionalValueForMin(object min, bool inclusiveOfMin)
            => new SearchConditionValue(inclusiveOfMin ? SearchConditionValue.GreaterThanOrEqual : SearchConditionValue.GreaterThan, min);

        internal static ISearchValue GetConditionalValueForMax(object max, bool inclusiveOfMax)
            => new SearchConditionValue(inclusiveOfMax ? SearchConditionValue.LessThanOrEqual : SearchConditionValue.LessThan, max);
    }
}

using System;
using OperatorType = Unity.Cloud.AssetsEmbedded.OperatorSearchValues.OperatorType;

namespace Unity.Cloud.AssetsEmbedded
{
    sealed class NumericRangePredicate
    {
        readonly OperatorSearchValues m_SearchValues;

        internal NumericRangePredicate(NumericRange numericRange)
            : this(OperatorType.Undefined)
        {
            var searchValue = numericRange.GetSearchValue();
            if (searchValue != null)
            {
                m_SearchValues.Add(searchValue);
            }
        }

        NumericRangePredicate(OperatorType operatorType)
        {
            m_SearchValues = new OperatorSearchValues(operatorType);
        }

        /// <summary>
        /// Returns a predicate with the negated numeric range predicate.
        /// </summary>
        public static NumericRangePredicate Not(NumericRangePredicate numericRangePredicate) => GetResult(OperatorType.Not, numericRangePredicate);

        /// <summary>
        /// Returns the predicate result of applying the AND operator to this and the specified numeric range predicate.
        /// </summary>
        public NumericRangePredicate And(NumericRangePredicate rhs) => GetResult(OperatorType.And, this, rhs);

        /// <summary>
        /// Returns the predicate result of applying the OR operator to this and the specified numeric range predicate.
        /// </summary>
        public NumericRangePredicate Or(NumericRangePredicate rhs) => GetResult(OperatorType.Or, this, rhs);

        /// <summary>
        /// Returns the predicate result of applying the AND operator to this and the negated numeric range predicate.
        /// </summary>
        public NumericRangePredicate AndNot(NumericRangePredicate rhs) => GetResult(OperatorType.And, this, Not(rhs));

        /// <summary>
        /// Returns the predicate result of applying the OR operator to this and the negated numeric range predicate.
        /// </summary>
        public NumericRangePredicate OrNot(NumericRangePredicate rhs) => GetResult(OperatorType.Or, this, Not(rhs));

        /// <summary>
        /// Implicitly converts a numeric range to a numeric range predicate.
        /// </summary>
        public static implicit operator NumericRangePredicate(NumericRange a) => new(a);

        internal ISearchValue GetSearchValue() => m_SearchValues.GetSearchValue();

        static NumericRangePredicate GetResult(OperatorType operatorType, params NumericRangePredicate[] components)
        {
            var result = new NumericRangePredicate(operatorType);
            foreach (var component in components)
            {
                result.m_SearchValues.PopulateResult(component.m_SearchValues);
            }

            return result;
        }
    }
}

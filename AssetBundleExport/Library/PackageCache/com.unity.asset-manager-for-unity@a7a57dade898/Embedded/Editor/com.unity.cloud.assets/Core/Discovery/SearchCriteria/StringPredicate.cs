using System;
using System.Linq;
using System.Text.RegularExpressions;
using OperatorType = Unity.Cloud.AssetsEmbedded.OperatorSearchValues.OperatorType;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Defines a string operator for searching.
    /// </summary>
    sealed class StringPredicate
    {
        readonly OperatorSearchValues m_SearchValues;

        public StringPredicate(string value)
            : this(value, StringSearchCriteria.k_WildcardChars.Any(value.Contains) ? StringSearchOption.Wildcard : StringSearchOption.ExactMatch) { }

        /// <summary>
        /// Initializes a string predicate with the specified string and option.
        /// </summary>
        /// <param name="value">The string to match. </param>
        /// <param name="searchOption">The matching option. </param>
        public StringPredicate(string value, StringSearchOption searchOption)
            : this(OperatorType.Undefined)
        {
            m_SearchValues.Add(StringSearchCriteria.BuildSearchValue(value, searchOption));
        }

        /// <summary>
        /// Initializes a string predicate with the specified string pattern.
        /// </summary>
        /// <param name="regex"></param>
        public StringPredicate(Regex regex)
            : this(OperatorType.Undefined)
        {
            m_SearchValues.Add(SearchStringValue.BuildRegexQuery(regex));
        }

        StringPredicate(OperatorType operatorType)
        {
            m_SearchValues = new OperatorSearchValues(operatorType);
        }

        internal ISearchValue GetSearchValue() => m_SearchValues.GetSearchValue();

        /// <summary>
        /// Returns a predicate with the negated string predicate.
        /// </summary>
        public static StringPredicate Not(StringPredicate stringPredicate) => GetResult(OperatorType.Not, stringPredicate);

        /// <summary>
        /// Returns a predicate with the negated string.
        /// </summary>
        public static StringPredicate Not(string value) => GetResult(OperatorType.Not, new StringPredicate(value));

        /// <summary>
        /// Returns the predicate result of applying the AND operator to this and the specified string predicate.
        /// </summary>
        public StringPredicate And(StringPredicate rhs)
            => GetResult(OperatorType.And, this, rhs);

        /// <summary>
        /// Returns the predicate result of applying the AND operator to this and the specified string.
        /// </summary>
        public StringPredicate And(string rhs)
            => GetResult(OperatorType.And, this, new StringPredicate(rhs));

        /// <summary>
        /// Returns the predicate result of applying the AND operator to this and the specified string.
        /// </summary>
        public StringPredicate And(string rhs, StringSearchOption searchOption)
            => GetResult(OperatorType.And, this, new StringPredicate(rhs, searchOption));

        /// <summary>
        /// Returns the predicate result of applying the OR operator to this and the specified string predicate.
        /// </summary>
        public StringPredicate Or(StringPredicate rhs)
            => GetResult(OperatorType.Or, this, rhs);

        /// <summary>
        /// Returns the predicate result of applying the OR operator to this and the specified string.
        /// </summary>
        public StringPredicate Or(string rhs)
            => GetResult(OperatorType.Or, this, new StringPredicate(rhs));

        /// <summary>
        /// Returns the predicate result of applying the OR operator to this and the specified string.
        /// </summary>
        public StringPredicate Or(string rhs, StringSearchOption searchOption)
            => GetResult(OperatorType.Or, this, new StringPredicate(rhs, searchOption));

        /// <summary>
        /// Returns the predicate result of applying the AND operator to this and the negated string predicate.
        /// </summary>
        public StringPredicate AndNot(StringPredicate rhs)
            => GetResult(OperatorType.And, this, Not(rhs));

        /// <summary>
        /// Returns the predicate result of applying the AND operator to this and the negated string.
        /// </summary>
        public StringPredicate AndNot(string rhs)
            => GetResult(OperatorType.And, this, Not(new StringPredicate(rhs)));

        /// <summary>
        /// Returns the predicate result of applying the AND operator to this and the negated string.
        /// </summary>
        public StringPredicate AndNot(string rhs, StringSearchOption searchOption)
            => GetResult(OperatorType.And, this, Not(new StringPredicate(rhs, searchOption)));

        /// <summary>
        /// Returns the predicate result of applying the OR operator to this and the negated string predicate.
        /// </summary>
        public StringPredicate OrNot(StringPredicate rhs)
            => GetResult(OperatorType.Or, this, Not(rhs));

        /// <summary>
        /// Returns the predicate result of applying the OR operator to this and the negated string.
        /// </summary>
        public StringPredicate OrNot(string rhs)
            => GetResult(OperatorType.Or, this, Not(new StringPredicate(rhs)));

        /// <summary>
        /// Returns the predicate result of applying the OR operator to this and the negated string.
        /// </summary>
        public StringPredicate OrNot(string rhs, StringSearchOption searchOption)
            => GetResult(OperatorType.Or, this, Not(new StringPredicate(rhs, searchOption)));

        /// <summary>
        /// Explicitly converts a string to a string predicate.
        /// </summary>
        public static explicit operator StringPredicate(string a) => new(a);

        /// <summary>
        /// Implicitly converts a regex to a string predicate.
        /// </summary>
        public static implicit operator StringPredicate(Regex a) => new(a);

        static StringPredicate GetResult(OperatorType searchOperator, params StringPredicate[] components)
        {
            var result = new StringPredicate(searchOperator);
            foreach (var component in components)
            {
                result.m_SearchValues.PopulateResult(component.m_SearchValues);
            }

            return result;
        }
    }
}

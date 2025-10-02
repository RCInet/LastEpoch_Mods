using System;
using System.Text.RegularExpressions;

namespace Unity.Cloud.AssetsEmbedded.SearchExtensions
{
    static class StringPredicateExtensions
    {
        #region StringPredicate factory methods

        public static StringPredicate AsWildcard(this string value) => new(value, StringSearchOption.Wildcard);

        public static StringPredicate AsPrefix(this string value) => new(value, StringSearchOption.Prefix);

        public static StringPredicate AsExactMatch(this string value) => new(value);

        public static StringPredicate AsFuzzyMatch(this string value) => new(value, StringSearchOption.Fuzzy);

        #endregion

        #region StringPredicate logical operators

        public static StringPredicate And(this string lhs, StringPredicate stringPredicate)
            => new StringPredicate(lhs).And(stringPredicate);

        public static StringPredicate And(this string lhs, string rhs)
            => new StringPredicate(lhs).And(rhs);

        public static StringPredicate And(this string lhs, string rhs, StringSearchOption searchOption)
            => new StringPredicate(lhs).And(rhs, searchOption);

        public static StringPredicate And(this string lhs, Regex rhs)
            => new StringPredicate(lhs).And(rhs);

        public static StringPredicate And(this Regex lhs, StringPredicate stringPredicate)
            => new StringPredicate(lhs).And(stringPredicate);

        public static StringPredicate And(this Regex lhs, string rhs)
            => new StringPredicate(lhs).And(rhs);

        public static StringPredicate And(this Regex lhs, string rhs, StringSearchOption searchOption)
            => new StringPredicate(lhs).And(rhs, searchOption);

        public static StringPredicate And(this Regex lhs, Regex rhs)
            => new StringPredicate(lhs).And(rhs);

        public static StringPredicate Or(this string lhs, StringPredicate stringPredicate)
            => new StringPredicate(lhs).Or(stringPredicate);

        public static StringPredicate Or(this string lhs, string rhs)
            => new StringPredicate(lhs).Or(rhs);

        public static StringPredicate Or(this string lhs, string rhs, StringSearchOption searchOption)
            => new StringPredicate(lhs).Or(rhs, searchOption);

        public static StringPredicate Or(this string lhs, Regex rhs)
            => new StringPredicate(lhs).Or(rhs);

        public static StringPredicate Or(this Regex lhs, StringPredicate stringPredicate)
            => new StringPredicate(lhs).Or(stringPredicate);

        public static StringPredicate Or(this Regex lhs, string rhs)
            => new StringPredicate(lhs).Or(rhs);

        public static StringPredicate Or(this Regex lhs, string rhs, StringSearchOption searchOption)
            => new StringPredicate(lhs).Or(rhs, searchOption);

        public static StringPredicate Or(this Regex lhs, Regex rhs)
            => new StringPredicate(lhs).Or(rhs);

        public static StringPredicate AndNot(this string lhs, StringPredicate stringPredicate)
            => new StringPredicate(lhs).AndNot(stringPredicate);

        public static StringPredicate AndNot(this string lhs, string rhs)
            => new StringPredicate(lhs).AndNot(rhs);

        public static StringPredicate AndNot(this string lhs, string rhs, StringSearchOption searchOption)
            => new StringPredicate(lhs).AndNot(rhs, searchOption);

        public static StringPredicate AndNot(this string lhs, Regex rhs)
            => new StringPredicate(lhs).AndNot(rhs);

        public static StringPredicate AndNot(this Regex lhs, StringPredicate stringPredicate)
            => new StringPredicate(lhs).AndNot(stringPredicate);

        public static StringPredicate AndNot(this Regex lhs, string rhs)
            => new StringPredicate(lhs).AndNot(rhs);

        public static StringPredicate AndNot(this Regex lhs, string rhs, StringSearchOption searchOption)
            => new StringPredicate(lhs).AndNot(rhs, searchOption);

        public static StringPredicate AndNot(this Regex lhs, Regex rhs)
            => new StringPredicate(lhs).AndNot(rhs);

        public static StringPredicate OrNot(this string lhs, StringPredicate stringPredicate)
            => new StringPredicate(lhs).OrNot(stringPredicate);

        public static StringPredicate OrNot(this string lhs, string rhs)
            => new StringPredicate(lhs).OrNot(rhs);

        public static StringPredicate OrNot(this string lhs, string rhs, StringSearchOption searchOption)
            => new StringPredicate(lhs).OrNot(rhs, searchOption);

        public static StringPredicate OrNot(this string lhs, Regex rhs)
            => new StringPredicate(lhs).OrNot(rhs);

        public static StringPredicate OrNot(this Regex lhs, StringPredicate stringPredicate)
            => new StringPredicate(lhs).OrNot(stringPredicate);

        public static StringPredicate OrNot(this Regex lhs, string rhs)
            => new StringPredicate(lhs).OrNot(rhs);

        public static StringPredicate OrNot(this Regex lhs, string rhs, StringSearchOption searchOption)
            => new StringPredicate(lhs).OrNot(rhs, searchOption);

        public static StringPredicate OrNot(this Regex lhs, Regex rhs)
            => new StringPredicate(lhs).OrNot(rhs);

        #endregion
    }
}

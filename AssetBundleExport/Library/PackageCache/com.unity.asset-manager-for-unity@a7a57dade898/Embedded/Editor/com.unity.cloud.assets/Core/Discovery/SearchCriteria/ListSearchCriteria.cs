using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Cloud.AssetsEmbedded
{
    sealed class ListSearchCriteria<T> : SearchCriteria<IEnumerable<T>>
    {
        internal ListSearchCriteria(string propertyName, string searchKey)
            : base(propertyName, searchKey) { }

        /// <summary>
        /// Sets the value of the list search criteria.
        /// </summary>
        public void WithValue(params T[] values)
        {
            base.WithValue(values?.ToArray());
        }

        private protected override bool IsValueEmpty(IEnumerable<T> value)
        {
            var enumerable = value?.ToArray();
            return enumerable == null || enumerable.Length == 0;
        }

        private protected override object TransformValue(IEnumerable<T> value)
        {
            return value?.ToArray();
        }
    }
}

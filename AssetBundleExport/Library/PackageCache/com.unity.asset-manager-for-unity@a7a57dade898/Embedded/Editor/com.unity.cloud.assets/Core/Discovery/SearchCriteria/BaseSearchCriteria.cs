using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Implement this interface to manage a type of criteria for searches.
    /// </summary>
    abstract class BaseSearchCriteria
    {
        /// <summary>
        /// Returns the name of the associated property.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Returns the search key.
        /// </summary>
        internal string SearchKey { get; }

        private protected BaseSearchCriteria(string propertyName, string searchKey)
        {
            PropertyName = propertyName;
            SearchKey = searchKey;
        }

        /// <summary>
        /// Populates the dictionary of <paramref name="includedValues"/> which will be sent to the request.
        /// </summary>
        /// <remarks>The properties in this dictionary will be combined with an <c>AND</c>. </remarks>
        /// <param name="includedValues">The collection in which to add a value to include in the search. </param>
        /// <param name="prefix">A prefix for the <see cref="PropertyName"/>; may be empty. </param>
        internal abstract void Include(Dictionary<string, object> includedValues, string prefix = "");

        /// <summary>
        /// Clears the criteria fields.
        /// </summary>
        public abstract void Clear();
    }
}

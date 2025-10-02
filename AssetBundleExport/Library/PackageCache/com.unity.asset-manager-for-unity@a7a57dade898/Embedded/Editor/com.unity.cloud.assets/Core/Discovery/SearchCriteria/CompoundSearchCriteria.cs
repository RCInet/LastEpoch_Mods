using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A structure for defining criteria which represent a reference type.
    /// </summary>
    abstract class CompoundSearchCriteria : BaseSearchCriteria
    {
        readonly BaseSearchCriteria[] m_AllCriteria;

        public IEnumerable<BaseSearchCriteria> AllCriteria => m_AllCriteria;

        private protected CompoundSearchCriteria(string propertyName, string searchKey)
            : base(propertyName, searchKey)
        {
            m_AllCriteria = GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => typeof(BaseSearchCriteria).IsAssignableFrom(x.PropertyType))
                .Select(x => x.GetValue(this) as BaseSearchCriteria)
                .ToArray();
        }

        /// <inheritdoc/>
        internal override void Include(Dictionary<string, object> includedValues, string prefix = "")
        {
            var searchKey = SearchKey.BuildSearchKey(prefix);
            foreach (var criterion in m_AllCriteria)
            {
                criterion.Include(includedValues, searchKey);
            }
        }

        /// <inheritdoc/>
        public override void Clear()
        {
            foreach (var criterion in m_AllCriteria)
            {
                criterion.Clear();
            }
        }
    }
}

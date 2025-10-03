using System.Linq;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A simple Asset type search but wrapped in a type.
    /// </summary>
    sealed class AssetTypeSearchCriteria : SearchCriteria<string>
    {
        const char k_SplitChar = ' ';

        /// <summary>
        /// The search key for the AssetType.
        /// </summary>
        public new static string SearchKey => "primaryType";

        internal AssetTypeSearchCriteria(string propertyName)
            : base(propertyName, SearchKey) { }

        /// <summary>
        /// Sets the value of the <see cref="AssetType"/> criteria.
        /// </summary>
        /// <param name="assetTypes">The asset types to match. </param>
        public void WithValue(params AssetType[] assetTypes)
        {
            WithValue(string.Join(k_SplitChar, assetTypes.Select(x => x.GetValueAsString())));
        }
    }
}

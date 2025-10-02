using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A simple Asset type search but wrapped in a type.
    /// </summary>
    sealed class AssetStateSearchCriteria : BaseSearchCriteria
    {
        AssetState? m_AssetState;

        internal AssetStateSearchCriteria(string propertyName)
            : base(propertyName, default) { }

        internal override void Include(Dictionary<string, object> includedValues, string prefix = "")
        {
            if (m_AssetState.HasValue)
            {
                switch (m_AssetState)
                {
                    case AssetState.Unfrozen:
                        includedValues.Add("isFrozen".BuildSearchKey(prefix), false);
                        break;
                    case AssetState.Frozen:
                        includedValues.Add("isFrozen".BuildSearchKey(prefix), true);
                        break;
                    case AssetState.PendingFreeze:
                        includedValues.Add("autoSubmit".BuildSearchKey(prefix), true);
                        break;
                }
            }
        }

        public override void Clear()
        {
            m_AssetState = null;
        }

        /// <summary>
        /// Sets the value of the <see cref="AssetState"/> criteria.
        /// </summary>
        /// <param name="assetState">The asset state to match. </param>
        public void WithValue(AssetState assetState)
        {
            m_AssetState = assetState;
        }
    }
}

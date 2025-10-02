using System;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A class the defines the search criteria for a <see cref="ILabel"/> query.
    /// </summary>
    sealed class LabelSearchFilter
    {
        /// <summary>
        /// Whether the results should include archived labels.
        /// </summary>
        public QueryParameter<bool?> IsArchived { get; } = new();

        /// <summary>
        /// Whether the results should include system labels.
        /// </summary>
        public QueryParameter<bool?> IsSystemLabel { get; } = new();
    }
}

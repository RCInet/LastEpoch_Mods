using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A class that defines search criteria for an <see cref="ITransformation"/> query.
    /// </summary>
    sealed class TransformationSearchFilter
    {
        /// <summary>
        /// Sets the asset id to use for the query.
        /// </summary>
        public QueryParameter<AssetId> AssetId { get; } = new(CommonEmbedded.AssetId.None);

        /// <summary>
        /// Sets the asset version to use for the query.
        /// </summary>
        public QueryParameter<AssetVersion> AssetVersion { get; } = new(CommonEmbedded.AssetVersion.None);

        /// <summary>
        /// Sets the dataset id to use for the query.
        /// </summary>
        public QueryParameter<DatasetId> DatasetId { get; } = new(CommonEmbedded.DatasetId.None);

        /// <summary>
        /// Sets the status to use for the query.
        /// </summary>
        public QueryParameter<TransformationStatus?> Status { get; } = new();

        /// <summary>
        /// Sets the user id to use for the query.
        /// </summary>
        public QueryParameter<UserId> UserId { get; } = new(CommonEmbedded.UserId.None);
    }
}

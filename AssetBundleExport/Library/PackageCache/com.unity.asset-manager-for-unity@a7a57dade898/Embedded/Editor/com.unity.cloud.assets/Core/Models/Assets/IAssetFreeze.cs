using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    enum AssetFreezeOperation
    {
        /// <summary>
        /// If there are no on-going transformations the asset will be frozen immediately.
        /// If there are on-going transformations the version will enter a pending state; once these transformations are completed the version will be automatically frozen.
        /// </summary>
        WaitOnTransformations,
        /// <summary>
        /// Any on-going transformations will be cancelled and the version will be frozen immediately.
        /// </summary>
        CancelTransformations,
        /// <summary>
        /// Ignores the freeze operation if there are on-going transformations.
        /// </summary>
        IgnoreIfTransformations
    }

    interface IAssetFreeze
    {
        /// <summary>
        /// A changelog for the version.
        /// </summary>
        string ChangeLog { get; }

        /// <summary>
        /// Defines the operation to perform when freezing the version.
        /// </summary>
        AssetFreezeOperation Operation => AssetFreezeOperation.WaitOnTransformations;
    }
}

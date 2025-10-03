using System;

namespace Unity.AssetManager.Core.Editor
{
    class AssetDependency
    {
        public string ReferenceId { get; set; }

        public AssetIdentifier TargetAssetIdentifier { get; }


        public AssetDependency(string referenceId, AssetIdentifier targetAssetIdentifier)
        {
            ReferenceId = referenceId;
            TargetAssetIdentifier = targetAssetIdentifier;
        }

        public AssetDependency(AssetIdentifier targetAssetIdentifier)
        {
            TargetAssetIdentifier = targetAssetIdentifier;
        }
    }
}

using System.Collections.Generic;

namespace Unity.AssetManager.Core.Editor
{
    struct ImportResultInternal
    {
        public bool Cancelled;
        public bool OperationInProgress;
        public IEnumerable<BaseAssetData> Assets;
        public IEnumerable<BaseAssetData> AssetsAndDependencies;
    }
}

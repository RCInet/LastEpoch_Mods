using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;

namespace Unity.AssetManager.UI.Editor
{
    [Serializable]
    abstract class LocalFilter : BaseFilter
    {
        internal LocalFilter(IPageFilterStrategy pageFilterStrategy) : base(pageFilterStrategy) { }

        public abstract Task<bool> Contains(BaseAssetData assetData, CancellationToken token = default);
    }
}

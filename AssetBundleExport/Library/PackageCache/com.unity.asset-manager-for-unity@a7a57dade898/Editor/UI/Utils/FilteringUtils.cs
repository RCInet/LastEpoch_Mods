using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;

namespace Unity.AssetManager.UI.Editor
{
    static class FilteringUtils
    {
        internal static async Task<IEnumerable<ImportedAssetInfo>> GetFilteredImportedAssets(IReadOnlyCollection<ImportedAssetInfo> importedAssetInfos, IEnumerable<LocalFilter> localFilters, CancellationToken token)
        {
            var tasks = (from assetInfo in importedAssetInfos where assetInfo.AssetData != null select IsKeptByLocalFilterAsync(assetInfo, localFilters, token)).ToList();

            await Task.WhenAll(tasks);

            return tasks.Select(t => t.Result).Where(a => a != null);
        }

        static async Task<ImportedAssetInfo> IsKeptByLocalFilterAsync(ImportedAssetInfo assetInfo, IEnumerable<LocalFilter> localFilters, CancellationToken token)
        {
            if (await IsDiscardedByLocalFilter(assetInfo.AssetData, localFilters, token))
            {
                return null;
            }

            return assetInfo;
        }

        internal static async Task<bool> IsDiscardedByLocalFilter(BaseAssetData assetData, IEnumerable<LocalFilter> localFilters, CancellationToken token)
        {
            var tasks = localFilters.Select(filter => filter.Contains(assetData, token)).ToList();

            await Task.WhenAll(tasks);

            return tasks.Exists(t => !t.Result);
        }
    }
}

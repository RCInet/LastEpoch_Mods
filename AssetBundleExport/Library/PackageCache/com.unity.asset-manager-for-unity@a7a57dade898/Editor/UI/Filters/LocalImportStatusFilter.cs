using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;

namespace Unity.AssetManager.UI.Editor
{
    class LocalImportStatusFilter : LocalFilter
    {
        static readonly FilterSelection[] k_Selections =
        {
            new(Constants.UpToDate),
            new(Constants.Outdated),
            new(Constants.Deleted)
        };

        public override string DisplayName => "Import Status";

        public LocalImportStatusFilter(IPageFilterStrategy pageFilterStrategy)
            : base(pageFilterStrategy) { }

        public override Task<List<FilterSelection>> GetSelections(bool _ = false)
        {
            return Task.FromResult(k_Selections.ToList());
        }

        public override async Task<bool> Contains(BaseAssetData assetData, CancellationToken token = default)
        {
            await Task.CompletedTask;

            if (SelectedFilters == null || !SelectedFilters.Any())
            {
                return true;
            }

            var status = assetData.AssetDataAttributeCollection?.GetAttribute<ImportAttribute>()?.Status;
            return status.HasValue && SelectedFilters.Any(selectedFilter => status == Map(selectedFilter));
        }

        static ImportAttribute.ImportStatus Map(string importStatus)
        {
            return importStatus switch
            {
                Constants.UpToDate => ImportAttribute.ImportStatus.UpToDate,
                Constants.Outdated => ImportAttribute.ImportStatus.OutOfDate,
                Constants.Deleted => ImportAttribute.ImportStatus.ErrorSync,
                _ => ImportAttribute.ImportStatus.NoImport
            };
        }
    }
}

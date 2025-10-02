using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    [Serializable]
    class InProjectPage : BasePage
    {
        public override bool DisplaySearchBar => false;
        public override bool DisplayTitle => true;
        public override bool DisplaySavedViewControls => false;
        public override string Title => L10n.Tr(Constants.InProjectTitle);
        public override bool SupportsUpdateAll => true;

        Task m_UpdateImportStatusTask;
        Task<IEnumerable<ImportedAssetInfo>> m_GetFilteredImportedAssetsTask;

        public InProjectPage(IAssetDataManager assetDataManager, IAssetsProvider assetsProvider,
            IProjectOrganizationProvider projectOrganizationProvider, IMessageManager messageManager,
            IPageManager pageManager, IDialogManager dialogManager)
            : base(assetDataManager, assetsProvider, projectOrganizationProvider, messageManager, pageManager,
                dialogManager) { }

        public override void OnEnable()
        {
            base.OnEnable();
            m_AssetDataManager.ImportedAssetInfoChanged += OnImportedAssetInfoChanged;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            m_AssetDataManager.ImportedAssetInfoChanged -= OnImportedAssetInfoChanged;
        }

        void OnImportedAssetInfoChanged(AssetChangeArgs args)
        {
            if (!m_PageManager.IsActivePage(this))
                return;

            var clearSelection = args.Removed.Any(trackedAsset =>
                SelectedAssets.Any(selectedAsset => trackedAsset.Equals(selectedAsset)));

            Clear(true, clearSelection);
        }

        public override void Clear(bool reloadImmediately, bool clearSelection = true)
        {
            m_UpdateImportStatusTask = null;
            m_GetFilteredImportedAssetsTask = null;

            base.Clear(reloadImmediately, clearSelection);
        }

        protected internal override async IAsyncEnumerable<BaseAssetData> LoadMoreAssets(
            [EnumeratorCancellation] CancellationToken token)
        {
            var importedAssetCount = m_AssetDataManager.ImportedAssetInfos.Count;

#if AM4U_DEV
            var t = new Stopwatch();
            t.Start();
#endif

            if (m_UpdateImportStatusTask == null)
            {
                // Only initiate an update of import statuses if required by the filter or search order
                if (HasFilter<LocalImportStatusFilter>() || m_PageManager.SortField == SortField.ImportStatus)
                {
                    Utilities.DevLog($"Updating import status for {importedAssetCount} asset(s)...");
                    m_UpdateImportStatusTask = UpdateImportStatusAsync(m_AssetDataManager.ImportedAssetInfos, token);
                }
                else
                {
                    m_UpdateImportStatusTask = Task.CompletedTask;
                }
            }

            await m_UpdateImportStatusTask;

            if (m_GetFilteredImportedAssetsTask == null)
            {
                Utilities.DevLog($"Initializing retrieval of import data for {importedAssetCount} asset(s)...");
                m_GetFilteredImportedAssetsTask = FilteringUtils.GetFilteredImportedAssets(m_AssetDataManager.ImportedAssetInfos, m_PageFilterStrategy.SelectedLocalFilters, token);
            }

            var filteredImportedAssets = await m_GetFilteredImportedAssetsTask;
            var sortedImportedAssets = SortImportedAssetsAsync(filteredImportedAssets);

            var maxIndex = Math.Min(m_NextStartIndex + Constants.DefaultPageSize, sortedImportedAssets.Length);
            var count = 0;
            for (; m_NextStartIndex < maxIndex; ++m_NextStartIndex, ++count)
            {
                token.ThrowIfCancellationRequested();
                yield return sortedImportedAssets[m_NextStartIndex].AssetData;
            }

#if AM4U_DEV
            t.Stop();
            var range = $"{m_NextStartIndex + 1}..{m_NextStartIndex + Constants.DefaultPageSize}";
            Utilities.DevLog($"Fetched {count} asset(s) from {range} in {t.ElapsedMilliseconds} ms");
#endif

            m_CanLoadMoreItems = m_NextStartIndex < sortedImportedAssets.Length;
        }

        protected override void OnLoadMoreSuccessCallBack()
        {
            m_PageFilterStrategy.EnableFilters(AssetList.Any());

            if (!AssetList.Any())
            {
                SetPageMessage(new Message(L10n.Tr(Constants.EmptyInProjectText)));
            }
            else
            {
                m_MessageManager.ClearAllMessages();
            }
        }

        protected override Dictionary<string, SortField> CreateSortField()
        {
            return new Dictionary<string, SortField>
            {
                {s_SortFieldsLabelMap[SortField.Name], SortField.Name},
                {s_SortFieldsLabelMap[SortField.Updated], SortField.Updated},
                {s_SortFieldsLabelMap[SortField.Created], SortField.Created},
                {s_SortFieldsLabelMap[SortField.Description], SortField.Description},
                {s_SortFieldsLabelMap[SortField.Status], SortField.Status},
                {s_SortFieldsLabelMap[SortField.ImportStatus], SortField.ImportStatus}
            };
        }

        ImportedAssetInfo[] SortImportedAssetsAsync(IEnumerable<ImportedAssetInfo> importedAssets)
        {
            Func<ImportedAssetInfo, object> firstOrderSorting = info => info.AssetData?.Name;
            Func<ImportedAssetInfo, object> secondOrderSorting = null;

            switch (m_PageManager.SortField)
            {
                case SortField.Name:
                    firstOrderSorting = info => info.AssetData?.Name;
                    break;
                case SortField.Updated:
                    firstOrderSorting = info => info.AssetData?.Updated;
                    secondOrderSorting = info => info.AssetData?.Name;
                    break;
                case SortField.Created:
                    firstOrderSorting = info => info.AssetData?.Created;
                    secondOrderSorting = info => info.AssetData?.Name;
                    break;
                case SortField.Description:
                    firstOrderSorting = info => info.AssetData?.Description;
                    secondOrderSorting = info => info.AssetData?.Name;
                    break;
                case SortField.PrimaryType:
                    firstOrderSorting = info => info.AssetData?.AssetType;
                    secondOrderSorting = info => info.AssetData?.Name;
                    break;
                case SortField.Status:
                    firstOrderSorting = info => info.AssetData?.Status;
                    secondOrderSorting = info => info.AssetData?.Name;
                    break;
                case SortField.ImportStatus:
                    firstOrderSorting = info =>
                        info.AssetData?.AssetDataAttributeCollection?.GetAttribute<ImportAttribute>()?.Status ??
                        ImportAttribute.ImportStatus.NoImport;
                    secondOrderSorting = info => info.AssetData?.Name;
                    break;
            }

            return m_PageManager.SortingOrder switch
            {
                SortingOrder.Ascending => secondOrderSorting == null
                    ? importedAssets.OrderBy(firstOrderSorting).ToArray()
                    : importedAssets.OrderBy(firstOrderSorting).ThenBy(secondOrderSorting).ToArray(),
                SortingOrder.Descending => secondOrderSorting == null
                    ? importedAssets.OrderByDescending(firstOrderSorting).ToArray()
                    : importedAssets.OrderByDescending(firstOrderSorting).ThenBy(secondOrderSorting).ToArray(),
                _ => importedAssets.ToArray()
            };
        }

        async Task UpdateImportStatusAsync(IEnumerable<ImportedAssetInfo> importedAssets, CancellationToken token)
        {
            var assetDatas = importedAssets.Select(x => x.AssetData).Where(x => x != null);
            var unityConnectProxy = ServicesContainer.instance.Resolve<IUnityConnectProxy>();

            if (unityConnectProxy.AreCloudServicesReachable)
            {
                // Only refresh the status for those that don't have it.
                var results = await m_AssetsProvider.GatherImportStatusesAsync(assetDatas, token);
                foreach (var assetData in assetDatas)
                {
                    if (results.TryGetValue(assetData.Identifier, out var status))
                    {
                        assetData.AssetDataAttributeCollection = new AssetDataAttributeCollection(new ImportAttribute(status));
                    }
                }
            }
        }
    }
}

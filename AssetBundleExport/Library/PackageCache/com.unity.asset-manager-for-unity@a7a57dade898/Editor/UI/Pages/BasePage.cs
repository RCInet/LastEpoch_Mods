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
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace Unity.AssetManager.UI.Editor
{
    [Serializable]
    abstract class BasePage : IPage
    {
        [SerializeField]
        List<AsyncLoadOperation> m_LoadMoreAssetsOperations = new();

        [SerializeField]
        protected bool m_CanLoadMoreItems;

        [SerializeField]
        protected int m_NextStartIndex;

        [SerializeReference]
        protected IPageFilterStrategy m_PageFilterStrategy;

        [SerializeField]
        List<AssetIdentifier> m_SelectedAssets = new();

        [SerializeField]
        bool m_ReTriggerSearchAfterDomainReload;

        [SerializeField]
        UIComponents m_EnabledUIComponents;

        [SerializeReference]
        protected IAssetDataManager m_AssetDataManager;

        [SerializeReference]
        protected IAssetsProvider m_AssetsProvider;

        [SerializeField]
        AssetDataCollection<BaseAssetData> m_AssetList = new();

        [SerializeReference]
        protected IProjectOrganizationProvider m_ProjectOrganizationProvider;

        [SerializeReference]
        protected IPageManager m_PageManager;

        [SerializeReference]
        protected IMessageManager m_MessageManager;

        [SerializeReference]
        protected IDialogManager m_DialogManager;

        protected VisualElement m_CustomUISection;
        protected Dictionary<string, SortField> m_SortField;
        protected IAsyncEnumerator<AssetData> m_SearchRequest;

        public UIComponents EnabledUIComponents => m_EnabledUIComponents;
        public virtual bool SupportsUpdateAll => false;

        public virtual bool DisplaySearchBar => true;
        public virtual bool DisplayBreadcrumbs => false;
        public virtual bool DisplaySideBar => true;
        public virtual bool DisplayFilters => true;
        public virtual bool DisplayFooter => true;
        public virtual bool DisplaySavedViewControls => true;
        public virtual bool DisplaySort => true;
        public virtual bool DisplayUploadMetadata => false;
        public virtual bool DisplayUpdateAllButton => true;
        public virtual bool DisplayTitle => false;
        public virtual string DefaultProjectName => L10n.Tr(Constants.AllAssetsFolderName);
        public virtual string Title => GetPageName();

        internal static readonly Dictionary<SortField, string> s_SortFieldsLabelMap = new()
        {
            {SortField.Name, "Name"},
            {SortField.Updated, "Last Modified"},
            {SortField.Created, "Upload Date"},
            {SortField.Description, "Description"},
            {SortField.Status, "Status"},
            {SortField.ImportStatus, "Import Status"}
        };

        public Dictionary<string, SortField> SortOptions
        {
            get
            {
                if (m_SortField == null)
                {
                    m_SortField = CreateSortField();
                }

                return m_SortField;
            }
        }

        public void EnableUIComponent(UIComponents uiComponents)
        {
            m_EnabledUIComponents |= uiComponents;
            UIComponentEnabledChanged?.Invoke(m_EnabledUIComponents);
        }

        public void DisableUIComponent(UIComponents uiComponents)
        {
            m_EnabledUIComponents &= ~uiComponents;
            UIComponentEnabledChanged?.Invoke(m_EnabledUIComponents);
        }

        public event Action<bool> LoadingStatusChanged;
        public event Action<IReadOnlyCollection<AssetIdentifier>> SelectedAssetsChanged;
        public event Action<IReadOnlyCollection<string>> SearchFiltersChanged;
        public event Action<UIComponents> UIComponentEnabledChanged;

        public bool IsLoading => m_LoadMoreAssetsOperations.Exists(op => op.IsLoading);
        public bool CanLoadMoreItems => m_CanLoadMoreItems;
        public IReadOnlyCollection<AssetIdentifier> SelectedAssets => m_SelectedAssets;
        public bool IsAssetSelected(AssetIdentifier asset) => m_SelectedAssets.Contains(asset);
        public AssetIdentifier LastSelectedAssetId => m_SelectedAssets.LastOrDefault();
        public IReadOnlyCollection<BaseAssetData> AssetList => m_AssetList;

        public static readonly Message MissingSelectedProjectMessage = new(L10n.Tr("Select the destination cloud project for the upload."));

        public static readonly Message ErrorRetrievingAssetsMessage =
            new(L10n.Tr("It seems there was an error while trying to retrieve assets."),
                RecommendedAction.Retry);

        protected BasePage(IAssetDataManager assetDataManager, IAssetsProvider assetsProvider,
            IProjectOrganizationProvider projectOrganizationProvider, IMessageManager messageManager,
            IPageManager pageManager, IDialogManager dialogManager)
        {
            m_AssetDataManager = assetDataManager;
            m_AssetsProvider = assetsProvider;
            m_ProjectOrganizationProvider = projectOrganizationProvider;
            m_PageManager = pageManager;
            m_MessageManager = messageManager;
            m_DialogManager = dialogManager;

            m_CanLoadMoreItems = true;
            m_EnabledUIComponents = UIComponents.All;
        }

        public void SetFilterStrategy(IPageFilterStrategy pageFilterStrategy)
        {
            ClearFilterStrategy();

            m_PageFilterStrategy = pageFilterStrategy;
            m_PageFilterStrategy.SearchFiltersChanged += OnSearchFiltersChanged;
            m_PageFilterStrategy.EnableFilters(false);
        }

        public void ClearFilterStrategy()
        {
            if (m_PageFilterStrategy == null)
                return;

            m_PageFilterStrategy.SearchFiltersChanged -= OnSearchFiltersChanged;
            m_PageFilterStrategy = null;
        }

        public virtual void OnActivated()
        {
            AnalyticsSender.SendEvent(new PageSelectedEvent(GetPageName()));
            Clear(true);
        }

        public virtual void OnDeactivated()
        {
            Clear(false);
        }

        public virtual void OnEnable()
        {
            m_ProjectOrganizationProvider.ProjectSelectionChanged += OnProjectSelectionChanged;
            m_ProjectOrganizationProvider.OrganizationChanged += OnOrganizationChanged;

            m_AssetList.RebuildAssetList(m_AssetDataManager);

            if (m_ReTriggerSearchAfterDomainReload)
            {
                m_ReTriggerSearchAfterDomainReload = false;
                LoadMore();
            }

            TaskUtils.TrackException(RefreshAssetDataAttributesAsync());
        }

        public virtual void OnDisable()
        {
            m_ProjectOrganizationProvider.ProjectSelectionChanged -= OnProjectSelectionChanged;
            m_ProjectOrganizationProvider.OrganizationChanged -= OnOrganizationChanged;

            if (!IsLoading)
                return;

            CancelAndClearLoadMoreOperations();
            m_ReTriggerSearchAfterDomainReload = true;
        }

        protected virtual Dictionary<string, SortField> CreateSortField()
        {
            return new()
            {
                {s_SortFieldsLabelMap[SortField.Name], SortField.Name},
                {s_SortFieldsLabelMap[SortField.Updated], SortField.Updated},
                {s_SortFieldsLabelMap[SortField.Created], SortField.Created},
                {s_SortFieldsLabelMap[SortField.Description], SortField.Description},
                {s_SortFieldsLabelMap[SortField.Status], SortField.Status}
            };
        }

        public void SelectAsset(AssetIdentifier asset, bool additive)
        {
            if (LastSelectedAssetId?.IsIdValid() != true && !asset.IsIdValid())
                return;

            if (!additive)
            {
                m_SelectedAssets.Clear();
            }

            if (m_SelectedAssets.Contains(asset))
            {
                m_SelectedAssets.Remove(asset);
            }
            else
            {
                m_SelectedAssets.Add(asset);
            }

            // Refresh the asset status
            var importedAssetInfo = m_AssetDataManager.GetImportedAssetInfo(asset);
            importedAssetInfo?.AssetData.RefreshAssetDataAttributesAsync();

            SelectedAssetsChanged?.Invoke(m_SelectedAssets);
        }

        public void ClearSelection()
        {
            m_SelectedAssets.Clear();
            SelectedAssetsChanged?.Invoke(m_SelectedAssets);
        }

        public void SelectAssets(IEnumerable<AssetIdentifier> assets)
        {
            m_SelectedAssets.AddRange(assets);
            m_SelectedAssets = m_SelectedAssets.Distinct().ToList();
            SelectedAssetsChanged?.Invoke(m_SelectedAssets);
        }

        public virtual void ToggleAsset(AssetIdentifier assetIdentifier, bool checkState)
        {
            SelectAsset(assetIdentifier, checkState);
        }

        public virtual void LoadMore()
        {
            if (!m_CanLoadMoreItems || IsLoading)
                return;

            var loadMoreAssetsOperation = new AsyncLoadOperation();
            m_LoadMoreAssetsOperations.Add(loadMoreAssetsOperation);
            _ = loadMoreAssetsOperation.Start(LoadMoreAssets,
                () =>
                {
                    m_MessageManager.ClearAllMessages();
                    LoadingStatusChanged?.Invoke(IsLoading);
                },
                cancelledCallback: null,
                exceptionCallback: e =>
                {
                    Debug.LogException(e);
                    SetPageMessage(ErrorRetrievingAssetsMessage);
                },
                successCallback: results =>
                {
                    var assetDatas = results as BaseAssetData[] ?? results.ToArray();

                    m_AssetDataManager.AddOrUpdateAssetDataFromCloudAsset(assetDatas);

                    foreach (var assetData in assetDatas)
                    {
                        var asset = m_AssetList.Find(asset => asset.Identifier.Equals(assetData.Identifier));
                        if (asset != null)
                        {
                            m_AssetList.Replace(asset, assetData);
                        }
                        else
                        {
                            m_AssetList.Add(assetData);
                        }
                    }

                    OnLoadMoreSuccessCallBack();
                    LoadingStatusChanged?.Invoke(IsLoading);
                },
                finallyCallback: () => m_LoadMoreAssetsOperations.Remove(loadMoreAssetsOperation)
            );
        }

        public virtual void Clear(bool reloadImmediately, bool clearSelection = true)
        {
            CancelAndClearLoadMoreOperations();

            m_AssetList.Clear();
            m_CanLoadMoreItems = true;
            m_SearchRequest = null;
            m_NextStartIndex = 0;
            m_MessageManager.ClearAllMessages();

            if (clearSelection)
            {
                ClearSelection();
            }

            if (reloadImmediately)
            {
                LoadMore();
            }
        }

        public VisualElement GetCustomUISection()
        {
            return m_CustomUISection ??= CreateCustomUISection();
        }

        protected internal abstract IAsyncEnumerable<BaseAssetData> LoadMoreAssets(CancellationToken token);
        protected abstract void OnLoadMoreSuccessCallBack();

        protected virtual void OnProjectSelectionChanged(ProjectInfo projectInfo, CollectionInfo collectionInfo)
        {
            if (projectInfo == null)
                return;

            m_PageManager.SetActivePage<CollectionPage>();
        }

        protected async IAsyncEnumerable<BaseAssetData> LoadMoreAssets(CollectionInfo collectionInfo,
            [EnumeratorCancellation] CancellationToken token)
        {
            await foreach (var assetData in LoadMoreAssets(collectionInfo.OrganizationId,
                               new List<string> {collectionInfo.ProjectId}, collectionInfo.GetFullPath(), token))
            {
                yield return assetData;
            }
        }

        protected async IAsyncEnumerable<BaseAssetData> LoadMoreAssets(OrganizationInfo organizationInfo,
            [EnumeratorCancellation] CancellationToken token)
        {
            await foreach (var assetData in LoadMoreAssets(organizationInfo.Id, null, null, token))
            {
                yield return assetData;
            }
        }

        protected void SetPageMessage(Message message)
        {
            m_MessageManager.SetGridViewMessage(message);
        }

        protected void ResetAssetDataAttributes()
        {
            foreach (var assetData in m_AssetDataManager.ImportedAssetInfos.Select(i => i.AssetData))
            {
                assetData.ResetAssetDataAttributes();
            }
        }

        protected async Task RefreshAssetDataAttributesAsync()
        {
            var tasks = m_AssetDataManager.ImportedAssetInfos
                .Select(i => i.AssetData.RefreshAssetDataAttributesAsync()).ToList();

            await TaskUtils.WaitForTasksWithHandleExceptions(tasks);
        }

        void OnOrganizationChanged(OrganizationInfo obj)
        {
            m_PageManager.ActivePage.ClearSelection();
        }

        async IAsyncEnumerable<BaseAssetData> LoadMoreAssets(string organizationId, IEnumerable<string> projectIds,
            string collectionPath, [EnumeratorCancellation] CancellationToken token)
        {
            InitializeSearchRequest(organizationId, projectIds, collectionPath, token);

            var isFaulted = false;

#if AM4U_DEV
            var t = new Stopwatch();
            t.Start();
#endif

            var count = 0;
            while (count < Constants.DefaultPageSize && await MoveNextAsync())
            {
                if (m_SearchRequest == null)
                    break;

                var cloudAssetData = m_SearchRequest.Current;

                // If an asset was imported, we need to display it's state and not the one from the cloud
                var importedAssetInfo = m_AssetDataManager.GetImportedAssetInfo(cloudAssetData?.Identifier);
                var assetData = importedAssetInfo != null ? importedAssetInfo.AssetData : cloudAssetData;

                if (await FilteringUtils.IsDiscardedByLocalFilter(assetData, m_PageFilterStrategy.SelectedLocalFilters, token))
                    continue;

                yield return assetData;

                ++count;
            }

#if AM4U_DEV
            t.Stop();
            var range = $"{m_NextStartIndex + 1}..{m_NextStartIndex + Constants.DefaultPageSize}";
            Utilities.DevLog($"Fetched {count} Assets from {range} in {t.ElapsedMilliseconds} ms");
#endif

            // When a fault occurs, allow the request to try again.
            m_CanLoadMoreItems = isFaulted || count == Constants.DefaultPageSize;
            m_NextStartIndex += count;

            async Task<bool> MoveNextAsync()
            {
                // If the request was cleared, exit the loop
                if (m_SearchRequest == null)
                    return false;

                try
                {
                    return await m_SearchRequest.MoveNextAsync();
                }
                catch (TaskCanceledException)
                {
                    // Ignore
                }
                catch (Exception e)
                {
                    isFaulted = true;
                    m_SearchRequest = null; // Clear the request so it can be tried again
                    Utilities.DevLogException(e);
                }

                return false;
            }
        }

        void InitializeSearchRequest(string organizationId, IEnumerable<string> projectIds, string collectionPath,
            CancellationToken token)
        {
            if (m_SearchRequest == null)
            {
                Utilities.DevLog(m_NextStartIndex > 0
                    ? $"Reviving search request at index {m_NextStartIndex}."
                    : "Initiating new search request.");

                var assetSearchFilter = m_PageFilterStrategy?.AssetSearchFilter;
                if (assetSearchFilter == null)
                {
                    Utilities.DevLogError("AssetSearchFilter is null. Ensure that PageFilterStrategy is initialized before calling LoadMoreAssets.");
                    return;
                }

                assetSearchFilter.Collection = new List<string> {collectionPath};
                assetSearchFilter.Searches = m_PageFilterStrategy.SearchFilters?.ToList();

                m_SearchRequest = m_AssetsProvider.SearchAsync(organizationId, projectIds, assetSearchFilter,
                        m_PageManager.SortField, m_PageManager.SortingOrder, m_NextStartIndex, 0, token)
                    .GetAsyncEnumerator(token);
            }
        }

        void OnSearchFiltersChanged(IReadOnlyCollection<string> searchFilters)
        {
            Clear(true);
            SearchFiltersChanged?.Invoke(searchFilters);
        }

        protected bool HasFilter<T>() where T : LocalFilter
        {
            var type = typeof(T);
            return m_PageFilterStrategy?.SelectedFilters.Any(x => type.IsInstanceOfType(x)) ?? false;
        }
        protected virtual VisualElement CreateCustomUISection()
        {
            return null;
        }

        protected virtual string GetPageName()
        {
            var name = GetType().Name;
            return name.EndsWith("Page") ? name[..^4] : name;
        }

        void CancelAndClearLoadMoreOperations()
        {
            foreach (var operation in m_LoadMoreAssetsOperations.Where(op => op.IsLoading))
            {
                operation.Cancel();
            }

            m_LoadMoreAssetsOperations.Clear();
        }

        protected bool TrySetNoResultsPageMessage()
        {
            // If there are search filters, show a message indicating no results found.
            // This is different from the empty state message which is shown when there are no assets at all.
            var hasSearchFilters = m_PageFilterStrategy.SearchFilters.Any();
            var hasSelectedFilters = m_PageFilterStrategy.SelectedFilters.Any();

            if (!hasSearchFilters && !hasSelectedFilters)
            {
                return false;
            }

            var message = L10n.Tr(Constants.NoResultsText);
            if (hasSearchFilters)
            {
                message += $"{L10n.Tr(Constants.NoResultsForSearchText)}{string.Join(" or ", m_PageFilterStrategy.SearchFilters.Select(f => $"\'{f}\'"))}";
            }

            if (hasSelectedFilters)
            {
                message += $"{L10n.Tr(Constants.NoResultsForFiltersText)}{string.Join(" and ", m_PageFilterStrategy.SelectedFilters.Select(f => f.DisplayName))}";
            }

            SetPageMessage(new Message(message));
            return true;
        }
    }
}

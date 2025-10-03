using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.AssetManager.UI.Editor
{
    interface IPageManager : IService
    {
        IPage ActivePage { get; }
        IPageFilterStrategy PageFilterStrategy { get; }
        SortField SortField { get; }
        SortingOrder SortingOrder { get; }
        bool IsActivePage(IPage page);
        event Action<IPage> ActivePageChanged;
        event Action<IPage, bool> LoadingStatusChanged;
        event Action<IPage, IEnumerable<string>> SearchFiltersChanged;
        event Action<IPage, IEnumerable<AssetIdentifier>> SelectedAssetChanged;
        event Action<IPage, UIComponents> UIComponentEnabledChanged;

        void SetActivePage<T>(bool forceChange = false) where T : IPage;
        void SetSortValues(SortField sortField, SortingOrder sortingOrder);
    }

    [Serializable]
    class PageManager : BaseService<IPageManager>, IPageManager, ISerializationCallbackReceiver
    {
        [SerializeReference]
        IUnityConnectProxy m_UnityConnectProxy;

        [SerializeReference]
        IAssetDataManager m_AssetDataManager;

        [SerializeReference]
        IAssetsProvider m_AssetsProvider;

        [SerializeReference]
        IProjectOrganizationProvider m_ProjectOrganizationProvider;

        [SerializeReference]
        IMessageManager m_MessageManager;

        [SerializeReference]
        IDialogManager m_DialogManager;

        [SerializeReference]
        ISettingsManager m_SettingsManager;

        [SerializeReference]
        IAssetOperationManager m_AssetOperationManager;

        [SerializeReference]
        IPage m_ActivePage;

        [SerializeReference]
        IPageFilterStrategy m_PageFilterStrategy;

        Dictionary<string, PageFilters> m_PageFiltersByType = new ();

        [SerializeField]
        List<string> m_SerializedPageFiltersByTypeKeys = new ();

        [SerializeReference]
        List<PageFilters> m_SerializedPageFiltersByTypeValues = new();

        [SerializeReference]
        ISavedAssetSearchFilterManager m_SavedSearchFilterManager;

        const string k_SortFieldPrefKey = "com.unity.asset-manager-for-unity.sortField";
        const string k_SortingOrderKey = "com.unity.asset-manager-for-unity.sortingOrder";

        public bool IsActivePage(IPage page) => m_ActivePage == page;

        string ActivePageTitle => m_ActivePage?.Title ?? string.Empty;
        string ActivePageSortFieldKey => $"{k_SortFieldPrefKey}_{ActivePageTitle}";
        string ActivePageSortingOrderKey => $"{k_SortingOrderKey}_{ActivePageTitle}";

        public SortField SortField
        {
            get => (SortField)EditorPrefs.GetInt(ActivePageSortFieldKey, (int)SortField.Name);
            set => EditorPrefs.SetInt(ActivePageSortFieldKey, (int)value);
        }

        public SortingOrder SortingOrder
        {
            get => (SortingOrder)EditorPrefs.GetInt(ActivePageSortingOrderKey, (int)SortingOrder.Ascending);
            set => EditorPrefs.SetInt(ActivePageSortingOrderKey, (int)value);
        }

        public event Action<IPage> ActivePageChanged;
        public event Action<IPage, bool> LoadingStatusChanged;
        public event Action<IPage, IEnumerable<string>> SearchFiltersChanged;
        public event Action<IPage, IEnumerable<AssetIdentifier>> SelectedAssetChanged;
        public event Action<IPage, UIComponents> UIComponentEnabledChanged;

        public IPage ActivePage => m_ActivePage;

        public IPageFilterStrategy PageFilterStrategy
        {
            get
            {
                if (m_PageFilterStrategy == null)
                    InitializePageFiltering();

                return m_PageFilterStrategy;
            }
        }

        [ServiceInjection]
        public void Inject(IUnityConnectProxy unityConnectProxy, IAssetsProvider assetsProvider,
            IAssetDataManager assetDataManager, IProjectOrganizationProvider projectOrganizationProvider,
            IAssetOperationManager assetOperationManager, IMessageManager messageManager,
            IDialogManager dialogManager, ISettingsManager settingsManager, ISavedAssetSearchFilterManager savedSearchFilterManager)
        {
            m_UnityConnectProxy = unityConnectProxy;
            m_AssetsProvider = assetsProvider;
            m_AssetDataManager = assetDataManager;
            m_ProjectOrganizationProvider = projectOrganizationProvider;
            m_AssetOperationManager = assetOperationManager;
            m_MessageManager = messageManager;
            m_DialogManager = dialogManager;
            m_SettingsManager = settingsManager;
            m_SavedSearchFilterManager = savedSearchFilterManager;
        }

        public override void OnEnable()
        {
            m_UnityConnectProxy.CloudServicesReachabilityChanged += OnCloudServicesReachabilityChanged;
            m_ProjectOrganizationProvider.OrganizationChanged += OnOrganizationChanged;
            m_SavedSearchFilterManager.FilterSelected += OnFilterSelected;

            m_ActivePage?.OnEnable();
            InitializePageFiltering();
        }

        protected override void ValidateServiceDependencies()
        {
            base.ValidateServiceDependencies();

            m_DialogManager = ServicesContainer.instance.Get<IDialogManager>();
            m_SettingsManager = ServicesContainer.instance.Get<ISettingsManager>();
            m_SavedSearchFilterManager = ServicesContainer.instance.Get<ISavedAssetSearchFilterManager>();
        }

        public override void OnDisable()
        {
            m_UnityConnectProxy.CloudServicesReachabilityChanged -= OnCloudServicesReachabilityChanged;
            m_ProjectOrganizationProvider.OrganizationChanged -= OnOrganizationChanged;
            m_SavedSearchFilterManager.FilterSelected -= OnFilterSelected;

            m_ActivePage?.OnDisable();
        }

        public void SetActivePage<T>(bool forceChange = false) where T : IPage
        {
            if (!forceChange && m_ActivePage is T)
                return;

            m_ActivePage?.OnDeactivated();
            m_ActivePage?.ClearFilterStrategy();
            m_ActivePage?.OnDisable();

            PageFilterStrategy.ClearPageFiltersObject();

            if (typeof(T) == typeof(CollectionPage) &&
                string.IsNullOrEmpty(m_ProjectOrganizationProvider.SelectedProject?.Id))
            {
                m_ActivePage = CreatePage<AllAssetsPage>();
            }
            else
            {
                m_ActivePage = CreatePage<T>();
            }

            PageFilterStrategy.SetPageFiltersObject(m_PageFiltersByType[m_ActivePage.GetType().Name]);

            m_ActivePage?.OnEnable();
            m_ActivePage?.SetFilterStrategy(PageFilterStrategy);
            m_ActivePage?.OnActivated();

            m_AssetOperationManager.ClearFinishedOperations();

            ActivePageChanged?.Invoke(m_ActivePage);
        }

        public void SetSortValues(SortField sortField, SortingOrder sortingOrder)
        {
            if(sortField == SortField && sortingOrder == SortingOrder)
                return;

            SortField = sortField;
            SortingOrder = sortingOrder;

            ActivePage?.Clear(reloadImmediately:true, clearSelection:false);
        }

        void OnCloudServicesReachabilityChanged(bool cloudServicesReachable)
        {
            if (!cloudServicesReachable && m_ActivePage is not InProjectPage)
            {
                SetActivePage<InProjectPage>();
            }
        }

        void RegisterPageEvents(IPage page)
        {
            page.LoadingStatusChanged += loading => LoadingStatusChanged?.Invoke(page, loading);
            page.SelectedAssetsChanged += data => SelectedAssetChanged?.Invoke(page, data);
            page.SearchFiltersChanged += data => SearchFiltersChanged?.Invoke(page, data);
            page.UIComponentEnabledChanged += data => UIComponentEnabledChanged?.Invoke(page, data);
        }

        IPage CreatePage<T>()
        {
            var page = (IPage) Activator.CreateInstance(typeof(T), m_AssetDataManager, m_AssetsProvider,
                m_ProjectOrganizationProvider, m_MessageManager, this, m_DialogManager);
            RegisterPageEvents(page);
            return page;
        }

        void InitializePageFiltering()
        {
            if (m_PageFilterStrategy != null)
                return;

            m_PageFilterStrategy = new PageFilterStrategy(this, m_ProjectOrganizationProvider, m_AssetsProvider);
            CreatePageFiltersByType();
        }

        void CreatePageFiltersByType()
        {
            var pageFiltersFactory = new PageFiltersFactory(PageFilterStrategy, m_ProjectOrganizationProvider, m_AssetDataManager);

            var collectionPageFilters = pageFiltersFactory.CreateCollectionPageFilters();
            var inProjectPageFilters = pageFiltersFactory.CreateInProjectPageFilters();
            var uploadPageFilters = pageFiltersFactory.CreateUploadPageFilters();

            m_PageFiltersByType[nameof(CollectionPage)] = collectionPageFilters;
            m_PageFiltersByType[nameof(AllAssetsPage)] = collectionPageFilters;
            m_PageFiltersByType[nameof(InProjectPage)] = inProjectPageFilters;
            m_PageFiltersByType[nameof(UploadPage)] = uploadPageFilters;
        }

        public void OnBeforeSerialize()
        {
            m_SerializedPageFiltersByTypeKeys.Clear();
            m_SerializedPageFiltersByTypeValues.Clear();

            if (m_PageFiltersByType == null)
                return;

            foreach (var kvp in m_PageFiltersByType)
            {
                m_SerializedPageFiltersByTypeKeys.Add(kvp.Key);
                m_SerializedPageFiltersByTypeValues.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            try
            {
                m_PageFiltersByType.Clear();
                for (var i = 0; i < m_SerializedPageFiltersByTypeKeys.Count; i++)
                {
                    m_PageFiltersByType[m_SerializedPageFiltersByTypeKeys[i]] = m_SerializedPageFiltersByTypeValues[i];
                }
            }
            catch (Exception e)
            {
                Utilities.DevLogException(e);
            }

            if (m_ActivePage != null)
            {
                RegisterPageEvents(m_ActivePage);

                PageFilterStrategy.SetPageFiltersObject(m_PageFiltersByType[m_ActivePage.GetType().Name]);
                m_ActivePage.SetFilterStrategy(PageFilterStrategy);
                PageFilterStrategy.EnableFilters();
            }
        }

        void OnOrganizationChanged(OrganizationInfo _)
        {
            CreatePageFiltersByType();

            if (m_ActivePage != null)
            {
                PageFilterStrategy.SetPageFiltersObject(m_PageFiltersByType[m_ActivePage.GetType().Name]);
            }
        }

        public void OnFilterSelected(SavedAssetSearchFilter savedAssetSearchFilter, bool applyFilter)
        {
            if (savedAssetSearchFilter == null)
            {
                m_PageFilterStrategy.ClearFilters();
                return;
            }

            // In the case where we're first saving a filter, it will already be applied so we don't need to apply it again.
            // It causes a short loading sequence when re-applied.
            if (applyFilter)
                m_PageFilterStrategy.ApplyFilterFromAssetSearchFilter(savedAssetSearchFilter.AssetSearchFilter.Clone());
        }
    }
}

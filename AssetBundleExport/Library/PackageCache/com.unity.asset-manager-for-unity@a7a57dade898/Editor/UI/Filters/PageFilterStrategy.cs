using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    interface IPageFilterStrategy
    {
        List<string> SearchFilters { get; }
        List<BaseFilter> SelectedFilters { get; }
        IEnumerable<LocalFilter> SelectedLocalFilters { get; }
        AssetSearchFilter AssetSearchFilter { get; }
        bool HasCustomMetadataFilters { get; }

        event Action<IReadOnlyCollection<string>> SearchFiltersChanged;
        event Action<bool> EnableStatusChanged;
        event Action<BaseFilter, bool> FilterAdded;
        event Action<BaseFilter> FilterApplied;
        event Action FiltersCleared;
        event Action SavedFilterApplied;

        void SetPageFiltersObject(PageFilters pageFilters);
        void ClearPageFiltersObject();

        Task<string> GetUserIdAsync(string userName);
        Task<List<FilterSelection>> GetUserNamesAsync(IEnumerable<string> userIds);
        Task<List<FilterSelection>> GetUserNamesAsync(IEnumerable<FilterSelection> userIds);
        Task<List<FilterSelection>> GetFilterSelectionsAsync(AssetSearchGroupBy groupBy);
        Task<List<FilterSelection>> GetFilterSelectionsAsync(string metadataField);

        void AddSearchFilter(IEnumerable<string> searchFiltersArg);
        void RemoveSearchFilter(string searchFilter);
        void ClearSearchFilters();
        void AddFilter(BaseFilter filter, bool showSelection);
        void RemoveFilter(BaseFilter filter);
        Task ApplyFilter(Type filterType, List<string> selectedFilters);
        void ApplyFilter(BaseFilter filter, List<string> selectedFilters);
        void EnableFilters(bool enable = true);
        bool IsAvailableFilters();
        void ApplyFilterFromAssetSearchFilter(AssetSearchFilter assetSearchFilter);
        List<BaseFilter> GetAvailablePrimaryMetadataFilters();
        List<CustomMetadataFilter> GetAvailableCustomMetadataFilters();
        void ClearFilters();
        void Cancel();
        void SetDirty();
        string ConvertAssetTypeFromLegacy(string assetTypeText);
        string ToString(AssetType assetType);
    }

    [Serializable]
    class PageFilterStrategy : IPageFilterStrategy
    {
        static readonly string k_UnknownUser = "<Unknown User>";

        [SerializeField]
        protected PageFilters m_PageFilters;

        [SerializeReference]
        protected IPageManager m_PageManager;

        [SerializeReference]
        protected IProjectOrganizationProvider m_ProjectOrganizationProvider;

        [SerializeReference]
        protected IAssetsProvider m_AssetsProvider;

        protected CancellationTokenSource m_TokenSource;

        string OrganizationId => m_ProjectOrganizationProvider.SelectedOrganization.Id;
        List<string> ProjectIds => GetProjectIds(m_PageManager.ActivePage);

        public List<string> SearchFilters => m_PageFilters.SearchFilters;
        public List<BaseFilter> SelectedFilters => m_PageFilters.SelectedFilters;
        public IEnumerable<LocalFilter> SelectedLocalFilters => m_PageFilters.SelectedLocalFilters;
        public AssetSearchFilter AssetSearchFilter => m_PageFilters.AssetSearchFilter;
        public bool HasCustomMetadataFilters => m_PageFilters.CustomMetadataFilters.Any();

        public event Action<IReadOnlyCollection<string>> SearchFiltersChanged;
        public event Action<bool> EnableStatusChanged;
        public event Action<BaseFilter, bool> FilterAdded;
        public event Action<BaseFilter> FilterApplied;
        public event Action FiltersCleared;
        public event Action SavedFilterApplied;

        public PageFilterStrategy(IPageManager pageManager, IProjectOrganizationProvider projectOrganizationProvider, IAssetsProvider assetsProvider, PageFilters pageFilters = null)
        {
            m_PageManager = pageManager;
            m_ProjectOrganizationProvider = projectOrganizationProvider;
            m_AssetsProvider = assetsProvider;

            m_PageFilters = new PageFilters();

            if (pageFilters != null)
                SetPageFiltersObject(pageFilters);
        }

        ~PageFilterStrategy()
        {
            ClearPageFiltersObject();
        }

        public void SetPageFiltersObject(PageFilters pageFilters)
        {
            ClearPageFiltersObject();

            m_PageFilters = pageFilters;
            m_PageFilters.SearchFiltersChanged += OnSearchFiltersChanged;
            m_PageFilters.EnableStatusChanged += OnEnableStatusChanged;
            m_PageFilters.FilterAdded += OnFilterAdded;
            m_PageFilters.FilterApplied += OnFilterApplied;
            m_PageFilters.ClearPageFilters += OnFiltersCleared;
            m_PageFilters.SetDirty();
        }

        public void ClearPageFiltersObject()
        {
            if (m_PageFilters == null)
                return;

            m_PageFilters.SearchFiltersChanged -= OnSearchFiltersChanged;
            m_PageFilters.EnableStatusChanged -= OnEnableStatusChanged;
            m_PageFilters.FilterAdded -= OnFilterAdded;
            m_PageFilters.FilterApplied -= OnFilterApplied;
            m_PageFilters.ClearPageFilters -= OnFiltersCleared;
            m_PageFilters = null;
        }

        public async Task<string> GetUserIdAsync(string userName)
        {
            return await m_ProjectOrganizationProvider.SelectedOrganization.GetUserIdAsync(userName);
        }

        public async Task<List<FilterSelection>> GetUserNamesAsync(IEnumerable<string> userIds)
        {
            var selections = new List<FilterSelection>();
            foreach (var userId in userIds)
            {
                var name = await m_ProjectOrganizationProvider.SelectedOrganization.GetUserNameAsync(userId);
                if (string.IsNullOrEmpty(name))
                {
                    // Fallback in case the user ID does not resolve to a name
                    // We know that Service Account ids are GUIDs, so we can use that to identify them
                    name = Guid.TryParse(userId, out _) ? "Service Account" : k_UnknownUser;
                }

                selections.Add(new FilterSelection(name, userId));
            }

            selections.Sort(Compare);

            return selections;
        }

        public async Task<List<FilterSelection>> GetUserNamesAsync(IEnumerable<FilterSelection> userIds)
        {
            return await GetUserNamesAsync(userIds.Select(x => x.Text));
        }

        public async Task<List<FilterSelection>> GetFilterSelectionsAsync(AssetSearchGroupBy groupBy)
        {
            m_TokenSource ??= new CancellationTokenSource();

            var results = await m_AssetsProvider.GetFilterSelectionsAsync(
                OrganizationId,
                ProjectIds,
                AssetSearchFilter,
                groupBy,
                m_TokenSource.Token);

            results.Sort();

            switch (groupBy)
            {
                case AssetSearchGroupBy.Type:
                    return results.Select(s => new FilterSelection(s, ParseAssetType(s).GetToolTip())).ToList();
                default:
                    return results.Select(s => new FilterSelection(s)).ToList();
            }
        }

        public async Task<List<FilterSelection>> GetFilterSelectionsAsync(string metadataField)
        {
            m_TokenSource ??= new CancellationTokenSource();

            var results = await m_AssetsProvider.GetFilterSelectionsAsync(
                OrganizationId,
                ProjectIds,
                AssetSearchFilter,
                metadataField,
                m_TokenSource.Token);

            results.Sort();

            return results.Select(s => new FilterSelection(s)).ToList();
        }

        public void AddSearchFilter(IEnumerable<string> searchFiltersArg)
        {
            m_PageFilters?.AddSearchFilter(searchFiltersArg);
        }

        public void RemoveSearchFilter(string searchFilter)
        {
            m_PageFilters?.RemoveSearchFilter(searchFilter);
        }

        public void ClearSearchFilters()
        {
            m_PageFilters?.ClearSearchFilters();
        }

        public void AddFilter(BaseFilter filter, bool showSelection)
        {
            m_PageFilters?.AddFilter(filter, showSelection);
        }

        public void RemoveFilter(BaseFilter filter)
        {
            m_PageFilters?.RemoveFilter(filter);
        }

        public Task ApplyFilter(Type filterType, List<string> selectedFilters)
        {
            return m_PageFilters?.ApplyFilter(filterType, selectedFilters) ?? Task.CompletedTask;
        }

        public void ApplyFilter(BaseFilter filter, List<string> selectedFilters)
        {
            m_PageFilters?.ApplyFilter(filter, selectedFilters);
        }

        public void EnableFilters(bool enable = true)
        {
            m_PageFilters?.EnableFilters(enable);
        }

        public bool IsAvailableFilters()
        {
            return m_PageFilters?.IsAvailableFilters() ?? false;
        }

        public void ApplyFilterFromAssetSearchFilter(AssetSearchFilter assetSearchFilter)
        {
            m_PageFilters.ApplyFiltersFromAssetSearchFilter(assetSearchFilter);
            SavedFilterApplied?.Invoke();

            OnFiltersCleared(true);
        }

        public List<BaseFilter> GetAvailablePrimaryMetadataFilters()
        {
            return m_PageFilters?.GetAvailablePrimaryMetadataFilters();
        }

        public List<CustomMetadataFilter> GetAvailableCustomMetadataFilters()
        {
            return m_PageFilters?.GetAvailableCustomMetadataFilters();
        }

        public void ClearFilters()
        {
            m_PageFilters?.ClearFilters();
        }

        public void Cancel()
        {
            if (m_TokenSource != null)
            {
                m_TokenSource.Cancel();
                m_TokenSource.Dispose();
                m_TokenSource = null;
            }

            foreach (var selectedFilter in SelectedFilters)
            {
                selectedFilter.Cancel();
            }
        }

        public void SetDirty()
        {
            m_PageFilters?.SetDirty();
        }

        public string ConvertAssetTypeFromLegacy(string assetTypeText)
        {
            return assetTypeText switch
            {
                "Texture" => m_AssetsProvider.GetValueAsString(AssetType.Asset2D),
                "Mesh" => m_AssetsProvider.GetValueAsString(AssetType.Model3D),
                "Audio Clip" => m_AssetsProvider.GetValueAsString(AssetType.Audio),
                "Animation Clip" => m_AssetsProvider.GetValueAsString(AssetType.Animation),
                _ => assetTypeText
            };
        }

        public string ToString(AssetType assetType) => m_AssetsProvider.GetValueAsString(assetType);

        AssetType ParseAssetType(string assetTypeText) => m_AssetsProvider.TryParse(assetTypeText, out AssetType assetType) ? assetType : AssetType.Other;

        List<string> GetProjectIds(IPage activePage)
        {
            List<string> projects;

            if (activePage is AllAssetsPage)
            {
                projects = m_ProjectOrganizationProvider.SelectedOrganization.ProjectInfos.Select(p => p.Id)
                    .ToList();
            }
            else
            {
                projects = new List<string> {m_ProjectOrganizationProvider.SelectedProject.Id};
            }

            return projects;
        }

        void OnSearchFiltersChanged(IReadOnlyCollection<string> searchFilters)
            => SearchFiltersChanged?.Invoke(searchFilters);

        void OnEnableStatusChanged(bool enableStatus)
            => EnableStatusChanged?.Invoke(enableStatus);

        void OnFilterAdded(BaseFilter filter, bool showSelection)
            => FilterAdded?.Invoke(filter, showSelection);

        void OnFilterApplied(BaseFilter filter)
            => FilterApplied?.Invoke(filter);

        void OnFiltersCleared(bool reloadImmediately)
        {
            if (reloadImmediately)
            {
                var activePage = m_PageManager?.ActivePage;
                activePage?.Clear(true);
            }

            FiltersCleared?.Invoke();
        }

        static int Compare(FilterSelection a, FilterSelection b)
        {
            if (a.Text == k_UnknownUser && b.Text != k_UnknownUser)
                return 1; // Unknown user goes to the end
            if (b.Text == k_UnknownUser && a.Text != k_UnknownUser)
                return -1; // Unknown user goes to the end
            return a.CompareTo(b);
        }
    }
}

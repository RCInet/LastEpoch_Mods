using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    interface ISavedAssetSearchFilterManager : IService
    {
        IReadOnlyCollection<SavedAssetSearchFilter> Filters { get; }
        SavedAssetSearchFilter SelectedFilter { get; }

        void SelectFilter(string filterId, bool applyFilter = true);
        void SelectFilter(SavedAssetSearchFilter targetFilter, bool applyFilter = true);
        void ClearSelectedFilter();
        void SaveNewFilter(string filterName, AssetSearchFilter filter, SortingOptions sortingOptions, bool selectFilter = true);
        void RenameFilter(SavedAssetSearchFilter targetFilter, string filterName, bool selectFilter = true);
        void UpdateFilter(SavedAssetSearchFilter targetFilter, AssetSearchFilter filter, SortingOptions sortingOptions, string filterName = null, bool selectFilter = true);
        void DeleteFilter(SavedAssetSearchFilter targetFilter);

        event Action <SavedAssetSearchFilter, bool> FilterSelected;
        event Action <SavedAssetSearchFilter> FilterAdded;
        event Action <SavedAssetSearchFilter> FilterUpdated;
        event Action <SavedAssetSearchFilter> FilterDeleted;
    }

    [Serializable]
    class SavedAssetSearchFilterManager : BaseService<ISavedAssetSearchFilterManager>, ISavedAssetSearchFilterManager
    {
        [SerializeReference]
        IProjectOrganizationProvider m_ProjectOrganizationProvider;

        [SerializeReference]
        ISettingsManager m_SettingsManager;

        [SerializeReference]
        SavedAssetSearchFilter m_SelectedFilter;
        public SavedAssetSearchFilter SelectedFilter => m_SelectedFilter;

        ISavedAssetSearchFilterSettings m_FilterSettings;
        ISavedAssetSearchFilterSettings FilterSettings => m_FilterSettings ??= m_SettingsManager.SavedAssetSearchFilterSettings;

        public IReadOnlyCollection<SavedAssetSearchFilter> Filters => FilterSettings?.SavedAssetSearchFilters;

        public event Action<SavedAssetSearchFilter, bool> FilterSelected;
        public event Action<SavedAssetSearchFilter> FilterAdded;
        public event Action<SavedAssetSearchFilter> FilterUpdated;
        public event Action<SavedAssetSearchFilter> FilterDeleted;

        public SavedAssetSearchFilterManager()
        { }

        internal SavedAssetSearchFilterManager(IProjectOrganizationProvider projectOrganizationProvider,
            ISettingsManager settingsManager, ISavedAssetSearchFilterSettings filterSettings = null)
        {
            m_ProjectOrganizationProvider = projectOrganizationProvider;
            m_SettingsManager = settingsManager;
            m_FilterSettings = filterSettings;
        }

        [ServiceInjection]
        public void Inject(IProjectOrganizationProvider projectOrganizationProvider,
            ISettingsManager settingsManager)
        {
            m_ProjectOrganizationProvider = projectOrganizationProvider;
            m_SettingsManager = settingsManager;
        }

        public void SelectFilter(string filterId, bool applyFilter = true)
        {
            SelectFilter(filterId, applyFilter, true);
        }

        public void SelectFilter(SavedAssetSearchFilter targetFilter, bool applyFilter = true)
        {
            SelectFilter(targetFilter, applyFilter, true);
        }

        void SelectFilter(string filterId, bool applyFilter, bool sendEvent)
        {
            var targetFilter = FilterSettings?.GetFilter(filterId);
            SelectFilter(targetFilter, applyFilter, sendEvent);
        }

        void SelectFilter(SavedAssetSearchFilter targetFilter, bool applyFilter, bool sendEvent)
        {
            m_SelectedFilter = targetFilter;
            FilterSelected?.Invoke(targetFilter, applyFilter);

            // We don't want to send a selection analytics event in situations where
            // the filter is already selected for modification purposes.
            if (sendEvent)
                AnalyticsSender.SendEvent(new SavedViewAppliedEvent(targetFilter != null));
        }

        public void ClearSelectedFilter()
        {
            m_SelectedFilter = null;
            FilterSelected?.Invoke(null, false);

            AnalyticsSender.SendEvent(new SavedViewAppliedEvent(false));
        }

        public void SaveNewFilter(string filterName, AssetSearchFilter filter, SortingOptions sortingOptions, bool selectFilter = true)
        {
            var newFilter = new SavedAssetSearchFilter(m_ProjectOrganizationProvider?.SelectedOrganization.Id, filterName, filter.Clone(), sortingOptions);
            FilterSettings.AddFilter(newFilter);
            FilterAdded?.Invoke(newFilter);
            if (selectFilter)
                SelectFilter(newFilter, false, false);

            AnalyticsSender.SendEvent(new SavedViewCreatedEvent());
        }

        public void RenameFilter(SavedAssetSearchFilter targetFilter, string filterName, bool selectFilter = true)
        {
            FilterSettings.RenameFilter(targetFilter, filterName);
            FilterUpdated?.Invoke(targetFilter);
            if (selectFilter)
                SelectFilter(targetFilter, false, false);

            AnalyticsSender.SendEvent(new SavedViewModifiedEvent(SavedViewModifiedEvent.ModificationType.Rename));
        }

        public void UpdateFilter(SavedAssetSearchFilter targetFilter, AssetSearchFilter filter, SortingOptions sortingOptions, string filterName = null, bool selectFilter = true)
        {
            FilterSettings.UpdateFilter(targetFilter, filter.Clone(), sortingOptions, filterName);
            FilterUpdated?.Invoke(targetFilter);
            if (selectFilter)
                SelectFilter(targetFilter, false, false);

            AnalyticsSender.SendEvent(new SavedViewModifiedEvent(SavedViewModifiedEvent.ModificationType.Update));
        }

        public void DeleteFilter(SavedAssetSearchFilter targetFilter)
        {
            if (m_SelectedFilter == targetFilter)
                ClearSelectedFilter();
            FilterSettings.RemoveFilter(targetFilter);

            FilterDeleted?.Invoke(targetFilter);

            AnalyticsSender.SendEvent(new SavedViewModifiedEvent(SavedViewModifiedEvent.ModificationType.Delete));
        }
    }
}

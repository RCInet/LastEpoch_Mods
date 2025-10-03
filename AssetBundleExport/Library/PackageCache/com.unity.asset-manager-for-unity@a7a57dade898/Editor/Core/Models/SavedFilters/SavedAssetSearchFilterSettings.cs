using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    interface ISavedAssetSearchFilterSettings
    {
        IReadOnlyCollection<SavedAssetSearchFilter> SavedAssetSearchFilters { get; }
        void AddFilter(SavedAssetSearchFilter filter);
        void RenameFilter(SavedAssetSearchFilter savedFilter, string filterName);
        void UpdateFilter(SavedAssetSearchFilter savedFilter, AssetSearchFilter filter, SortingOptions sortingOptions, string filterName = null);
        void RemoveFilter(SavedAssetSearchFilter savedFilter);
        SavedAssetSearchFilter GetFilter(string filterId);
    }

    [Serializable]
    abstract class BaseSavedAssetSearchFilterSettings : ISavedAssetSearchFilterSettings
    {
        [SerializeField]
        List<SavedAssetSearchFilter> m_SavedAssetSearchFilters = new ();

        public IReadOnlyCollection<SavedAssetSearchFilter> SavedAssetSearchFilters => m_SavedAssetSearchFilters.AsReadOnly();

        protected abstract void Save();



        public void AddFilter(SavedAssetSearchFilter filter)
        {
            m_SavedAssetSearchFilters.Add(filter);
            Save();
        }

        public void RenameFilter(SavedAssetSearchFilter savedFilter, string filterName)
        {
            var filterToRename = GetFilter(savedFilter.FilterId);
            if (filterToRename == null)
                return;

            filterToRename.SetFilterName(filterName);
            Save();
        }

        public void UpdateFilter(SavedAssetSearchFilter savedFilter, AssetSearchFilter filter, SortingOptions sortingOptions, string filterName = null)
        {
            var filterToUpdate = GetFilter(savedFilter.FilterId);
            if (filterToUpdate == null)
                return;

            if (filterName != null)
                filterToUpdate.SetFilterName(filterName);
            filterToUpdate.SetAssetSearchFilter(filter);
            filterToUpdate.SetSortingOptions(sortingOptions);
            Save();
        }

        public void RemoveFilter(SavedAssetSearchFilter savedFilter)
        {
            var filterToRemove = GetFilter(savedFilter.FilterId);
            if (filterToRemove == null)
                return;

            m_SavedAssetSearchFilters.Remove(filterToRemove);
            Save();
        }

        public SavedAssetSearchFilter GetFilter(string filterId)
        {
            var filter = m_SavedAssetSearchFilters.Find(f => f.FilterId == filterId);
            if (filter == null)
            {
                Utilities.DevLogWarning($"Filter with ID {filterId} not found.");
                return null;
            }

            return filter;
        }
    }

    [Serializable]
    class SavedAssetSearchFilterSettings : BaseSavedAssetSearchFilterSettings
    {
        const string k_SavedAssetSearchFilterSettingsKey = "AM4U.savedAssetSearchFilterSettings";

        public static SavedAssetSearchFilterSettings Load(UnityEditor.SettingsManagement.Settings settings)
            => settings.Get(k_SavedAssetSearchFilterSettingsKey, SettingsScope.Project, new SavedAssetSearchFilterSettings());

        protected override void Save()
        {
            var settings = new UnityEditor.SettingsManagement.Settings(AssetManagerCoreConstants.PackageName);
            settings.Set(k_SavedAssetSearchFilterSettingsKey, this, SettingsScope.Project);
            settings.Save();
        }
    }
}

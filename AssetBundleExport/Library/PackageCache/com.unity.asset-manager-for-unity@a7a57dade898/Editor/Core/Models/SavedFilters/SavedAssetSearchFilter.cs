using System;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    [Serializable]
    struct SortingOptions
    {
        public SortField SortField;
        public SortingOrder SortingOrder;

        public SortingOptions(SortField sortField, SortingOrder sortingOrder)
        {
            SortField = sortField;
            SortingOrder = sortingOrder;
        }
    }

    [Serializable]
    class SavedAssetSearchFilter
    {
        [SerializeField]
        string m_FilterId;

        [SerializeField]
        string m_OrganizationId;

        [SerializeField]
        string m_FilterName;

        [SerializeField]
        AssetSearchFilter m_AssetSearchFilter;

        [SerializeField]
        SortingOptions m_SortingOptions;

        public string FilterId => m_FilterId;

        public string OrganizationId => m_OrganizationId;
        public string FilterName => m_FilterName;
        public AssetSearchFilter AssetSearchFilter => m_AssetSearchFilter;
        public SortingOptions SortingOptions => m_SortingOptions;

        public SavedAssetSearchFilter(string organizationId, string filterName, AssetSearchFilter assetSearchFilter, SortingOptions sortingOptions)
        {
            m_FilterId = Guid.NewGuid().ToString();

            m_OrganizationId = organizationId;
            m_FilterName = filterName;
            m_AssetSearchFilter = assetSearchFilter;
            m_SortingOptions = sortingOptions;
        }

        public void SetFilterName(string filterName)
        {
            m_FilterName = filterName;
        }

        public void SetAssetSearchFilter(AssetSearchFilter assetSearchFilter)
        {
            m_AssetSearchFilter = assetSearchFilter;
        }

        public void SetSortingOptions(SortingOptions sortingOptions)
        {
            m_SortingOptions = sortingOptions;
        }
    }
}

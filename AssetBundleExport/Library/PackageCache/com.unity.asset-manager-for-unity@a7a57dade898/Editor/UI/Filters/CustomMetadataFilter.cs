using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    [Serializable]
    abstract class CustomMetadataFilter : CloudFilter
    {
        [SerializeReference]
        IMetadata m_Metadata;

        public override string DisplayName => m_Metadata.Name;

        protected CustomMetadataFilter(IPageFilterStrategy pageFilterStrategy, IMetadata metadata)
            : base(pageFilterStrategy)
        {
            m_Metadata = metadata;
        }
        protected override AssetSearchGroupBy GroupBy => AssetSearchGroupBy.Name;
        public override void ResetSelectedFilter(AssetSearchFilter assetSearchFilter)
        {
            assetSearchFilter.CustomMetadata ??= new List<IMetadata>();

            // Remove existing metadata with the same key to avoid duplicates
            assetSearchFilter.CustomMetadata.RemoveAll(m => m.FieldKey == m_Metadata.FieldKey);
            assetSearchFilter.CustomMetadata.Add(m_Metadata);
        }

        protected override void IncludeFilter(List<string> selectedFilters)
        {
            ResetSelectedFilter(m_PageFilterStrategy.AssetSearchFilter);
        }

        protected override void ClearFilter()
        {
            m_PageFilterStrategy.AssetSearchFilter.CustomMetadata?.RemoveAll(m => m.FieldKey == m_Metadata.FieldKey);
        }

        protected override async Task<List<FilterSelection>> GetFilterSelectionsAsync()
        {
            return await m_PageFilterStrategy.GetFilterSelectionsAsync(m_Metadata.FieldKey);
        }
    }
}

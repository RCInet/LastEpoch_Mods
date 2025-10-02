using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;

namespace Unity.AssetManager.UI.Editor
{
    class PageFiltersFactory
    {
        IPageFilterStrategy m_PageFilterStrategy;
        IProjectOrganizationProvider m_ProjectOrganizationProvider;
        IAssetDataManager m_AssetDataManager;

        internal PageFiltersFactory(IPageFilterStrategy pageFilterStrategy, IProjectOrganizationProvider projectOrganizationProvider, IAssetDataManager assetDataManager)
        {
            m_PageFilterStrategy = pageFilterStrategy;
            m_ProjectOrganizationProvider = projectOrganizationProvider;
            m_AssetDataManager = assetDataManager;
        }

        static PageFilters CreateAndInitializePageFilters(Func<List<BaseFilter>> createPrimaryMetadataFilters, Func<List<CustomMetadataFilter>> createCustomMetadataFilters)
        {
            var pageFilters = new PageFilters();
            var primaryMetadataFilters = createPrimaryMetadataFilters();
            var customMetadataFilters = createCustomMetadataFilters();
            pageFilters.Initialize(primaryMetadataFilters, customMetadataFilters);

            return pageFilters;
        }

        internal PageFilters CreateCollectionPageFilters()
        {
            return CreateAndInitializePageFilters(CreatePrimaryMetadataFilters, CreateCustomMetadataFilters);

            List<BaseFilter> CreatePrimaryMetadataFilters()
            {
                return new List<BaseFilter>
                {
                    new StatusFilter(m_PageFilterStrategy),
                    new UnityTypeFilter(m_PageFilterStrategy),
                    new CreatedByFilter(m_PageFilterStrategy),
                    new UpdatedByFilter(m_PageFilterStrategy)
                }.OrderBy(f => f.DisplayName).ToList();
            }

            List<CustomMetadataFilter> CreateCustomMetadataFilters()
            {
                var metadataFilters = new List<CustomMetadataFilter>();

                var organizationInfo = m_ProjectOrganizationProvider.SelectedOrganization;
                if (organizationInfo == null)
                {
                    return metadataFilters;
                }

                foreach (var metadataField in organizationInfo.MetadataFieldDefinitions)
                {
                    CustomMetadataFilter metadataFilter = null;

                    switch (metadataField.Type)
                    {
                        case MetadataFieldType.Text:
                            metadataFilter = new TextMetadataFilter(m_PageFilterStrategy, new TextMetadata(metadataField.Key, metadataField.DisplayName, string.Empty));
                            break;
                        case MetadataFieldType.Boolean:
                            metadataFilter = new BooleanMetadataFilter(m_PageFilterStrategy, new BooleanMetadata(metadataField.Key, metadataField.DisplayName, false));
                            break;
                        case MetadataFieldType.Number:
                            metadataFilter = new NumberMetadataFilter(m_PageFilterStrategy, new NumberMetadata(metadataField.Key, metadataField.DisplayName, 0));
                            break;
                        case MetadataFieldType.Url:
                            metadataFilter = new UrlMetadataFilter(m_PageFilterStrategy, new UrlMetadata(metadataField.Key, metadataField.DisplayName, new UriEntry(default, string.Empty)));
                            break;
                        case MetadataFieldType.Timestamp:
                            metadataFilter = new TimestampMetadataFilter(m_PageFilterStrategy, new TimestampMetadata(metadataField.Key, metadataField.DisplayName, new DateTimeEntry(DateTime.Today)));
                            break;
                        case MetadataFieldType.User:
                            metadataFilter = new UserMetadataFilter(m_PageFilterStrategy, new UserMetadata(metadataField.Key, metadataField.DisplayName, string.Empty));
                            break;
                        case MetadataFieldType.SingleSelection:
                            metadataFilter = new SingleSelectionMetadataFilter(m_PageFilterStrategy, new SingleSelectionMetadata(metadataField.Key, metadataField.DisplayName, string.Empty));
                            break;
                        case MetadataFieldType.MultiSelection:
                            metadataFilter = new MultiSelectionMetadataFilter(m_PageFilterStrategy, new MultiSelectionMetadata(metadataField.Key, metadataField.DisplayName, null));
                            break;
                    }

                    metadataFilters.Add(metadataFilter);
                }

                return metadataFilters.OrderBy(f => f.DisplayName).ToList();
            }
        }

        internal PageFilters CreateInProjectPageFilters()
        {
            return CreateAndInitializePageFilters(CreatePrimaryMetadataFilters, CreateCustomMetadataFilters);

            List<BaseFilter> CreatePrimaryMetadataFilters()
            {
                return new List<BaseFilter>
                {
                    new LocalImportStatusFilter(m_PageFilterStrategy),
                    new LocalStatusFilter(m_PageFilterStrategy, m_AssetDataManager),
                    new LocalUnityTypeFilter(m_PageFilterStrategy, m_AssetDataManager)
                };
            }

            List<CustomMetadataFilter> CreateCustomMetadataFilters()
            {
                return new List<CustomMetadataFilter>();
            }
        }

        internal PageFilters CreateUploadPageFilters()
        {
            return CreateAndInitializePageFilters(CreatePrimaryMetadataFilters, CreateCustomMetadataFilters);

            List<BaseFilter> CreatePrimaryMetadataFilters()
            {
                return new List<BaseFilter>();
            }

            List<CustomMetadataFilter> CreateCustomMetadataFilters()
            {
                return new List<CustomMetadataFilter>();
            }
        }
    }
}

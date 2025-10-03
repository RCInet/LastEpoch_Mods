using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.AssetsEmbedded
{
    static class ServiceExtensions
    {
        public static SearchRequestFilter From(this IAssetSearchFilter assetSearchFilter)
        {
            assetSearchFilter ??= new AssetSearchFilter();

            var anyQuery = assetSearchFilter.AccumulateAnyCriteria();

            return new SearchRequestFilter(assetSearchFilter.AccumulateIncludedCriteria(),
                assetSearchFilter.AccumulateExcludedCriteria(),
                anyQuery.criteria,
                anyQuery.criteria is {Count: > 0} ? anyQuery.minimumMatches : null,
                assetSearchFilter.Collections.GetValue());
        }

        public static async Task<MetadataValueType> GetMetadataValueTypeAsync(this IAssetDataSource dataSource, FieldDefinitionDescriptor fieldDefinitionDescriptor, CancellationToken cancellationToken)
        {
            try
            {
                var fieldDefinition = await dataSource.GetFieldDefinitionAsync(fieldDefinitionDescriptor, cancellationToken);
                var multiSelection = fieldDefinition.Multiselection ?? false;
                return fieldDefinition.Type switch
                {
                    FieldDefinitionType.Boolean => MetadataValueType.Boolean,
                    FieldDefinitionType.Number => MetadataValueType.Number,
                    FieldDefinitionType.Text => MetadataValueType.Text,
                    FieldDefinitionType.Timestamp => MetadataValueType.Timestamp,
                    FieldDefinitionType.Url => MetadataValueType.Url,
                    FieldDefinitionType.User => MetadataValueType.User,
                    FieldDefinitionType.Selection => multiSelection ? MetadataValueType.MultiSelection : MetadataValueType.SingleSelection,
                    _ => MetadataValueType.Unknown
                };
            }
            catch (Exception)
            {
                // ignored - we'll just leave it as unknown
                return MetadataValueType.Unknown;
            }
        }
    }
}

using System;
using System.Linq;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    static partial class EntityMapper
    {
        internal static void MapFrom(this FieldDefinitionEntity entity, IFieldDefinitionData data)
        {
            if (entity.CacheConfiguration.CacheProperties)
                entity.Properties = data.From();
        }

        internal static FieldDefinitionProperties From(this IFieldDefinitionData data)
        {
            return new FieldDefinitionProperties
            {
                Type = data.Type,
                IsDeleted = data.Status == "Deleted",
                DisplayName = data.DisplayName,
                AuthoringInfo = new AuthoringInfo(data.CreatedBy, data.Created, data.UpdatedBy, data.Updated),
                Origin = new FieldDefinitionOrigin(data.Origin),
                AcceptedValues = data.AcceptedValues?.ToArray() ?? Array.Empty<string>(),
                Multiselection = data.Multiselection ?? false
            };
        }

        internal static SelectionFieldDefinitionProperties From(this FieldDefinitionProperties properties)
        {
            return new SelectionFieldDefinitionProperties
            {
                IsDeleted = properties.IsDeleted,
                DisplayName = properties.DisplayName,
                AuthoringInfo = properties.AuthoringInfo,
                Origin = properties.Origin,
                AcceptedValues = properties.AcceptedValues,
                Multiselection = properties.Multiselection
            };
        }

        internal static IFieldDefinition From(this IFieldDefinitionData data, IAssetDataSource assetDataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration,
            FieldDefinitionDescriptor fieldDefinitionDescriptor, FieldDefinitionCacheConfiguration? localConfiguration = null)
        {
            var entity = data.Type switch
            {
                FieldDefinitionType.Selection => new SelectionFieldDefinitionEntity(assetDataSource, defaultCacheConfiguration, fieldDefinitionDescriptor, localConfiguration),
                _ => new FieldDefinitionEntity(assetDataSource, defaultCacheConfiguration, fieldDefinitionDescriptor, localConfiguration)
            };

            entity.MapFrom(data);

            return entity;
        }

        internal static IFieldDefinition From(this IFieldDefinitionData data, IAssetDataSource assetDataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration,
            OrganizationId organizationId, FieldDefinitionCacheConfiguration? localConfiguration = null)
        {
            return data.From(assetDataSource, defaultCacheConfiguration, new FieldDefinitionDescriptor(organizationId, data.Name), localConfiguration);
        }

        internal static IFieldDefinitionBaseData From(this IFieldDefinitionUpdate update)
        {
            return new FieldDefinitionBaseData
            {
                DisplayName = update.DisplayName
            };
        }

        internal static IFieldDefinitionCreateData From(this IFieldDefinitionCreation create)
        {
            var data = new FieldDefinitionCreateData
            {
                Name = create.Key,
                Type = create.Type,
                DisplayName = create.DisplayName,
            };

            if (create is ISelectionFieldDefinitionCreation selectionFieldDefinitionCreation)
            {
                data.Multiselection = selectionFieldDefinitionCreation.Multiselection;
                data.AcceptedValues = selectionFieldDefinitionCreation.AcceptedValues?.ToArray() ?? Array.Empty<string>();

                if (data.AcceptedValues.Length == 0)
                {
                    throw new ArgumentException("Accepted values must not be empty.");
                }
            }
            else if (create.Type == FieldDefinitionType.Selection)
            {
                data.Multiselection = false;
                data.AcceptedValues = new[] {"default"};
            }

            return data;
        }
    }
}

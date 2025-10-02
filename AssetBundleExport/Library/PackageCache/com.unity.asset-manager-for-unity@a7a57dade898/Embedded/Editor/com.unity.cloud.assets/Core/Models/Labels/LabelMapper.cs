using System.Drawing;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    static partial class EntityMapper
    {
        internal static void MapFrom(this LabelEntity label, ILabelData labelData)
        {
            label.Properties = labelData.From();
        }

        internal static LabelProperties From(this ILabelData data)
        {
            return new LabelProperties
            {
                Description = data.Description,
                DisplayColor = data.DisplayColor,
                IsSystemLabel = data.IsSystemLabel,
                IsAssignable = data.IsUserAssignable,
                AuthoringInfo = new AuthoringInfo(data.CreatedBy, data.Created, data.UpdatedBy, data.Updated)
            };
        }

        internal static ILabelBaseData From(this ILabelCreation labelCreation)
        {
            return new LabelBaseData
            {
                Name = labelCreation.Name,
                Description = labelCreation.Description,
                DisplayColor = labelCreation.DisplayColor
            };
        }

        internal static ILabelBaseData From(this ILabelUpdate labelUpdate)
        {
            return new LabelBaseData
            {
                Description = labelUpdate.Description,
                DisplayColor = labelUpdate.DisplayColor
            };
        }

        internal static LabelEntity From(this ILabelData data, IAssetDataSource assetDataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration,
            OrganizationId organizationId, LabelCacheConfiguration? cacheConfigurationOverride = null)
        {
            return data.From(assetDataSource, defaultCacheConfiguration, new LabelDescriptor(organizationId, data.Name), cacheConfigurationOverride);
        }

        internal static LabelEntity From(this ILabelData data, IAssetDataSource assetDataSource, AssetRepositoryCacheConfiguration defaultCacheConfiguration,
            LabelDescriptor labelDescriptor, LabelCacheConfiguration? cacheConfigurationOverride = null)
        {
            var label = new LabelEntity(assetDataSource, defaultCacheConfiguration, labelDescriptor, cacheConfigurationOverride);
            label.MapFrom(data);
            return label;
        }
    }
}

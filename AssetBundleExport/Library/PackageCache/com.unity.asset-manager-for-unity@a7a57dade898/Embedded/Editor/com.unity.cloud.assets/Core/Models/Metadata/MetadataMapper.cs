using System.Collections.Generic;
using System.Linq;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    static partial class EntityMapper
    {
        static MetadataObject From(object obj, IAssetDataSource dataSource, FieldDefinitionDescriptor fieldDefinitionDescriptor)
        {
            return new FieldDefinitionMetadataObject(IsolatedSerialization.ToObject(obj), dataSource, fieldDefinitionDescriptor);
        }

        static object From(this MetadataValue metadataValue)
        {
            return metadataValue.GetValue();
        }

        static Dictionary<string, object> From(this IDictionary<string, MetadataObject> metadataDictionary)
        {
            return metadataDictionary.ToDictionary(pair => pair.Key, pair => pair.Value.From());
        }

        static Dictionary<string, object> From(this MetadataContainerEntity metadataContainer)
        {
            return metadataContainer.Properties?.From();
        }

        internal static Dictionary<string, MetadataObject> From(this IDictionary<string, object> dictionary, IAssetDataSource dataSource, OrganizationId organizationId)
        {
            return dictionary.ToDictionary(pair => pair.Key, pair => From(pair.Value, dataSource, new FieldDefinitionDescriptor(organizationId, pair.Key)));
        }

        internal static Dictionary<string, MetadataObject> From(this IDictionary<string, object> dictionary)
        {
            return dictionary.ToDictionary(pair => pair.Key, pair => new MetadataObject(IsolatedSerialization.ToObject(pair.Value)));
        }
    }
}

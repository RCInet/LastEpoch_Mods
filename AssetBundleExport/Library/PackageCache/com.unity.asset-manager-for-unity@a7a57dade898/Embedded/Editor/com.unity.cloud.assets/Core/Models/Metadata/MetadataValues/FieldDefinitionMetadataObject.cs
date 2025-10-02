using System;
using System.Collections;
using System.Threading.Tasks;

namespace Unity.Cloud.AssetsEmbedded
{
    class FieldDefinitionMetadataObject : MetadataObject
    {
        internal FieldDefinitionMetadataObject(object value)
            : base(ParseType(value), value) { }

        internal FieldDefinitionMetadataObject(object value, IAssetDataSource dataSource, FieldDefinitionDescriptor fieldDefinitionDescriptor)
            : base(ParseType(value), value)
        {
            _ = ValidateAsync(dataSource, fieldDefinitionDescriptor);
        }

        static MetadataValueType ParseType(object value)
        {
            return value switch
            {
                bool => MetadataValueType.Boolean,
                ICollection => MetadataValueType.MultiSelection,
                double or int or float or long or short or byte or sbyte or decimal => MetadataValueType.Number,
                DateTime => MetadataValueType.Timestamp,
                string stringValue => UrlMetadata.TryParse(stringValue, out _, out _) ? MetadataValueType.Url : MetadataValueType.Unknown,
                _ => MetadataValueType.Unknown
            };
        }

        async Task ValidateAsync(IAssetDataSource dataSource, FieldDefinitionDescriptor fieldDefinitionDescriptor)
        {
            if (ValueType == MetadataValueType.Unknown)
            {
                ValueType = await dataSource.GetMetadataValueTypeAsync(fieldDefinitionDescriptor, default);
            }
        }
    }
}

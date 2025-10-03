using System;

namespace Unity.Cloud.AssetsEmbedded
{
    class MetadataObject : MetadataValue
    {
        readonly object m_Value;

        internal MetadataObject(object value)
            : base(ParseType(value))
        {
            m_Value = value;
        }

        internal MetadataObject(MetadataValueType valueType, string value)
            : base(valueType)
        {
            m_Value = value;
        }

        private protected MetadataObject(MetadataValueType valueType, object value)
            : base(valueType)
        {
            m_Value = value;
        }

        internal override object GetValue()
        {
            return m_Value;
        }

        static MetadataValueType ParseType(object value)
        {
            return value switch
            {
                bool => MetadataValueType.Boolean,
                double or int or float or long or short or byte or sbyte or decimal => MetadataValueType.Number,
                DateTime => MetadataValueType.Timestamp,
                string stringValue => UrlMetadata.TryParse(stringValue, out _, out _) ? MetadataValueType.Url : MetadataValueType.Text,
                _ => MetadataValueType.Unknown
            };
        }
    }
}

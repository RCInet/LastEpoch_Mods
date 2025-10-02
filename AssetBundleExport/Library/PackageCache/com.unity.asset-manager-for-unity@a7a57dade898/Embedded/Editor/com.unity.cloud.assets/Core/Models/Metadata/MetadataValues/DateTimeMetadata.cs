using System;

namespace Unity.Cloud.AssetsEmbedded
{
    sealed class DateTimeMetadata : MetadataValue
    {
        public DateTime Value { get; set; }

        public DateTimeMetadata(DateTime value = default)
            : base(MetadataValueType.Timestamp)
        {
            Value = value;
        }

        internal DateTimeMetadata(MetadataValueType valueType, object value)
            : base(valueType)
        {
            Value = value as DateTime? ?? DateTime.Parse(value?.ToString() ?? DateTime.MinValue.ToString());
        }

        internal override object GetValue()
        {
            return Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }
    }
}

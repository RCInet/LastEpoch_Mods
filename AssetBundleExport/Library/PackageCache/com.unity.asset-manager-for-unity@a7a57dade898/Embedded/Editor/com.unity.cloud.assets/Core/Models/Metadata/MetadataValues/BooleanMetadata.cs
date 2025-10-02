namespace Unity.Cloud.AssetsEmbedded
{
    sealed class BooleanMetadata : MetadataValue
    {
        public bool Value { get; set; }

        public BooleanMetadata(bool value = default)
            : base(MetadataValueType.Boolean)
        {
            Value = value;
        }

        internal BooleanMetadata(MetadataValueType valueType, object value)
            : base(valueType)
        {
            Value = value as bool? ?? bool.Parse(value?.ToString() ?? false.ToString());
        }

        internal override object GetValue()
        {
            return Value;
        }
    }
}

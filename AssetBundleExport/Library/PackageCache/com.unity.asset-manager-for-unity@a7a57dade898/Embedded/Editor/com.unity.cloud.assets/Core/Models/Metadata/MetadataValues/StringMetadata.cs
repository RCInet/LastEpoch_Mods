namespace Unity.Cloud.AssetsEmbedded
{
    sealed class StringMetadata : MetadataValue
    {
        public string Value { get; set; }

        public StringMetadata(string value = default)
            : base(MetadataValueType.Text)
        {
            Value = value;
        }

        internal StringMetadata(MetadataValueType valueType, object value)
            : base(valueType)
        {
            Value = value?.ToString() ?? string.Empty;
        }

        internal override object GetValue()
        {
            return Value;
        }
    }
}

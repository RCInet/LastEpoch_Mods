namespace Unity.Cloud.AssetsEmbedded
{
    class FieldDefinitionCreation : IFieldDefinitionCreation
    {
        /// <inheritdoc/>
        public string Key { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string DisplayName { get; set; } = string.Empty;

        /// <inheritdoc/>
        public FieldDefinitionType Type { get; set; }
    }
}

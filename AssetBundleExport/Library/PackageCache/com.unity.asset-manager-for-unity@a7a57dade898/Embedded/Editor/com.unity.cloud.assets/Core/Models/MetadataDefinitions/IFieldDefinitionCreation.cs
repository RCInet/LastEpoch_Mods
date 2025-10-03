namespace Unity.Cloud.AssetsEmbedded
{
    interface IFieldDefinitionCreation
    {
        /// <inheritdoc cref="FieldDefinitionDescriptor.FieldKey"/>
        string Key { get; }

        /// <inheritdoc cref="IFieldDefinition.DisplayName"/>
        string DisplayName { get; }

        /// <inheritdoc cref="IFieldDefinition.Type"/>
        FieldDefinitionType Type { get; }
    }
}

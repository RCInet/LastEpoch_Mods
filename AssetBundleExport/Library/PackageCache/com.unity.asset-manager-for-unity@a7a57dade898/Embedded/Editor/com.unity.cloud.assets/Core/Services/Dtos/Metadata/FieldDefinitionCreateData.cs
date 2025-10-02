using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class FieldDefinitionCreateData : FieldDefinitionBaseData, IFieldDefinitionCreateData
    {
        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public FieldDefinitionType Type { get; set; }

        /// <inheritdoc />
        public bool? Multiselection { get; set; }
    }
}

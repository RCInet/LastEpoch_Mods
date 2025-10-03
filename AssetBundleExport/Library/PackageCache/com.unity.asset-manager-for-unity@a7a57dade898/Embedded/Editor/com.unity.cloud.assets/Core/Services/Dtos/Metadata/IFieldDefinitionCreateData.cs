using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    interface IFieldDefinitionCreateData : IFieldDefinitionBaseData
    {
        [DataMember(Name = "name")]
        string Name { get; }

        [DataMember(Name = "type")]
        FieldDefinitionType Type { get; }

        [DataMember(Name = "multiselection")]
        bool? Multiselection { get; }
    }
}

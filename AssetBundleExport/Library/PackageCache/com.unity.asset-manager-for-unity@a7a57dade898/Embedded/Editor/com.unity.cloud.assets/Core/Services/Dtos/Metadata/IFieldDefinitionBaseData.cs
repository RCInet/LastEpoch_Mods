using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    interface IFieldDefinitionBaseData
    {
        [DataMember(Name = "displayName")]
        string DisplayName { get; }

        [DataMember(Name = "acceptedValues")]
        string[] AcceptedValues { get; }
    }
}

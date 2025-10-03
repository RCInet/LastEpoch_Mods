using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    interface IFieldDefinitionData : IFieldDefinitionCreateData, IAuthoringData
    {
        [DataMember(Name = "status")]
        string Status { get; }

        [DataMember(Name = "fieldOrigin")]
        string Origin { get; }
    }
}

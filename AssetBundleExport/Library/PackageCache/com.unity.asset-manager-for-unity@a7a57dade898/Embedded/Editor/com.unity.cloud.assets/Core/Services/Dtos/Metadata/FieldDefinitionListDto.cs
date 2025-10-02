using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class FieldDefinitionListDto
    {
        [DataMember(Name = "fieldDefinitions")]
        public FieldDefinitionData[] FieldDefinitions { get; set; }

        [DataMember(Name = "next")]
        public string NextPageToken { get; set; }
    }
}

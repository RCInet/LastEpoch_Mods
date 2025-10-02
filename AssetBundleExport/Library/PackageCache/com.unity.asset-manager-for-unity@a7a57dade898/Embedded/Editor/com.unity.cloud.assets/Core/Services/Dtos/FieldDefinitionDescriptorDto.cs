using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    struct FieldDefinitionDescriptorDto
    {
        [DataMember(Name = "organizationId")]
        public string OrganizationId { get; set; }

        [DataMember(Name = "fieldKey")]
        public string FieldKey { get; set; }
    }
}

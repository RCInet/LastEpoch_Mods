using System.Runtime.Serialization;

namespace Unity.Cloud.CommonEmbedded
{
    [DataContract]
    struct CollectionDescriptorDto
    {
        [DataMember(Name = "projectDescriptor")]
        public string ProjectDescriptor { get; set; }

        [DataMember(Name = "collectionPath")]
        public string CollectionPath { get; set; }
    }
}

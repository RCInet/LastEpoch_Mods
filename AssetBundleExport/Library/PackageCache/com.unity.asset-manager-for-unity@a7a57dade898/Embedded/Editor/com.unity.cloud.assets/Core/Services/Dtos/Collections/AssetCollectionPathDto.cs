using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    struct AssetCollectionPathDto
    {
        [DataMember(Name = "path")]
        public CollectionPath Path { get; set; }
    }
}

using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    struct CreatedLabelDto
    {
        [DataMember(Name = "labelName")]
        public string Name { get; set; }
    }
}

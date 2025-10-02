using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    struct StartedTransformationDto
    {
        [DataMember(Name = "transformationId")]
        public TransformationId TransformationId { get; set; }
    }
}

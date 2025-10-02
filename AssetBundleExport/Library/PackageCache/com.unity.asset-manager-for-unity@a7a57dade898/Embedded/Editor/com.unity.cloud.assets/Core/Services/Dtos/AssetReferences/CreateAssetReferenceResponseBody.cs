using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    struct CreateAssetReferenceResponseBody
    {
        [DataMember(Name = "referenceId")]
        public string ReferenceId { get; set; }
    }
}

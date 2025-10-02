using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    struct UploadUrlDto
    {
        [DataMember(Name = "uploadUrl")]
        public string UploadUrl { get; set; }
    }
}

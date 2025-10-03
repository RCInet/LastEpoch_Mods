using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    struct AssetDownloadUrlsDto
    {
        [DataMember(Name = "files")]
        public List<FileUrl> FileUrls { get; set; }
    }
}

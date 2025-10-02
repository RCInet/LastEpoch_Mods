using System;

namespace Unity.Cloud.AssetsEmbedded
{
    struct AssetDownloadUrl
    {
        public string FilePath { get; set; }
        public Uri DownloadUrl { get; set; }
    }
}

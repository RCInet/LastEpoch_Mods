using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class AssetPageDto
    {
        [DataMember(Name = "next")]
        public string Token { get; set; }

        [DataMember(Name = "assets")]
        AssetData[] m_Assets;

        [DataMember(Name = "results")]
        AssetData[] m_Results;

        public AssetData[] Assets
        {
            get => m_Results ?? m_Assets;
            set => m_Assets = value;
        }
    }
}

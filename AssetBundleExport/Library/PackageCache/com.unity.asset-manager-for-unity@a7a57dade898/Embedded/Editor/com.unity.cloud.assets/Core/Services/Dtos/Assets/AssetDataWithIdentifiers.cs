using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class AssetDataWithIdentifiers
    {
        public static readonly string SerializedType = typeof(AssetDataWithIdentifiers).FullName;

        [DataMember(Name = "type")]
        string Type { get; set; } = SerializedType;

        [Obsolete("Replaced by Descriptor; maintained for backwards compatibility of serialization.")]
        [DataMember(Name = "ids")]
        public AssetIdentifier Identifier { get; set; }

        [DataMember(Name = "descriptor")]
        public string Descriptor { get; set; }

        [DataMember(Name = "data")]
        public AssetData Data { get; set; }
    }
}

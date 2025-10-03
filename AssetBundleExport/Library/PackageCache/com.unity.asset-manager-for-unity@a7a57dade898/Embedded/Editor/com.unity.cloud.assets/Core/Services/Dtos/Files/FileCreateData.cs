using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class FileCreateData : FileBaseData, IFileCreateData
    {
        /// <inheritdoc />
        public string Path { get; set; }

        /// <inheritdoc />
        public long SizeBytes { get; set; }

        /// <inheritdoc />
        public string UserChecksum { get; set; }
    }
}

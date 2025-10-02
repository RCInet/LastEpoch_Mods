namespace Unity.Cloud.AssetsEmbedded
{
    class AssetFreeze : IAssetFreeze
    {
        /// <inheritdoc />
        public string ChangeLog { get; set; }

        /// <inheritdoc />
        public AssetFreezeOperation Operation { get; set; }

        public AssetFreeze() { }

        public AssetFreeze(string changeLog)
        {
            ChangeLog = changeLog;
        }
    }
}

namespace Unity.Cloud.AssetsEmbedded
{
    class AssetProjectCreation : IAssetProjectCreation
    {
        public string Name { get; set; }
        public IDeserializable Metadata { get; set; }
    }
}

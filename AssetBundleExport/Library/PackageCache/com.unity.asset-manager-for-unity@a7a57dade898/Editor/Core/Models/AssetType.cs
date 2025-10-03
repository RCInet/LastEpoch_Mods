namespace Unity.AssetManager.Core.Editor
{
    enum AssetType
    {
        Other = 0,
        Asset2D = 1,
        Model3D = 2,
        Audio = 3,
        Material = 4,
        Script = 5,
        Video = 6,
        UnityEditor = 7,
        Animation = 8,
        AssemblyDefinition = 9,
        Asset = 10,
        AudioMixer = 11,
        Configuration = 12,
        Document = 13,
        Environment = 14,
        Font = 15,
        PhysicsMaterial = 16,
        Playable = 17,
        Prefab = 18,
        Scene = 19,
        Shader = 20,
        ShaderGraph = 21,
        UnityPackage = 22,
        UnityScene = 23,
        VisualEffect = 24,
        Image = 25
    }

    static class AssetTypeExtensions
    {
        public static string GetToolTip(this AssetType assetType)
        {
            return assetType switch
            {
                AssetType.Asset2D => "Texture",
                AssetType.Model3D => "Mesh",
                AssetType.Audio => "Audio Clip",
                AssetType.UnityEditor => "Any Unity Editor asset",
                AssetType.Animation => "Animation Clip",
                AssetType.Environment => "Terrain, Lighting, or other environment-related asset",
                AssetType.Playable => "Timeline",
                _ => string.Empty
            };
        }
    }
}

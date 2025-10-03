using System;

namespace Unity.AssetManager.Core.Editor
{
    [Serializable]
    class CachedItem
    {
        public string CacheKey { get; set; }
        public string Path { get; set; }
    }
}

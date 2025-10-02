using System;

namespace Unity.AssetManager.Core.Editor
{
    internal static class AssetManagerCoreConstants
    {
        public const string PackageName = "com.unity.asset-manager-for-unity";
        public const string AssetsFolderName = "Assets";
        public const string AssetManagerCacheLocationFolderName = "AssetManager";
        public const string CacheThumbnailsFolderName = "Thumbnails";
        public const string CacheTexturesFolderName = "Textures";

        public const string StatusInReview = "InReview";
        public const string StatusApproved = "Approved";
        public const string CancelImportActionText = "Cancel Import";
        public const string Cancel = "Cancel";

        public const int DefaultCacheSizeGb = 2;
        public const int DefaultCacheSizeMb = DefaultCacheSizeGb * 1024;
        public const int ShrinkSizeInMb = 200;

        // This exists here for compatibility with 2020.x versions
        public static readonly DateTime UnixEpoch = DateTime.UnixEpoch;
    }
}

using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;

namespace Unity.AssetManager.Core.Editor
{
    interface ISettingsManager : IService
    {
        string DefaultImportLocation { get; set; }
        bool IsSubfolderCreationEnabled { get; }
        bool IsKeepHigherVersionEnabled { get; }
        string BaseCacheLocation { get; }
        string ThumbnailsCacheLocation { get; }
        int MaxCacheSizeGb { get; }
        int MaxCacheSizeMb { get; }
        bool IsTagsCreationUploadEnabled { get; }
        int TagsConfidenceThresholdPercent { get; }
        float TagsConfidenceThreshold { get; }
        bool IsUploadDependenciesUsingLatestLabel { get; }
        bool IsReimportModalDisabled { get; }
        bool IsProjectWindowIconOverlayEnabled { get; }
        int ProjectWindowIconOverlayPosition { get; }
        bool DisplayDetailedProjectWindowIconOverlay { get; }
        bool IsDebugLogsEnabled { get; }
        PrivateCloudSettings PrivateCloudSettings { get; }
        SavedAssetSearchFilterSettings SavedAssetSearchFilterSettings { get; }

        void SetIsSubfolderCreationEnabled(bool value);
        void SetIsKeepHigherVersionEnabled(bool value);
        void SetCacheLocation(string cacheLocation);
        void SetMaxCacheSize(int cacheSize);
        void SetIsTagsCreationUploadEnabled(bool value);
        void SetTagsCreationConfidenceThresholdPercent(int value);

        string ResetCacheLocation();
        string ResetImportLocation();

        void SetUploadDependenciesUsingLatestLabel(bool value);
        void SetDisableReimportModal(bool value);

        void SetProjectWindowIconOverlayEnabled(bool value);
        void SetProjectWindowIconOverlayPosition(int value);
        void SetProjectWindowIconOverlayDisplayType(bool value);

        void SetDebugLogsEnabled(bool value);
    }

    [Serializable]
    class AssetManagerSettingsManager : BaseService<ISettingsManager>, ISettingsManager
    {
        [SerializeReference]
        ICachePathHelper m_CachePathHelper;

        const string k_DefaultImportLocationKey = "AM4U.defaultImportLocation";
        const string k_IsSubfolderCreationEnabledKey = "AM4U.isSubfolderCreationEnabled";
        const string k_IsKeepHigherVersionEnabledKey = "AM4U.isKeepHigherVersionEnabled";
        const string k_IsTagsCreationUploadEnabledKey = "AM4U.isTagsCreationUploadEnabled";
        const string k_CacheLocationKey = "AM4U.cacheLocation";
        const string k_MaxCacheSizeKey = "AM4U.cacheSize";
        const string k_TexturesCacheLocationKey = "AM4U.texturesCacheLocation";
        const string k_ThumbnailsCacheLocationKey = "AM4U.thumbnailsCaheLocation";
        const string k_AssetManagerCacheLocationKey = "AM4U.assetManagerCacheLocation";
        const string k_TagsCreationConfidenceThreshold = "AM4U.tagsCreationConfidenceThreshold";
        const int k_DefaultConfidenceLevel = 80;
        const string k_UploadDependenciesUsingLatestLabel = "AM4U.uploadDependenciesUsingLatestLabel";
        const string k_ReimportModalDisabled = "AM4U.reimportModalDisabled";
        const string k_ProjectWindowIconOverlayEnabled = "AM4U.projectWindowIconOverlayEnabled";
        const string k_ProjectWindowIconOverlayPosition = "AM4U.projectWindowIconOverlayPosition";
        const string k_ProjectWindowIconOverlayType = "AM4U.projectWindowIconOverlayType";
        const string k_DebugLogsEnabled = "AM4U.debugLogsEnabled";

        enum ProjectIconOverlayPosition
        {
            TopRight = 0,
            TopLeft = 1,
            BottomLeft = 2,
            BottomRight = 3,
        }

        UnityEditor.SettingsManagement.Settings m_Settings;

        UnityEditor.SettingsManagement.Settings Instance
        {
            get
            {
                if (m_Settings == null)
                {
                    m_Settings = new UnityEditor.SettingsManagement.Settings(AssetManagerCoreConstants.PackageName);
                }

                return m_Settings;
            }
        }

        public string DefaultImportLocation
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                var relativePath = Utilities.GetPathRelativeToAssetsFolderIncludeAssets(value);
                Utilities.DevAssert(relativePath.StartsWith(AssetManagerCoreConstants.AssetsFolderName));

                Instance.Set(k_DefaultImportLocationKey, relativePath, SettingsScope.User);
            }

            get => Instance.Get(k_DefaultImportLocationKey, SettingsScope.User, GetDefaultImportLocation());
        }

        public bool IsSubfolderCreationEnabled => Instance.Get(k_IsSubfolderCreationEnabledKey, SettingsScope.User, false);
        public bool IsKeepHigherVersionEnabled => Instance.Get(k_IsKeepHigherVersionEnabledKey, SettingsScope.User, true);

        public bool IsTagsCreationUploadEnabled => Instance.Get(k_IsTagsCreationUploadEnabledKey, SettingsScope.User, false);

        public int TagsConfidenceThresholdPercent => Instance.Get(k_TagsCreationConfidenceThreshold, SettingsScope.User, k_DefaultConfidenceLevel);
        public float TagsConfidenceThreshold => TagsConfidenceThresholdPercent / 100f;
        public bool IsUploadDependenciesUsingLatestLabel => Instance.Get(k_UploadDependenciesUsingLatestLabel, SettingsScope.User, false);
        public bool IsReimportModalDisabled => Instance.Get(k_ReimportModalDisabled, SettingsScope.User, false);

        public bool IsProjectWindowIconOverlayEnabled => Instance.Get(k_ProjectWindowIconOverlayEnabled, SettingsScope.User, true);
        public int ProjectWindowIconOverlayPosition => Instance.Get(k_ProjectWindowIconOverlayPosition, SettingsScope.User, (int)ProjectIconOverlayPosition.TopRight);
        public bool DisplayDetailedProjectWindowIconOverlay => Instance.Get(k_ProjectWindowIconOverlayType, SettingsScope.User, true);

        public bool IsDebugLogsEnabled => Instance.Get(k_DebugLogsEnabled, SettingsScope.User, false);

        public PrivateCloudSettings PrivateCloudSettings => PrivateCloudSettings.Load(Instance);
        public SavedAssetSearchFilterSettings SavedAssetSearchFilterSettings => SavedAssetSearchFilterSettings.Load(Instance);

        public string BaseCacheLocation
        {
            get
            {
                var cacheLocation = Instance.Get<string>(k_CacheLocationKey, SettingsScope.User);
                return GetCacheLocationOrDefault(cacheLocation);
            }
        }

        public string ThumbnailsCacheLocation
        {
            get
            {
                var thumbnailsCacheLocation = Instance.Get<string>(k_ThumbnailsCacheLocationKey, SettingsScope.User);
                return GetCacheLocationOrDefault(thumbnailsCacheLocation);
            }
        }

        public int MaxCacheSizeGb
        {
            get
            {
                var cacheSize = Instance.Get<int>(k_MaxCacheSizeKey, SettingsScope.User);
                if (cacheSize > 0)
                {
                    return cacheSize;
                }

                // if the cache size is lower than 0
                SetMaxCacheSize(AssetManagerCoreConstants.DefaultCacheSizeGb);
                return AssetManagerCoreConstants.DefaultCacheSizeGb;
            }
        }



        public int MaxCacheSizeMb => MaxCacheSizeGb * 1024;

        [ServiceInjection]
        public void Inject(ICachePathHelper cachePathHelper)
        {
            m_CachePathHelper = cachePathHelper;
        }

        public void SetIsSubfolderCreationEnabled(bool value)
        {
            Instance.Set(k_IsSubfolderCreationEnabledKey, value, SettingsScope.User);
        }

        public void SetIsKeepHigherVersionEnabled(bool value)
        {
            Instance.Set(k_IsKeepHigherVersionEnabledKey, value, SettingsScope.User);
        }

        public void SetIsTagsCreationUploadEnabled(bool value)
        {
            Instance.Set(k_IsTagsCreationUploadEnabledKey, value, SettingsScope.User);
        }

        public void SetTagsCreationConfidenceThresholdPercent(int value)
        {
            Instance.Set(k_TagsCreationConfidenceThreshold, value, SettingsScope.User);
        }

        public void SetCacheLocation(string cacheLocation)
        {
            if (string.IsNullOrEmpty(cacheLocation))
            {
                cacheLocation = m_CachePathHelper.GetDefaultCacheLocation();
            }

            Instance.Set(k_CacheLocationKey, cacheLocation, SettingsScope.User);
            var assetManagerCacheLocation = m_CachePathHelper.CreateAssetManagerCacheLocation(cacheLocation);
            Instance.Set(k_AssetManagerCacheLocationKey, m_CachePathHelper.CreateAssetManagerCacheLocation(cacheLocation),
                SettingsScope.User);
            Instance.Set(k_ThumbnailsCacheLocationKey,
                Path.Combine(assetManagerCacheLocation, AssetManagerCoreConstants.CacheThumbnailsFolderName), SettingsScope.User);
            Instance.Set(k_TexturesCacheLocationKey,
                Path.Combine(assetManagerCacheLocation, AssetManagerCoreConstants.CacheTexturesFolderName), SettingsScope.User);
        }

        public void SetMaxCacheSize(int cacheSize)
        {
            if (cacheSize <= 0)
            {
                return;
            }

            Instance.Set(k_MaxCacheSizeKey, cacheSize, SettingsScope.User);
        }

        static string GetDefaultImportLocation()
        {
            return AssetManagerCoreConstants.AssetsFolderName;
        }

        public string ResetCacheLocation()
        {
            var defaultLocation = m_CachePathHelper.GetDefaultCacheLocation();
            SetCacheLocation(defaultLocation);
            return defaultLocation;
        }

        public string ResetImportLocation()
        {
            var defaultLocation = GetDefaultImportLocation();
            DefaultImportLocation = defaultLocation;
            return defaultLocation;
        }

        string GetCacheLocationOrDefault(string cachePath)
        {
            var validationResult = m_CachePathHelper.EnsureBaseCacheLocation(cachePath);

            if (validationResult.Success)
            {
                return cachePath;
            }

            // Provided path is not valid, set default
            var defaultLocation = m_CachePathHelper.GetDefaultCacheLocation();
            SetCacheLocation(defaultLocation);
            return Path.Combine(defaultLocation, AssetManagerCoreConstants.CacheTexturesFolderName);
        }

        public void SetUploadDependenciesUsingLatestLabel(bool value)
        {
            Instance.Set(k_UploadDependenciesUsingLatestLabel, value, SettingsScope.User);
        }

        public void SetDisableReimportModal(bool value)
        {
            Instance.Set(k_ReimportModalDisabled, value, SettingsScope.User);
            AnalyticsSender.SendEvent(new DisableImportModalToggleEvent(value));
        }

        public void SetProjectWindowIconOverlayEnabled(bool value)
        {
            Instance.Set(k_ProjectWindowIconOverlayEnabled, value, SettingsScope.User);
            AnalyticsSender.SendEvent(new ProjectIconOverlayToggleEvent(value));
        }

        public void SetProjectWindowIconOverlayPosition(int value)
        {
            Instance.Set(k_ProjectWindowIconOverlayPosition, value, SettingsScope.User);
        }

        public void SetProjectWindowIconOverlayDisplayType(bool value)
        {
            Instance.Set(k_ProjectWindowIconOverlayType, value, SettingsScope.User);
        }

        public void SetDebugLogsEnabled(bool value)
        {
            Instance.Set(k_DebugLogsEnabled, value, SettingsScope.User);
        }
    }
}

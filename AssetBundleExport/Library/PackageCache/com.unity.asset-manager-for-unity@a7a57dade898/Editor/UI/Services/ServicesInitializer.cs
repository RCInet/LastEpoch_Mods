using Unity.AssetManager.Core.Editor;
using Unity.AssetManager.Upload.Editor;
using UnityEditor;
using UnityEngine;
using AssetImporter = Unity.AssetManager.Core.Editor.AssetImporter;

namespace Unity.AssetManager.UI.Editor
{
    static class ServicesInitializer
    {
        [InitializeOnLoadMethod]
        static void Init()
        {
            InitializeServices();

            // For backwards compatibility, register services that did not previously exist.
            ServicesContainer.instance.TryInitializeServices(
                new DialogManager(),
                new FileUtility(),
                new SavedAssetSearchFilterManager());
        }

        public static void ResetServices()
        {
            InitializeServices(true);
        }

        static void InitializeServices(bool forceReset = false)
        {
            if (!forceReset && ServicesContainer.instance.IsInitialized())
            {
                Utilities.DevLog("Services already initialized");
                return;
            }

            ServicesContainer.instance.InitializeServices(
                // Core
                new IOProxy(),
                new ApplicationProxy(),
                new AssetOperationManager(),
                new DownloadManager(),
                new UploadManager(),
                new CachePathHelper(),
                new AssetManagerSettingsManager(),
                new CacheEvictionManager(),
                new ThumbnailDownloader(),
                new UnityConnectProxy(),
                new AssetsSdkProvider(),
                new ProjectOrganizationProvider(),
                new AssetDataManager(),
                new ProjectIconDownloader(),
                new AssetDatabaseProxy(),
                new ImportedAssetsTracker(),
                new EditorUtilityProxy(),
                new AssetImporter(),
                new AssetImportResolver(),
                new PermissionsManager(),
                new UrlProvider(),
                new UIPreferences(),
                new DragAndDropProjectBrowserProxy(),
                new FileUtility(),
                new MessageManager(),
                new SavedAssetSearchFilterManager(),

                // UI
                new StateManager(),
                new LinksProxy(),
                new PopupManager(),
                new PageManager(),
                new ContextMenuBuilder(),
                new DialogManager(),
                new ProjectWindowProxy(),
                new ProjectWindowIconOverlay());

            // Post-initialization configurations
            var assetImportResolver = ServicesContainer.instance.Resolve<IAssetImportResolver>();
            if (assetImportResolver != null)
            {
                assetImportResolver.SetConflictResolver(new AssetImportDecisionMaker());
            }
        }
    }
}

using System.Linq;
using Unity.AssetManager.Core.Editor;
using Unity.AssetManager.Upload.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class UploadContextMenu : AssetContextMenu
    {
        public UploadContextMenu(IUnityConnectProxy unityConnectProxy, IAssetDataManager assetDataManager,
            IAssetImporter assetImporter, ILinksProxy linksProxy, IAssetDatabaseProxy assetDatabaseProxy,
            IPageManager pageManager)
            : base(unityConnectProxy, assetDataManager, assetImporter, linksProxy, assetDatabaseProxy, pageManager) { }

        public override void SetupContextMenuEntries(ContextualMenuPopulateEvent evt)
        {
            IncludeAllScripts(evt);
            RemoveAssetEntry(evt);
            IgnoreAssetEntry(evt);
            ShowInProjectEntry(evt);
        }

        void ShowInProjectEntry(ContextualMenuPopulateEvent evt)
        {
            AddMenuEntry(evt, L10n.Tr(Constants.ShowInProjectActionText), true,
                (_) =>
                {
                    var uploadAssetData = (UploadAssetData)TargetAssetData;
                    m_AssetDatabaseProxy.PingAssetByGuid(uploadAssetData.Guid);
                    AnalyticsSender.SendEvent(new GridContextMenuItemSelectedEvent(GridContextMenuItemSelectedEvent.ContextMenuItemType.ShowInProject));
                });
        }

        void IgnoreAssetEntry(ContextualMenuPopulateEvent evt)
        {
            var uploadAssetData = (UploadAssetData)TargetAssetData;

            if (!uploadAssetData.CanBeIgnored)
                return;

            var selectedAssets = m_PageManager.ActivePage.SelectedAssets;
            var isTargetSelected = selectedAssets.Contains(uploadAssetData.Identifier);

            AddMenuEntry(evt, isTargetSelected ? L10n.Tr(Constants.IgnoreSelectedAssets) : L10n.Tr(Constants.IgnoreAsset), true, uploadAssetData.IsIgnored,
                (_) =>
                {
                    if (ServicesContainer.instance.Resolve<IPageManager>().ActivePage is not UploadPage uploadPage)
                        return;

                    uploadPage.ToggleAsset(uploadAssetData.Identifier, uploadAssetData.IsIgnored);

                    AnalyticsSender.SendEvent(new GridContextMenuItemSelectedEvent(GridContextMenuItemSelectedEvent.ContextMenuItemType.IgnoreUploadedAsset));
                });
        }

        void RemoveAssetEntry(ContextualMenuPopulateEvent evt)
        {
            var uploadAssetData = (UploadAssetData)TargetAssetData;

            if (!uploadAssetData.CanBeRemoved)
                return;

            AddMenuEntry(evt, L10n.Tr(Constants.RemoveAsset), true,
                (_) =>
                {
                    if (ServicesContainer.instance.Resolve<IPageManager>().ActivePage is not UploadPage uploadPage)
                        return;

                    uploadPage.RemoveAsset(uploadAssetData);
                });
        }

        void IncludeAllScripts(ContextualMenuPopulateEvent evt)
        {
            var uploadAssetData = (UploadAssetData)TargetAssetData;
            var scriptsIncludedToggleValue = false;
            var includeAllScriptsOptionEnabled = true;

            UploadPage uploadPage = m_PageManager?.ActivePage as UploadPage;
            if (uploadPage != null && uploadPage.UploadStaging != null)
            {
                includeAllScriptsOptionEnabled = uploadPage.UploadStaging.DependencyMode != UploadDependencyMode.Ignore;
                scriptsIncludedToggleValue = uploadPage.UploadStaging.HasIncludeAllScripts(uploadAssetData);
            }

            AddMenuEntry(evt, L10n.Tr(Constants.IncludeAllScripts), includeAllScriptsOptionEnabled, scriptsIncludedToggleValue,
                (_) =>
                {
                    if (uploadPage == null)
                        return;

                    uploadPage.SetIncludeAllScripts(uploadAssetData, !scriptsIncludedToggleValue);
                });
        }
    }
}

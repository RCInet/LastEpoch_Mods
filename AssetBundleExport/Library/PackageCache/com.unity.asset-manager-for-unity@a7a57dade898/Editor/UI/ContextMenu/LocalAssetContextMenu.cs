using Unity.AssetManager.Core.Editor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class LocalAssetContextMenu : AssetContextMenu
    {
        public LocalAssetContextMenu(IUnityConnectProxy unityConnectProxy, IAssetDataManager assetDataManager,
            IAssetImporter assetImporter, ILinksProxy linksProxy, IAssetDatabaseProxy assetDatabaseProxy,
            IPageManager pageManager) :
            base(unityConnectProxy, assetDataManager, assetImporter, linksProxy, assetDatabaseProxy, pageManager)
        {
        }

        public override void SetupContextMenuEntries(ContextualMenuPopulateEvent evt)
        {
            ShowInProjectEntry(evt);
        }

        void ShowInProjectEntry(ContextualMenuPopulateEvent evt)
        {
            AddMenuEntry(evt, Constants.ShowInProjectActionText, TargetAssetData != null,
                (_) =>
                {
                    if (TargetAssetData is { PrimarySourceFile: null })
                    {
                        return;
                    }

                    var guid = m_AssetDataManager.GetImportedFileGuid(TargetAssetData.Identifier,
                        TargetAssetData.PrimarySourceFile.Path);

                    if (!string.IsNullOrEmpty(guid))
                    {
                        m_AssetDatabaseProxy.PingAssetByGuid(guid);
                    }

                    AnalyticsSender.SendEvent(new GridContextMenuItemSelectedEvent(GridContextMenuItemSelectedEvent.ContextMenuItemType.ShowInProject));
                });
        }
    }
}

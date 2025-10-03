using System;
using Unity.AssetManager.Core.Editor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    abstract class AssetContextMenu: ContextMenu
    {
        internal readonly IAssetDatabaseProxy m_AssetDatabaseProxy;
        internal readonly IAssetDataManager m_AssetDataManager;
        internal readonly IAssetImporter m_AssetImporter;
        internal readonly ILinksProxy m_LinksProxy;
        internal readonly IPageManager m_PageManager;
        internal readonly IUnityConnectProxy m_UnityConnectProxy;

        BaseAssetData m_TargetAssetData;

        public BaseAssetData TargetAssetData
        {
            get => m_TargetAssetData;
            set => m_TargetAssetData = value;
        }

        protected AssetContextMenu(IUnityConnectProxy unityConnectProxy, IAssetDataManager assetDataManager, IAssetImporter assetImporter,
            ILinksProxy linksProxy, IAssetDatabaseProxy assetDatabaseProxy, IPageManager pageManager)
        {
            m_UnityConnectProxy = unityConnectProxy;
            m_AssetDataManager = assetDataManager;
            m_AssetImporter = assetImporter;
            m_LinksProxy = linksProxy;
            m_AssetDatabaseProxy = assetDatabaseProxy;
            m_PageManager = pageManager;
        }
    }
}

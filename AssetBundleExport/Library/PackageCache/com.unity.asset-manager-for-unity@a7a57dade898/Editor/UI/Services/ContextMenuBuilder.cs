using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.AssetManager.Core.Editor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    interface IContextMenuBuilder : IService
    {
        void RegisterContextMenu(Type assetDataType, Type typeContextMenu);
        bool IsContextMenuRegistered(Type assetDataType);
        object BuildContextMenu(Type assetDataType);
        public bool IsContextMenuMatchingAssetDataType(Type assetDataType, Type typeContextMenu);
    }

    [Serializable]
    class ContextMenuBuilder : BaseService<IContextMenuBuilder>, IContextMenuBuilder
    {
        [SerializeReference]
        IAssetDataManager m_AssetDataManager;

        [SerializeReference]
        IAssetImporter m_AssetImporter;

        [SerializeReference]
        ILinksProxy m_LinksProxy;

        [SerializeReference]
        IUnityConnectProxy m_UnityConnectProxy;

        [SerializeReference]
        IAssetDatabaseProxy m_AssetDatabaseProxy;

        [SerializeReference]
        IPageManager m_PageManager;

        readonly Dictionary<Type, Type> m_AssetDataTypeToContextMenuType = new();

        [ServiceInjection]
        public void Inject(IUnityConnectProxy unityConnectProxy, IAssetDataManager assetDataManager, IAssetImporter assetImporter, ILinksProxy linksProxy,
            IAssetDatabaseProxy assetDatabaseProxy, IPageManager pageManager)
        {
            m_UnityConnectProxy = unityConnectProxy;
            m_AssetDataManager = assetDataManager;
            m_AssetImporter = assetImporter;
            m_LinksProxy = linksProxy;
            m_AssetDatabaseProxy = assetDatabaseProxy;
            m_PageManager = pageManager;
        }

        public object BuildContextMenu(Type assetDataType)
        {
            if (!m_AssetDataTypeToContextMenuType.TryGetValue(assetDataType, out var contextType))
            {
                Debug.LogError("No context menu registered for asset data type: " + assetDataType);
                return null;
            }

            return Activator.CreateInstance(contextType,
                BindingFlags.Public |
                BindingFlags.Instance,
                null, new object[] {m_UnityConnectProxy, m_AssetDataManager, m_AssetImporter, m_LinksProxy, m_AssetDatabaseProxy, m_PageManager },
                null);
        }

        public void RegisterContextMenu(Type assetDataType, Type typeContextMenu)
        {
            m_AssetDataTypeToContextMenuType.TryAdd(assetDataType, typeContextMenu);
        }

        public bool IsContextMenuRegistered(Type assetDataType)
        {
            return m_AssetDataTypeToContextMenuType.ContainsKey(assetDataType);
        }

        public bool IsContextMenuMatchingAssetDataType(Type assetDataType, Type typeContextMenu)
        {
            if (m_AssetDataTypeToContextMenuType.TryGetValue(assetDataType, out var value))
            {
                return value == typeContextMenu;
            }

            return false;
        }
    }
}

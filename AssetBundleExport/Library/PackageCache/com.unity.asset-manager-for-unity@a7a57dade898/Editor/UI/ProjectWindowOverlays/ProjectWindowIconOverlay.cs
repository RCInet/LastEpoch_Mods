using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    enum ProjectIconOverlayPosition
    {
        TopRight = 0,
        TopLeft = 1,
        BottomLeft = 2,
        BottomRight = 3,
    }

    [InitializeOnLoad]
    static class ProjectWindowIconOverlayUpdater
    {
        static ProjectWindowIconOverlayUpdater()
        {
            EditorApplication.update += Update;
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        }

        static IProjectWindowIconOverlay overlay;

        static void Update()
        {
            // We must always resolve the service due to the fact that the ServicesInitializer.ResetServices()
            // method will force refresh all services, meaning the previous instances are disregarded and new
            // instances are created.
            overlay = ServicesContainer.instance.Resolve<IProjectWindowIconOverlay>();
            overlay?.Update();
        }

        static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            // We must always resolve the service due to the fact that the ServicesInitializer.ResetServices()
            // method will force refresh all services, meaning the previous instances are disregarded and new
            // instances are created.
            overlay = ServicesContainer.instance.Resolve<IProjectWindowIconOverlay>();
            overlay?.OnProjectWindowItemGUI(guid, selectionRect);
        }
    }

    interface IProjectWindowIconOverlay : IService
    {
        void Update();
        void OnProjectWindowItemGUI(string guid, Rect selectionRect);
    }

    [Serializable]
    class ProjectWindowIconOverlay : BaseService<IProjectWindowIconOverlay>, IProjectWindowIconOverlay, ISerializationCallbackReceiver
    {
        static ProjectIconAssetStatus s_NoImportStatus = new (ImportAttribute.ImportStatus.NoImport, string.Empty, string.Empty);
        static ProjectIconAssetStatus s_ConnectedStatus = new (ImportAttribute.ImportStatus.UpToDate, Constants.ImportedText, ProjectIcons.k_ConnectedIcon);
        static ProjectIconAssetStatus s_ImportedStatus = new (ImportAttribute.ImportStatus.UpToDate, Constants.UpToDateText, ProjectIcons.k_ImportedIcon);
        static ProjectIconAssetStatus s_OutOfDateStatus = new (ImportAttribute.ImportStatus.OutOfDate, Constants.OutOfDateText, ProjectIcons.k_OutOfDateIcon);
        static ProjectIconAssetStatus s_ErrorStatus = new (ImportAttribute.ImportStatus.ErrorSync, Constants.StatusErrorText, ProjectIcons.k_ErrorIcon);

        [Serializable]
        struct ProjectWindowSettingsState
        {
            public bool IsEnabled;
            public int Position;
            public bool DisplayDetailedStatus;

            public void SetState(ISettingsManager settingsManager)
            {
                IsEnabled = settingsManager.IsProjectWindowIconOverlayEnabled;
                Position = settingsManager.ProjectWindowIconOverlayPosition;
                DisplayDetailedStatus = settingsManager.DisplayDetailedProjectWindowIconOverlay;
            }

            public void ClearState()
            {
                IsEnabled = false;
                Position = 0;
                DisplayDetailedStatus = false;
            }

            public bool HasStateChanged(ISettingsManager settingsManager)
            {
                if (settingsManager == null)
                    return false;

                return IsEnabled != settingsManager.IsProjectWindowIconOverlayEnabled ||
                       Position != settingsManager.ProjectWindowIconOverlayPosition ||
                       DisplayDetailedStatus != settingsManager.DisplayDetailedProjectWindowIconOverlay;
            }
        }

        [SerializeReference]
        IAssetDataManager m_AssetDataManager;

        [SerializeReference]
        ISettingsManager m_SettingsManager;

        [SerializeReference]
        IProjectWindowProxy m_ProjectWindowProxy;

        [SerializeField]
        List<BaseAssetData> m_SerializedPendingAssetDataList = new();

        [SerializeField]
        ProjectWindowSettingsState m_SettingsState ;

        HashSet<BaseAssetData> m_PendingAssetData = new ();

        public override void OnDisable()
        {
            base.OnDisable();

            foreach (var assetData in m_PendingAssetData)
            {
                assetData.AssetDataChanged -= OnAssetDataChanged;
            }
            m_PendingAssetData.Clear();
        }

        [ServiceInjection]
        public void Inject(IAssetDataManager assetDataManager, ISettingsManager settingsManager, IProjectWindowProxy projectWindowProxy)
        {
            m_AssetDataManager = assetDataManager;
            m_SettingsManager = settingsManager;
            m_ProjectWindowProxy = projectWindowProxy;
        }

        public void OnBeforeSerialize()
        {
            m_SerializedPendingAssetDataList.AddRange(m_PendingAssetData.ToList());
        }

        public void OnAfterDeserialize()
        {
            m_PendingAssetData.Clear();
            m_PendingAssetData.UnionWith(m_SerializedPendingAssetDataList.Where(assetData => assetData != null));

            // Repaint the project window to apply the icon overlay settings
            m_SettingsState.ClearState();
            m_ProjectWindowProxy?.Repaint();
        }

        public void Update()
        {
            if (m_SettingsState.HasStateChanged(m_SettingsManager))
            {
                m_SettingsState.SetState(m_SettingsManager);
                m_ProjectWindowProxy?.Repaint();
            }
        }

        public void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            if (!m_SettingsState.IsEnabled || string.IsNullOrEmpty(guid))
                return;

            if (Event.current.type != EventType.Repaint)
                return;

            var assetStatus = GetStatus(guid);
            if (assetStatus.Status == ImportAttribute.ImportStatus.NoImport)
                return;

            // We do not draw the overlay icon in list view
            var isListView = selectionRect.width > selectionRect.height;
            if (!isListView)
            {
                var position = (ProjectIconOverlayPosition)m_SettingsManager.ProjectWindowIconOverlayPosition;
                DrawOverlayIcon.ForStatus(selectionRect, assetStatus, position);
            }
        }

        ProjectIconAssetStatus GetStatus(string guid)
        {
            // Check if the asset is imported
            var importedAssetInfos = m_AssetDataManager?.GetImportedAssetInfosFromFileGuid(guid);
            if (importedAssetInfos == null || importedAssetInfos.Count == 0)
                return s_NoImportStatus;

            var assetData = importedAssetInfos[0].AssetData;
            if (assetData == null)
                return s_NoImportStatus;

            // If the asset is imported and we are not using detailed status, return the connected status
            var useDetailedStatus = m_SettingsManager?.DisplayDetailedProjectWindowIconOverlay ?? false;
            if (!useDetailedStatus)
                return s_ConnectedStatus;

            // If using detailed status, load the asset data attributes if not already loaded
            if (assetData.AssetDataAttributeCollection == null && m_PendingAssetData.Add(assetData))
            {
                assetData.AssetDataChanged += OnAssetDataChanged;
                assetData.RefreshAssetDataAttributesAsync();
                return s_ConnectedStatus;
            }

            // Return the detailed status based on the import attribute
            return importedAssetInfos[0].AssetData?.AssetDataAttributeCollection?.GetAttribute<ImportAttribute>()?.Status switch
            {
                ImportAttribute.ImportStatus.NoImport => s_NoImportStatus,
                ImportAttribute.ImportStatus.UpToDate => s_ImportedStatus,
                ImportAttribute.ImportStatus.OutOfDate => s_OutOfDateStatus,
                ImportAttribute.ImportStatus.ErrorSync => s_ErrorStatus,
                _ => s_NoImportStatus
            };
        }

        void OnAssetDataChanged(BaseAssetData assetData, AssetDataEventType eventType)
        {
            if (eventType == AssetDataEventType.AssetDataAttributesChanged)
            {
                m_ProjectWindowProxy?.Repaint();

                assetData.AssetDataChanged -= OnAssetDataChanged;
                m_PendingAssetData.Remove(assetData);
            }
        }
    }
}

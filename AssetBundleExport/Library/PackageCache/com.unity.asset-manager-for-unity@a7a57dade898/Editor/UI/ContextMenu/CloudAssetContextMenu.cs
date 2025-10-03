using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ImportSettings = Unity.AssetManager.Editor.ImportSettings;

namespace Unity.AssetManager.UI.Editor
{
    class CloudAssetContextMenu : AssetContextMenu
    {
        public CloudAssetContextMenu(IUnityConnectProxy unityConnectProxy, IAssetDataManager assetDataManager, IAssetImporter assetImporter,
            ILinksProxy linksProxy, IAssetDatabaseProxy assetDatabaseProxy, IPageManager pageManager)
            : base(unityConnectProxy, assetDataManager, assetImporter,
                linksProxy, assetDatabaseProxy, pageManager) { }

        bool IsImporting => m_AssetImporter.IsImporting(TargetAssetData.Identifier);
        bool IsInProject => m_AssetDataManager.IsInProject(TargetAssetData.Identifier);

        public override void SetupContextMenuEntries(ContextualMenuPopulateEvent evt)
        {
            TaskUtils.TrackException(SetupContextMenuEntriesAsync(evt));
        }

        async Task SetupContextMenuEntriesAsync(ContextualMenuPopulateEvent evt)
        {
            ClearMenuEntries(evt);
            UpdateAllToLatest(evt);
            RemoveFromProjectEntry(evt);
            ShowInProjectEntry(evt);
            ShowInDashboardEntry(evt);
            await ImportEntry(evt);
            CancelImportEntry(evt);
            UntrackAssetEntry(evt);
        }

        static void ClearMenuEntries(ContextualMenuPopulateEvent evt)
        {
            for (var i = 0; i < evt.menu.MenuItems().Count; i++)
            {
                evt.menu.MenuItems().RemoveAt(0);
            }
        }

        async Task ImportEntry(ContextualMenuPopulateEvent evt)
        {
            if (IsImporting || !m_UnityConnectProxy.AreCloudServicesReachable)
                return;

            var permissionsManager = ServicesContainer.instance.Resolve<IPermissionsManager>();
            var importPermission = await permissionsManager.CheckPermissionAsync(TargetAssetData.Identifier.OrganizationId, TargetAssetData.Identifier.ProjectId, Constants.ImportPermission);

            var enabled = UIEnabledStates.HasPermissions.GetFlag(importPermission);
            enabled |= UIEnabledStates.ServicesReachable.GetFlag(m_UnityConnectProxy.AreCloudServicesReachable);
            enabled |= UIEnabledStates.IsImporting.GetFlag(IsImporting);
            enabled |= UIEnabledStates.CanImport.GetFlag(true); // We unfortunately don't have a way to check instantly if the asset is not empty, so we need to assume it is.

            var selectedAssetData = m_PageManager.ActivePage.SelectedAssets.Select(x => m_AssetDataManager.GetAssetData(x)).ToList();
            if (selectedAssetData.Count > 1 && selectedAssetData.Exists(ad => ad.Identifier.AssetId == TargetAssetData.Identifier.AssetId))
            {
                ImportEntryMultiple(evt, selectedAssetData, enabled);
            }
            else if (!selectedAssetData.Any() || selectedAssetData.First().Identifier.AssetId == TargetAssetData.Identifier.AssetId)
            {
                ImportEntrySingle(evt, enabled);
            }
        }

        void ImportEntrySingle(ContextualMenuPopulateEvent evt, UIEnabledStates states)
        {
            var status = AssetDataStatus.GetStatusOfImport(TargetAssetData?.AssetDataAttributeCollection);
            states |= UIEnabledStates.ValidStatus.GetFlag(status == null || !string.IsNullOrEmpty(status.ActionText));

            var text = AssetDetailsPageExtensions.GetImportButtonLabel(null, status);

            ImportTrigger trigger;
            if (status == null || string.IsNullOrEmpty(status.ActionText))
            {
                trigger = ImportTrigger.ImportContextMenu;
            }
            else
            {
                trigger = status.ActionText == Constants.ReimportActionText
                    ? ImportTrigger.ReimportContextMenu
                    : ImportTrigger.UpdateToLatestContextMenu;
            }

            AddMenuEntry(evt, text, states.IsImportAvailable(),
                _ =>
                {
                    m_AssetImporter.StartImportAsync(trigger, new List<BaseAssetData> {TargetAssetData}, new ImportSettings {Type = ImportOperation.ImportType.UpdateToLatest});
                    AnalyticsSender.SendEvent(new GridContextMenuItemSelectedEvent(!IsInProject
                        ? GridContextMenuItemSelectedEvent.ContextMenuItemType.Import
                        : GridContextMenuItemSelectedEvent.ContextMenuItemType.Reimport));
                });
        }

        void ImportEntryMultiple(ContextualMenuPopulateEvent evt, List<BaseAssetData> selectedAssetData, UIEnabledStates states)
        {
            var isContainedInvalidStatus = false;
            foreach (var status in selectedAssetData.Select(x => x?.AssetDataAttributeCollection.GetStatusOfImport()))
            {
                if (!(status == null || !string.IsNullOrEmpty(status.ActionText)))
                {
                    isContainedInvalidStatus = true;
                    break;
                }
            }

            states |= UIEnabledStates.ValidStatus.GetFlag(!isContainedInvalidStatus);

            AddMenuEntry(evt, L10n.Tr(Constants.ImportAllSelectedActionText), states.IsImportAvailable(),
                _ =>
                {
                    m_AssetImporter.StartImportAsync(ImportTrigger.ImportAllContextMenu, selectedAssetData, new ImportSettings {Type = ImportOperation.ImportType.UpdateToLatest});
                    AnalyticsSender.SendEvent(new GridContextMenuItemSelectedEvent(GridContextMenuItemSelectedEvent
                        .ContextMenuItemType.ImportAll));
                });
        }

        void CancelImportEntry(ContextualMenuPopulateEvent evt)
        {
            if (!IsImporting)
                return;

            AddMenuEntry(evt, L10n.Tr(AssetManagerCoreConstants.CancelImportActionText), true,
                _ =>
                {
                    m_AssetImporter.CancelImport(TargetAssetData.Identifier, true);
                    AnalyticsSender.SendEvent(new GridContextMenuItemSelectedEvent(GridContextMenuItemSelectedEvent
                        .ContextMenuItemType.CancelImport));
                });
        }

        void UntrackAssetEntry(ContextualMenuPopulateEvent evt)
        {
            if (!IsInProject || IsImporting)
                return;

            var selectedAssetData = m_PageManager.ActivePage.SelectedAssets.Select(x => m_AssetDataManager.GetAssetData(x)).ToList();
            if (selectedAssetData.Count > 1
                && selectedAssetData.Exists(asset => asset.Identifier.AssetId == TargetAssetData.Identifier.AssetId)
                && selectedAssetData.TrueForAll(x => x.AssetDataAttributeCollection.GetAttribute<ImportAttribute>()?.Status == ImportAttribute.ImportStatus.ErrorSync))
            {
                AddMenuEntry(evt, L10n.Tr(Constants.UntrackAssetsActionText), true,
                    _ =>
                    {
                        m_AssetImporter.StopTrackingAssets(selectedAssetData.Select(x => x.Identifier).ToList());
                    });
            }
            else if ((!selectedAssetData.Any() || selectedAssetData[0].Identifier.AssetId == TargetAssetData.Identifier.AssetId)
                     && TargetAssetData.AssetDataAttributeCollection.GetAttribute<ImportAttribute>()?.Status == ImportAttribute.ImportStatus.ErrorSync)
            {
                AddMenuEntry(evt, L10n.Tr(Constants.UntrackAssetActionText), true,
                    _ =>
                    {
                        m_AssetImporter.StopTrackingAssets(new List<AssetIdentifier> {TargetAssetData.Identifier});
                    });
            }
        }

        void RemoveFromProjectEntry(ContextualMenuPopulateEvent evt)
        {
            if (!IsInProject || IsImporting)
                return;

            var selectedAssetData = m_PageManager.ActivePage.SelectedAssets.Select(x => m_AssetDataManager.GetAssetData(x)).ToList();
            var removeAssets = m_AssetDataManager.FindExclusiveDependencies(selectedAssetData.Select(x => x.Identifier).ToList());
            if (selectedAssetData.Count > 1
                && selectedAssetData.Exists(asset => asset.Identifier.AssetId == TargetAssetData.Identifier.AssetId)
                && selectedAssetData.TrueForAll(x => m_AssetDataManager.IsInProject(x.Identifier)))
            {
                AddMenuEntry(evt, L10n.Tr(Constants.RemoveFromProjectAllSelectedActionText), true,
                    _ =>
                    {
                        m_AssetImporter.RemoveImports(removeAssets.ToList(), true);
                        AnalyticsSender.SendEvent(new GridContextMenuItemSelectedEvent(GridContextMenuItemSelectedEvent
                            .ContextMenuItemType.RemoveAll));
                    });
            }
            else if (!selectedAssetData.Any() || selectedAssetData[0].Identifier.AssetId == TargetAssetData.Identifier.AssetId)
            {
                AddMenuEntry(evt, L10n.Tr(Constants.RemoveFromProjectActionText), true,
                    _ =>
                    {
                        if (removeAssets.Count > 1)
                        {
                            m_AssetImporter.RemoveImports(removeAssets.ToList(), true);
                        }
                        else
                        {
                            m_AssetImporter.RemoveImport(TargetAssetData.Identifier, true);
                        }

                        AnalyticsSender.SendEvent(new GridContextMenuItemSelectedEvent(GridContextMenuItemSelectedEvent
                            .ContextMenuItemType.Remove));
                    });
            }
        }

        void ShowInProjectEntry(ContextualMenuPopulateEvent evt)
        {
            if (m_PageManager.ActivePage.SelectedAssets.Count > 1)
                return;

            if (!IsInProject || IsImporting)
                return;

            var hasFiles = TargetAssetData?.GetFiles()?.Where(f
                    => !string.IsNullOrEmpty(f?.Path) && !AssetDataDependencyHelper.IsASystemFile(Path.GetExtension(f.Path)))
                .Any() ?? false;
            if (!hasFiles)
                return;

            AddMenuEntry(evt, Constants.ShowInProjectActionText, true,
                _ =>
                {
                    m_AssetImporter.ShowInProject(TargetAssetData.Identifier);
                    AnalyticsSender.SendEvent(new GridContextMenuItemSelectedEvent(GridContextMenuItemSelectedEvent
                        .ContextMenuItemType.ShowInProject));
                });
        }

        void ShowInDashboardEntry(ContextualMenuPopulateEvent evt)
        {
            if (m_PageManager.ActivePage.SelectedAssets.Count > 1)
                return;

            if (!m_UnityConnectProxy.AreCloudServicesReachable)
                return;

            if (m_LinksProxy.CanOpenAssetManagerDashboard)
            {
                AddMenuEntry(evt, Constants.ShowInDashboardActionText, true,
                    _ =>
                    {
                        var identifier = TargetAssetData.Identifier;
                        m_LinksProxy.OpenAssetManagerDashboard(identifier);
                        AnalyticsSender.SendEvent(new GridContextMenuItemSelectedEvent(GridContextMenuItemSelectedEvent
                            .ContextMenuItemType.ShowInDashboard));
                    });
            }
        }

        void UpdateAllToLatest(ContextualMenuPopulateEvent evt)
        {
            if (!m_UnityConnectProxy.AreCloudServicesReachable)
                return;

            var selectedAssetData = m_PageManager.ActivePage.SelectedAssets.Select(x => m_AssetDataManager.GetAssetData(x)).ToList();
            if (selectedAssetData.Count == 1)
                return;

            var projectOrganizationProvider = ServicesContainer.instance.Resolve<IProjectOrganizationProvider>();
            string optionName = Constants.UpdateAllToLatestActionText;
            if (selectedAssetData.Count > 1)
            {
                // Show only if at least one of the selected assets is imported
                var anyImported = m_AssetDataManager.ImportedAssetInfos.Any(x =>
                    selectedAssetData.Any(y => y.Identifier.AssetId == x.Identifier.AssetId));
                if (!anyImported)
                    return;

                optionName = Constants.UpdateSelectedToLatestActionText;
            }
            else if (m_PageManager.ActivePage is CollectionPage)
            {
                optionName = projectOrganizationProvider.SelectedCollection.Name == null
                    ? Constants.UpdateProjectToLatestActionText
                    : Constants.UpdateCollectionToLatestActionText;
            }

            var enabled = m_AssetDataManager.ImportedAssetInfos.Any() && !IsImporting;
            AddMenuEntry(evt, optionName, enabled,
                (_) =>
                {
                    if (selectedAssetData.Count == 0)
                    {
                        ProjectInfo selectedProject = null;
                        CollectionInfo selectedCollection = null;

                        if (m_PageManager.ActivePage is CollectionPage)
                        {
                            selectedProject = projectOrganizationProvider.SelectedProject;
                            selectedCollection = projectOrganizationProvider.SelectedCollection;
                        }

                        TaskUtils.TrackException(m_AssetImporter.UpdateAllToLatestAsync(ImportTrigger.UpdateAllToLatestContextMenu, selectedProject, selectedCollection, CancellationToken.None));
                    }
                    else
                    {
                        TaskUtils.TrackException(m_AssetImporter.UpdateAllToLatestAsync(ImportTrigger.UpdateAllToLatestContextMenu, selectedAssetData, CancellationToken.None));
                    }

                    AnalyticsSender.SendEvent(new GridContextMenuItemSelectedEvent(GridContextMenuItemSelectedEvent
                        .ContextMenuItemType.UpdateAllToLatest));
                });
        }
    }
}

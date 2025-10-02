using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using Unity.AssetManager.Upload.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ImportSettings = Unity.AssetManager.Editor.ImportSettings;

namespace Unity.AssetManager.UI.Editor
{
    class MultiAssetDetailsPage : SelectionInspectorPage
    {
        static readonly string k_InspectorScrollviewContainerClassName = "inspector-page-content-container";

        static readonly string k_MultiSelectionFoldoutExpandedClassName = "multi-selection-foldout-expanded";
        static readonly string k_MultiSelectionRemoveName = "multi-selection-remove-button";
        static readonly string k_InspectorFooterContainerName = "footer-container";

        static readonly string k_UnimportedFoldoutName = "multi-selection-unimported-foldout";
        static readonly string k_ImportedFoldoutName = "multi-selection-imported-foldout";
        static readonly string k_UploadRemovedFoldoutName = "multi-selection-upload-removed-foldout";
        static readonly string k_UploadIgnoredFoldoutName = "multi-selection-upload-ignored-foldout";
        static readonly string k_UploadIncludedFoldoutName = "multi-selection-upload-included-foldout";

        static readonly string k_UnimportedFoldoutTitle = "Unimported";
        static readonly string k_ImportedFoldoutTitle = "Imported";
        static readonly string k_UploadRemovedFoldoutTitle = "Included";
        static readonly string k_UploadIgnoredFoldoutTitle = "Ignored Dependencies";
        static readonly string k_UploadIncludedFoldoutTitle = "Included Dependencies";

        readonly AssetDataSelection m_SelectedAssetsData = new();

        public enum FoldoutName
        {
            Unimported = 0,
            Imported = 1,
            UploadIgnored = 2,
            UploadIncluded = 3,
            UploadRemoved = 4
        }

        readonly Dictionary<FoldoutName, MultiSelectionFoldout> m_Foldouts = new();

        RemoveButton m_RemoveButton;
        VisualElement m_FooterContainer;
        OperationProgressBar m_OperationProgressBar;

        public MultiAssetDetailsPage(IAssetImporter assetImporter, IAssetOperationManager assetOperationManager,
            IStateManager stateManager, IPageManager pageManager, IAssetDataManager assetDataManager,
            IAssetDatabaseProxy assetDatabaseProxy, IProjectOrganizationProvider projectOrganizationProvider,
            ILinksProxy linksProxy, IUnityConnectProxy unityConnectProxy, IProjectIconDownloader projectIconDownloader,
            IPermissionsManager permissionsManager, IDialogManager dialogManager)
            : base(assetImporter, assetOperationManager, stateManager, pageManager, assetDataManager,
                assetDatabaseProxy, projectOrganizationProvider, linksProxy, unityConnectProxy, projectIconDownloader,
                permissionsManager, dialogManager)
        {
            BuildUxmlDocument();

            m_SelectedAssetsData.AssetDataChanged += OnAssetDataEvent;
        }

        public override bool IsVisible(int selectedAssetCount)
        {
            return selectedAssetCount > 1;
        }

        protected sealed override void BuildUxmlDocument()
        {
            base.BuildUxmlDocument();

            var container = m_ScrollView.Q<VisualElement>(k_InspectorScrollviewContainerClassName);

            m_Foldouts[FoldoutName.Unimported] = new MultiSelectionFoldout(container, k_UnimportedFoldoutTitle, k_UnimportedFoldoutName,
                Constants.ImportActionText, ImportUnimportedAssetsAsync, k_MultiSelectionFoldoutExpandedClassName);

            m_Foldouts[FoldoutName.Imported] = new MultiSelectionFoldout(container, k_ImportedFoldoutTitle, k_ImportedFoldoutName,
                Constants.ReimportActionText, ReImportAssetsAsync, k_MultiSelectionFoldoutExpandedClassName);

            m_Foldouts[FoldoutName.UploadRemoved] = new MultiSelectionFoldout(container, k_UploadRemovedFoldoutTitle, k_UploadRemovedFoldoutName,
                Constants.RemoveAll, RemoveUploadAssets, k_MultiSelectionFoldoutExpandedClassName);

            m_Foldouts[FoldoutName.UploadIgnored] = new MultiSelectionFoldout(container, k_UploadIgnoredFoldoutTitle, k_UploadIgnoredFoldoutName,
                Constants.IncludeAll, IncludeUploadAssets, k_MultiSelectionFoldoutExpandedClassName);

            m_Foldouts[FoldoutName.UploadIncluded] = new MultiSelectionFoldout(container, k_UploadIncludedFoldoutTitle, k_UploadIncludedFoldoutName,
                Constants.IgnoreAll, IgnoreUploadAssets, k_MultiSelectionFoldoutExpandedClassName);

            foreach (var foldout in m_Foldouts)
            {
                foldout.Value.RegisterValueChangedCallback(value =>
                {
                    m_StateManager.MultiSelectionFoldoutsValues[(int)foldout.Key] = value;
                    RefreshScrollView();
                });
                foldout.Value.Expanded = m_StateManager.MultiSelectionFoldoutsValues[(int)foldout.Key];
            }

            m_FooterContainer = this.Q<VisualElement>(k_InspectorFooterContainerName);
            m_OperationProgressBar = new OperationProgressBar(CancelOrClearImport);
            m_FooterContainer.contentContainer.hierarchy.Add(m_OperationProgressBar);

            m_RemoveButton = new RemoveButton(true)
            {
                text = L10n.Tr(Constants.RemoveAllFromProjectActionText),
                tooltip = L10n.Tr(Constants.RemoveAllFromProjectToolTip),
                name = k_MultiSelectionRemoveName
            };
            m_RemoveButton.RemoveWithExclusiveDependencies += RemoveAllFromLocalProject;
            m_RemoveButton.RemoveOnlySelected += RemoveSelectedFromLocalProject;
            m_RemoveButton.StopTracking += StopTracking;
            m_RemoveButton.StopTrackingOnlySelected += StopTrackingOnlySelected;

            m_FooterContainer.contentContainer.hierarchy.Add(m_RemoveButton);

            // We need to manually refresh once to make sure the UI is updated when the window is opened.
            if (m_PageManager.ActivePage == null)
                return;

            m_SelectedAssetsData.Selection = m_AssetDataManager.GetAssetsData(m_PageManager.ActivePage.SelectedAssets);
            RefreshUI();
        }

        void CancelOrClearImport()
        {
            var operations = new List<AssetDataOperation>();
            foreach (var id in m_SelectedAssetsData.Selection.Select(x => x.Identifier))
            {
                var operation = m_AssetOperationManager.GetAssetOperation(id);
                Utilities.DevAssert(operation != null, $"Operation for asset {id} not found");
                if (operation != null)
                {
                    operations.Add(operation);
                }
            }

            if (operations.Exists(o => o.Status == OperationStatus.InProgress))
            {
                m_AssetImporter.CancelBulkImport(m_SelectedAssetsData.Selection.Select(x => x.Identifier).ToList(), true);
            }
            else
            {
                m_AssetOperationManager.ClearFinishedOperations();
            }
        }

        protected override Task SelectAssetDataAsync(IReadOnlyCollection<BaseAssetData> assetData)
        {
            if (assetData == null || assetData.Count == 0)
            {
                m_SelectedAssetsData.Clear();
                return Task.CompletedTask;
            }

            // Check if assetData is a subset of m_SelectedAssetsData
            if (assetData.Count < m_SelectedAssetsData.Selection.Count && !assetData.Except(m_SelectedAssetsData.Selection).Any())
            {
                RemoveItemsFromFoldouts(m_SelectedAssetsData.Selection.Except(assetData));
                m_SelectedAssetsData.Selection = assetData;
                RefreshTitleAndButtons();
            }
            else
            {
                m_SelectedAssetsData.Selection = assetData;
                RefreshUI();
            }

            RefreshScrollView();
            return Task.CompletedTask;
        }

        protected override void OnOperationProgress(AssetDataOperation operation)
        {
            if(!UIElementsUtils.IsDisplayed(this) || m_SelectedAssetsData == null || !m_SelectedAssetsData.Selection.Any() || !m_SelectedAssetsData.Exists(x => x.Identifier.Equals(operation.Identifier)))
                return;

            m_OperationProgressBar.Refresh(operation);

            RefreshUI();
        }

        protected override void OnOperationFinished(AssetDataOperation operation)
        {
            if (!UIElementsUtils.IsDisplayed(this) || m_SelectedAssetsData == null || !m_SelectedAssetsData.Selection.Any() || !m_SelectedAssetsData.Exists(x => x.Identifier.Equals(operation.Identifier)))
                return;

            RefreshUI();
        }

        protected override void OnImportedAssetInfoChanged(AssetChangeArgs args)
        {
            if (!UIElementsUtils.IsDisplayed(this))
                return;

            if (m_SelectedAssetsData == null || !m_SelectedAssetsData.Selection.Any())
                return;

            var last = m_SelectedAssetsData.Selection.Last();
            foreach (var assetData in m_SelectedAssetsData.Selection)
            {
                if (args.Added.Concat(args.Updated).Concat(args.Removed)
                    .Any(a => a.Equals(assetData?.Identifier)))
                {
                    break;
                }

                if (assetData.Equals(last))
                    return;
            }

            // In case of an import, force a full refresh of the displayed information
            TaskUtils.TrackException(SelectAssetDataAsync(m_SelectedAssetsData.Selection));
        }

        protected override void OnAssetDataChanged(AssetChangeArgs args)
        {
            m_SelectedAssetsData.Selection = m_AssetDataManager.GetAssetsData(m_PageManager.ActivePage.SelectedAssets);
            RefreshUI();
        }

        protected override void OnCloudServicesReachabilityChanged(bool cloudServiceReachable)
        {
            RefreshUI();
        }

        void OnAssetDataEvent(BaseAssetData assetData, AssetDataEventType eventType)
        {
            RefreshUI();
        }

        void RefreshTitleAndButtons()
        {
            // Refresh Title
            m_TitleLabel.text = L10n.Tr(m_SelectedAssetsData.Selection.Count + " " + Constants.AssetsSelectedTitle);

            // Refresh RemoveImportButton
            if (m_PageManager.ActivePage is not UploadPage)
            {
                var removable = m_SelectedAssetsData.Selection.Where(x => m_AssetDataManager.IsInProject(x.Identifier)).ToList();
                var isEnabled = removable.Count > 0;
                m_RemoveButton.SetEnabled(isEnabled);
                m_RemoveButton.text = isEnabled ?
                    $"{L10n.Tr(Constants.RemoveAllFromProjectActionText)} ({m_AssetDataManager.FindExclusiveDependencies(removable.Select(x => x.Identifier)).Count})" :
                    L10n.Tr(Constants.RemoveAllFromProjectActionText);
            }

            // Refresh ProgressBar
            bool atLeastOneProcess = false;
            foreach (var assetData in m_SelectedAssetsData.Selection)
            {
                var operation = m_AssetOperationManager.GetAssetOperation(assetData.Identifier);
                if (operation != null)
                {
                    atLeastOneProcess = true;
                    m_OperationProgressBar.Refresh(operation);
                }
            }

            if (!atLeastOneProcess)
            {
                UIElementsUtils.Hide(m_OperationProgressBar);
            }
        }

        void RefreshUI()
        {
            if (!IsVisible(m_SelectedAssetsData.Selection.Count))
                return;

            RefreshFoldoutUI();
            RefreshTitleAndButtons();
            RefreshUploadMetadataContainer();
        }

        void RefreshFoldoutUI()
        {
            // Check which page is displayed
            if(m_PageManager.ActivePage is UploadPage)
            {
                RefreshUploadPageFoldoutUI();
            }
            else
            {
                RefreshAssetPageFoldoutUI();
            }
        }

        void ClearFoldout(FoldoutName foldoutName)
        {
            m_Foldouts[foldoutName].StartPopulating();
            m_Foldouts[foldoutName].Clear();
            m_Foldouts[foldoutName].StopPopulating();
            m_Foldouts[foldoutName].RefreshFoldoutStyleBasedOnExpansionStatus();
        }

        void PopulateFoldout(FoldoutName foldoutName, IEnumerable<BaseAssetData> items)
        {
            m_Foldouts[foldoutName].StartPopulating();
            var assetDatas = items.ToList();
            if (assetDatas.Any())
            {
                m_Foldouts[foldoutName].Populate(null, assetDatas);
            }
            else
            {
                m_Foldouts[foldoutName].Clear();
            }

            m_Foldouts[foldoutName].StopPopulating();
            m_Foldouts[foldoutName].RefreshFoldoutStyleBasedOnExpansionStatus();
        }

        void RemoveItemsFromFoldouts(IEnumerable<BaseAssetData> items)
        {
            foreach (var foldout in m_Foldouts)
            {
                m_Foldouts[foldout.Key].RemoveItems(items);
            }
        }

        void RefreshAssetPageFoldoutUI()
        {
            UIElementsUtils.SetDisplay(m_RemoveButton, true);

            ClearFoldout(FoldoutName.UploadRemoved);
            ClearFoldout(FoldoutName.UploadIgnored);
            ClearFoldout(FoldoutName.UploadIncluded);

            PopulateFoldout(FoldoutName.Unimported, m_SelectedAssetsData.Selection.Where(x => !m_AssetDataManager.IsInProject(x.Identifier)));
            PopulateFoldout(FoldoutName.Imported, m_SelectedAssetsData.Selection.Where(x => m_AssetDataManager.IsInProject(x.Identifier)));
        }

        void RefreshUploadPageFoldoutUI()
        {
            UIElementsUtils.SetDisplay(m_RemoveButton, false);

            ClearFoldout(FoldoutName.Unimported);
            ClearFoldout(FoldoutName.Imported);

            var uploadAssetData = new List<UploadAssetData>();
            if (m_SelectedAssetsData.Exists(x => x is UploadAssetData))
            {
                uploadAssetData = m_SelectedAssetsData.Selection.Cast<UploadAssetData>().ToList();
            }

            PopulateFoldout(FoldoutName.UploadRemoved, uploadAssetData.Where(x => x.CanBeRemoved));
            PopulateFoldout(FoldoutName.UploadIgnored, uploadAssetData.Where(x => x.CanBeIgnored && x.IsIgnored));
            PopulateFoldout(FoldoutName.UploadIncluded, uploadAssetData.Where(x => x.CanBeIgnored && !x.IsIgnored));
        }

        void RemoveUploadAssets()
        {
            foreach (var assetData in m_SelectedAssetsData.Selection.Cast<UploadAssetData>().Where(x => x.CanBeRemoved).ToList())
            {
                // Similar code to UploadContextMenu.RemoveAssetEntry, find a way to reuse it
                if (ServicesContainer.instance.Resolve<IPageManager>().ActivePage is not UploadPage uploadPage)
                    return;

                uploadPage.RemoveAsset(assetData);
            }
        }

        void IgnoreUploadAssets()
        {
            m_PageManager.ActivePage.ToggleAsset(m_SelectedAssetsData.Selection.Cast<UploadAssetData>().Where(x => x.CanBeIgnored && !x.IsIgnored).Select(a => a.Identifier).FirstOrDefault(), false);
        }

        void IncludeUploadAssets()
        {
            m_PageManager.ActivePage.ToggleAsset(m_SelectedAssetsData.Selection.Cast<UploadAssetData>().Where(x => x.CanBeIgnored && x.IsIgnored).Select(a => a.Identifier).FirstOrDefault(), true);
        }

        void ImportListAsync(List<BaseAssetData> assetsData, bool isReimport)
        {
            try
            {
                var source = isReimport ? ImportTrigger.ReimportMultiselect : ImportTrigger.ImportMultiselect;
                m_AssetImporter.StartImportAsync(source, assetsData, new ImportSettings {Type = ImportOperation.ImportType.UpdateToLatest});
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        void ImportUnimportedAssetsAsync()
        {
            var unimportedAssets = m_PageManager.ActivePage.SelectedAssets.Where(x => !m_AssetDataManager.IsInProject(x))
                .Select(x => m_AssetDataManager.GetAssetData(x)).ToList();

            AnalyticsSender.SendEvent(unimportedAssets.Count > 1
                ? new DetailsButtonClickedEvent(DetailsButtonClickedEvent.ButtonType.ImportAll)
                : new DetailsButtonClickedEvent(DetailsButtonClickedEvent.ButtonType.Import));

            ImportListAsync(unimportedAssets, false);
        }

        void ReImportAssetsAsync()
        {
            var importedAssets = m_PageManager.ActivePage.SelectedAssets.Where(x => m_AssetDataManager.IsInProject(x))
                .Select(x => m_AssetDataManager.GetAssetData(x)).ToList();

            AnalyticsSender.SendEvent(importedAssets.Count > 1
                ? new DetailsButtonClickedEvent(DetailsButtonClickedEvent.ButtonType.ReImportAll)
                : new DetailsButtonClickedEvent(DetailsButtonClickedEvent.ButtonType.Reimport));

            ImportListAsync(importedAssets, true);
        }

        void RemoveAllFromLocalProject()
        {
            var importedAssetIdentifiers = m_PageManager.ActivePage.SelectedAssets.Where(x => m_AssetDataManager.IsInProject(x)).ToList();

            AnalyticsSender.SendEvent(importedAssetIdentifiers.Count > 1
                ? new DetailsButtonClickedEvent(DetailsButtonClickedEvent.ButtonType.RemoveAll)
                : new DetailsButtonClickedEvent(DetailsButtonClickedEvent.ButtonType.Remove));

            try
            {
                var removeAssets = m_AssetDataManager.FindExclusiveDependencies(importedAssetIdentifiers);
                m_AssetImporter.RemoveImports(removeAssets.ToList(), true);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        void RemoveSelectedFromLocalProject()
        {
            var importedAssetIdentifiers = m_PageManager.ActivePage.SelectedAssets.Where(x => m_AssetDataManager.IsInProject(x)).ToList();

            AnalyticsSender.SendEvent(importedAssetIdentifiers.Count > 1
                ? new DetailsButtonClickedEvent(DetailsButtonClickedEvent.ButtonType.RemoveSelectedAll)
                : new DetailsButtonClickedEvent(DetailsButtonClickedEvent.ButtonType.RemoveSelected));

            try
            {
                m_AssetImporter.RemoveImports(importedAssetIdentifiers, true);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        void StopTracking()
        {
            var importedAssetIdentifiers = m_PageManager.ActivePage.SelectedAssets.Where(x => m_AssetDataManager.IsInProject(x)).ToList();

            AnalyticsSender.SendEvent(importedAssetIdentifiers.Count > 1
                ? new DetailsButtonClickedEvent(DetailsButtonClickedEvent.ButtonType.StopTrackingAll)
                : new DetailsButtonClickedEvent(DetailsButtonClickedEvent.ButtonType.StopTracking));

            try
            {
                var removeAssets = m_AssetDataManager.FindExclusiveDependencies(importedAssetIdentifiers);
                m_AssetImporter.StopTrackingAssets(removeAssets.ToList());
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        void StopTrackingOnlySelected()
        {
            var importedAssetIdentifiers = m_PageManager.ActivePage.SelectedAssets.Where(x => m_AssetDataManager.IsInProject(x)).ToList();

            AnalyticsSender.SendEvent(importedAssetIdentifiers.Count > 1
                ? new DetailsButtonClickedEvent(DetailsButtonClickedEvent.ButtonType.StopTrackingSelectedAll)
                : new DetailsButtonClickedEvent(DetailsButtonClickedEvent.ButtonType.StopTrackingSelected));

            try
            {
                m_AssetImporter.StopTrackingAssets(importedAssetIdentifiers);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }
    }
}

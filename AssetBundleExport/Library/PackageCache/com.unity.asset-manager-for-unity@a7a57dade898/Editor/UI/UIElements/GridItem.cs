using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using Unity.AssetManager.Upload.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    static partial class UssStyle
    {
        public static readonly string GridViewStyleClassName = "grid-view";
        public static readonly string GridViewRowStyleClassName = GridViewStyleClassName + "--row";
        public static readonly string GridViewDummyItemUssClassName = GridViewStyleClassName + "--item-dummy";
        public static readonly string GridItemStyleClassName = "grid-view--item";
        public static readonly string ItemLabel = GridItemStyleClassName + "-label";
        public static readonly string ItemHighlight = GridItemStyleClassName + "-selected";
        public static readonly string ItemOverlay = GridItemStyleClassName + "-overlay";
        public static readonly string ItemIgnore = GridItemStyleClassName + "-ignore";
        public static readonly string ItemToggle = GridItemStyleClassName + "-toggle";
        public static readonly string ItemHovered = GridItemStyleClassName + "-hovered";
    }

    class GridItem : VisualElement, IGridItem
    {
        readonly Label m_AssetNameLabel;
        readonly AssetPreview m_AssetPreview;
        readonly LoadingIcon m_LoadingIcon;
        readonly IAssetOperationManager m_OperationManager;
        readonly IUnityConnectProxy m_UnityConnectProxy;
        readonly OperationProgressBar m_OperationProgressBar;
        readonly IPageManager m_PageManager;
        readonly IAssetDataManager m_AssetDataManager;
        readonly IUploadManager m_UploadManager;

        AssetContextMenu m_ContextMenu;
        ContextualMenuManipulator m_ContextualMenuManipulator;
        BaseAssetData m_AssetData;

        public BaseAssetData AssetData => m_AssetData;

        internal GridItem(IUnityConnectProxy unityConnectProxy, IAssetOperationManager operationManager, IPageManager pageManager, IAssetDataManager assetDataManager, IUploadManager uploadManager)
        {
            m_UnityConnectProxy = unityConnectProxy;
            m_PageManager = pageManager;
            m_OperationManager = operationManager;
            m_AssetDataManager = assetDataManager;
            m_UploadManager = uploadManager;

            AddToClassList(UssStyle.GridItemStyleClassName);

            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeave);

            _ = new ClickOrDragStartManipulator(this, OnPointerUp, OnPointerDown, OnDragStart);

            m_AssetPreview = new AssetPreview();

            m_AssetNameLabel = new Label();
            m_AssetNameLabel.AddToClassList(UssStyle.ItemLabel);

            m_LoadingIcon = new LoadingIcon();
            UIElementsUtils.Hide(m_LoadingIcon);

            m_OperationProgressBar = new OperationProgressBar
            {
                pickingMode = PickingMode.Ignore
            };

            var overlay = new VisualElement
            {
                pickingMode = PickingMode.Ignore
            };
            overlay.AddToClassList(UssStyle.ItemOverlay);

            Add(m_AssetPreview);
            Add(overlay);
            Add(m_AssetNameLabel);
            Add(m_LoadingIcon);
            Add(m_OperationProgressBar);
        }

        void OnAssetPreviewToggleValueChanged(bool value)
        {
            if (m_AssetData == null)
                return;

            m_PageManager.ActivePage.ToggleAsset(m_AssetData.Identifier, value);
        }

        public void BindWithItem(BaseAssetData assetData)
        {
            if (m_AssetData != null && m_AssetData.Identifier.Equals(assetData?.Identifier))
                return;

            if (m_AssetData != null)
            {
                m_AssetData.AssetDataChanged -= OnAssetDataChanged;
            }

            m_AssetData = assetData;

            if (m_AssetData != null)
            {
                m_AssetData.AssetDataChanged += OnAssetDataChanged;
            }

            // Clear the thumbnail before setting the new one to avoid seeing the old thumbnail while the new one is loading
            m_AssetPreview.ClearPreview();

            Refresh();
        }

        void OnAssetDataChanged(BaseAssetData obj, AssetDataEventType eventType)
        {
            if (obj != m_AssetData)
                return;

            switch (eventType)
            {
                case AssetDataEventType.ThumbnailChanged:
                    m_AssetPreview.SetThumbnail(obj.Thumbnail);
                    break;
                case AssetDataEventType.AssetDataAttributesChanged:
                    m_AssetPreview.SetStatuses(AssetDataStatus.GetOverallStatus(obj.AssetDataAttributeCollection));
                    break;
                case AssetDataEventType.PrimaryFileChanged:
                    m_AssetPreview.SetAssetType(obj.PrimaryExtension);
                    break;
                case AssetDataEventType.ToggleValueChanged:
                    RefreshToggle();
                    break;
            }
        }

        void Refresh()
        {
            if (m_AssetData == null)
                return;
            
            if (m_ContextMenu == null)
            {
                InitContextMenu(m_AssetData);
            }
            else if (!ServicesContainer.instance.Resolve<IContextMenuBuilder>()
                         .IsContextMenuMatchingAssetDataType(m_AssetData.GetType(), m_ContextMenu.GetType()))
            {
                this.RemoveManipulator(m_ContextualMenuManipulator);
                InitContextMenu(m_AssetData);
            }

            if (m_ContextMenu != null)
            {
                m_ContextMenu.TargetAssetData = m_AssetData;
            }

            RefreshHighlight();

            m_AssetNameLabel.text = m_AssetData.Name;
            m_AssetNameLabel.tooltip = m_AssetData.Name;

            m_OperationProgressBar.Refresh(m_OperationManager.GetAssetOperation(m_AssetData.Identifier));

            m_AssetPreview.SetStatuses(AssetDataStatus.GetOverallStatus(m_AssetData.AssetDataAttributeCollection));
            m_AssetPreview.SetThumbnail(m_AssetData.Thumbnail);
            m_AssetPreview.SetAssetType(m_AssetData.PrimaryExtension);

            RefreshToggle();

            var tasks = new List<Task>
            {
                m_AssetData.ResolveDatasetsAsync(),
                m_AssetData.GetThumbnailAsync(),
                m_AssetData.RefreshAssetDataAttributesAsync()
            };

            _ = WaitForResultsAsync(tasks);
        }

        public event Action<PointerDownEvent> PointerDownAction;
        public event Action<PointerUpEvent> PointerUpAction;
        public event Action DragStartedAction;

        void OnAttachToPanel(AttachToPanelEvent evt)
        {
            m_UnityConnectProxy.CloudServicesReachabilityChanged += OnCloudServicesReachabilityChanged;
            m_PageManager.SelectedAssetChanged += OnSelectedAssetChanged;
            m_OperationManager.OperationProgressChanged += RefreshOperationProgress;
            m_OperationManager.OperationFinished += RefreshOperationProgress;
            m_OperationManager.OperationCleared += OnOperationCleared;
            m_AssetDataManager.ImportedAssetInfoChanged += OnImportedAssetInfoChanged;
            m_AssetPreview.ToggleValueChanged += OnAssetPreviewToggleValueChanged;
            m_UploadManager.UploadBegan += OnUploadBegan;
        }

        void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            m_UnityConnectProxy.CloudServicesReachabilityChanged -= OnCloudServicesReachabilityChanged;
            m_PageManager.SelectedAssetChanged -= OnSelectedAssetChanged;
            m_OperationManager.OperationProgressChanged -= RefreshOperationProgress;
            m_OperationManager.OperationFinished -= RefreshOperationProgress;
            m_OperationManager.OperationCleared -= OnOperationCleared;
            m_AssetDataManager.ImportedAssetInfoChanged -= OnImportedAssetInfoChanged;
            m_AssetPreview.ToggleValueChanged -= OnAssetPreviewToggleValueChanged;
            m_UploadManager.UploadBegan -= OnUploadBegan;
        }

        void OnMouseEnter(MouseEnterEvent evt)
        {
            AddToClassList(UssStyle.ItemHovered);
        }

        void OnMouseLeave(MouseLeaveEvent evt)
        {
            RemoveFromClassList(UssStyle.ItemHovered);
        }

        void InitContextMenu(BaseAssetData assetData)
        {
            if(assetData == null)
                return;

            m_ContextMenu = (AssetContextMenu) ServicesContainer.instance.Resolve<IContextMenuBuilder>()
                .BuildContextMenu(assetData.GetType());
            m_ContextualMenuManipulator = new ContextualMenuManipulator(m_ContextMenu.SetupContextMenuEntries);
            this.AddManipulator(m_ContextualMenuManipulator);
        }

        void RefreshOperationProgress(AssetDataOperation operation)
        {
            if(TrackedAssetIdentifier.IsFromSameAsset(operation.Identifier, m_AssetData?.Identifier))
            {
                m_OperationProgressBar.Refresh(operation);
            }
        }

        void OnOperationCleared(TrackedAssetIdentifier identifier)
        {
            if (TrackedAssetIdentifier.IsFromSameAsset(identifier, m_AssetData?.Identifier))
            {
                m_OperationProgressBar.Refresh(null);
            }
        }

        void OnCloudServicesReachabilityChanged(bool cloudServicesReachable)
        {
            Refresh();
        }

        void OnImportedAssetInfoChanged(AssetChangeArgs assetChangeArgs)
        {
            if (m_AssetData == null)
                return;

            if (assetChangeArgs.Added.Any(a => a.AssetId == m_AssetData.Identifier.AssetId)
                || assetChangeArgs.Removed.Any(a => a.AssetId == m_AssetData.Identifier.AssetId)
                || assetChangeArgs.Updated.Any(a => a.AssetId == m_AssetData.Identifier.AssetId))
            {
                var assetData = m_AssetDataManager.GetAssetData(m_AssetData.Identifier);
                m_AssetData = null;
                BindWithItem(assetData);

                var operation = m_OperationManager.GetAssetOperation(assetData.Identifier);
                m_OperationProgressBar.Refresh(operation);
            }
        }

        void OnSelectedAssetChanged(IPage page, IEnumerable<AssetIdentifier> assets)
        {
            if (m_AssetData == null)
                return;

            var isSelected = m_PageManager.ActivePage.SelectedAssets.Any(i =>
                new TrackedAssetIdentifier(i).Equals(new TrackedAssetIdentifier(AssetData.Identifier)));
            if (isSelected)
            {
                Refresh();
            }
            else
            {
                RefreshHighlight();
            }
        }

        void OnUploadBegan()
        {
            m_AssetPreview.Toggle.SetEnabled(false);
        }

        async Task WaitForResultsAsync(IReadOnlyCollection<Task> tasks)
        {
            m_LoadingIcon.PlayAnimation();
            UIElementsUtils.Show(m_LoadingIcon);

            await TaskUtils.WaitForTasksWithHandleExceptions(tasks);

            m_LoadingIcon.StopAnimation();
            UIElementsUtils.Hide(m_LoadingIcon);
        }

        void RefreshHighlight()
        {
            if (m_AssetData == null)
                return;

            var isSelected = m_PageManager.ActivePage.SelectedAssets.Any(i =>
                TrackedAssetIdentifier.IsFromSameAsset(i, m_AssetData.Identifier));
            if (isSelected)
            {
                AddToClassList(UssStyle.ItemHighlight);
            }
            else
            {
                RemoveFromClassList(UssStyle.ItemHighlight);
            }
        }

        void RefreshToggle()
        {
            if (m_AssetData is not UploadAssetData uploadAssetData) // TODO GridItem should not be aware of UploadAssetData
                return;

            m_AssetPreview.EnableInClassList("asset-preview--upload", uploadAssetData.CanBeIgnored);

            m_AssetPreview.Toggle.SetValueWithoutNotify(!uploadAssetData.IsIgnored);
            m_AssetPreview.Toggle.tooltip = uploadAssetData.IsIgnored
                ? L10n.Tr(Constants.IncludeToggleTooltip)
                : L10n.Tr(Constants.IgnoreToggleTooltip);
            m_AssetPreview.Toggle.SetEnabled(!m_UploadManager.IsUploading);

            EnableInClassList(UssStyle.ItemIgnore, uploadAssetData.IsIgnored);
            tooltip = uploadAssetData.IsIgnored ? L10n.Tr(Constants.IgnoreAssetToolTip) : "";
        }

        void OnPointerDown(PointerDownEvent e)
        {
            PointerDownAction?.Invoke(e);
        }

        void OnPointerUp(PointerUpEvent e)
        {
            PointerUpAction?.Invoke(e);
        }

        void OnDragStart()
        {
            DragStartedAction?.Invoke();
        }
    }
}

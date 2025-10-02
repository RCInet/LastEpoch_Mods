using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    abstract class SelectionInspectorPage : VisualElement
    {
        protected readonly IAssetImporter m_AssetImporter;
        protected readonly IAssetOperationManager m_AssetOperationManager;
        protected readonly IPageManager m_PageManager;
        protected readonly IAssetDataManager m_AssetDataManager;
        protected readonly IProjectOrganizationProvider m_ProjectOrganizationProvider;
        protected readonly IProjectIconDownloader m_ProjectIconDownloader;
        protected readonly IPermissionsManager m_PermissionsManager;
        protected readonly IAssetDatabaseProxy m_AssetDatabaseProxy;
        protected readonly ILinksProxy m_LinksProxy;
        protected readonly IUnityConnectProxy m_UnityConnectProxy;
        protected readonly IStateManager m_StateManager;
        protected readonly IDialogManager m_DialogManager;

        static readonly string k_InspectorPageContainerUxml = "InspectorPageContainer";
        static readonly string k_InspectorPageScrollviewClassName = "inspector-page-scrollview";
        static readonly string k_InspectorPageCloseButtonClassName = "closeButton";
        static readonly string k_InspectorPageTitleClassName = "inspector-page-title";

        protected ScrollView m_ScrollView;
        protected Button m_CloseButton;
        protected Label m_TitleLabel;
        protected VisualElement m_UploadMetadataContainer;

        protected SelectionInspectorPage(IAssetImporter assetImporter,
            IAssetOperationManager assetOperationManager,
            IStateManager stateManager,
            IPageManager pageManager,
            IAssetDataManager assetDataManager,
            IAssetDatabaseProxy assetDatabaseProxy,
            IProjectOrganizationProvider projectOrganizationProvider,
            ILinksProxy linksProxy,
            IUnityConnectProxy unityConnectProxy,
            IProjectIconDownloader projectIconDownloader,
            IPermissionsManager permissionsManager,
            IDialogManager dialogManager)
        {
            m_AssetImporter = assetImporter;
            m_AssetOperationManager = assetOperationManager;
            m_PageManager = pageManager;
            m_AssetDataManager = assetDataManager;
            m_ProjectOrganizationProvider = projectOrganizationProvider;
            m_ProjectIconDownloader = projectIconDownloader;
            m_PermissionsManager = permissionsManager;
            m_AssetDatabaseProxy = assetDatabaseProxy;
            m_LinksProxy = linksProxy;
            m_UnityConnectProxy = unityConnectProxy;
            m_StateManager = stateManager;
            m_DialogManager = dialogManager;
        }

        protected virtual void BuildUxmlDocument()
        {
            var treeAsset = UIElementsUtils.LoadUXML(k_InspectorPageContainerUxml);
            treeAsset.CloneTree(this);

            m_ScrollView = this.Q<ScrollView>(k_InspectorPageScrollviewClassName);
            m_ScrollView.viewDataKey = k_InspectorPageScrollviewClassName;
            m_CloseButton = this.Q<Button>(k_InspectorPageCloseButtonClassName);
            m_TitleLabel = this.Q<Label>(k_InspectorPageTitleClassName);

            // Upload metadata container
            m_UploadMetadataContainer = m_ScrollView.contentContainer.Q<VisualElement>("upload-metadata-container");
            m_UploadMetadataContainer.Add(new UploadMetadataContainer(m_PageManager, m_AssetDataManager, m_ProjectOrganizationProvider, m_LinksProxy));
            RefreshUploadMetadataContainer(); // Hide the container by default

            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
        }

        protected void RefreshScrollView()
        {
            // Bug in UI Toolkit where the scrollview does not update its size when the foldout is expanded/collapsed.
            // NOTE: Cannot reproduce on 2022.3
            schedule.Execute(_ => { m_ScrollView.verticalScrollerVisibility = ScrollerVisibility.Auto; })
                .StartingIn(25);
        }

        protected void RefreshUploadMetadataContainer()
        {
            if(m_UploadMetadataContainer == null || m_PageManager?.ActivePage == null)
                return;

            UIElementsUtils.SetDisplay(m_UploadMetadataContainer,
                ((BasePage)m_PageManager.ActivePage).DisplayUploadMetadata);
        }

        public async Task SelectedAsset(List<AssetIdentifier> assets)
        {
            var data = new List<BaseAssetData>();

            foreach (var asset in assets)
            {
                var assetData = await m_AssetDataManager.GetOrSearchAssetData(asset, default);
                if (assetData != null)
                {
                    data.Add(assetData);
                }
            }

            await SelectAssetDataAsync(data);
        }

        public async Task SelectionCleared()
        {
            await SelectAssetDataAsync(null);
        }

        public abstract bool IsVisible(int selectedAssetCount);

        protected abstract Task SelectAssetDataAsync(IReadOnlyCollection<BaseAssetData> assetData);

        protected virtual void OnAttachToPanel(AttachToPanelEvent evt)
        {
            m_UnityConnectProxy.CloudServicesReachabilityChanged += OnCloudServicesReachabilityChanged;
            m_AssetOperationManager.OperationProgressChanged += OnOperationProgress;
            m_AssetOperationManager.OperationFinished += OnOperationFinished;
            m_AssetDataManager.ImportedAssetInfoChanged += OnImportedAssetInfoChanged;
            m_AssetDataManager.AssetDataChanged += OnAssetDataChanged;
            m_CloseButton.clicked += OnCloseButton;
        }

        protected virtual void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            m_UnityConnectProxy.CloudServicesReachabilityChanged -= OnCloudServicesReachabilityChanged;
            m_AssetOperationManager.OperationProgressChanged -= OnOperationProgress;
            m_AssetOperationManager.OperationFinished -= OnOperationFinished;
            m_AssetDataManager.ImportedAssetInfoChanged -= OnImportedAssetInfoChanged;
            m_AssetDataManager.AssetDataChanged -= OnAssetDataChanged;
            m_CloseButton.clicked -= OnCloseButton;
        }

        protected void OnCloseButton()
        {
            m_PageManager.ActivePage.ClearSelection();
        }

        protected abstract void OnOperationProgress(AssetDataOperation operation);
        protected abstract void OnOperationFinished(AssetDataOperation operation);
        protected abstract void OnImportedAssetInfoChanged(AssetChangeArgs args);
        protected abstract void OnAssetDataChanged(AssetChangeArgs args);
        protected abstract void OnCloudServicesReachabilityChanged(bool cloudServiceReachable);
    }
}

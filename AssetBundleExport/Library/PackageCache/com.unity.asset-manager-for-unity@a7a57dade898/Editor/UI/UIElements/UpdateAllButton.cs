using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    static partial class UssStyle
    {
        public const string UpdateAllButtonContainer = "unity-update-all-button-container";
        public const string UpdateAllButtonIcon = "unity-update-all-button-icon";
    }

    class UpdateAllButton : GridTool
    {
        private readonly Button m_UpdateAllButton;
        private readonly VisualElement m_Icon;
        private readonly IAssetDataManager m_AssetDataManager;
        private readonly HashSet<BaseAssetData> m_TrackedAssets = new();
        private readonly IApplicationProxy m_ApplicationProxy;

        bool IsAvailable => m_PageManager.ActivePage?.SupportsUpdateAll ?? false;

        public UpdateAllButton(IAssetImporter assetImporter, IPageManager pageManager, IProjectOrganizationProvider projectOrganizationProvider, IApplicationProxy applicationProxy)
        :base(pageManager, projectOrganizationProvider)
        {
            m_UpdateAllButton = new Button(() =>
            {
                var project = pageManager.ActivePage is InProjectPage ? null : projectOrganizationProvider.SelectedProject;
                var collection = pageManager.ActivePage is InProjectPage ? null : projectOrganizationProvider.SelectedCollection;

                TaskUtils.TrackException(assetImporter.UpdateAllToLatestAsync(ImportTrigger.UpdateAllToLatest, project, collection, CancellationToken.None));

                AnalyticsSender.SendEvent(new UpdateAllLatestButtonClickEvent());
            });
            Add(m_UpdateAllButton);

            var container = new VisualElement();
            container.AddToClassList(UssStyle.UpdateAllButtonContainer);
            m_UpdateAllButton.Add(container);

            m_Icon = new VisualElement();
            m_Icon.AddToClassList(UssStyle.UpdateAllButtonIcon);

            m_AssetDataManager = ServicesContainer.instance.Resolve<IAssetDataManager>();

            m_AssetDataManager.AssetDataChanged += OnAssetDataChanged;
            m_AssetDataManager.ImportedAssetInfoChanged += OnAssetDataChanged;

            EnableButton(false);

            container.Add(m_Icon);

            m_ApplicationProxy = applicationProxy;

            tooltip = L10n.Tr(Constants.UpdateAllButtonTooltip);
        }

        protected override void InitDisplay(IPage page)
        {
            base.InitDisplay(page);
            _ = UpdateButtonStatus();
        }

        public void Refresh()
        {
            _ = UpdateButtonStatus();
        }

        async Task UpdateButtonStatus()
        {
            // Only update the button status if the button is available for the current page and the application is reachable
            if (!IsAvailable || !m_ApplicationProxy.InternetReachable)
            {
                EnableButton(false);
                return;
            }

            // Unsubscribe from assets to be cleared
            foreach (var asset in m_TrackedAssets)
            {
                asset.AssetDataChanged -= OnAssetDataAttributesChanged;
            }
            m_TrackedAssets.Clear();

            // Get the relevant imported assets for the current page
            var importedAssets = await GetImportedAssetsForCurrentPage();
            if (!importedAssets.Any())
            {
                EnableButton(false);
                return;
            }

            // Subscribe to imported assets
            foreach (var asset in importedAssets)
            {
                asset.AssetDataChanged += OnAssetDataAttributesChanged;
                m_TrackedAssets.Add(asset);
            }

            CheckAssetsStatus(importedAssets);
        }

        async void OnAssetDataChanged(AssetChangeArgs obj)
        {
            // Wait for the UI to update before processing the asset data changes
            await Task.Yield();
            await UpdateButtonStatus();
        }

        async Task<List<BaseAssetData>> GetImportedAssetsForCurrentPage()
        {
            var assets = new List<BaseAssetData>();

            // Populate the tracked assets with all imported assets based on local filtering
            var localFilters = m_PageManager.PageFilterStrategy.SelectedLocalFilters;
            if (localFilters != null && localFilters.Any())
            {
                // Filter the imported assets based on the local filters
                var filteredAssets = await FilteringUtils.GetFilteredImportedAssets(m_AssetDataManager.ImportedAssetInfos, localFilters, CancellationToken.None);
                assets.AddRange(filteredAssets.Select(info => info.AssetData));
            }
            else
            {
                // If no local filters, use all imported assets
                assets.AddRange(m_AssetDataManager.ImportedAssetInfos.Select(info => info.AssetData));
            }

            return assets;
        }

        void OnAssetDataAttributesChanged(BaseAssetData asset, AssetDataEventType changeType)
        {
            if (IsAvailable && changeType == AssetDataEventType.AssetDataAttributesChanged)
            {
                // Enable the button if the asset has an import attribute that is out of date
                var importAttribute = asset.AssetDataAttributeCollection?.GetAttribute<ImportAttribute>();
                if (importAttribute != null && importAttribute.Status == ImportAttribute.ImportStatus.OutOfDate)
                {
                    EnableButton(true);
                }
            }
        }

        void CheckAssetsStatus(List<BaseAssetData> assets)
        {
            // Reset the button state, only enable if we find an asset that is out of date
            EnableButton(false);

            // Only update the button status if the button is available for the current page
            if (!IsAvailable || assets == null || assets.Count == 0)
                return;

            var assetsWithNoAttributes = new List<BaseAssetData>();
            foreach (var asset in assets)
            {
                var importAttribute = asset.AssetDataAttributeCollection?.GetAttribute<ImportAttribute>();
                if (importAttribute == null)
                {
                    // If the asset has no import attribute, we need to fetch it
                    assetsWithNoAttributes.Add(asset);
                    continue;
                }

                if (importAttribute.Status == ImportAttribute.ImportStatus.OutOfDate)
                {
                    EnableButton(true);
                    return;
                }
            }

            // If we have no outdated assets, but some assets without attributes, we need to fetch the attributes
            foreach (var assetsWithNoAttribute in assetsWithNoAttributes)
                TaskUtils.TrackException(assetsWithNoAttribute.RefreshAssetDataAttributesAsync());
        }

        ~UpdateAllButton()
        {
            foreach (var asset in m_TrackedAssets)
            {
                asset.AssetDataChanged -= OnAssetDataAttributesChanged;
            }
            m_TrackedAssets.Clear();

            m_AssetDataManager.AssetDataChanged -= OnAssetDataChanged;
            m_AssetDataManager.ImportedAssetInfoChanged -= OnAssetDataChanged;
        }

        void EnableButton(bool enable)
        {
            m_UpdateAllButton.visible = IsAvailable;

            m_Icon.RemoveFromClassList(enable ? "inactive" : "active");
            m_Icon.AddToClassList(enable ? "active" : "inactive");
            m_UpdateAllButton.SetEnabled(enable);
        }

        protected override bool IsDisplayed(IPage page)
        {
            if (page is BasePage basePage)
            {
                return basePage.DisplayUpdateAllButton;
            }

            return base.IsDisplayed(page);
        }
    }
}

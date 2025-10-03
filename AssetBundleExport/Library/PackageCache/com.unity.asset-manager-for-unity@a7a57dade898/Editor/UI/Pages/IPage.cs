using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;

namespace Unity.AssetManager.UI.Editor
{
    interface IPage
    {
        bool IsLoading { get; }
        string DefaultProjectName { get; }
        string Title { get; }
        AssetIdentifier LastSelectedAssetId { get; }
        IReadOnlyCollection<BaseAssetData> AssetList { get; }
        IReadOnlyCollection<AssetIdentifier> SelectedAssets { get; }
        Dictionary<string, SortField> SortOptions { get; }
        UIComponents EnabledUIComponents { get; }
        bool SupportsUpdateAll { get; }

        event Action<bool> LoadingStatusChanged;
        event Action<IReadOnlyCollection<AssetIdentifier>> SelectedAssetsChanged;
        event Action<IReadOnlyCollection<string>> SearchFiltersChanged;
        event Action<UIComponents> UIComponentEnabledChanged;

        void SelectAsset(AssetIdentifier asset, bool additive);
        void SelectAssets(IEnumerable<AssetIdentifier> assets);
        public void ToggleAsset(AssetIdentifier assetIdentifier, bool checkState);
        void LoadMore();
        void Clear(bool reloadImmediately, bool clearSelection = true);
        void ClearSelection();

        // Called after the page is created, and after a domain reload
        void OnEnable();

        // Called when the window is closed, and before a domain reload
        void OnDisable();

        // Called before the page is activated
        void SetFilterStrategy(IPageFilterStrategy pageFilterStrategy);

        // Called after the page is deactivated
        void ClearFilterStrategy();

        // Called when a page got activated (when it became the current visible page)
        // Not called after a domain reload
        void OnActivated();

        // Called when a page got deactivated (when it went from the current page to the previous page)
        void OnDeactivated();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class AllAssetsPage : BasePage
    {
        public AllAssetsPage(IAssetDataManager assetDataManager, IAssetsProvider assetsProvider,
            IProjectOrganizationProvider projectOrganizationProvider, IMessageManager messageManager,
            IPageManager pageManager, IDialogManager dialogManager)
            : base(assetDataManager, assetsProvider, projectOrganizationProvider, messageManager, pageManager,
                dialogManager) { }

        public override bool DisplayBreadcrumbs => true;

        public override void OnActivated()
        {
            base.OnActivated();

            m_ProjectOrganizationProvider.SelectProject(string.Empty);
        }

        public override void LoadMore()
        {
            if (m_ProjectOrganizationProvider?.SelectedOrganization == null)
                return;

            base.LoadMore();
        }

        protected internal override async IAsyncEnumerable<BaseAssetData> LoadMoreAssets(
            [EnumeratorCancellation] CancellationToken token)
        {
            await foreach (var assetData in LoadMoreAssets(m_ProjectOrganizationProvider.SelectedOrganization, token))
            {
                yield return assetData;
            }
        }

        protected override void OnLoadMoreSuccessCallBack()
        {
            if (!AssetList.Any())
            {
                if (!TrySetNoResultsPageMessage())
                {
                    SetPageMessage(new Message(L10n.Tr(Constants.EmptyAllAssetsText),
                        RecommendedAction.OpenAssetManagerDashboardLink));
                }
            }
            else
            {
                m_PageFilterStrategy.EnableFilters();

                m_MessageManager.ClearAllMessages();
            }
        }
    }
}

using System;
using Unity.AssetManager.Core.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class SideBarAllAssetsFoldout : SideBarFoldout
    {
        readonly Image m_AllAssetsImage;

        internal SideBarAllAssetsFoldout(IUnityConnectProxy unityConnectProxy, IPageManager pageManager,
            IStateManager stateManager, IMessageManager messageManager, IProjectOrganizationProvider projectOrganizationProvider,
            string foldoutName)
            : base(unityConnectProxy, pageManager, stateManager, messageManager, projectOrganizationProvider,
                foldoutName)
        {
            RegisterCallback<PointerDownEvent>(e =>
            {
                var target = (VisualElement)e.target;
                // We skip the user's click if they aimed the check mark of the foldout
                // to only select foldouts when they click on it's title/label
                if (e.button != 0 || target.name == k_CheckMarkName)
                    return;

                m_PageManager.SetActivePage<AllAssetsPage>();
            }, TrickleDown.TrickleDown);
        }

        protected override void OnActivePageChanged(IPage page)
        {
            SetEnabled(page is not UploadPage);

            var selected = page is AllAssetsPage;
            m_Toggle.EnableInClassList(k_UnityListViewItemSelected, selected);
        }
    }
}

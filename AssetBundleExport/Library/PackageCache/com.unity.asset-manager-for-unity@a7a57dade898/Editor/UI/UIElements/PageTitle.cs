using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class PageTitle : VisualElement
    {
        Label m_Title;

        public PageTitle(IPageManager pageManager)
        {
            m_Title = new Label();

            Add(m_Title);

            Refresh(pageManager.ActivePage);
            pageManager.ActivePageChanged += Refresh;
        }

        void Refresh(IPage page)
        {
            var basePage = page as BasePage;
            m_Title.text = basePage?.Title ?? string.Empty;
            UIElementsUtils.SetDisplay(this, basePage?.DisplayTitle ?? false);
        }
    }
}

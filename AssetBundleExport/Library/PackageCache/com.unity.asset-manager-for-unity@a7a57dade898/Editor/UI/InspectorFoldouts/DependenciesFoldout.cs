using System;
using Unity.AssetManager.Core.Editor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class DependenciesFoldout : ItemFoldout<AssetIdentifier, DetailsPageDependencyItem>
    {
        readonly IPageManager m_PageManager;

        public DependenciesFoldout(VisualElement parent, string foldoutTitle, IPageManager pageManager)
            : base(parent, foldoutTitle, "dependencies-foldout", "dependencies-list", "details-files-foldout", "details-files-list")
        {
            m_PageManager = pageManager;
        }

        protected override DetailsPageDependencyItem MakeItem()
        {
            return new DetailsPageDependencyItem(m_PageManager);
        }

        protected override void BindItem(DetailsPageDependencyItem element, int index)
        {
            var dependency = (AssetIdentifier)Items[index];
            TaskUtils.TrackException(element.Bind(dependency));
        }
    }
}

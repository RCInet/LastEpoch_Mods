using System;
using Unity.AssetManager.Core.Editor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    interface IPopupManager : IService
    {
        VisualElement Container { get; }
        void CreatePopupContainer(VisualElement parent);
        void Show(VisualElement target, PopupContainer.PopupAlignment alignment);
        void Hide();
        void Clear();
    }

    [Serializable]
    class PopupManager : BaseService<IPopupManager>, IPopupManager
    {
        PopupContainer m_PopupContainer;

        public VisualElement Container => m_PopupContainer;

        public void CreatePopupContainer(VisualElement parent)
        {
            m_PopupContainer = new PopupContainer();
            parent.Add(m_PopupContainer);
        }

        public void Show(VisualElement target, PopupContainer.PopupAlignment alignment)
        {
            m_PopupContainer.SetPosition(target, alignment);
            m_PopupContainer.Show();
        }

        public void Hide()
        {
            m_PopupContainer.Hide();
        }

        public void Clear()
        {
            m_PopupContainer.Clear();
        }
    }
}

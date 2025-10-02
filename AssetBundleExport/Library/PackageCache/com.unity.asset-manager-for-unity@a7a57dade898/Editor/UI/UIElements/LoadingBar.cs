using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class LoadingBar : VisualElement
    {
        const string k_LoadingBarUssClassName = "loading-bar";
        const string k_LoadingBarLabelUssClassName = "loading-bar-label";
        const string k_LoadingBarHighPositionUssClassName = "loading-bar--high-position";

        readonly Label m_Label;
        readonly LoadingIcon m_LoadingIcon;

        public LoadingBar()
        {
            pickingMode = PickingMode.Ignore;

            m_LoadingIcon = new LoadingIcon();
            m_Label = new Label(L10n.Tr("Loading Assets"));
            m_Label.AddToClassList(k_LoadingBarLabelUssClassName);

            AddToClassList(k_LoadingBarUssClassName);

            Add(m_LoadingIcon);
            Add(m_Label);
        }

        public void SetPosition(bool highPosition)
        {
            if (highPosition)
            {
                AddToClassList(k_LoadingBarHighPositionUssClassName);
            }
            else
            {
                RemoveFromClassList(k_LoadingBarHighPositionUssClassName);
            }
        }

        public void Show()
        {
            m_LoadingIcon.PlayAnimation();
            UIElementsUtils.Show(this);
        }

        public void Hide()
        {
            RemoveFromClassList(k_LoadingBarHighPositionUssClassName);
            m_LoadingIcon.StopAnimation();
            UIElementsUtils.Hide(this);
        }
    }
}

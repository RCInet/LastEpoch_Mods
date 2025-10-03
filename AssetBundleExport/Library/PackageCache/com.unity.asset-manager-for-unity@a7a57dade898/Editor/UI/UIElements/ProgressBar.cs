using System;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    static partial class UssStyle
    {
        public static readonly string ProgressBarContainer = "progress-bar-container";
        public static readonly string ProgressBarBackground = "progress-bar-background";
        public static readonly string ProgressBarColor = "progress-bar";
    }

    class ProgressBar : VisualElement
    {
        readonly IVisualElementScheduledItem m_AnimationUpdate;
        protected readonly VisualElement m_ProgressBar;
        protected readonly Button m_CancelButton;
        protected readonly VisualElement m_ProgressBarContainer;

        float m_AnimationLeftOffset;
        protected bool m_IsIndefinite;
        protected bool m_IsFinished;

        readonly Action m_CancelCallback;

        bool IsIndefinite
        {
            get => m_IsIndefinite;

            set
            {
                if (!value)
                {
                    m_AnimationUpdate.Pause();
                }

                if (m_IsIndefinite == value)
                    return;

                m_IsIndefinite = value;

                if (!m_IsIndefinite)
                    return;

                m_AnimationLeftOffset = 0.0f;
                m_AnimationUpdate.Resume();
            }
        }

        internal ProgressBar(Action cancelCallback = null)
        {
            UIElementsUtils.Show(this);

            m_ProgressBarContainer = new VisualElement();
            m_ProgressBar = new VisualElement();
            m_ProgressBar.AddToClassList(UssStyle.ProgressBarColor);

            Add(m_ProgressBarContainer);
            m_ProgressBarContainer.Add(m_ProgressBar);
            m_ProgressBarContainer.AddToClassList(UssStyle.ProgressBarBackground);


            if (cancelCallback == null)
            {
                m_ProgressBarContainer.AddToClassList(UssStyle.ProgressBarContainer);
            }
            if (cancelCallback != null)
            {
                AddToClassList(UssStyle.ProgressBarContainer);

                m_CancelCallback = cancelCallback;

                m_CancelButton = new Button();
                Add(m_CancelButton);

                m_CancelButton.RemoveFromClassList("unity-button");
                m_CancelButton.tooltip = L10n.Tr(AssetManagerCoreConstants.CancelImportActionText);
                m_CancelButton.clicked += OnCancelClicked;
            }

            m_AnimationUpdate = schedule.Execute(UpdateProgressBar).Every(30);
            IsIndefinite = false;
        }

        void OnCancelClicked()
        {
            m_CancelCallback?.Invoke();

            if (m_IsFinished)
            {
                Hide();
            }
        }

        void UpdateProgressBar(TimerState timerState)
        {
            if (!m_IsIndefinite)
                return;

            m_AnimationLeftOffset = (m_AnimationLeftOffset + 0.001f * timerState.deltaTime) % 1.0f;

            m_ProgressBar.style.width =
                Length.Percent(Mathf.Min(m_AnimationLeftOffset + 0.3f, 1.0f - m_AnimationLeftOffset) * 100.0f);
            m_ProgressBar.style.left = Length.Percent(m_AnimationLeftOffset * 100.0f);
        }

        protected void Hide()
        {
            UIElementsUtils.Hide(this);
            m_IsIndefinite = false;
        }

        public void SetProgress(float progress)
        {
            if (progress < 1f)
            {
                m_IsFinished = false;
                UpdateTooltip();
            }

            UIElementsUtils.Show(this);
            IsIndefinite = progress <= 0.0f;

            if (!IsIndefinite)
            {
                m_ProgressBar.style.left = 0.0f;
                m_ProgressBar.style.width = Length.Percent(progress * 100);
            }
        }

        protected virtual void UpdateTooltip() { }
    }
}

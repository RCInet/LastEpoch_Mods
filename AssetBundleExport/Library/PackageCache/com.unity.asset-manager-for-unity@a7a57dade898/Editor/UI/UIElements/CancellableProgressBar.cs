using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class CancellableProgressBar : VisualElement
    {
        const string k_UssClassName = "cancellable-progress-bar";
        const string k_ProgressBarClassName = k_UssClassName + "--foreground";
        const string k_CancelButtonClassName = k_UssClassName + "--cancel-button";

        VisualElement m_ProgressBar;

        public event Action Cancel;

        /// <summary>
        /// Allows the visual indication of progress to be set and updated
        /// </summary>
        public float Progress
        {
            set => m_ProgressBar.style.width = new Length(Mathf.Clamp01(value) * 100f, LengthUnit.Percent);
        }

        public CancellableProgressBar()
        {
            AddToClassList(k_UssClassName);
            pickingMode = PickingMode.Ignore;

            m_ProgressBar = new VisualElement();
            m_ProgressBar.AddToClassList(k_ProgressBarClassName);
            m_ProgressBar.style.width = new Length(0, LengthUnit.Percent);
            m_ProgressBar.pickingMode = PickingMode.Ignore;

            var cancelButton = new Button();
            cancelButton.RegisterCallback<ClickEvent>(OnClickEvent);
            cancelButton.AddToClassList(k_CancelButtonClassName);
            cancelButton.style.backgroundImage =
                EditorGUIUtility.Load(EditorGUIUtility.isProSkin ? "d_winbtn_win_close_a" : "winbtn_win_close_a") as
                    Texture2D;

            Add(m_ProgressBar);
            Add(cancelButton);
        }

        void OnClickEvent(ClickEvent clickEvent)
        {
            clickEvent.StopImmediatePropagation();
            Cancel?.Invoke();
        }
    }
}

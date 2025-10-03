using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class LoadingIcon : VisualElement
    {
        const string k_StyleSheetClassName = "loading-icon";

        // Height must be set programmatically for rotation of icon
        const int k_Height = 20;
        const int k_Width = k_Height;

        static Quaternion s_LastRotation;
        static Vector3 s_LastPosition;
        static float s_LastAngle;

        readonly IVisualElementScheduledItem m_Scheduler;
        float m_CurrentAngle;

        internal LoadingIcon()
        {
            AddToClassList(k_StyleSheetClassName);

            style.height = k_Height;
            style.width = k_Width;

            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);

            m_Scheduler = schedule.Execute(UpdateAnimation).Every(50);
            m_Scheduler.Pause();
        }

        public void PlayAnimation()
        {
            m_Scheduler.Resume();
        }

        public void StopAnimation()
        {
            m_Scheduler.Pause();
        }

        void UpdateAnimation(TimerState timerState)
        {
            if (style.visibility == Visibility.Hidden)
                return;

            transform.rotation = Quaternion.Euler(0, 0, m_CurrentAngle);
            m_CurrentAngle += 0.6f * timerState.deltaTime;
        }

        void OnAttachToPanel(AttachToPanelEvent evt)
        {
            transform.rotation = s_LastRotation.normalized;
            transform.position = s_LastPosition;
            m_CurrentAngle = s_LastAngle;
        }

        void OnDetachFromPanel(DetachFromPanelEvent e)
        {
            s_LastRotation = transform.rotation;
            s_LastPosition = transform.position;
            s_LastAngle = m_CurrentAngle;
        }
    }
}

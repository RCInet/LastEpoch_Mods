using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class PopupContainer : VisualElement
    {
        public enum PopupAlignment
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        static readonly float k_Gap = 8f;

        VisualElement m_Target;
        PopupAlignment m_Alignment;

        public PopupContainer()
        {
            focusable = true;
            UIElementsUtils.Hide(this);

            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        void OnAttachToPanel(AttachToPanelEvent evt)
        {
            RegisterCallback<FocusOutEvent>(OnFocusOut);
            RegisterCallback<GeometryChangedEvent>(OnResized);
            parent.RegisterCallback<GeometryChangedEvent>(OnResized);
        }

        void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            UnregisterCallback<FocusOutEvent>(OnFocusOut);
            UnregisterCallback<GeometryChangedEvent>(OnResized);
            parent.UnregisterCallback<GeometryChangedEvent>(OnResized);
        }

        public void Show()
        {
            UIElementsUtils.Show(this);
            Focus();
        }

        public void Hide()
        {
            UIElementsUtils.Hide(this);
            Clear();
        }

        public void SetPosition(VisualElement target, PopupAlignment alignment)
        {
            m_Target = target;
            m_Alignment = alignment;
            TaskUtils.TrackException(SetPositionAsync(target, alignment));
        }

        async Task SetPositionAsync(VisualElement target, PopupAlignment alignment)
        {
            // Wait for the next frame to make sure the PopupContainer is correctly resize
            await Task.Delay(1);

            var worldPos = target.LocalToWorld(Vector2.zero);
            var localPos = parent.WorldToLocal(worldPos);

            switch (alignment)
            {
                case PopupAlignment.TopLeft:
                    if(localPos.x + resolvedStyle.width > parent.resolvedStyle.width)
                    {
                        style.left = parent.resolvedStyle.width - resolvedStyle.width;
                    }
                    else
                    {
                        style.left = localPos.x;
                    }
                    style.maxHeight = worldPos.y - k_Gap;
                    style.top = localPos.y - resolvedStyle.height;
                    break;
                case PopupAlignment.TopRight:
                    style.left = localPos.x + target.resolvedStyle.width - resolvedStyle.width;
                    if (style.left.value.value < 0)
                    {
                        style.left = 0;
                    }
                    style.maxHeight = localPos.y - k_Gap;
                    style.top = localPos.y - resolvedStyle.height;
                    break;
                case PopupAlignment.BottomLeft:
                    if(localPos.x + resolvedStyle.width > parent.resolvedStyle.width)
                    {
                        style.left = parent.resolvedStyle.width - resolvedStyle.width;
                    }
                    else
                    {
                        style.left = localPos.x;
                    }
                    style.top = localPos.y + target.resolvedStyle.height;
                    await Task.Delay(1);
                    style.maxHeight = parent.resolvedStyle.height - (resolvedStyle.top + k_Gap);
                    break;
                case PopupAlignment.BottomRight:
                    style.left = localPos.x + target.resolvedStyle.width - resolvedStyle.width;
                    if (style.left.value.value < 0)
                    {
                        style.left = 0;
                    }
                    style.top = localPos.y + target.resolvedStyle.height;
                    await Task.Delay(1);
                    style.maxHeight = parent.resolvedStyle.height - (resolvedStyle.top + k_Gap);
                    break;
            }
        }

        void OnResized(GeometryChangedEvent evt)
        {
            if (m_Target != null)
            {
                SetPosition(m_Target, m_Alignment);
            }
        }

        void OnFocusOut(FocusOutEvent e)
        {
            if (Contains((VisualElement)e.relatedTarget))
            {
                Focus();
            }
            else if (e.relatedTarget != this)
            {
                Clear();
                UIElementsUtils.Hide(this);
            }
        }
    }
}

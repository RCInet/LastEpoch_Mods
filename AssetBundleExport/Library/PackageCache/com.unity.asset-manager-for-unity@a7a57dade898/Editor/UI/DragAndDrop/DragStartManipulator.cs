using System;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    /// <summary>
    /// Manipulator that fires a Drag Event when you press the pointer down on its target.
    /// </summary>
    class DragStartManipulator : PointerManipulator
    {
        Action m_DragStart;

        internal DragStartManipulator(VisualElement root, Action dragStartCallback)
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            target = root;
            m_DragStart = dragStartCallback;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        }

        void OnPointerDown(PointerDownEvent e)
        {
            if (CanStartManipulation(e))
            {
                e.StopPropagation();
                m_DragStart?.Invoke();
            }
        }
    }
}

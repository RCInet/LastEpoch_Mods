using System;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class TagChip : Chip
    {
        internal event Action<string> TagChipPointerUpAction;

        public TagChip(string label)
            : base(label)
        {
            _ = new ClickOrDragStartManipulator(this, OnPointerUp, null, null);
        }

        void OnPointerUp(PointerUpEvent e)
        {
            TagChipPointerUpAction?.Invoke(m_Label.text);
        }
    }
}

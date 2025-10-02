using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class Chip : VisualElement
    {
        protected Label m_Label;

        public Chip(string text, bool isSelectable = false)
        {

            m_Label = new Label(text)
            {
                pickingMode = !isSelectable ? PickingMode.Ignore : PickingMode.Position,
                selection =
                {
                    isSelectable = isSelectable
                }
            };


            Add(m_Label);
        }
    }
}

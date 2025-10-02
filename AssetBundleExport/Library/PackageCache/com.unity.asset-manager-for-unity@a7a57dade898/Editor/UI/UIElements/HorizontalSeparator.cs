using System;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class HorizontalSeparator : VisualElement
    {
        public HorizontalSeparator()
        {
            AddToClassList("horizontal-separator");
        }
#pragma warning disable CS0618 // Type or member is obsolete
        public new class UxmlFactory : UxmlFactory<HorizontalSeparator> { }
#pragma warning restore CS0618 // Type or member is obsolete

    }
}

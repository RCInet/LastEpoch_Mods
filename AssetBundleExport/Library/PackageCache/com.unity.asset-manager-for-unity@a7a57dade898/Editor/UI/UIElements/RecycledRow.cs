using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class RecycledRow : VisualElement
    {
        internal const int UndefinedIndex = -1;

        internal readonly List<int> Ids;

        internal readonly List<int> Indices;

        internal RecycledRow(float height)
        {
            AddToClassList(UssStyle.GridViewRowStyleClassName);
            style.height = height;

            Indices = new List<int>();
            Ids = new List<int>();
        }

        internal int FirstIndex => Indices.Count > 0 ? Indices[0] : UndefinedIndex;
    }
}

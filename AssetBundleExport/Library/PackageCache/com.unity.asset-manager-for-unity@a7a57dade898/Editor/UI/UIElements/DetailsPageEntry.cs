using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    static partial class UssStyle
    {
        public const string DetailsPageEntry = "details-page-entry";
        public const string DetailsPageEntryLabel = "details-page-entry-label";
        public const string DetailsPageEntryValue = "details-page-entry-value";
        public const string DetailsPageChipContainer = "details-page-chip-container";
        public const string DetailsPageEntryValueText = "details-page-entry-value-text";
    }

    class DetailsPageEntry : VisualElement
    {
        readonly Label m_Text;

        public DetailsPageEntry(string title)
        {
            AddToClassList(UssStyle.DetailsPageEntry);

            if (!string.IsNullOrEmpty(title))
            {
                var titleElement = new Label(L10n.Tr(title))
                {
                    name = "entry-label",
                    tooltip = title
                };
                titleElement.AddToClassList(UssStyle.DetailsPageEntryLabel);
                hierarchy.Add(titleElement);
            }
        }

        public DetailsPageEntry(string title, string details, bool allowSelection = false)
            : this(title)
        {
            m_Text = new Label(L10n.Tr(details))
            {
                name = "entry-value",
                selection = { isSelectable = allowSelection }
            };
            m_Text.AddToClassList(UssStyle.DetailsPageEntryValue);
            if (string.IsNullOrEmpty(title))
            {
                m_Text.AddToClassList(UssStyle.DetailsPageEntryValueText);
            }
            hierarchy.Add(m_Text);
        }

        public VisualElement AddChipContainer()
        {
            var chipContainer = new VisualElement();
            chipContainer.AddToClassList(UssStyle.DetailsPageChipContainer);
            hierarchy.Add(chipContainer);

            return chipContainer;
        }

        public void SetText(string text)
        {
            if (m_Text != null)
            {
                m_Text.text = text;
            }
        }
    }
}

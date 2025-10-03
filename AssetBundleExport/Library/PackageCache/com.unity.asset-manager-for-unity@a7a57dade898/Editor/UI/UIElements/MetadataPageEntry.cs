using System;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    static partial class UssStyle
    {
        public const string MetadataPageEntry = "metadata-page-entry";
        public const string KebabButton = "metadata-page-entry-kebab-button";
        public const string MetadataPageEntryLabel = "metadata-page-entry-label";
        public const string MetadataPageEntryMetadataField = "metadata-page-entry-metadata-field";
        public const string MetadataFieldAndButtonContainer = "metadata-field-and-button-container";
    }

    class MetadataPageEntry : VisualElement
    {
        public MetadataPageEntry(string title, VisualElement metadataField, Action entryRemovedCallback)
        {
            AddToClassList(UssStyle.MetadataPageEntry);

            if (!string.IsNullOrEmpty(title))
            {
                var titleElement = new Label(L10n.Tr(title));
                titleElement.AddToClassList(UssStyle.MetadataPageEntryLabel);
                titleElement.tooltip = title;
                Add(titleElement);
            }

            var fieldAndButtonContainer = new VisualElement();
            fieldAndButtonContainer.AddToClassList(UssStyle.MetadataFieldAndButtonContainer);

            metadataField.AddToClassList(UssStyle.MetadataPageEntryMetadataField);
            fieldAndButtonContainer.Add(metadataField);

            fieldAndButtonContainer.Add(CreateKebabButton(entryRemovedCallback));

            Add(fieldAndButtonContainer);
        }

        VisualElement CreateKebabButton(Action entryRemovedCallback)
        {
            var kebabMenu = new GenericMenu();
            kebabMenu.AddItem(new GUIContent("Remove"), false, () =>
            {
                parent.Remove(this);

                entryRemovedCallback?.Invoke();
            });

            var kebabButton = new Button();
            kebabButton.ClearClassList();
            kebabButton.focusable = false;
            kebabButton.AddToClassList(UssStyle.KebabButton);
            kebabButton.clicked += () => kebabMenu.ShowAsContext();

            return kebabButton;
        }
    }
}

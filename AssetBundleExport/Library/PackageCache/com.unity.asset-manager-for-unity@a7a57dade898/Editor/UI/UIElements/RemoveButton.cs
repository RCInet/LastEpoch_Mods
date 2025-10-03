using System;
using System.Collections.Generic;
using System.IO;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    static partial class UssStyle
    {
        public const string RemoveButtonsContainer = "remove-buttons-container";
        public const string RemoveButton = "remove-button";
        public const string RemoveDropdownButton = "remove-dropdown-button";
    }

    class RemoveButton : VisualElement
    {
        readonly Button m_RemoveButton;

        public event Action RemoveWithExclusiveDependencies;
        public event Action RemoveOnlySelected;
        public event Action StopTracking;
        public event Action StopTrackingOnlySelected;

        public string text
        {
            get => m_RemoveButton.text;
            set => m_RemoveButton.text = value;
        }

#pragma warning disable CS0108 // 'RemoveButton.tooltip' hides inherited member 'VisualElement.tooltip'
        public string tooltip
        {
            get => m_RemoveButton.tooltip;
            set => m_RemoveButton.tooltip = value;
        }
#pragma warning restore CS0108 // 'RemoveButton.tooltip' hides inherited member 'VisualElement.tooltip'

        public RemoveButton(bool isPlural)
        {
            AddToClassList(UssStyle.RemoveButtonsContainer);

            m_RemoveButton = CreateRemoveButton(isPlural);
            CreateRemoveMoreButton(isPlural);

            SetEnabled(false);
        }

        Button CreateRemoveButton(bool isPlural)
        {
            var button = new Button
            {
                text = isPlural ? L10n.Tr(Constants.RemoveAllFromProjectActionText) : L10n.Tr(Constants.RemoveFromProjectActionText),
                focusable = false
            };
            button.AddToClassList(UssStyle.RemoveButton);
            button.clicked += () => RemoveWithExclusiveDependencies?.Invoke();

            Add(button);

            return button;
        }

        void CreateRemoveMoreButton(bool isPlural)
        {
            var removeDropdown = new DropdownField
            {
                focusable = false
            };
            UIElementsUtils.Hide(removeDropdown.Q(null, "unity-base-popup-field__text"));
            removeDropdown.AddToClassList(UssStyle.RemoveDropdownButton);

            var firstChoice = isPlural ? L10n.Tr(Constants.RemoveAssetsOnlyText) : L10n.Tr(Constants.RemoveAssetOnlyText);
            var secondChoice = isPlural ? L10n.Tr(Constants.StopTrackingAssetsActionText) : L10n.Tr(Constants.StopTrackingAssetActionText);
            var thirdChoice = isPlural ? L10n.Tr(Constants.StopTrackingAssetsOnlyActionText) : L10n.Tr(Constants.StopTrackingAssetOnlyActionText);
            removeDropdown.choices = new List<string> { firstChoice, secondChoice, thirdChoice };

            removeDropdown.RegisterValueChangedCallback(value =>
            {
                if (value.newValue == firstChoice)
                {
                    RemoveOnlySelected?.Invoke();
                }
                else if (value.newValue == secondChoice)
                {
                    StopTracking?.Invoke();
                }
                else if (value.newValue == thirdChoice)
                {
                    StopTrackingOnlySelected?.Invoke();
                }

                removeDropdown.value = null;
            });

            Add(removeDropdown);
        }
    }
}

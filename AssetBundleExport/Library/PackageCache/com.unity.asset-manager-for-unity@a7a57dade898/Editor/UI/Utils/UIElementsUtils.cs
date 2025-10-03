using System;
using System.IO;
using Unity.AssetManager.Core.Editor;
using Unity.AssetManager.Upload.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    /// <summary>
    /// An utility class for common UIElements setup method
    /// </summary>
    static class UIElementsUtils
    {
        static readonly string k_UIResourcesLocation = $"Packages/{AssetManagerCoreConstants.PackageName}/Editor/UI/EditorResources/";

        public static void SetSequenceNumberText(Label label, BaseAssetData assetData, string versionLabel = "")
        {
            string versionText;

            if(!string.IsNullOrEmpty(versionLabel))
            {
                versionText = L10n.Tr(versionLabel);
            }
            else if (assetData.SequenceNumber > 0)
            {
                versionText = L10n.Tr(Constants.VersionText) + assetData.SequenceNumber;
            }
            else
            {
                versionText = assetData.SequenceNumber == UploadAssetData.NewVersionSequenceNumber
                    ? L10n.Tr(Constants.NewVersionText)
                    : L10n.Tr(Constants.PendingVersionText);
            }

            label.text = versionText;
            label.tooltip = assetData.Identifier.Version;
        }

        internal static Texture GetPackageIcon()
        {
            var filename = $"Package-Icon-{(EditorGUIUtility.isProSkin ? "Dark" : "Light")}.png";
            var filepath = Path.Combine(k_UIResourcesLocation, "Images/PackageIcon/", filename);
            return GetTexture(filepath);
        }

        internal static Texture GetTexture(string filepath)
        {
            return ServicesContainer.instance.Resolve<IAssetDatabaseProxy>().LoadAssetAtPath(
                Path.Combine(k_UIResourcesLocation, filepath),
                typeof(Texture)) as Texture;
        }

        internal static Button SetupButton(string buttonName, Action clickAction, bool isEnabled,
            VisualElement parent, string text = "", string tooltip = "", bool showIfEnabled = true)
        {
            Button button = parent.Query<Button>(buttonName);
            button.SetEnabled(isEnabled);
            button.clickable = new Clickable(() => clickAction?.Invoke());
            button.text = text;
            button.tooltip = string.IsNullOrEmpty(tooltip) ? button.text : tooltip;

            if (showIfEnabled && isEnabled)
            {
                Show(button);
            }

            return button;
        }

        internal static Label SetupLabel(string labelName, string text, VisualElement parent,
            Manipulator manipulator = null)
        {
            Label label = parent.Query<Label>(labelName);
            label.text = text;
            if (manipulator != null)
            {
                label.AddManipulator(manipulator);
            }

            return label;
        }

        internal static ToolbarSearchField SetupToolbarSearchField(string name,
            EventCallback<ChangeEvent<string>> valueChangedCallback, VisualElement parent)
        {
            var uxmlField = parent.Q<ToolbarSearchField>(name);
            uxmlField.value = string.Empty;
            uxmlField.SetEnabled(true);
            uxmlField.RegisterCallback(valueChangedCallback);
            return uxmlField;
        }

        internal static void Hide(VisualElement element)
        {
            if (element == null)
                return;

            element.style.display = DisplayStyle.None;
        }

        internal static void Show(VisualElement element)
        {
            if (element == null)
                return;

            element.style.display = DisplayStyle.Flex;
        }

        internal static void SetDisplay(VisualElement element, bool displayed)
        {
            if (displayed)
            {
                Show(element);
            }
            else
            {
                Hide(element);
            }
        }

        internal static bool IsDisplayed(VisualElement element)
        {
            return element.style.display.value == DisplayStyle.Flex;
        }

        internal static VisualTreeAsset LoadUXML(string tabName)
        {
            var path = $"{k_UIResourcesLocation}/UXML/{tabName}.uxml";
            return ServicesContainer.instance.Resolve<IAssetDatabaseProxy>().LoadAssetAtPath(path, typeof(VisualTreeAsset)) as VisualTreeAsset;
        }

        internal static void LoadCommonStyleSheet(VisualElement target)
        {
            LoadCustomStyleSheet(target, "Main");
        }

        internal static void LoadCustomStyleSheet(VisualElement target, string Stylesheet)
        {
            var styleSheetFilePath = $"{k_UIResourcesLocation}/StyleSheets/{Stylesheet}.uss";
            var styleSheet = ServicesContainer.instance.Resolve<IAssetDatabaseProxy>().LoadAssetAtPath(styleSheetFilePath, typeof(StyleSheet)) as StyleSheet;
            if (styleSheet != null)
            {
                target.styleSheets.Add(styleSheet);
            }
        }

        internal static void RemoveCustomStylesheets(VisualElement target)
        {
            for (var i = target.styleSheets.count - 1; i > 0; i--)
            {
                target.styleSheets.Remove(target.styleSheets[i]);
            }
        }

        internal static VisualElement GetRootVisualElement(VisualElement child)
        {
            var currentElement = child;
            while (currentElement.parent != null)
            {
                currentElement = currentElement.parent;
            }

            return currentElement;
        }
    }
}

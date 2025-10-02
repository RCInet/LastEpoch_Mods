using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class AssetPreview : VisualElement
    {
        static class UssStyles
        {
            public static readonly string Thumbnail = "asset-preview-thumbnail";
            public static readonly string AssetPreview = "asset-preview";
            public static readonly string AssetTypeIcon = "asset-preview-asset-type-icon";
            public static readonly string Toggle = "asset-preview-toggle";
            public static readonly string DefaultAssetIcon = "default-asset-icon";
            public static readonly string ImportedStatus = UssStyle.GridItemStyleClassName + "-imported_status";
            public static readonly string NoThumbnail = "no-thumbnail";
        }

        readonly VisualElement m_ThumbnailImage;
        readonly VisualElement m_AssetTypeIcon;
        readonly VisualElement m_ImportedStatusIcon;
        readonly Toggle m_Toggle;

        public Action<bool> ToggleValueChanged;

        public Toggle Toggle => m_Toggle;

        public AssetPreview()
        {
            m_ThumbnailImage = new VisualElement();
            m_AssetTypeIcon = new VisualElement();
            m_Toggle = new Toggle();
            m_Toggle.RegisterValueChangedCallback(evt => ToggleValueChanged?.Invoke(evt.newValue));

            AddToClassList(UssStyles.AssetPreview);
            m_ThumbnailImage.AddToClassList(UssStyles.Thumbnail);
            m_AssetTypeIcon.AddToClassList(UssStyles.AssetTypeIcon);

            m_Toggle.AddToClassList(UssStyles.Toggle);

            m_ImportedStatusIcon = new VisualElement();
            m_ImportedStatusIcon.AddToClassList(UssStyles.ImportedStatus);
            m_ImportedStatusIcon.pickingMode = PickingMode.Ignore;

            Add(m_ThumbnailImage);
            Add(m_AssetTypeIcon);
            Add(m_Toggle);
            Add(m_ImportedStatusIcon);
        }

        public interface IStatus
        {
            string Description { get; }
            string ActionText { get; }
            string Details { get; }
            VisualElement CreateVisualTree();
        }

        public void SetStatuses(IEnumerable<IStatus> statuses)
        {
            var validStatuses = statuses?.Where(s => s != null).ToList();
            var hasStatuses = validStatuses != null && validStatuses.Any();
            UIElementsUtils.SetDisplay(m_ImportedStatusIcon, hasStatuses);

            m_ImportedStatusIcon.Clear();

            if (!hasStatuses)
                return;

            foreach (var status in validStatuses)
            {
                SetStatus(status);
            }
        }

        void SetStatus(IStatus status)
        {
            var statusElement = status.CreateVisualTree();
            statusElement.tooltip = L10n.Tr(status.Description);

            if (!string.IsNullOrEmpty(status.Details))
            {
                statusElement.tooltip += $"\n\nDetails:\n{status.Details}";
            }

            m_ImportedStatusIcon.Add(statusElement);
        }

        public void SetAssetType(string extension)
        {
            var icon = AssetDataTypeHelper.GetIconForExtension(extension);

            UIElementsUtils.Show(m_AssetTypeIcon);
            m_AssetTypeIcon.EnableInClassList(UssStyles.DefaultAssetIcon, icon == null);
            m_AssetTypeIcon.style.backgroundImage = icon == null ? StyleKeyword.Null : icon;
            m_AssetTypeIcon.tooltip = string.IsNullOrEmpty(extension) ? "no extension" : extension;
        }

        public void ClearPreview()
        {
            SetThumbnail(null);

            m_AssetTypeIcon.EnableInClassList(UssStyles.DefaultAssetIcon, false);
            m_AssetTypeIcon.style.backgroundImage = StyleKeyword.Null;
            m_AssetTypeIcon.tooltip = null;
        }

        public void SetThumbnail(Texture2D texture2D)
        {
            EnableInClassList(UssStyles.NoThumbnail, texture2D == null);
            m_ThumbnailImage.style.backgroundImage = texture2D;
        }
    }
}

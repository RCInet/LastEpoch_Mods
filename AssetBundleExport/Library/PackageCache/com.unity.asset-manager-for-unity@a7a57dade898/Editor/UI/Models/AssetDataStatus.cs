using System.Collections.Generic;
using Unity.AssetManager.Core.Editor;
using Unity.AssetManager.Upload.Editor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    static class AssetDataStatus
    {
        // Import
        public static readonly AssetPreview.IStatus Imported = new PreviewStatus(Constants.ImportedText, Constants.ReimportActionText, UssStyles.StatusIcon, UssStyles.StatusImported);
        public static readonly AssetPreview.IStatus UpToDate = new PreviewStatus(Constants.UpToDateText, Constants.ReimportActionText, UssStyles.StatusUpToDate);
        public static readonly AssetPreview.IStatus OutOfDate = new PreviewStatus(Constants.OutOfDateText, Constants.UpdateToLatestActionText, UssStyles.StatusOutOfDate);
        public static readonly AssetPreview.IStatus Error = new PreviewStatus(Constants.StatusErrorText, string.Empty, UssStyles.StatusError);

        // Upload
        public static readonly AssetPreview.IStatus Linked = new PreviewStatus(Constants.LinkedText, string.Empty, "grid-view--item-linked");
        public static readonly AssetPreview.IStatus UploadAdd = new PreviewStatus(Constants.UploadAddText, string.Empty, "grid-view--item-upload-add");
        public static readonly AssetPreview.IStatus UploadSkip = new PreviewStatus(Constants.UploadSkipText, string.Empty, "grid-view--item-upload-skip");
        public static readonly AssetPreview.IStatus UploadOverride = new PreviewStatus(Constants.UploadNewVersionText, string.Empty, "grid-view--item-upload-override");
        public static readonly AssetPreview.IStatus UploadDuplicate = new PreviewStatus(Constants.UploadDuplicateText, string.Empty, "grid-view--item-upload-duplicate");
        public static readonly AssetPreview.IStatus UploadOutside = new PreviewStatus(Constants.UploadOutsideText, string.Empty, "grid-view--item-upload-outside");
        public static readonly AssetPreview.IStatus UploadSourceControlled = new PreviewStatus(Constants.UploadSourceControlledText, string.Empty, UssStyles.StatusError);

        static class UssStyles
        {
            public static readonly string StatusIcon = "grid-view--status-icon";
            static readonly string k_Status = UssStyle.GridItemStyleClassName + "-imported_status";
            public static readonly string StatusImported = k_Status + "-imported";
            public static readonly string StatusUpToDate = k_Status + "-up_to_date";
            public static readonly string StatusOutOfDate = k_Status + "-out_of_date";
            public static readonly string StatusError = k_Status + "-error";
        }

        internal static IEnumerable<AssetPreview.IStatus> GetOverallStatus(this AssetDataAttributeCollection assetDataAttributeCollection)
        {
            var assetPreviewStatusList = new List<AssetPreview.IStatus>();

            if (assetDataAttributeCollection == null)
            {
                return assetPreviewStatusList;
            }

            assetPreviewStatusList.Add(assetDataAttributeCollection.GetStatusOfImport());
            assetPreviewStatusList.Add(assetDataAttributeCollection.GetStatusOfUpload());
            if (assetDataAttributeCollection.GetAttribute<LinkedDependencyAttribute>()?.IsLinked == true)
            {
                assetPreviewStatusList.Add(Linked);
            }

            return assetPreviewStatusList;
        }

        internal static AssetPreview.IStatus GetStatusOfImport(this AssetDataAttributeCollection attributeCollection)
        {
            if (attributeCollection == null || !attributeCollection.HasAttribute<ImportAttribute>())
                return null;

            return attributeCollection.GetAttribute<ImportAttribute>()?.Status switch
            {
                ImportAttribute.ImportStatus.NoImport => null,
                ImportAttribute.ImportStatus.UpToDate => UpToDate,
                ImportAttribute.ImportStatus.OutOfDate => OutOfDate,
                ImportAttribute.ImportStatus.ErrorSync => Error,
                _ => null
            };
        }

        static AssetPreview.IStatus GetStatusOfUpload(this AssetDataAttributeCollection attributeCollection)
        {
            if (attributeCollection == null || !attributeCollection.HasAttribute<UploadAttribute>())
                return null;

            var attribute = attributeCollection.GetAttribute<UploadAttribute>();

            if (attribute == null)
            {
                return null;
            }

            var templateStatus = attribute.Status.GetStatusOfUpload();

            return templateStatus == null ? null : new PreviewStatus(templateStatus, attribute.Details);
        }

        static AssetPreview.IStatus GetStatusOfUpload(this UploadAttribute.UploadStatus status)
        {
            return status switch
            {
                UploadAttribute.UploadStatus.DontUpload => null,
                UploadAttribute.UploadStatus.Add => UploadAdd,
                UploadAttribute.UploadStatus.Skip => UploadSkip,
                UploadAttribute.UploadStatus.Override => UploadOverride,
                UploadAttribute.UploadStatus.Duplicate => UploadDuplicate,
                UploadAttribute.UploadStatus.ErrorOutsideProject => UploadOutside,
                UploadAttribute.UploadStatus.SourceControlled => UploadSourceControlled,
                _ => null
            };
        }
    }

    class PreviewStatus : AssetPreview.IStatus
    {
        readonly string[] m_Styles;

        public string Description { get; }
        public string ActionText { get; }
        public string Details { get; }

        public PreviewStatus(string description, string actionText, params string[] ussStyles)
        {
            Description = description;
            ActionText = actionText;
            Details = null;
            m_Styles = ussStyles;
        }

        public PreviewStatus(AssetPreview.IStatus otherStatus, string details)
        {
            Description = otherStatus.Description;
            ActionText = otherStatus.ActionText;
            Details = details;
            m_Styles = ((PreviewStatus)otherStatus).m_Styles;
        }

        public VisualElement CreateVisualTree()
        {
            var visualElement = new VisualElement();
            foreach (var style in m_Styles)
            {
                visualElement.AddToClassList(style);
            }

            return visualElement;
        }
    }
}

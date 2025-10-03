using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using Unity.AssetManager.Upload.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    static partial class UssStyle
    {
        public const string DetailsPageEntriesContainer = "details-page-entries-container";
        public const string DetailsPageThumbnailContainer = "details-page-thumbnail-container";
        public const string ImageContainer = "image-container";
    }

    class AssetDetailsTab : AssetTab
    {
        const string k_FileSizeName = "file-size";
        const string k_FileCountName = "file-count";

        readonly AssetPreview m_AssetPreview;
        readonly VisualElement m_OutdatedWarningBox;
        readonly VisualElement m_EntriesContainer;
        private readonly IPageManager m_PageManager;
        private readonly IStateManager m_StateManager;

        BaseAssetData m_AssetData;

        public override AssetDetailsPageTabs.TabType Type => AssetDetailsPageTabs.TabType.Details;
        public override bool IsFooterVisible => true;
        public override bool EnabledWhenDisconnected => true;
        public override VisualElement Root { get; }

        readonly Func<bool> m_IsFilterActive;

        AssetDependenciesComponent m_DependenciesComponent;


        public AssetDetailsTab(VisualElement visualElement, Func<bool> isFilterActive = null, IPageManager pageManager = null, IStateManager stateManager = null)
        {
            var root = visualElement.Q("details-page-content-container");
            Root = root;
            m_PageManager = pageManager;
            m_StateManager = stateManager;

            m_EntriesContainer = new VisualElement();
            m_EntriesContainer.AddToClassList(UssStyle.DetailsPageEntriesContainer);
            root.Add(m_EntriesContainer);
            m_EntriesContainer.SendToBack();

            m_OutdatedWarningBox = root.Q("outdated-warning-box");
            m_OutdatedWarningBox.Q<Label>().text = L10n.Tr(Constants.FilteredAssetOutdatedWarning);
            m_OutdatedWarningBox.SendToBack();

            m_AssetPreview = new AssetPreview {name = "details-page-asset-preview"};
            m_AssetPreview.AddToClassList(UssStyle.DetailsPageThumbnailContainer);
            m_AssetPreview.AddToClassList(UssStyle.ImageContainer);

            m_IsFilterActive = isFilterActive;
        }

        public override void OnSelection(BaseAssetData assetData)
        {
            m_AssetPreview.ClearPreview();
        }

        public override void RefreshUI(BaseAssetData assetData, bool isLoading = false)
        {
            // Remove asset preview from hierarchy to avoid it being destroyed when clearing the container
            m_AssetPreview.RemoveFromHierarchy();

            m_EntriesContainer.Clear();

            AddText(m_EntriesContainer, null, assetData.Description);

            m_AssetPreview.SetAssetType(assetData.PrimaryExtension);
            m_EntriesContainer.Add(m_AssetPreview);
            SetPreviewImage(assetData.Thumbnail);

            UpdatePreviewStatus(AssetDataStatus.GetOverallStatus(assetData.AssetDataAttributeCollection));
            UpdateStatusWarning(assetData.AssetDataAttributeCollection);

            AddSpace(m_EntriesContainer);

            AddText(m_EntriesContainer, Constants.StatusText, assetData.Status, isSelectable: true);

            var projectIds = GetProjectIdsDisplayList(assetData.Identifier.ProjectId, assetData.LinkedProjects);
            var projectEntryTitle = projectIds.Length > 1 ? Constants.ProjectsText : Constants.ProjectText;
            AddProjectChips(m_EntriesContainer, projectEntryTitle, projectIds, "entry-project");

            AddTagChips(m_EntriesContainer, Constants.TagsText, assetData.Tags);

            m_DependenciesComponent = new AssetDependenciesComponent(m_EntriesContainer, m_PageManager, m_StateManager);
            m_DependenciesComponent.RefreshUI(assetData, isLoading);
            DisplayMetadata(assetData);

            AddText(m_EntriesContainer, Constants.FilesSizeText, "-", isSelectable:false, k_FileSizeName);
            AddText(m_EntriesContainer, Constants.TotalFilesText, "-", isSelectable:false, k_FileCountName);

            // Temporary solution to display targeted assets during re-upload. Very handy to understand which assets the system is re-uploading to.
            var identifier = assetData.Identifier;
            if (assetData is UploadAssetData uploadAssetData)
            {
                identifier = uploadAssetData.TargetAssetIdentifier ?? identifier;
            }
            AddAssetIdentifier(m_EntriesContainer, Constants.AssetIdText, identifier);

            var assetsProvider = ServicesContainer.instance.Resolve<IAssetsProvider>();
            AddText(m_EntriesContainer, Constants.AssetTypeText, assetsProvider.GetValueAsString(assetData.AssetType), isSelectable: true);

            AddText(m_EntriesContainer, Constants.LastModifiedText, Utilities.DatetimeToString(assetData.Updated));
            AddUser(m_EntriesContainer, Constants.LastEditByText, assetData.UpdatedBy, typeof(UpdatedByFilter));
            AddText(m_EntriesContainer, Constants.UploadDateText, Utilities.DatetimeToString(assetData.Created));
            AddUser(m_EntriesContainer, Constants.CreatedByText, assetData.CreatedBy, typeof(CreatedByFilter));

            if (isLoading)
            {
                AddLoadingText(m_EntriesContainer);
            }
        }

        void DisplayMetadata(BaseAssetData assetData)
        {
            // Dot not display metadata for local assets (a.k.a UploadAssetData)
            if (assetData.Identifier.IsLocal())
                return;

            foreach (var metadata in assetData.Metadata)
            {
                switch (metadata.Type)
                {
                    case MetadataFieldType.Text:
                    {
                        var textMetadata = (Core.Editor.TextMetadata)metadata;
                        AddText(m_EntriesContainer, textMetadata.Name, textMetadata.Value, isSelectable: true);
                        break;
                    }
                    case MetadataFieldType.Boolean:
                    {
                        var booleanMetadata = (Core.Editor.BooleanMetadata)metadata;
                        AddToggle(m_EntriesContainer, booleanMetadata.Name, booleanMetadata.Value);
                        break;
                    }
                    case MetadataFieldType.Number:
                    {
                        var numberMetadata = (Core.Editor.NumberMetadata)metadata;
                        AddText(m_EntriesContainer, numberMetadata.Name,
                            numberMetadata.Value.ToString(CultureInfo.CurrentCulture), isSelectable: true);
                        break;
                    }
                    case MetadataFieldType.Timestamp:
                    {
                        var timestampMetadata = (Core.Editor.TimestampMetadata)metadata;
                        AddText(m_EntriesContainer, timestampMetadata.Name,
                            Utilities.DatetimeToString(timestampMetadata.Value.DateTime), isSelectable: true);
                        break;
                    }
                    case MetadataFieldType.Url:
                    {
                        var urlMetadata = (Core.Editor.UrlMetadata)metadata;
                        AddText(m_EntriesContainer, urlMetadata.Name,
                            urlMetadata.Value.Uri == null ? string.Empty : urlMetadata.Value.Uri.ToString(), isSelectable: true);
                        break;
                    }
                    case MetadataFieldType.User:
                    {
                        var userMetadata = (Core.Editor.UserMetadata)metadata;
                        AddUser(m_EntriesContainer, metadata.Name, userMetadata.Value, null);
                        break;
                    }
                    case MetadataFieldType.SingleSelection:
                    {
                        var singleSelectionMetadata = (Core.Editor.SingleSelectionMetadata)metadata;
                        AddSelectionChips(m_EntriesContainer, metadata.Name, new List<string> {singleSelectionMetadata.Value}, isSelectable: true);
                        break;
                    }
                    case MetadataFieldType.MultiSelection:
                    {
                        var multiSelectionMetadata = (Core.Editor.MultiSelectionMetadata)metadata;
                        AddSelectionChips(m_EntriesContainer, metadata.Name, multiSelectionMetadata.Value, isSelectable: true);
                        break;
                    }
                    default:
                        throw new InvalidOperationException("Unexpected metadata field type was encountered.");
                }
            }
        }

        public override void RefreshButtons(UIEnabledStates enabled, BaseAssetData assetData, BaseOperation operationInProgress)
        {
            m_EntriesContainer.Q("entry-project")?.SetEnabled(enabled.HasFlag(UIEnabledStates.ServicesReachable));
        }

        public void UpdatePreviewStatus(IEnumerable<AssetPreview.IStatus> status)
        {
            m_AssetPreview.SetStatuses(status);
        }

        public void UpdateDependencyComponent(BaseAssetData assetData)
        {
            m_DependenciesComponent.RefreshUI(assetData, false);
        }

        public void UpdateStatusWarning(AssetDataAttributeCollection assetDataAttributeCollection)
        {
            // If the selected asset is outdated and selected via filtered results, we want to warn
            // the user that the asset may not be displaying all up-to-date information.

            var status = assetDataAttributeCollection?.GetAttribute<ImportAttribute>()?.Status;

            var filterActive = m_IsFilterActive?.Invoke() ?? false;
            var displayOutdatedWarning = filterActive && status == ImportAttribute.ImportStatus.OutOfDate;
            UIElementsUtils.SetDisplay(m_OutdatedWarningBox, displayOutdatedWarning);
        }

        public void SetPreviewImage(Texture2D texture)
        {
            m_AssetPreview.SetThumbnail(texture);
        }

        public void SetFileCount(string fileCount)
        {
            m_EntriesContainer.Q<DetailsPageEntry>(k_FileCountName)?.SetText(fileCount);
        }

        public void SetFileSize(string fileSize)
        {
            m_EntriesContainer.Q<DetailsPageEntry>(k_FileSizeName)?.SetText(fileSize);
        }

        public void SetPrimaryExtension(string primaryExtension)
        {
            m_AssetPreview.SetAssetType(primaryExtension);
        }

        static string[] GetProjectIdsDisplayList(string currentProjectId, IEnumerable<ProjectIdentifier> linkedProjects)
        {
            // Ensure the current project is first in the list
            var uniqueProjectIds = new HashSet<string>() { currentProjectId };
            foreach (var linkedProject in linkedProjects)
                uniqueProjectIds.Add(linkedProject.ProjectId);

            return uniqueProjectIds.ToArray();
        }
    }
}

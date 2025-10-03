using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using Unity.AssetManager.Upload.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Unity.AssetManager.UI.Editor
{
    static partial class UssStyle
    {
        public const string k_CustomFieldListItem = "custom-field-list-item";
        public const string k_CustomFieldListItemIcon = "custom-field-list-item-icon";
        public const string k_HorizontalSeparator = "horizontal-separator";
        public const string k_UploadMetadataTitle = "upload-metadata-title";
        public const string k_AddCustomFieldButton = "add-custom-field-button";
        public const string k_AddCustomFieldSearchField = "add-custom-field-search-field";
        public const string k_AddCustomFieldScrollView = "add-custom-field-scroll-view";
        public const string k_AddCustomFieldPopup = "add-custom-field-popup";
        public const string k_AddCustomFieldSection = "add-custom-field-section";
        public const string k_NoMetadataFieldLabel = "no-metadata-field-label";
    }

    sealed class CustomMetadataButton : Button
    {
        public CustomMetadataButton(MetadataSharingType sharingType, string text, Action onClick)
        {
            ClearClassList();
            AddToClassList(UssStyle.k_CustomFieldListItem);

            if (sharingType == MetadataSharingType.All)
            {
                SetEnabled(false);
            }
            else
            {
                clicked += onClick;
            }

            var icon = new Label();
            icon.AddToClassList(UssStyle.k_CustomFieldListItemIcon);
            icon.text = sharingType switch
            {
                MetadataSharingType.Partial => "—",
                MetadataSharingType.All => "\u2713",
                _ => icon.text
            };

            Add(icon);
            Add(new Label(text));
        }
    }

    class UploadMetadataContainer : VisualElement
    {
        VisualElement m_NewMetadataContainer;
        VisualElement m_AddCustomFieldPopup;
        VisualElement m_PartialMetadataMessageHelpBox;
        ScrollView m_MetadataScrollView;

        readonly IPageManager m_PageManager;
        readonly IAssetDataManager m_AssetDataManager;
        readonly IProjectOrganizationProvider m_ProjectOrganizationProvider;
        readonly ILinksProxy m_LinksProxy;

        readonly AssetDataSelection m_SelectedAssetsData = new();

        public UploadMetadataContainer(IPageManager pageManager, IAssetDataManager assetDataManager,
            IProjectOrganizationProvider projectOrganizationProvider, ILinksProxy linksProxy)
        {
            m_PageManager = pageManager;
            m_AssetDataManager = assetDataManager;
            m_ProjectOrganizationProvider = projectOrganizationProvider;
            m_LinksProxy = linksProxy;

            BuildUI();

            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        void BuildUI()
        {
            var separator = new VisualElement();
            separator.AddToClassList(UssStyle.k_HorizontalSeparator);
            Add(separator);

            var title = new Label(L10n.Tr(Constants.UploadMetadata));
            title.AddToClassList(UssStyle.k_UploadMetadataTitle);
            Add(title);

            m_PartialMetadataMessageHelpBox = new HelpBox(L10n.Tr(Constants.MetadataPartialEditing), HelpBoxMessageType.None);
            Add(m_PartialMetadataMessageHelpBox);
            UIElementsUtils.Hide(m_PartialMetadataMessageHelpBox);

            m_NewMetadataContainer = new VisualElement();
            Add(m_NewMetadataContainer);

            var searchField = new ToolbarSearchField();
            searchField.AddToClassList(UssStyle.k_AddCustomFieldSearchField);
            searchField.RegisterValueChangedCallback(OnSearchFieldValueChanged);

            m_MetadataScrollView = new ScrollView
            {
                horizontalScrollerVisibility = ScrollerVisibility.Auto
            };
            m_MetadataScrollView.AddToClassList(UssStyle.k_AddCustomFieldScrollView);

            m_AddCustomFieldPopup = new VisualElement
            {
                focusable = true
            };
            m_AddCustomFieldPopup.AddToClassList(UssStyle.k_AddCustomFieldPopup);
            m_AddCustomFieldPopup.Add(searchField);
            m_AddCustomFieldPopup.Add(m_MetadataScrollView);
            m_AddCustomFieldPopup.RegisterCallback<FocusOutEvent>(OnPopupFocusOut);
            UIElementsUtils.Hide(m_AddCustomFieldPopup);

            var addFieldButton = new Button
            {
                text = L10n.Tr(Constants.AddCustomField)
            };
            addFieldButton.AddToClassList(UssStyle.k_AddCustomFieldButton);
            addFieldButton.clicked += () =>
            {
                UpdateMetadataScrollView(m_SelectedAssetsData.Selection, searchField.value);
                searchField.value = string.Empty;
                UIElementsUtils.Show(m_AddCustomFieldPopup);
                m_AddCustomFieldPopup.Focus();
            };

            var addCustomFieldSection = new VisualElement();
            addCustomFieldSection.AddToClassList(UssStyle.k_AddCustomFieldSection);
            addCustomFieldSection.Add(addFieldButton);
            addCustomFieldSection.Add(m_AddCustomFieldPopup);
            Add(addCustomFieldSection);

            if (m_PageManager.ActivePage == null)
                return;

            m_SelectedAssetsData.Selection = m_AssetDataManager.GetAssetsData(m_PageManager.ActivePage.SelectedAssets);
        }

        void OnAttachToPanel(AttachToPanelEvent evt)
        {
            DisplaySelectedAssetsMetadata();

            m_PageManager.SelectedAssetChanged += OnSelectedAssetChanged;
        }

        void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            m_PageManager.SelectedAssetChanged -= OnSelectedAssetChanged;
        }

        void OnSelectedAssetChanged(IPage page, IEnumerable<AssetIdentifier> identifiers)
        {
            m_SelectedAssetsData.Selection = m_AssetDataManager.GetAssetsData(identifiers);

            DisplaySelectedAssetsMetadata();
        }

        void DisplaySelectedAssetsMetadata()
        {
            m_NewMetadataContainer.Clear();

            if (m_SelectedAssetsData.Selection.Count == 0)
                return;

            // Check if all selected IDs have staged metadata
            if (m_SelectedAssetsData.Selection.All(assetData => !assetData.Metadata.Any()))
                return;

            // Filter metadata to only include selected keys
            var selectedMetadata = m_SelectedAssetsData.Selection.Select(x => x.Metadata).ToList();

            if (!selectedMetadata.Any())
                return;

            var hasSameMetadataFieldKeys = MetadataHelpers.HasSameMetadataFieldKeys(selectedMetadata);
            UIElementsUtils.SetDisplay(m_PartialMetadataMessageHelpBox, !hasSameMetadataFieldKeys);

            foreach (var metadata in selectedMetadata[0])
            {
                Utilities.DevAssert(metadata != null);
                if (metadata == null)
                    continue;

                var matchingMetadata = selectedMetadata
                    .SelectMany(valueList => valueList)
                    .Where(y => y.FieldKey.Equals(metadata.FieldKey))
                    .ToList();

                if (matchingMetadata.Count == m_SelectedAssetsData.Selection.Count)
                {
                    var fieldDefinition = m_ProjectOrganizationProvider.SelectedOrganization.MetadataFieldDefinitions
                        .Find(x => x.Key == metadata.FieldKey);

                    if (fieldDefinition == null)
                    {
                        Utilities.DevLogWarning($"Metadata field definition not found for key '{metadata.FieldKey}'");
                        continue;
                    }

                    m_NewMetadataContainer.Add(CreateMetadataPageEntryFromFieldDefinition(fieldDefinition, matchingMetadata));
                }
            }
        }

        void OnSearchFieldValueChanged(ChangeEvent<string> evt)
        {
            UpdateMetadataScrollView(m_SelectedAssetsData.Selection, evt.newValue);
        }

        void UpdateMetadataScrollView(IReadOnlyCollection<BaseAssetData> selection, string searchFieldValue)
        {
            m_MetadataScrollView.Clear();

            var matchingFields = m_ProjectOrganizationProvider.SelectedOrganization.MetadataFieldDefinitions
                .Where(x => x.DisplayName.Contains(searchFieldValue, StringComparison.OrdinalIgnoreCase));

            var allMetadata = selection.Select(x => x.Metadata).ToList();

            if (!matchingFields.Any())
            {
                var labelText = L10n.Tr(Constants.NoMatchingFields_WithoutLink);
                if (m_LinksProxy.TryGetAssetManagerDashboardUrl(out var url, true))
                {
                    labelText = L10n.Tr(Constants.NoMatchingFields);
                    labelText = labelText.Replace("<a>", $"<a href=\"{url}\">");
                }

                var primaryLabel = new Label(labelText)
                {
                    enableRichText = true
                };

                primaryLabel.AddToClassList(UssStyle.k_NoMetadataFieldLabel);
                m_MetadataScrollView.Add(primaryLabel);

                return;
            }

            foreach (var field in matchingFields.OrderBy(x => x.DisplayName))
            {
                var presence = MetadataHelpers.GetMetadataSharingType(allMetadata, field.Key);
                var button = new CustomMetadataButton(presence, field.DisplayName, () =>
                {
                    CreateNewMetadataField(field);
                    UIElementsUtils.Hide(m_AddCustomFieldPopup);
                    DisplaySelectedAssetsMetadata();
                });

                m_MetadataScrollView.Add(button);
            }
        }

        void CreateNewMetadataField(IMetadataFieldDefinition fieldDefinition)
        {
            var list = new List<(UploadAssetData, IMetadata)>();

            foreach (var assetData in m_SelectedAssetsData.Selection)
            {
                var metadata = CreateMetadataFromFieldDefinition(fieldDefinition);
                list.Add(new((UploadAssetData)assetData, metadata));
            }

            ((UploadPage)m_PageManager.ActivePage).AddMetadata(list);
        }

        static IMetadata CreateMetadataFromFieldDefinition(IMetadataFieldDefinition def) =>
            def.Type switch
            {
                MetadataFieldType.Text => new Core.Editor.TextMetadata(def.Key, def.DisplayName, string.Empty),
                MetadataFieldType.Number => new Core.Editor.NumberMetadata(def.Key, def.DisplayName, 0),
                MetadataFieldType.Boolean => new Core.Editor.BooleanMetadata(def.Key, def.DisplayName, false),
                MetadataFieldType.Url => new Core.Editor.UrlMetadata(def.Key, def.DisplayName, new UriEntry(null, string.Empty)),
                MetadataFieldType.Timestamp => new Core.Editor.TimestampMetadata(def.Key, def.DisplayName, new DateTimeEntry(DateTime.Now)),
                MetadataFieldType.User => new Core.Editor.UserMetadata(def.Key, def.DisplayName, string.Empty),
                MetadataFieldType.SingleSelection => new Core.Editor.SingleSelectionMetadata(def.Key, def.DisplayName, string.Empty),
                MetadataFieldType.MultiSelection => new Core.Editor.MultiSelectionMetadata(def.Key, def.DisplayName, new List<string>()),
                _ => throw new InvalidOperationException(Constants.UnexpectedFieldDefinitionType)
            };

        MetadataPageEntry CreateMetadataPageEntryFromFieldDefinition(IMetadataFieldDefinition fieldDefinition,
            IEnumerable<IMetadata> metadata)
        {
            MetadataElement metadataField = fieldDefinition.Type switch
            {
                MetadataFieldType.Text => new TextMetadataField(metadata.Cast<Core.Editor.TextMetadata>().ToList()),
                MetadataFieldType.Number => new NumberMetadataField(metadata.Cast<Core.Editor.NumberMetadata>().ToList()),
                MetadataFieldType.Boolean => new BooleanMetadataField(metadata.Cast<Core.Editor.BooleanMetadata>().ToList()),
                MetadataFieldType.Url => new UrlMetadataField(metadata.Cast<Core.Editor.UrlMetadata>().ToList()),
                MetadataFieldType.Timestamp => new TimestampMetadataField(metadata.Cast<Core.Editor.TimestampMetadata>().ToList()),
                MetadataFieldType.User => new UserMetadataField(metadata.Cast<Core.Editor.UserMetadata>().ToList(), GetUserInfoList().ToList()),
                MetadataFieldType.SingleSelection => new SingleSelectionMetadataField(metadata.Cast<Core.Editor.SingleSelectionMetadata>().ToList(),
                    ((SelectionFieldDefinition)fieldDefinition).AcceptedValues.ToList()),
                MetadataFieldType.MultiSelection => new MultiSelectionMetadataField(fieldDefinition.DisplayName,
                    metadata.Cast<Core.Editor.MultiSelectionMetadata>().ToList(),
                    ((SelectionFieldDefinition)fieldDefinition).AcceptedValues.ToList()),
                _ => throw new InvalidOperationException(Constants.UnexpectedFieldDefinitionType)
            };

            metadataField.ValueChanged += () =>
            {
                ((UploadPage)m_PageManager.ActivePage).RefreshLocalStatus();
            };

            return new MetadataPageEntry(fieldDefinition.Type == MetadataFieldType.MultiSelection
                ? string.Empty
                : fieldDefinition.DisplayName, metadataField, () =>
            {
                ((UploadPage)m_PageManager.ActivePage).RemoveMetadata(m_SelectedAssetsData.Selection.Cast<UploadAssetData>(), fieldDefinition.Key);
            });
        }

        List<UserInfo> GetUserInfoList()
        {
            var userInfos = new List<UserInfo>();
            _ = m_ProjectOrganizationProvider.SelectedOrganization.GetUserInfosAsync(x => { userInfos = x; });

            return userInfos;
        }

        void OnPopupFocusOut(FocusOutEvent evt)
        {
            if (evt.relatedTarget == m_AddCustomFieldPopup
                || m_AddCustomFieldPopup.Contains((VisualElement)evt.relatedTarget))
            {
                m_AddCustomFieldPopup.Focus();
            }
            else
            {
                UIElementsUtils.Hide(m_AddCustomFieldPopup);
            }
        }
    }
}

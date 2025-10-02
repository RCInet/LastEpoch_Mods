using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class DetailsPageFileItem : VisualElement
    {
        const string k_DetailsPageFileItemUssStyle = "details-page-file-item";
        const string k_DetailsPageFileIconItemUssStyle = "details-page-file-item-icon";
        const string k_DetailsPageFileLabelItemUssStyle = "details-page-file-item-label";
        const string k_DetailsPageThreeDotsItemUssStyle = "details-page-three-dots-item";
        const string k_IncompleteFileIcon = "incomplete-file-icon";

        readonly IAssetDatabaseProxy m_AssetDatabaseProxy;

        readonly Label m_FileName;
        readonly VisualElement m_Icon;
        readonly VisualElement m_ErrorIcon;
        readonly Button m_ThreeDots;

        string m_Guid;
        GenericMenu m_ThreeDotsMenu;
        Dictionary<string, bool> m_ThreeDotsMenuItems;

        public Action RemoveClicked;

        public DetailsPageFileItem(IAssetDatabaseProxy assetDatabaseProxy)
        {
            m_AssetDatabaseProxy = assetDatabaseProxy;

            m_FileName = new Label("");
            m_Icon = new VisualElement();
            m_ErrorIcon = new VisualElement();
            m_ThreeDots = new Button();
            m_ThreeDots.ClearClassList();
            m_ThreeDots.focusable = false;

            AddToClassList(k_DetailsPageFileItemUssStyle);
            m_Icon.AddToClassList(k_DetailsPageFileIconItemUssStyle);

            m_ErrorIcon.AddToClassList(k_DetailsPageFileIconItemUssStyle);
            m_ErrorIcon.AddToClassList(k_IncompleteFileIcon);
            m_ErrorIcon.tooltip = L10n.Tr("This file is currently uploading or it failed to upload.");

            m_FileName.AddToClassList(k_DetailsPageFileLabelItemUssStyle);
            m_ThreeDots.AddToClassList(k_DetailsPageThreeDotsItemUssStyle);

            InitializeThreeDotsMenu(false);
            m_ThreeDots.clicked += () => m_ThreeDotsMenu.ShowAsContext();

            Add(m_Icon);
            Add(m_ErrorIcon);
            Add(m_FileName);
            Add(m_ThreeDots);
        }

        public void Refresh(string fileName, string guid, bool enabled, bool uploaded, bool removable)
        {
            var extension = string.IsNullOrEmpty(fileName) ? null : Path.GetExtension(fileName);

            if (uploaded)
            {
                UIElementsUtils.Hide(m_ErrorIcon);
                UIElementsUtils.Show(m_Icon);

                m_Icon.style.backgroundImage = AssetDataTypeHelper.GetIconForExtension(extension);
                m_Icon.tooltip = extension;
            }
            else
            {
                UIElementsUtils.Hide(m_Icon);
                UIElementsUtils.Show(m_ErrorIcon);
            }

            m_FileName.text = fileName;
            m_Guid = guid;

            InitializeThreeDotsMenu(removable);

            m_ThreeDots.visible = !MetafilesHelper.IsMetafile(fileName) && IsAnyMenuItemEnabled();

            SetEnabled(enabled);
        }

        void InitializeThreeDotsMenu(bool removable)
        {
            m_ThreeDotsMenu = new GenericMenu();
            m_ThreeDotsMenuItems = new Dictionary<string, bool>();

            var text = Constants.ShowInProjectActionText;
            var guiContent = new GUIContent(text);

            if (IsShowInProjectEnabled())
            {
                m_ThreeDotsMenu.AddItem(guiContent, false, ShowInProjectBrowser);
                m_ThreeDotsMenuItems.Add(text, true);
            }
            else
            {
                m_ThreeDotsMenu.AddDisabledItem(guiContent);
                m_ThreeDotsMenuItems.Add(text, false);
            }

            if (removable)
            {
                text = "Remove";

                m_ThreeDotsMenu.AddItem(new GUIContent(text), false, () => { RemoveClicked?.Invoke(); });
                m_ThreeDotsMenuItems.Add(text, true);
            }
        }

        bool IsShowInProjectEnabled()
        {
            return m_AssetDatabaseProxy.CanPingAssetByGuid(m_Guid);
        }

        void ShowInProjectBrowser()
        {
            m_AssetDatabaseProxy.PingAssetByGuid(m_Guid);
        }

        bool IsAnyMenuItemEnabled()
        {
            return m_ThreeDotsMenuItems.Any(x => x.Value);
        }
    }
}

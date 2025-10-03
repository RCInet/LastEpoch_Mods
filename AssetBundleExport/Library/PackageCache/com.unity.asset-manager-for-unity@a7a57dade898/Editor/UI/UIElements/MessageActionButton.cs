using System;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class MessageActionButton : Button
    {
        const string k_ButtonClassName = "messageView-button";
        const string k_LinkClassName = "messageView-action-button-link";
        readonly ILinksProxy m_LinksProxy;

        readonly IPageManager m_PageManager;
        readonly IProjectOrganizationProvider m_ProjectOrganizationProvider;

        public MessageActionButton(IPageManager pageManager,
            IProjectOrganizationProvider projectOrganizationProvider,
            ILinksProxy linksProxy)
        {
            m_PageManager = pageManager;
            m_ProjectOrganizationProvider = projectOrganizationProvider;
            m_LinksProxy = linksProxy;
            focusable = false;
        }

        public void SetRecommendedAction(RecommendedAction action)
        {
            clickable = null;
            UIElementsUtils.Show(this);

            switch (action)
            {
                case RecommendedAction.OpenServicesSettingButton:
                {
                    RemoveFromClassList(k_LinkClassName);
                    AddToClassList(k_ButtonClassName);
                    clicked += () => m_LinksProxy.OpenProjectSettings(ProjectSettingsMenu.Services);

                    tooltip = L10n.Tr("Open Project Settings");
                    text = tooltip;
                }
                    break;
                case RecommendedAction.EnableProject:
                {
                    RemoveFromClassList(k_LinkClassName);
                    AddToClassList(k_ButtonClassName);
                    clicked += m_ProjectOrganizationProvider.EnableProjectForAssetManager;

                    tooltip = L10n.Tr("Enable Project");
                    text = tooltip;
                }
                    break;
                case RecommendedAction.OpenAssetManagerDashboardLink:
                {
                    if (!m_LinksProxy.CanOpenAssetManagerDashboard)
                    {
                        UIElementsUtils.Hide(this);
                        break;
                    }
                    RemoveFromClassList(k_ButtonClassName);
                    AddToClassList(k_LinkClassName);
                    clicked += m_LinksProxy.OpenAssetManagerDashboard;

                    tooltip = L10n.Tr("Open the Asset Manager Dashboard");
                    text = tooltip;
                }
                    break;
                case RecommendedAction.OpenAssetManagerDocumentationPage:
                {
                    if (!m_LinksProxy.CanOpenAssetManagerDocumentation)
                    {
                        UIElementsUtils.Hide(this);
                        break;
                    }
                    
                    RemoveFromClassList(k_LinkClassName);
                    AddToClassList(k_ButtonClassName);
                    clicked += () => m_LinksProxy.OpenAssetManagerDocumentationPage("unity-editor/upload-editor-assets-to-cloud");

                    tooltip = L10n.Tr("Open Documentation");
                    text = tooltip;
                }
                    break;
                case RecommendedAction.Retry:
                {
                    RemoveFromClassList(k_LinkClassName);
                    AddToClassList(k_ButtonClassName);
                    clicked += RefreshWindow;

                    tooltip = L10n.Tr("Refresh");
                    text = tooltip;
                }
                    break;
                default:
                    UIElementsUtils.Hide(this);
                    break;
            }
        }

        static void RefreshWindow()
        {
            Utilities.DevAssert(AssetManagerWindow.Instance != null);
            AssetManagerWindow.Instance.RefreshAll();
        }
    }
}

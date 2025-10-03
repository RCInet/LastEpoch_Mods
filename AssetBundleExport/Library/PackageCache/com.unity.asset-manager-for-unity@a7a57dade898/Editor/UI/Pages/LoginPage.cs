using System;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class LoginPage : VisualElement
    {
        const string k_SignInUXMLName = "SignIn";
        
        readonly Label m_SignInLabel;
        readonly Button m_ProjectSettingsButton;

        public LoginPage(ILinksProxy linksProxy)
        {
            VisualTreeAsset windowContent = UIElementsUtils.LoadUXML(k_SignInUXMLName);
            windowContent.CloneTree(this);

            UIElementsUtils.RemoveCustomStylesheets(this);
            UIElementsUtils.LoadCommonStyleSheet(this);

            UIElementsUtils.SetupLabel("lblTitle", L10n.Tr("Sign in"), this);

            m_SignInLabel = UIElementsUtils.SetupLabel("lblSubtitle", L10n.Tr("Please sign in with your Unity ID"), this);

            m_ProjectSettingsButton = new Button
            {
                text = L10n.Tr("Open Project Settings"),
                tooltip = L10n.Tr("Open Project Settings Asset Manager Private Cloud Services"),
            };
            m_ProjectSettingsButton.clicked += () => linksProxy.OpenProjectSettings(ProjectSettingsMenu.PrivateCloudServices);
            m_ProjectSettingsButton.AddToClassList("messageView-action-button-link");
            this.Q<VisualElement>("MainWindow").Add(m_ProjectSettingsButton);
        }

        public void Refresh()
        {
            if (PrivateCloudSettings.Load().ServicesEnabled)
            {
                m_SignInLabel.text = L10n.Tr("Please sign in to Private Cloud Services");
                UIElementsUtils.Show(m_ProjectSettingsButton);
            }
            else
            {
                m_SignInLabel.text = L10n.Tr("Please sign in with your Unity ID");
                UIElementsUtils.Hide(m_ProjectSettingsButton);
            }
        }
    }
}

using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class ActionHelpBox : HelpBox
    {
        readonly IUnityConnectProxy m_UnityConnectProxy;
        readonly IApplicationProxy m_ApplicationProxy;
        readonly IPageManager m_PageManager;
        readonly ISettingsManager m_SettingsManager;

        readonly MessageActionButton m_MessageActionButton;

        HelpBoxMessage m_HelpBoxMessage;

        static readonly string k_NoConnectionMessage = L10n.Tr("You are offline.");
        static readonly string k_ServiceNotReachableMessage = L10n.Tr("Cannot reach Unity Cloud Services.");
        static readonly string k_VPCServiceNotReachableMessage = L10n.Tr("Cannot reach Private Cloud Services.");
        static readonly string k_NoConnectionUploadPageMessage = L10n.Tr("Connect to the internet to upload your assets.");

        public ActionHelpBox(IUnityConnectProxy unityConnectProxy, IApplicationProxy applicationProxy,
            IPageManager pageManager, IProjectOrganizationProvider projectOrganizationProvider,
            IMessageManager messageManager, ILinksProxy linksProxy, ISettingsManager settingsManager)
        {
            m_UnityConnectProxy = unityConnectProxy;
            m_ApplicationProxy = applicationProxy;
            m_PageManager = pageManager;
            m_SettingsManager = settingsManager;

            m_MessageActionButton = new MessageActionButton(pageManager, projectOrganizationProvider,
                linksProxy);

            Add(m_MessageActionButton);

            m_PageManager.ActivePageChanged += OnActivePageChanged;
            messageManager.HelpBoxMessageSet += OnHelpBoxMessageSet;
            messageManager.HelpBoxMessageCleared += OnHelpBoxMessageCleared;
        }

        public void Refresh()
        {
            m_MessageActionButton.visible = false;

            if (!m_UnityConnectProxy.AreCloudServicesReachable)
            {
                UIElementsUtils.Show(this);
                messageType = HelpBoxMessageType.Warning;
                if (m_ApplicationProxy.InternetReachable)
                {
                    text = m_SettingsManager.PrivateCloudSettings.ServicesEnabled ? k_VPCServiceNotReachableMessage : k_ServiceNotReachableMessage;
                }
                else
                {
                    text = m_PageManager.ActivePage is UploadPage ? $"{k_NoConnectionMessage} {k_NoConnectionUploadPageMessage}" : k_NoConnectionMessage;
                }

                return;
            }

            if (m_HelpBoxMessage == null)
            {
                UIElementsUtils.Hide(this);
                return;
            }

            messageType = m_HelpBoxMessage.MessageType;
            var hasErrorMessage = !string.IsNullOrEmpty(m_HelpBoxMessage.Content);

            if (!hasErrorMessage)
            {
                UIElementsUtils.Hide(this);
                return;
            }

            text = m_HelpBoxMessage.Content;
            m_MessageActionButton.SetRecommendedAction(m_HelpBoxMessage.RecommendedAction);
            m_MessageActionButton.visible = true;

            UIElementsUtils.Show(this);
        }

        void OnHelpBoxMessageSet(HelpBoxMessage helpBoxMessage)
        {
            m_HelpBoxMessage = helpBoxMessage;
            Refresh();
        }

        void OnHelpBoxMessageCleared()
        {
            m_HelpBoxMessage = null;
            Refresh();
        }

        void OnActivePageChanged(IPage page) => Refresh();
    }
}

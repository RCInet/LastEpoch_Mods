using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    interface IStorageInfoHelpBox
    {
        void RefreshCloudStorageAsync(TimerState timerState);
        Button DismissButton { get; }
    }

    class StorageInfoHelpBox : HelpBox, IStorageInfoHelpBox
    {
        readonly IProjectOrganizationProvider m_ProjectOrganizationProvider;

        readonly IUnityConnectProxy m_UnityConnectProxy;
        readonly IPageManager m_PageManager;
        readonly ISettingsManager m_SettingsManager;

        StorageUsage m_CloudStorageUsage;
        OrganizationInfo m_OrganizationInfo;

        readonly Button m_DismissButton;

        public Button DismissButton => m_DismissButton;

        static readonly string k_StorageUsageWarningMessage = L10n.Tr("{0} asset storage has reached {1}% usage.");
        static readonly string k_UploadPageErrorLevelMessage = L10n.Tr("You will not be able to upload new assets.");
        static readonly string k_Dismiss = L10n.Tr("Dismiss");
        static readonly string k_Upgrade = L10n.Tr("Upgrade Storage");

        static readonly int? k_StoragePercentUsageInfoThreshold = 75;
        static readonly int? k_StoragePercentUsageWarningThreshold = 90;
        static readonly int? k_StoragePercentUsageErrorThreshold = 100;

        static List<string> m_DismissedOrganizationInfoLevelMessage = new();

        public StorageInfoHelpBox(IPageManager pageManager, IProjectOrganizationProvider projectOrganizationProvider,
            ILinksProxy linksProxy, IUnityConnectProxy unityConnectProxy, ISettingsManager settingsManager)
        {
            m_PageManager = pageManager;
            m_ProjectOrganizationProvider = projectOrganizationProvider;
            m_UnityConnectProxy = unityConnectProxy;
            m_SettingsManager = settingsManager;

            messageType = HelpBoxMessageType.Info;

            m_DismissButton = new Button(DismissOrganizationInfoLevelMessage)
            {
                text = k_Dismiss
            };
            Add(m_DismissButton);

            var cloudStorageUpgradeButton = new Button(linksProxy.OpenCloudStorageUpgradePlan)
            {
                text = k_Upgrade
            };
            Add(cloudStorageUpgradeButton);


            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
        }

        void OnAttachToPanel(AttachToPanelEvent evt)
        {
            m_ProjectOrganizationProvider.OrganizationChanged += OnOrganizationChanged;
            m_PageManager.ActivePageChanged += OnActivePageChanged;
            OnOrganizationChanged(m_ProjectOrganizationProvider.SelectedOrganization);
            Refresh();
        }

        void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            m_ProjectOrganizationProvider.OrganizationChanged -= OnOrganizationChanged;
            m_PageManager.ActivePageChanged -= OnActivePageChanged;
        }

        void OnActivePageChanged(IPage page) => Refresh();

        void DismissOrganizationInfoLevelMessage()
        {
            if (!string.IsNullOrEmpty(m_ProjectOrganizationProvider.SelectedOrganization?.Id) &&
                !m_DismissedOrganizationInfoLevelMessage.Contains(m_ProjectOrganizationProvider.SelectedOrganization?.Id))
            {
                m_DismissedOrganizationInfoLevelMessage.Add(m_ProjectOrganizationProvider.SelectedOrganization?.Id);
            }
        }

        public async void RefreshCloudStorageAsync(TimerState timerState)
        {
            m_CloudStorageUsage = await GetCloudStorageUsageAsync();
            Refresh();
        }

        async Task<StorageUsage> GetCloudStorageUsageAsync()
        {
            if (m_SettingsManager.PrivateCloudSettings.ServicesEnabled)
            {
                return await Task.FromResult(new StorageUsage(1, 10));
            }

            if (m_OrganizationInfo == null || !m_UnityConnectProxy.AreCloudServicesReachable || string.IsNullOrEmpty(m_ProjectOrganizationProvider.SelectedOrganization?.Id))
                return null;

            return await m_ProjectOrganizationProvider.GetStorageUsageAsync(m_OrganizationInfo.Id);
        }

        void Refresh()
        {
            if (m_CloudStorageUsage == null || !m_CloudStorageUsage.IsUsageBytesKnown || !m_CloudStorageUsage.IsTotalStorageQuotaBytesKnown)
            {
                UIElementsUtils.Hide(this);
                return;
            }

            var percentUsage = FormatUsage(m_CloudStorageUsage.UsageBytes * 1.0 / m_CloudStorageUsage.TotalStorageQuotaBytes * 1.0);
            if (percentUsage < k_StoragePercentUsageInfoThreshold)
            {
                UIElementsUtils.Hide(this);
                return;
            }

            // Only hide if dismissed by user and usage is under the warning threshold
            if (percentUsage < k_StoragePercentUsageWarningThreshold &&
                !string.IsNullOrEmpty(m_ProjectOrganizationProvider.SelectedOrganization?.Id) &&
                m_DismissedOrganizationInfoLevelMessage.Contains(m_ProjectOrganizationProvider.SelectedOrganization?.Id))
            {
                UIElementsUtils.Hide(this);
                return;
            }

            DisplayUsageMessage(percentUsage);
        }

        void DisplayUsageMessage(int percentUsage)
        {
            text = $"{string.Format(k_StorageUsageWarningMessage, m_OrganizationInfo.Name,  percentUsage)}";

            // Hide dismiss button if usage reached the warning level
            if (percentUsage >= k_StoragePercentUsageWarningThreshold)
            {
                messageType = HelpBoxMessageType.Warning;
                UIElementsUtils.Hide(m_DismissButton);
            }

            if (percentUsage >= k_StoragePercentUsageErrorThreshold)
            {
                messageType = HelpBoxMessageType.Error;
                if (m_PageManager.ActivePage is UploadPage)
                {
                    text = $"{text} {k_UploadPageErrorLevelMessage}";
                }
            }

            UIElementsUtils.Show(this);
        }

        int FormatUsage(double usage)
        {
            return Convert.ToInt32(Math.Round(usage * 100.0));
        }


        async void OnOrganizationChanged(OrganizationInfo organizationInfo)
        {
            m_CloudStorageUsage = null;
            m_OrganizationInfo = organizationInfo;
            m_CloudStorageUsage = await GetCloudStorageUsageAsync();
            Refresh();
        }
    }
}

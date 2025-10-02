using System;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    enum ProjectSettingsMenu
    {
        Services,
        AssetManager,
        PrivateCloudServices
    }
    
    interface ILinksProxy : IService
    {
        bool CanOpenAssetManagerDashboard { get; }
        bool CanOpenAssetManagerDocumentation { get; }
        void OpenAssetManagerDashboard();
        void OpenAssetManagerDashboard(AssetIdentifier assetIdentifier);
        void OpenAssetManagerDocumentationPage(string page);
        void OpenProjectSettings(ProjectSettingsMenu menu);
        void OpenPreferences();
        void OpenCloudStorageUpgradePlan();
        bool TryGetAssetManagerDashboardUrl(out string url, bool limitUrlLength = false);
    }

    [Serializable]
    class LinksProxy : BaseService<ILinksProxy>, ILinksProxy
    {
        [SerializeReference]
        IApplicationProxy m_ApplicationProxy;

        [SerializeReference]
        IProjectOrganizationProvider m_ProjectOrganizationProvider;

        [SerializeReference]
        IPageManager m_PageManager;

        [SerializeReference]
        IUrlProvider m_UrlProvider;

        public bool CanOpenAssetManagerDashboard => m_UrlProvider?.CanGetAssetManagerDashboardUrl ?? false;
        public bool CanOpenAssetManagerDocumentation => m_UrlProvider?.CanGetDocumentationUrl ?? false;

        static readonly string k_CloudStorageUpgradePlanRoute = "/products/compare-plans/unity-cloud";
        static readonly string k_HttpsUriScheme = "https://";
        static readonly string k_UnityDomain = "unity.com";

        [ServiceInjection]
        public void Inject(IApplicationProxy applicationProxy, IProjectOrganizationProvider projectOrganizationProvider, IPageManager pageManager, IUrlProvider urlProvider)
        {
            m_ApplicationProxy = applicationProxy;
            m_ProjectOrganizationProvider = projectOrganizationProvider;
            m_PageManager = pageManager;
            m_UrlProvider = urlProvider;
        }

        protected override void ValidateServiceDependencies()
        {
            base.ValidateServiceDependencies();

            m_ApplicationProxy ??= ServicesContainer.instance.Get<IApplicationProxy>();
            m_UrlProvider ??= ServicesContainer.instance.Get<IUrlProvider>();
        }

        public void OpenAssetManagerDashboard()
        {
            if (!TryGetAssetManagerDashboardUrl(out var url) || string.IsNullOrEmpty(url))
                return;

            m_ApplicationProxy.OpenUrl(url);

            AnalyticsSender.SendEvent(new ExternalLinkClickedEvent(ExternalLinkClickedEvent.ExternalLinkType.OpenDashboard));
            AnalyticsSender.SendEvent(new MenuItemSelectedEvent(MenuItemSelectedEvent.MenuItemType.GoToDashboard));
        }

        public void OpenAssetManagerDashboard(AssetIdentifier assetIdentifier)
        {
            if (!CanOpenAssetManagerDashboard)
                return;

            var organizationId = assetIdentifier?.OrganizationId ?? m_ProjectOrganizationProvider?.SelectedOrganization?.Id;
            var projectId = assetIdentifier?.ProjectId;
            var assetId = assetIdentifier?.AssetId;
            var assetVersion = assetIdentifier?.Version;

            if (!string.IsNullOrEmpty(projectId) && !string.IsNullOrEmpty(assetId))
            {
                if (m_UrlProvider.TryGetAssetManagerDashboardUrl(out var url,
                        $"/organizations/{organizationId}/projects/{projectId}/assets?assetId={assetId}:{assetVersion}"))
                {
                    m_ApplicationProxy.OpenUrl(url);
                    AnalyticsSender.SendEvent(new ExternalLinkClickedEvent(ExternalLinkClickedEvent.ExternalLinkType.OpenAsset));
                }
            }
            else
            {
                OpenAssetManagerDashboard();
            }
        }

        public void OpenCloudStorageUpgradePlan()
        {
            m_ApplicationProxy.OpenUrl($"{k_HttpsUriScheme}{k_UnityDomain}{k_CloudStorageUpgradePlanRoute}");
            AnalyticsSender.SendEvent(new ExternalLinkClickedEvent(ExternalLinkClickedEvent.ExternalLinkType.UpgradeCloudStoragePlan));
        }

        public void OpenAssetManagerDocumentationPage(string page)
        {
            if (m_UrlProvider.TryGetDocumentationUrl(out var url, $"/cloud/en-us/asset-manager/{page}"))
            {
                m_ApplicationProxy.OpenUrl(url);
                AnalyticsSender.SendEvent(new MenuItemSelectedEvent(MenuItemSelectedEvent.MenuItemType.GotoSubscriptions));
            }
        }

        public void OpenProjectSettings(ProjectSettingsMenu menu)
        {
            var menuPath = menu switch
            {
                ProjectSettingsMenu.Services => "Project/Services",
                ProjectSettingsMenu.AssetManager => "Project/Asset Manager",
                ProjectSettingsMenu.PrivateCloudServices => "Project/Asset Manager/Private Cloud Services",
                _ => throw new ArgumentOutOfRangeException(nameof(menu), menu, null)
            };
            SettingsService.OpenProjectSettings(menuPath);
            AnalyticsSender.SendEvent(new MenuItemSelectedEvent(MenuItemSelectedEvent.MenuItemType.ProjectSettings));
        }

        public void OpenPreferences()
        {
            SettingsService.OpenUserPreferences("Preferences/Asset Manager");
            AnalyticsSender.SendEvent(new MenuItemSelectedEvent(MenuItemSelectedEvent.MenuItemType.Preferences));
        }

        public bool TryGetAssetManagerDashboardUrl(out string url, bool limitUrlLength = false)
        {
            if (!m_UrlProvider.TryGetAssetManagerDashboardUrl(out url))
                return false;

            var organizationId = m_ProjectOrganizationProvider?.SelectedOrganization?.Id;
            var projectId = m_ProjectOrganizationProvider?.SelectedProject?.Id;
            var collectionPath = m_ProjectOrganizationProvider?.SelectedCollection?.GetFullPath();
            var isProjectSelected = m_PageManager.ActivePage is CollectionPage or UploadPage;
            
            if (limitUrlLength)
            {
                if (organizationId != null)
                {
                    url = $"{url}/organizations/{organizationId}/assets/all";
                }
            }
            else if (isProjectSelected && organizationId != null && projectId != null && !string.IsNullOrEmpty(collectionPath))
            {
                url = $"{url}/organizations/{organizationId}/projects/{projectId}/assets/collectionPath/{Uri.EscapeDataString(collectionPath)}";
            }
            else if (isProjectSelected && organizationId != null && projectId != null)
            {
                url = $"{url}/organizations/{organizationId}/projects/{projectId}/assets";
            }
            else if (organizationId != null && m_PageManager.ActivePage is AllAssetsPage)
            {
                url = $"{url}/organizations/{organizationId}/assets/all";
            }

            return true;
        }
    }
}

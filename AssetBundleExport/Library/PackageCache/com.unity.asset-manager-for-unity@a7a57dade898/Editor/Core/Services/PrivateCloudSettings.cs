using System;
using UnityEditor;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    [Serializable]
    class PrivateCloudSettings
    {
        const string k_PrivateCloudSettingsKey = "AM4U.privateCloudSettings";

        public static event Action SettingsUpdated;

        [SerializeField]
        bool m_ServicesEnabled = false;
        [SerializeField]
        string m_FullyQualifiedDomainName = string.Empty;
        [SerializeField]
        string m_PathPrefix = string.Empty;
        [SerializeField]
        string m_OpenIdManifestUrl = string.Empty;
        [SerializeField]
        string m_SelectedOrganizationId = string.Empty;
        [SerializeField]
        string m_SelectedProjectId = string.Empty;

        public bool ServicesEnabled => m_ServicesEnabled;
        public string FullyQualifiedDomainName => m_FullyQualifiedDomainName;
        public string PathPrefix => m_PathPrefix;
        public string OpenIdManifestUrl => m_OpenIdManifestUrl;

        public string SelectedOrganizationId => m_SelectedOrganizationId;
        public string SelectedProjectId => m_SelectedProjectId;

        PrivateCloudSettings() { }

        /// <summary>
        /// For testing purposes only.
        /// </summary>
        internal PrivateCloudSettings(bool enabled)
        {
            m_ServicesEnabled = enabled;
        }

        /// <summary>
        /// Should only be called directly by the `BaseSDKService` and the `PrivateCloudAuthenticator`.
        /// </summary>
        public static PrivateCloudSettings Load() => Load(new UnityEditor.SettingsManagement.Settings(AssetManagerCoreConstants.PackageName));

        /// <summary>
        /// Should only be called directly by the `AssetManagerUserSettingsProvider`.
        /// </summary>
        public static PrivateCloudSettings Load(UnityEditor.SettingsManagement.Settings settings) => settings.Get(k_PrivateCloudSettingsKey, SettingsScope.User, new PrivateCloudSettings());

        public static void SetEnabled(bool enable)
        {
            var settings = Load();
            settings.m_ServicesEnabled = enable;
            settings.Save();
        }

        public static void EstablishConnection(string domainName, string pathPrefix, string openIdManifestUrl)
        {
            var settings = Load();
            settings.m_FullyQualifiedDomainName = domainName ?? string.Empty;
            settings.m_PathPrefix = pathPrefix ?? string.Empty;
            settings.m_OpenIdManifestUrl = openIdManifestUrl ?? string.Empty;
            settings.Save();
        }

        public static void SetLinkedOrganizationAndProject(string organizationId, string projectId)
        {
            var settings = Load();
            settings.m_SelectedOrganizationId = organizationId ?? string.Empty;
            settings.m_SelectedProjectId = projectId ?? string.Empty;
            settings.Save(false);
        }

        void Save(bool withNotify = true)
        {
            var settings = new UnityEditor.SettingsManagement.Settings(AssetManagerCoreConstants.PackageName);
            settings.Set(k_PrivateCloudSettingsKey, this, SettingsScope.User);

            if (withNotify)
            {
                SettingsUpdated?.Invoke();
            }
        }
    }
}

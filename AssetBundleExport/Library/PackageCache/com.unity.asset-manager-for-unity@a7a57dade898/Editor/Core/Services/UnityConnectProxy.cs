using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.AssetManager.Core.Editor
{
    interface IUnityConnectProxy : IService
    {
        event Action<bool> CloudServicesReachabilityChanged;
        event Action OrganizationIdChanged;
        event Action ProjectIdChanged;
        string OrganizationId { get; }
        string ProjectId { get; }

        bool HasValidOrganizationId { get; }
        bool HasValidProjectId { get; }

        bool AreCloudServicesReachable { get; }
    }

    [Serializable]
    [ExcludeFromCodeCoverage]
    class UnityConnectProxy : BaseService<IUnityConnectProxy>, IUnityConnectProxy
    {
        public event Action<bool> CloudServicesReachabilityChanged;
        public event Action OrganizationIdChanged;
        public event Action ProjectIdChanged;

        public string OrganizationId => m_ConnectedOrganizationId;

        public string ProjectId => m_ConnectedProjectId;

        public bool HasValidOrganizationId => m_ConnectedOrganizationId != k_NoValue && !string.IsNullOrEmpty(m_ConnectedOrganizationId);
        public bool HasValidProjectId => m_ConnectedProjectId != k_NoValue && !string.IsNullOrEmpty(m_ConnectedProjectId);

        public bool AreCloudServicesReachable => m_AreCloudServicesReachable != CloudServiceReachability.NotReachable;

        static readonly string k_NoValue = "none";
        static readonly string k_CloudServiceHealthCheckUrl = "https://services.api.unity.com";

        [SerializeField]
        string m_ConnectedOrganizationId = k_NoValue;

        [SerializeField]
        string m_ConnectedProjectId = k_NoValue;

        [SerializeField]
        CloudServiceReachability m_AreCloudServicesReachable = CloudServiceReachability.Unknown;

        enum CloudServiceReachability
        {
            Unknown,
            Reachable,
            NotReachable
        }

        [SerializeReference]
        IApplicationProxy m_ApplicationProxy;

        [SerializeReference]
        IPermissionsManager m_PermissionsManager;

        [SerializeReference]
        ISettingsManager m_SettingsManager;

        [SerializeField]
        bool m_IsReachingPrivateCloudServices;

        [SerializeField]
        double m_LastInternetCheck;

        [SerializeField]
        bool m_IsCouldServicesReachableRequestComplete;

        [ServiceInjection]
        public void Inject(IApplicationProxy applicationProxy, IPermissionsManager permissionsManager, ISettingsManager settingsManager)
        {
            m_ApplicationProxy = applicationProxy;
            m_PermissionsManager = permissionsManager;
            m_SettingsManager = settingsManager;
        }

        public override void OnEnable()
        {
            base.OnEnable();

            m_LastInternetCheck = m_ApplicationProxy.TimeSinceStartup;
            CheckCloudServicesHealth();

            m_ApplicationProxy.Update += Update;
            m_PermissionsManager.AuthenticationStateChanged += CheckCloudServicesReachability;
        }

        protected override void ValidateServiceDependencies()
        {
            base.ValidateServiceDependencies();

            m_ApplicationProxy ??= ServicesContainer.instance.Get<IApplicationProxy>();
            m_PermissionsManager ??= ServicesContainer.instance.Get<IPermissionsManager>();
            m_SettingsManager ??= ServicesContainer.instance.Get<ISettingsManager>();
        }

        public override void OnDisable()
        {
            if (m_ApplicationProxy != null)
                m_ApplicationProxy.Update -= Update;

            if (m_PermissionsManager != null)
                m_PermissionsManager.AuthenticationStateChanged -= CheckCloudServicesReachability;
        }

        void Update()
        {
            var isReachingPrivateCloudServices = false;

            var settings = m_SettingsManager.PrivateCloudSettings;

            // Private Cloud must set its own organization and project ids.
            // We cannot rely on the CloudProjectSettings linked organization/project as these remain connected to public Unity services.
            if (settings.ServicesEnabled)
            {
                isReachingPrivateCloudServices = true;
                if (!m_ConnectedOrganizationId.Equals(settings.SelectedOrganizationId))
                {
                    m_ConnectedOrganizationId = settings.SelectedOrganizationId;
                    m_ConnectedProjectId = string.Empty;
                    OrganizationIdChanged?.Invoke();
                }
                else if (!m_ConnectedProjectId.Equals(settings.SelectedProjectId))
                {
                    m_ConnectedProjectId = settings.SelectedProjectId;
                }
            }
            else if (!CloudProjectSettings.projectBound)
            {
                m_ConnectedOrganizationId = k_NoValue;
                m_ConnectedProjectId = k_NoValue;
                OrganizationIdChanged?.Invoke();
            }
#if UNITY_2021
            else if (CloudProjectSettings.organizationId != k_NoValue && !m_ConnectedOrganizationId.Equals(CloudProjectSettings.organizationId))
            {
                m_ConnectedOrganizationId = CloudProjectSettings.organizationId;
#else
            else if (!m_ConnectedOrganizationId.Equals(CloudProjectSettings.organizationKey))
            {
                m_ConnectedOrganizationId = CloudProjectSettings.organizationKey;
#endif
                m_ConnectedProjectId = k_NoValue;
                OrganizationIdChanged?.Invoke();
            }
            else if (!m_ConnectedProjectId.Equals(CloudProjectSettings.projectId))
            {
                m_ConnectedProjectId = CloudProjectSettings.projectId;
                ProjectIdChanged?.Invoke();
            }

            CheckCloudServicesReachability(m_IsReachingPrivateCloudServices != isReachingPrivateCloudServices);
            m_IsReachingPrivateCloudServices = isReachingPrivateCloudServices;
        }

        void CheckCloudServicesReachability(AuthenticationState _)
        {
            CheckCloudServicesReachability(forceCheck: true);
        }

        void CheckCloudServicesReachability(bool forceCheck = false)
        {
            var timeSinceStartup = m_ApplicationProxy.TimeSinceStartup;

            if ((!forceCheck && timeSinceStartup - m_LastInternetCheck < 2.0) || !m_IsCouldServicesReachableRequestComplete)
                return;

            m_LastInternetCheck = timeSinceStartup;

            if (!m_ApplicationProxy.InternetReachable && m_AreCloudServicesReachable != CloudServiceReachability.NotReachable)
            {
                m_AreCloudServicesReachable = CloudServiceReachability.NotReachable;
                CloudServicesReachabilityChanged?.Invoke(AreCloudServicesReachable);
            }
            else if (m_ApplicationProxy.InternetReachable)
            {
                CheckCloudServicesHealth();
            }
        }

        void CheckCloudServicesHealth()
        {
            m_IsCouldServicesReachableRequestComplete = false;

            if (m_SettingsManager.PrivateCloudSettings.ServicesEnabled)
            {
                var privateCloudReachability = m_PermissionsManager?.AuthenticationState == AuthenticationState.AwaitingInitialization
                    ? CloudServiceReachability.NotReachable
                    : CloudServiceReachability.Reachable;
                if (privateCloudReachability != m_AreCloudServicesReachable)
                {
                    m_AreCloudServicesReachable = privateCloudReachability;
                    CloudServicesReachabilityChanged?.Invoke(AreCloudServicesReachable);
                }

                m_IsCouldServicesReachableRequestComplete = true;
                return;
            }

            var request = UnityWebRequest.Head(k_CloudServiceHealthCheckUrl);
            var asyncOperation = request.SendWebRequest();
            try
            {
                asyncOperation.completed += _ =>
                {
                    var cloudServiceReachability = request.result == UnityWebRequest.Result.Success
                        ? CloudServiceReachability.Reachable
                        : CloudServiceReachability.NotReachable;

                    if (m_AreCloudServicesReachable != cloudServiceReachability)
                    {
                        m_AreCloudServicesReachable = cloudServiceReachability;
                        CloudServicesReachabilityChanged?.Invoke(AreCloudServicesReachable);
                    }
                };
            }
            catch (Exception)
            {
                if (AreCloudServicesReachable)
                {
                    m_AreCloudServicesReachable = CloudServiceReachability.NotReachable;
                    CloudServicesReachabilityChanged?.Invoke(AreCloudServicesReachable);
                }
            }
            finally
            {
                m_IsCouldServicesReachableRequestComplete = true;
            }
        }
    }
}

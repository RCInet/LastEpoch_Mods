using System;
using System.Reflection;
using System.Threading.Tasks;
using Unity.Cloud.AppLinkingEmbedded;
using Unity.Cloud.AssetsEmbedded;
using Unity.Cloud.CommonEmbedded;
using Unity.Cloud.CommonEmbedded.Runtime;
using Unity.Cloud.IdentityEmbedded;
using Unity.Cloud.IdentityEmbedded.Editor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    /// <summary>
    /// A common base for the implementation of services that requires the cloud SDK.
    /// </summary>
    abstract class BaseSdkService : BaseService
    {
        /// <summary>
        /// An override class that allows the sdk service to be overridden. Only used for testing.
        /// </summary>
        [Serializable]
        internal class SdkServiceOverride
        {
            public IAssetRepository AssetRepository { get; set; }
            public IOrganizationRepository OrganizationRepository { get; set; }
            public IWebAppUrlComposer WebAppUrlComposer { get; set; }

            public Unity.Cloud.IdentityEmbedded.AuthenticationState AuthenticationState { get; set; }

            public Action AuthenticationStateChanged;
        }

        [SerializeReference]
        SdkServiceOverride m_ServiceOverride;

        protected IOrganizationRepository OrganizationRepository => m_ServiceOverride?.OrganizationRepository ?? Services.OrganizationRepository;
        protected IAssetRepository AssetRepository => m_ServiceOverride?.AssetRepository ?? Services.AssetRepository;
        protected IWebAppUrlComposer WebAppUrlComposer => m_ServiceOverride?.WebAppUrlComposer ?? Services.WebAppUrlComposer;

        protected BaseSdkService() { }

        /// <summary>
        /// Internal constructor that allows the sdk service to be overridden. Only used for testing.
        /// </summary>
        /// <param name="sdkServiceOverride"></param>
        protected BaseSdkService(SdkServiceOverride sdkServiceOverride)
        {
            m_ServiceOverride = sdkServiceOverride;
        }
        
        public async Task AuthenticationStateMoveNextAsync()
        {
            if (m_ServiceOverride == null)
            {
                await Services.PrivateCloudAuthenticatorStateMoveNextAsync();
            }
        }

        protected void InitAuthenticatedServices()
        {
            if (m_ServiceOverride == null)
            {
                Services.InitAuthenticatedServices();
            }
        }

        protected Unity.Cloud.IdentityEmbedded.AuthenticationState GetAuthenticationState() => m_ServiceOverride?.AuthenticationState ?? Services.AuthenticationState;

        protected void RegisterOnAuthenticationStateChanged(Action callback)
        {
            if (m_ServiceOverride == null)
            {
                Services.AuthenticationStateChanged += callback;
            }
            else
            {
                m_ServiceOverride.AuthenticationStateChanged += callback;
            }
        }

        protected void UnregisterOnAuthenticationStateChanged(Action callback)
        {
            if (m_ServiceOverride == null)
            {
                Services.AuthenticationStateChanged -= callback;
            }
            else
            {
                m_ServiceOverride.AuthenticationStateChanged -= callback;
            }
        }

        static class Services
        {
            static IAssetRepository s_AssetRepository;
            static PrivateCloudAuthenticator s_PrivateCloudAuthenticator;
            static IWebAppUrlComposer s_WebAppUrlComposer;

            public static IAssetRepository AssetRepository
            {
                get
                {
                    InitAuthenticatedServices();
                    return s_AssetRepository;
                }
            }

            public static IOrganizationRepository OrganizationRepository => s_PrivateCloudAuthenticator == null ? UnityEditorServiceAuthorizer.instance : s_PrivateCloudAuthenticator;

            public static IWebAppUrlComposer WebAppUrlComposer
            {
                get
                {
                    InitAuthenticatedServices();
                    return s_WebAppUrlComposer;
                }
            }

            public static Unity.Cloud.IdentityEmbedded.AuthenticationState AuthenticationState => s_PrivateCloudAuthenticator?.AuthenticationState ?? UnityEditorServiceAuthorizer.instance.AuthenticationState;

            public static event Action AuthenticationStateChanged;

            public static async Task PrivateCloudAuthenticatorStateMoveNextAsync()
            {
                if (s_PrivateCloudAuthenticator == null)
                    return;

                switch (AuthenticationState)
                {
                    case Unity.Cloud.IdentityEmbedded.AuthenticationState.LoggedIn:
                        await s_PrivateCloudAuthenticator.LogoutAsync();
                        break;
                    case Unity.Cloud.IdentityEmbedded.AuthenticationState.AwaitingLogin:
                        s_PrivateCloudAuthenticator.CancelLogin();
                        break;
                    default:
                        await s_PrivateCloudAuthenticator.LoginAsync();
                        break;
                }
            }

            public static void InitAuthenticatedServices()
            {
                if (s_AssetRepository == null)
                {
                    CreateServices();
                }
            }

            static void CreateServices()
            {
                var pkgInfo = PackageInfo.FindForAssembly(Assembly.GetAssembly(typeof(Services)));
                var httpClient = new UnityHttpClient();
                IServiceHostResolver serviceHostResolver;
                IServiceHttpClient serviceHttpClient;

                PrivateCloudSettings.SettingsUpdated -= OnPrivateCloudServicesAvailable;
                PrivateCloudSettings.SettingsUpdated += OnPrivateCloudServicesAvailable;

                var privateCloudSettings = PrivateCloudSettings.Load();
                if (privateCloudSettings.ServicesEnabled)
                {
                    serviceHostResolver = ServiceHostResolverFactory.CreateForFullyQualifiedDomainName(privateCloudSettings.FullyQualifiedDomainName, privateCloudSettings.PathPrefix);
                    s_PrivateCloudAuthenticator = new PrivateCloudAuthenticator();
                    serviceHttpClient =
                        new ServiceHttpClient(httpClient, s_PrivateCloudAuthenticator, new AppIdProvider())
                            .WithApiSourceHeaders(pkgInfo.name, pkgInfo.version);

                    UnityEditorServiceAuthorizer.instance.AuthenticationStateChanged -= OnAuthenticationStateChanged;
                    s_PrivateCloudAuthenticator.AuthenticationStateChanged += OnAuthenticationStateChanged;
                }
                else
                {
                    serviceHostResolver = ServiceHostResolverFactory.Create();
                    serviceHttpClient =
                        new ServiceHttpClient(httpClient, UnityEditorServiceAuthorizer.instance, new AppIdProvider())
                            .WithApiSourceHeaders(pkgInfo.name, pkgInfo.version);

                    UnityEditorServiceAuthorizer.instance.AuthenticationStateChanged += OnAuthenticationStateChanged;
                }

                s_AssetRepository = AssetRepositoryFactory.Create(serviceHttpClient, serviceHostResolver, AssetRepositoryCacheConfiguration.NoCaching);
                s_WebAppUrlComposer = new WebAppUrlComposer(serviceHostResolver, serviceHttpClient);

                // When services are initialized, we need to invoke the authentication state changed event
                AuthenticationStateChanged?.Invoke();
            }

            static void OnPrivateCloudServicesAvailable()
            {
                s_AssetRepository = null;
                s_PrivateCloudAuthenticator = null;
                s_WebAppUrlComposer = null;
                InitAuthenticatedServices();
            }

            static void OnAuthenticationStateChanged(Unity.Cloud.IdentityEmbedded.AuthenticationState state)
            {
                AuthenticationStateChanged?.Invoke();
            }

            class AppIdProvider : IAppIdProvider
            {
                public AppId GetAppId()
                {
                    return new AppId();
                }
            }
        }
    }
}

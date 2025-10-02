using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    interface IUrlProvider : IService
    {
        bool CanGetAssetManagerDashboardUrl { get; }
        bool CanGetDocumentationUrl { get; }
        bool TryGetAssetManagerDashboardUrl(out string url, string pathAndQuery = null);
        bool TryGetDocumentationUrl(out string url, string pathAndQuery = null);
    }

    [Serializable]
    class UrlProvider : BaseSdkService, IUrlProvider
    {
        public override Type RegistrationType => typeof(IUrlProvider);

        [SerializeField]
        bool m_AssetManagerDashboardUrlSupported;

        [SerializeField]
        bool m_DocumentationUrlSupported;

        [SerializeField]
        string m_DocumentationBaseUrl;

        [SerializeField]
        string m_AssetManagerDashboardBaseUrl;

        [SerializeReference]
        IApplicationProxy m_ApplicationProxy;

        CancellationTokenSource m_InitializationCancellationTokenSource;

        public bool CanGetAssetManagerDashboardUrl => m_AssetManagerDashboardUrlSupported;
        public bool CanGetDocumentationUrl => m_DocumentationUrlSupported;

        public UrlProvider() { }

        /// <inheritdoc />
        internal UrlProvider(SdkServiceOverride sdkServiceOverride)
            : base(sdkServiceOverride) { }
        
        [ServiceInjection]
        public void Inject(IApplicationProxy applicationProxy)
        {
            m_ApplicationProxy = applicationProxy;
        }

        public override void OnEnable()
        {
            base.OnEnable();

            if (!m_ApplicationProxy.IsBatchMode)
            {
                // Initialization when in batch mode causes the Editor process to hang indefinitely.
                // When in batch mode, the service will only be initialized when the user logs in.
                StartInitialization();
            }

            RegisterOnAuthenticationStateChanged(OnAuthenticationStateChanged);
        }

        public override void OnDisable()
        {
            base.OnDisable();

            UnregisterOnAuthenticationStateChanged(OnAuthenticationStateChanged);
        }

        public bool TryGetAssetManagerDashboardUrl(out string url, string pathAndQuery = null)
        {
            if (!m_AssetManagerDashboardUrlSupported)
            {
                url = null;
                return false;
            }

            url = string.IsNullOrEmpty(pathAndQuery) ? m_AssetManagerDashboardBaseUrl : $"{m_AssetManagerDashboardBaseUrl}{pathAndQuery}";
            return true;
        }

        public bool TryGetDocumentationUrl(out string url, string pathAndQuery = null)
        {
            if (!m_DocumentationUrlSupported)
            {
                url = null;
                return false;
            }

            url = string.IsNullOrEmpty(pathAndQuery) ? m_DocumentationBaseUrl : $"{m_DocumentationBaseUrl}{pathAndQuery}";
            return true;
        }

        async void StartInitialization()
        {
            try
            {
                if (m_InitializationCancellationTokenSource != null)
                {
                    m_InitializationCancellationTokenSource.Cancel();
                    m_InitializationCancellationTokenSource.Dispose();
                }

                m_InitializationCancellationTokenSource = new CancellationTokenSource();
                await InitializeAsync(m_InitializationCancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // Initialization was cancelled, do nothing
            }
            catch (Exception e)
            {
                Utilities.DevLogException(e);
            }
        }

        async Task InitializeAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            m_DocumentationUrlSupported = await WebAppUrlComposer.IsWebAppSupportedAsync(Unity.Cloud.AppLinkingEmbedded.WebAppNames.Documentation);
            if (m_DocumentationUrlSupported)
            {
                cancellationToken.ThrowIfCancellationRequested();
                m_DocumentationBaseUrl = await WebAppUrlComposer.ComposeUrlAsync(Unity.Cloud.AppLinkingEmbedded.WebAppNames.Documentation);
            }

            cancellationToken.ThrowIfCancellationRequested();

            m_AssetManagerDashboardUrlSupported = await WebAppUrlComposer.IsWebAppSupportedAsync(Unity.Cloud.AppLinkingEmbedded.WebAppNames.AssetManager);
            if (m_AssetManagerDashboardUrlSupported)
            {
                cancellationToken.ThrowIfCancellationRequested();
                m_AssetManagerDashboardBaseUrl = await WebAppUrlComposer.ComposeUrlAsync(Unity.Cloud.AppLinkingEmbedded.WebAppNames.AssetManager);
            }
        }

        void OnAuthenticationStateChanged()
        {
            switch (GetAuthenticationState())
            {
                case Cloud.IdentityEmbedded.AuthenticationState.LoggedIn:
                    StartInitialization();
                    break;

                default:
                    m_AssetManagerDashboardUrlSupported = false;
                    m_DocumentationUrlSupported = false;
                    break;
            }
        }
    }
}

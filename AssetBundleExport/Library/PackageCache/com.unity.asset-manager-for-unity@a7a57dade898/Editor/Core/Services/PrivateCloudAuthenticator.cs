using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;
using Unity.Cloud.CommonEmbedded.Runtime;
using Unity.Cloud.AppLinkingEmbedded.Runtime;
using Unity.Cloud.IdentityEmbedded;
using Unity.Cloud.IdentityEmbedded.Runtime;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    /// <summary>
    /// A class containing Unity Player Settings to connect unity Cloud services to a Fully Qualified Domain Name.
    /// </summary>
    class PrivateCloudAuthenticator : IUrlRedirectionAuthenticator
    {
        /// <inheritdoc/>
        public Unity.Cloud.IdentityEmbedded.AuthenticationState AuthenticationState
        {
            get => m_PkceAuthenticator == null ? Unity.Cloud.IdentityEmbedded.AuthenticationState.AwaitingInitialization : m_AuthenticationState;
            private set
            {
                if (m_AuthenticationState == value)
                    return;
                m_AuthenticationState = value;
                AuthenticationStateChanged?.Invoke(m_AuthenticationState);
            }
        }

        public event Action<Unity.Cloud.IdentityEmbedded.AuthenticationState> AuthenticationStateChanged;

        PkceAuthenticator m_PkceAuthenticator;
        Unity.Cloud.IdentityEmbedded.AuthenticationState m_AuthenticationState = Unity.Cloud.IdentityEmbedded.AuthenticationState.AwaitingInitialization;

        public PrivateCloudAuthenticator()
        {
            _ = InitAuthenticator();
        }

        async Task InitAuthenticator()
        {
            var settings = PrivateCloudSettings.Load();
            if (!settings.ServicesEnabled || string.IsNullOrEmpty(settings.FullyQualifiedDomainName) || string.IsNullOrEmpty(settings.OpenIdManifestUrl))
                return;

            if (m_PkceAuthenticator == null)
            {
                m_PkceAuthenticator = CreatePkceAuthenticator(settings);
                m_PkceAuthenticator.AuthenticationStateChanged += OnAuthenticationStateChanged;
                try
                {
                    await m_PkceAuthenticator.InitializeAsync();
                }
                catch (Exception e)
                {
                    Utilities.DevLogError("Failed to initialize private cloud authenticator with exception:\n" + e);
                }

                OnAuthenticationStateChanged(m_PkceAuthenticator.AuthenticationState);
            }
        }

        static PkceAuthenticator CreatePkceAuthenticator(PrivateCloudSettings settings)
        {
            var editorProjectPersistentDataPath = $"{Application.persistentDataPath}/Unity Cloud Asset Manager";
            if (!System.IO.Directory.Exists(editorProjectPersistentDataPath))
            {
                System.IO.Directory.CreateDirectory(editorProjectPersistentDataPath);
            }

            var httpClient = new UnityHttpClient();
            var playerSettings = UnityCloudPlayerSettings.Instance;
            var platformSupport = new EditorPkcePlatformSupport(
                UrlRedirectionInterceptor.GetInstance(),
                new UnityRuntimeUrlProcessor(),
                playerSettings,
                playerSettings,
                editorProjectPersistentDataPath
            );

            var pathPrefix = string.IsNullOrEmpty(settings.PathPrefix)
                ? "/"
                : settings.PathPrefix;

            var serviceHostResolver =  ServiceHostResolverFactory.CreateForFullyQualifiedDomainName(settings.FullyQualifiedDomainName, pathPrefix);
            var pkceConfigurationProvider = PkceConfigurationProviderFactory.CreateForFullyQualifiedDomainName(serviceHostResolver, httpClient, settings.OpenIdManifestUrl, "sdk");

            // Build settings for PkceAuthenticator
            var pkceAuthenticatorSettingsBuilder = new PkceAuthenticatorSettingsBuilder(platformSupport, serviceHostResolver);
            pkceAuthenticatorSettingsBuilder.AddConfigurationProvider(pkceConfigurationProvider)
                .AddAppIdProvider(playerSettings)
                .AddAppNamespaceProvider(playerSettings)
                .AddHttpClient(httpClient);

            return new PkceAuthenticator(pkceAuthenticatorSettingsBuilder.Build());
        }

        async void OnAuthenticationStateChanged(Unity.Cloud.IdentityEmbedded.AuthenticationState authenticationState)
        {
            var settings = PrivateCloudSettings.Load();
            if (authenticationState.Equals(Unity.Cloud.IdentityEmbedded.AuthenticationState.LoggedIn) &&
                string.IsNullOrEmpty(settings.SelectedOrganizationId) &&
                string.IsNullOrEmpty(settings.SelectedProjectId))
            {
                await AutoSelectLoggedInUserOrganizationAndProjectAsync();
            }

            AuthenticationState = authenticationState;
        }

        async Task AutoSelectLoggedInUserOrganizationAndProjectAsync()
        {
            await InitAuthenticator();
            var organizations = new List<IOrganization>();
            var organizationsAsyncEnumerable = m_PkceAuthenticator.ListOrganizationsAsync(Range.All);
            await foreach (var organization in organizationsAsyncEnumerable)
            {
                organizations.Add(organization);
            }

            var selectedOrganizationId = string.Empty;
            var selectedProjectId = string.Empty;

            if (organizations.Count > 0)
            {
                selectedOrganizationId = organizations[0].Id.ToString();
                var projects = new List<IProject>();
                var projectsAsyncEnumerable = organizations[0].ListProjectsAsync(Range.All);
                await foreach (var project in projectsAsyncEnumerable)
                {
                    projects.Add(project);
                }
                if (projects.Count > 0)
                {
                    selectedProjectId = projects[0].Descriptor.ProjectId.ToString();
                }
            }

            PrivateCloudSettings.SetLinkedOrganizationAndProject(selectedOrganizationId, selectedProjectId);
        }

        public async Task AddAuthorization(HttpHeaders headers)
        {
            await InitAuthenticator();
            while (m_PkceAuthenticator.AuthenticationState.Equals(Unity.Cloud.IdentityEmbedded.AuthenticationState.AwaitingInitialization) ||
                   m_PkceAuthenticator.AuthenticationState.Equals(Unity.Cloud.IdentityEmbedded.AuthenticationState.AwaitingLogin))
            {
                await Task.Delay(100);
            }

            if (!m_PkceAuthenticator.AuthenticationState.Equals(Unity.Cloud.IdentityEmbedded.AuthenticationState.LoggedIn))
            {
                throw new ForbiddenException("Cannot add authorization header to http request. User is not logged in.");
            }

            await m_PkceAuthenticator.AddAuthorization(headers);
        }

        public IAsyncEnumerable<IOrganization> ListOrganizationsAsync(Range range, CancellationToken cancellationToken = default)
        {
            return m_PkceAuthenticator.ListOrganizationsAsync(range, cancellationToken);
        }

        public async Task<IOrganization> GetOrganizationAsync(OrganizationId organizationId)
        {
            if (m_PkceAuthenticator == null)
            {
                await InitAuthenticator();
            }
            return await m_PkceAuthenticator.GetOrganizationAsync(organizationId);
        }

        public async Task<IUserInfo> GetUserInfoAsync()
        {
            if (m_PkceAuthenticator == null)
            {
                await InitAuthenticator();
            }
            return await m_PkceAuthenticator.GetUserInfoAsync();
        }

        public async Task<bool> HasValidPreconditionsAsync()
        {
            return await Task.FromResult(true);
        }

        public async Task InitializeAsync()
        {
            await InitAuthenticator();
        }

        public async Task LoginAsync()
        {
            await InitAuthenticator();
            if (m_PkceAuthenticator.AuthenticationState == Unity.Cloud.IdentityEmbedded.AuthenticationState.LoggedIn)
                return;
            await m_PkceAuthenticator.LoginAsync();
        }

        public void CancelLogin()
        {
            if (m_PkceAuthenticator.AuthenticationState == Unity.Cloud.IdentityEmbedded.AuthenticationState.AwaitingLogin)
            {
                m_PkceAuthenticator.CancelLogin();
            }
        }

        public async Task LogoutAsync(bool clearBrowserCache = false)
        {
            await InitAuthenticator();
            if (m_PkceAuthenticator.AuthenticationState == Unity.Cloud.IdentityEmbedded.AuthenticationState.LoggedOut)
                return;
            await m_PkceAuthenticator.LogoutAsync(clearBrowserCache);
        }
    }
}

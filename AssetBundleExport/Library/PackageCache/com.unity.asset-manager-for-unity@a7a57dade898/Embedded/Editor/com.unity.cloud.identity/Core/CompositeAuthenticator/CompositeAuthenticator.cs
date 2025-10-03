using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// Uses a list of injected <see cref="IAuthenticator"/> to provide support for multiple authentication flows.
    /// </summary>
    /// <remarks>
    /// The `CompositeAuthenticator` uses internally the first <see cref="IAuthenticator"/> from the injected list that returns a true value when invoking <see cref="IAuthenticator.HasValidPreconditionsAsync"/> method.
    /// Depending on the validated <see cref="IAuthenticator"/>, the authentication flow can require manual interaction with a UI. Use the <see cref="ICompositeAuthenticator.RequiresGUI"/> value to decide if you need to enable manual login features.
    /// </remarks>
    /// <example>
    /// <code source="../../Samples/Documentation/Scripting/CompositeAuthenticatorExample.cs" region="InitializeAndShutdown"/>
    /// </example>
    class CompositeAuthenticator : ICompositeAuthenticator, IDisposable
    {
        IAuthenticator m_Authenticator { get; set; }
        IUrlRedirectionAuthenticator m_UrlRedirectionAuthenticator { get; set; }
        readonly IEnumerable<IAuthenticator> m_Authenticators;

        /// <inheritdoc/>
        public bool RequiresGUI => m_UrlRedirectionAuthenticator != null;

        AuthenticationState m_AuthenticationState = AuthenticationState.AwaitingInitialization;

        /// <inheritdoc/>
        public event Action<AuthenticationState> AuthenticationStateChanged;

        /// <summary>
        /// Provides a <see cref="ICompositeAuthenticator"/> that accepts a <see cref="CompositeAuthenticatorSettings"/> to handle and prioritize runtime execution contexts.
        /// </summary>
        /// <param name="compositeAuthenticatorSettings">A <see cref="CompositeAuthenticatorSettings"/> that contains the prioritized list of <see cref="IAuthenticator"/>.</param>
        public CompositeAuthenticator(CompositeAuthenticatorSettings compositeAuthenticatorSettings)
        {
            m_Authenticators = compositeAuthenticatorSettings.Authenticators;
        }

        async Task<IAuthenticator> GetFirstValidAuthenticator(IEnumerable<IAuthenticator> authenticators)
        {
            using (var enumerator = authenticators.GetEnumerator())
            while (enumerator.MoveNext())
            {
                if (await enumerator.Current.HasValidPreconditionsAsync())
                    return enumerator.Current;
            }
            return null;
        }

        /// <inheritdoc/>
        public AuthenticationState AuthenticationState
        {
            get => m_AuthenticationState;
            private set
            {
                m_AuthenticationState = value;
                AuthenticationStateChanged?.Invoke(m_AuthenticationState);
            }
        }

        void OnAuthenticationStateChanged(AuthenticationState newAuthenticationState)
        {
            AuthenticationState = newAuthenticationState;
        }

        /// <inheritdoc/>
        public Task LoginAsync()
        {
            if (!RequiresGUI)
                 throw new InvalidOperationException("Current instance does not require graphical user interface.");

            return m_UrlRedirectionAuthenticator.LoginAsync();
        }

        /// <inheritdoc/>
        public void CancelLogin()
        {
            if (!RequiresGUI)
                throw new InvalidOperationException("Current instance does not require graphical user interface.");

            m_UrlRedirectionAuthenticator.CancelLogin();
        }

        /// <inheritdoc/>
        public Task LogoutAsync(bool clearBrowserCache = false)
        {
            if (!RequiresGUI)
                throw new InvalidOperationException("Current instance does not require graphical user interface.");

            return m_UrlRedirectionAuthenticator.LogoutAsync(clearBrowserCache);
        }

        /// <inheritdoc/>
        /// <remarks>The <see cref="CompositeAuthenticator"/> always returns false to prevent nested <see cref="ICompositeAuthenticator"/> architecture.</remarks>
        public Task<bool> HasValidPreconditionsAsync()
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        async public Task InitializeAsync()
        {
            m_Authenticator = await GetFirstValidAuthenticator(m_Authenticators);

            if (m_Authenticator == null)
                throw new InvalidOperationException("Cannot execute CompositeAuthenticator. No valid IAuthenticator found in list.");

            m_Authenticator.AuthenticationStateChanged += OnAuthenticationStateChanged;
            if (m_Authenticator is IUrlRedirectionAuthenticator urlRedirectionAuthenticator)
            {
                m_UrlRedirectionAuthenticator = urlRedirectionAuthenticator;
            }
            await m_Authenticator.InitializeAsync();
        }

        /// <inheritdoc cref="Unity.Cloud.CommonEmbedded.IServiceAuthorizer.AddAuthorization"/>
        public Task AddAuthorization(HttpHeaders headers)
        {
            return m_Authenticator.AddAuthorization(headers);
        }

        /// <summary>
        /// Disposes `IDisposable` references.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes `IDisposable` references internally.
        /// </summary>
        /// <param name="disposing">The Boolean value received from the public `Dispose` method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            m_Authenticator.AuthenticationStateChanged -= OnAuthenticationStateChanged;
            if (m_Authenticator is IDisposable disposableAuthenticator)
                disposableAuthenticator.Dispose();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<IOrganization> ListOrganizationsAsync(Range range,
            CancellationToken cancellationToken = default)
        {
            return m_Authenticator.ListOrganizationsAsync(range, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IOrganization> GetOrganizationAsync(OrganizationId organizationId)
        {
            return await m_Authenticator.GetOrganizationAsync(organizationId);
        }

        /// <inheritdoc/>
        public async Task<IUserInfo> GetUserInfoAsync()
        {
            return await m_Authenticator.GetUserInfoAsync();
        }
    }
}


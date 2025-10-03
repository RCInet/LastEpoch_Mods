using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;
using Unity.Cloud.CommonEmbedded.Runtime;
using UnityEngine;

namespace Unity.Cloud.IdentityEmbedded.Editor
{
    /// <summary>
    /// An <see cref="IUnityEditorAccessTokenProvider"/> implementation to provide a Unity Editor access token from launch arguments.
    /// </summary>
    class LaunchArgumentsUnityEditorAccessTokenProvider : IUnityEditorAccessTokenProvider, IDisposable
    {
        readonly string m_Username;
        readonly string m_Password;
        readonly string m_UnitySubdomain;
        DeviceToken m_DeviceToken;
        DateTime m_DeviceTokenRetrievalTime;
        readonly SemaphoreSlim m_GetAccessTokenSemaphore;

        /// <summary>
        /// Returns an <see cref="IUnityEditorAccessTokenProvider"/> implementation that provides a Unity Editor access token from launch arguments.
        /// </summary>
        /// <param name="serviceHostResolver">A <see cref="IServiceHostResolver"/> instance.</param>
        public LaunchArgumentsUnityEditorAccessTokenProvider(IServiceHostResolver serviceHostResolver)
        {
            // Use SemaphoreSlim to refresh token in multi thread context.
            m_GetAccessTokenSemaphore = new SemaphoreSlim(1, 1);

            var allArgs  = Environment.GetCommandLineArgs();
            for(var i=0;i<allArgs.Length;i++)
            {
                var arg = allArgs[i];
                if (arg.Equals("-username") && i < allArgs.Length -1)
                {
                    m_Username = allArgs[i + 1];
                }
                if (arg.Equals("-password") && i < allArgs.Length -1)
                {
                    m_Password = allArgs[i + 1];
                }
            }

            var serviceEnvironment = serviceHostResolver?.GetResolvedEnvironment();
            m_UnitySubdomain = serviceEnvironment switch
            {
                ServiceEnvironment.Staging => "api-staging",
                ServiceEnvironment.Test => "api-staging",
                _ => "api",
            };
        }

        async Task GetDeviceTokenFromCredential() {
            var loginCredentials = new LoginCredentials() { username = m_Username, password = m_Password, grant_type = "password" };
            var httpClient = new UnityHttpClient();
            var response = await httpClient.PostAsync($"https://{m_UnitySubdomain}.unity.com/v1/core/api/login", new StringContent(JsonSerialization.Serialize(loginCredentials), Encoding.UTF8, "application/json"));

            var loginTokenJson = await response.JsonDeserializeAsync<LoginTokenJson>();
            m_DeviceToken = new DeviceToken(loginTokenJson.refresh_token, loginTokenJson.access_token,
                loginTokenJson.expires_in);

            m_DeviceTokenRetrievalTime = DateTime.Now;
        }

        /// <inheritdoc/>
        public async Task<string> GetAccessTokenAsync()
        {
            // refresh if less than a minute before expiry
            await m_GetAccessTokenSemaphore.WaitAsync();
            var isExpired = DateTime.Now > m_DeviceTokenRetrievalTime + m_DeviceToken.AccessTokenExpiresIn - TimeSpan.FromSeconds(60);
            if (isExpired)
            {
                await GetDeviceTokenFromCredential();
            }
            m_GetAccessTokenSemaphore.Release();
            return m_DeviceToken.AccessToken;
        }

        /// <inheritdoc/>
        public string GetAccessToken()
        {
            return m_DeviceToken?.AccessToken;
        }

        /// <summary>
        /// Disposes of any `IDisposable` references.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of any `IDisposable` references.
        /// </summary>
        /// <param name="disposing">Dispose pattern boolean value received from public Dispose method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_GetAccessTokenSemaphore?.Dispose();
            }
        }
    }

    class LoginCredentials
    {
        public string username { get; set; }
        public string password { get; set; }
        public string grant_type { get; set; }
    }

    class LoginTokenJson
    {
        public string access_token;
        public string refresh_token;
        public int expires_in;
    }

}

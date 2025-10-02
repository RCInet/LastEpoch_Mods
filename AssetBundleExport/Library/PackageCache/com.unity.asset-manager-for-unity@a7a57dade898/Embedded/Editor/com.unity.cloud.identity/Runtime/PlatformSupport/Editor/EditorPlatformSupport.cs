using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Cloud.AppLinkingEmbedded;
using Unity.Cloud.AppLinkingEmbedded.Runtime;
using Unity.Cloud.CommonEmbedded;
using Unity.Cloud.CommonEmbedded.Runtime;
using UnityEngine;

namespace Unity.Cloud.IdentityEmbedded.Runtime
{
    /// <summary>
    /// This class contains Unity Editor platform-specific logic to handle app activation from an url or key value pairs.
    /// </summary>
    class EditorActivatePlatformSupport : BasePkcePlatformSupport
    {
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<EditorActivatePlatformSupport>();

        /// <inheritdoc/>
        public EditorActivatePlatformSupport(IUrlRedirectionInterceptor urlRedirectionInterceptor, IUrlProcessor urlProcessor, IAppIdProvider appIdProvider, IAppNamespaceProvider appNamespaceProvider, string cacheStorePath, string activationUrl = null)
            : base(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNamespaceProvider, cacheStorePath, activationUrl)
        {
            var activateAppFromUrl = UnityEngine.Object.FindObjectOfType(typeof(ActivateAppFromUrl)) as ActivateAppFromUrl;
            if (activateAppFromUrl != null && activateAppFromUrl.ActivateAtStartUp && Uri.TryCreate(activateAppFromUrl.ActivationUrl, UriKind.Absolute, out Uri _))
            {
                s_Logger.LogDebug($"User provided activation Url: {activateAppFromUrl.ActivationUrl}");
                ActivationUrl = activateAppFromUrl.ActivationUrl;
            }
            else
            {
                ActivationUrl = string.Empty;
            }
            ActivationKeyValue = new Dictionary<string, string>();
        }
    }

    /// <summary>
    /// This class handles Unity Editor platform-specific features in the authentication flow.
    /// </summary>
    class EditorPkcePlatformSupport : EditorActivatePlatformSupport
    {
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<EditorPkcePlatformSupport>();

        // Only allowed for development/playmode
        static readonly string k_LocalHostHttp = "http://";
        static readonly string k_LocalHostCancellationUri = "?&code=none&state=cancelled";

        string m_UniqueResponseRoute;
        readonly IUrlProcessor m_UrlProcessor;
        string m_LocalHostRedirectUri = "";

        string m_RedirectOperation = "";
        List<string> m_AwaitedArguments;

        /// <inheritdoc/>
        public EditorPkcePlatformSupport(IUrlRedirectionInterceptor urlRedirectionInterceptor, IUrlProcessor urlProcessor, IAppIdProvider appIdProvider, IAppNamespaceProvider appNamespaceProvider, string cacheStorePath, string activationUrl = null)
            : base(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNamespaceProvider, cacheStorePath, activationUrl)
        {
            m_UrlProcessor = urlProcessor;
            GenerateUniquePath();
        }

        void OpenUrlAction(string url)
        {
            if (m_UrlProcessor != null)
            {
                m_UrlProcessor.ProcessURL(url);
            }
        }

        /// <inheritdoc/>
        public override async Task<UrlRedirectResult> OpenUrlAndWaitForRedirectAsync(string url, List<string> awaitedQueryArguments = null)
        {
            m_AwaitedArguments = awaitedQueryArguments;
            var httpListener = new HttpListener();
            httpListener.Prefixes.Add($"{k_LocalHostHttp}{m_LocalHostRedirectUri}{m_UniqueResponseRoute}/");
            try
            {
                httpListener.Start();
                httpListener.BeginGetContext(OnHttpListenerRequest, httpListener);

                OpenUrlAction(url);

                var urlRedirectResult = await UrlRedirectionInterceptor.AwaitRedirectAsync(m_AwaitedArguments);
                return urlRedirectResult;
            }
            finally
            {
                httpListener.Close();
            }
        }

        /// <inheritdoc/>
        public override string GetRedirectUri(string operation = null)
        {
            m_RedirectOperation = operation;
            GenerateUniquePath();
            return $"{k_LocalHostHttp}{m_LocalHostRedirectUri}{m_UniqueResponseRoute}";
        }

        /// <inheritdoc/>
        public override Task<string> GetRedirectUriAsync(string operation = null)
        {
            return Task.FromResult(GetRedirectUri(operation));
        }

        void GenerateUniquePath()
        {
            // Generate unique path to increase obfuscation of response url
            var bytes = new byte[64];
            var randomNumber = RandomNumberGenerator.Create();
            randomNumber.GetBytes(bytes);
            m_LocalHostRedirectUri = $"localhost:{GetRandomUnusedPort()}";
            m_UniqueResponseRoute = $"/{UrlEncodeBase64String(Convert.ToBase64String(bytes))}";
        }

        static string UrlEncodeBase64String(string base64String)
        {
            return base64String.TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        /// <inheritdoc/>
        public override string GetCancellationUri()
        {
            return $"{k_LocalHostHttp}{m_LocalHostRedirectUri}{m_UniqueResponseRoute}{k_LocalHostCancellationUri}";
        }

        /// <inheritdoc/>
        public override void ProcessActivationUrl(List<string> awaitedQueryArguments = null)
        {
            if (!string.IsNullOrEmpty(ActivationUrl))
            {
                s_Logger.LogDebug("Activation Url not currently supported in PlayMode.");
                // Only process once
                ActivationUrl = string.Empty;
            }
        }

        void OnHttpListenerRequest(IAsyncResult result)
        {
            var httpListener = (HttpListener)result.AsyncState;
            var context = httpListener.EndGetContext(result);

            // Write HTML response
            var responseString = HttpListenerHtmlResponse.GetHtmlResponse(m_RedirectOperation);
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            context.Response.ContentLength64 = buffer.Length;
            var output = context.Response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();

            // Process Url
            UrlRedirectionInterceptor.InterceptAwaitedUrl(context.Request.Url.OriginalString, m_AwaitedArguments);
        }
    }
}

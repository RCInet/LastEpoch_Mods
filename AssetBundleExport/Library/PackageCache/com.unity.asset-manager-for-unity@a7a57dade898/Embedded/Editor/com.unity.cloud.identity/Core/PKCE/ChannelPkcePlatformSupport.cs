using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Cloud.AppLinkingEmbedded;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <inheritdoc cref="BasePkcePlatformSupport"/>
    class ChannelPkcePlatformSupport : BasePkcePlatformSupport
    {
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<ChannelPkcePlatformSupport>();

        readonly IChannelProvider m_ChannelProvider;

        const string k_LoginOperationString = "login";
        const string k_SignoutOperationString = "signout";

        bool m_Cancelled = false;
        string m_ChannelId;
        string m_RedirectOperation = string.Empty;

        /// <inheritdoc/>
        public ChannelPkcePlatformSupport(IHttpClient httpClient, IServiceHostResolver serviceHostResolver, IUrlRedirectionInterceptor urlRedirectionInterceptor, IUrlProcessor urlProcessor, IAppIdProvider appIdProvider, IAppNamespaceProvider appNamespaceProvider, string cacheStorePath, string activationUrl = null)
            : base(urlRedirectionInterceptor, urlProcessor, appIdProvider, appNamespaceProvider, cacheStorePath, activationUrl)
        {
            m_ChannelProvider = new ChannelProvider(httpClient, serviceHostResolver, appNamespaceProvider);
        }

        // Here we launch the browser and awaits for the login to complete
        /// <inheritdoc/>
        public override async Task<UrlRedirectResult> OpenUrlAndWaitForRedirectAsync(string url, List<string> awaitedQueryArguments = null)
        {
            m_LoginUrl = url;
            s_Logger.LogDebug($"Open and await redirect on url: {url}");

            m_UrlProcessor?.ProcessURL(url);
            await Task.Delay(50);
            if (m_RedirectOperation.Equals(k_SignoutOperationString))
            {
                return new UrlRedirectResult
                {
                    Status = UrlRedirectStatus.NotApplicable
                };
            }

            return await AwaitChannelResponse(awaitedQueryArguments);
        }

        async Task<UrlRedirectResult> AwaitChannelResponse(List<string> awaitedQueryArguments = null)
        {
            var hasResponse = false;
            ChannelInfo channelInfo = default;
            var tryGet = 120;
            var tryGetTotal = 0;

            while (!hasResponse && tryGetTotal < tryGet)
            {
                try
                {
                    channelInfo = await m_ChannelProvider.GetChannelAsync(m_ChannelId);
                    if (!string.IsNullOrEmpty(channelInfo.Response))
                    {
                        hasResponse = true;
                    }
                    else
                    {
                        hasResponse = m_Cancelled;
                    }
                }
                catch (Exception ex)
                {
                    s_Logger.LogDebug($"Error getting channel: {m_ChannelId}. {ex.Message}");
                }
                finally
                {
                    tryGetTotal++;
                    await Task.Delay(1000);
                }
            }

            if (!m_Cancelled && hasResponse)
            {
                UrlRedirectionInterceptor.InterceptAwaitedUrl(channelInfo.Response, awaitedQueryArguments);
            }
            else
            {
                s_Logger.LogInformation("Login awaiting timed out or was cancelled");
            }

            m_Cancelled = false;
            return UrlRedirectionInterceptor.GetRedirectionResult().GetValueOrDefault();
        }

        /// <inheritdoc/>
        public override UrlRedirectResult? GetRedirectionResult()
        {
            return UrlRedirectionInterceptor.GetRedirectionResult();
        }

        /// <inheritdoc/>
        public override async Task<string> GetRedirectUriAsync(string operation = null)
        {
            m_RedirectOperation = operation;
            // create channel first
            if (!string.IsNullOrEmpty(operation) && operation.Equals(k_LoginOperationString))
            {
                m_ChannelId = await m_ChannelProvider.CreateChannelAsync(ChannelServiceRequestDefinition.PKCE_LOGIN);
                s_Logger.LogDebug($"Created channel with Id: {m_ChannelId}");
            }
            else
            {
                m_ChannelId = "";
            }

            return $"https://implicit/callback/{operation}/channel/{m_ChannelId}";
        }

        /// <inheritdoc/>
        public override string GetCancellationUri()
        {
            if (string.IsNullOrEmpty(m_LoginUrl))
                throw new InvalidOperationException(
                    "No cancellation Uri available. Awaiting login operation to be initiated.");

            m_Cancelled = true;

            var loginHost = new Uri(m_LoginUrl).Host;
            return $"https://{loginHost}?code=none&state=cancelled";
        }
    }
}

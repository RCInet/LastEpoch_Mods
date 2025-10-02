using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AppLinkingEmbedded
{
    /// <summary>
    /// An <see cref="IUrlRedirectionInterceptor"/> using the service channel to intercept redirection response.
    /// </summary>
    class ChannelUrlRedirectionInterceptor : IUrlRedirectionInterceptor
    {
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<ChannelUrlRedirectionInterceptor>();

        readonly IUrlRedirectAwaiter m_Awaiter;
        readonly string m_HostDomain;
        bool m_DisposedValue;

        string m_ChannelId;

        /// <inheritdoc />
        public List<string> AwaitedQueryArguments { get; private set; }

        /// <summary>
        /// Creates an <see cref="IUrlRedirectionInterceptor"/> using the service channel to intercept redirection response.
        /// </summary>
        public ChannelUrlRedirectionInterceptor(IUrlRedirectAwaiter urlRedirectAwaiter, string hostDomain = null)
        {
            m_Awaiter = urlRedirectAwaiter;
            m_HostDomain = hostDomain;
        }

        /// <inheritdoc />
        public void InterceptAwaitedUrl(string url, List<string> awaitedQueryArguments = null)
        {
            try
            {
                UrlRedirectUtils.ValidateUrlArgument(url, out Uri uri);
                if (!UrlRedirectUtils.TryInterceptRedirectionUrl(uri, awaitedQueryArguments,
                        out UrlRedirectResult urlRedirectResult)) return;

                // Only WebGL is hosted and could have callback login query arguments in its url.
                if (!string.IsNullOrEmpty(m_HostDomain))
                {
                    s_Logger.LogDebug($"Hosted app on '{m_HostDomain}' received callback url redirection.");
                    m_Awaiter.SetResult(urlRedirectResult);
                }
                else
                {
                    m_Awaiter.SetResult(urlRedirectResult);
                }
            }
            catch (Exception ex)
            {
                s_Logger.LogWarning($"Intercept Awaited Url Exception: '{ex.Message}'");
            }
        }

        /// <inheritdoc />
        public event Action<Uri> DeepLinkForwarded;

        /// <inheritdoc />
        public ProcessId GetRedirectProcessId()
        {
            return ProcessId.None;
        }

        /// <inheritdoc />
        public UrlRedirectResult? GetRedirectionResult()
        {
            return m_Awaiter.RedirectResult;
        }

        /// <inheritdoc />
        public Task<UrlRedirectResult> AwaitRedirectAsync(List<string> awaitedQueryArguments = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs tasks related to disposing of associated resources.
        /// </summary>
        /// <param name="disposing">Whether we want to dispose of associated resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_DisposedValue)
            {
                if (disposing)
                {
                    // Do any memory cleanup
                }
                m_DisposedValue = true;
            }
        }

        /// <summary>
        /// Performs tasks related to disposing of associated resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

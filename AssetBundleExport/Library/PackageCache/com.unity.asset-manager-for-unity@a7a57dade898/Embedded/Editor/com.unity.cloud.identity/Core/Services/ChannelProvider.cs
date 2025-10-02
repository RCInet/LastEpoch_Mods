using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// Create request and Read response using the Unity Cloud Channel service.
    /// </summary>
    internal class ChannelProvider : IChannelProvider
    {
        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<ChannelProvider>();
        readonly IHttpClient m_HttpClient;
        readonly IServiceHostResolver m_ServiceHostResolver;
        readonly IAppNamespaceProvider m_AppNamespaceProvider;

        const string k_UnauthorizedResponse = "unauthorized";

        /// <summary>
        /// Creates an IChannelProvider.
        /// </summary>
        /// <param name="httpClient">An IHttpClient instance.</param>
        /// <param name="serviceHostResolver">The service host resolver for the service Url.</param>
        /// <param name="appNamespaceProvider">The application display name provider for the registered application.</param>
        public ChannelProvider(IHttpClient httpClient, IServiceHostResolver serviceHostResolver,
            IAppNamespaceProvider appNamespaceProvider)
        {
            m_HttpClient = httpClient;
            m_ServiceHostResolver = serviceHostResolver;
            m_AppNamespaceProvider = appNamespaceProvider;
        }

        /// <inheritdoc/>
        public async Task<string> CreateChannelAsync(ChannelServiceRequest channelServiceRequest,
            string requestDetails = default)
        {
            var request = channelServiceRequest.Action;
            if (!string.IsNullOrEmpty(requestDetails))
            {
                // use base64 encoding for safe transport in urls and Json formatted string
                var plainTextBytes = Encoding.UTF8.GetBytes(requestDetails);
                requestDetails = Convert.ToBase64String(plainTextBytes);
            }

            var appDisplayName = m_AppNamespaceProvider.GetAppNamespace();
            var content = new ChannelRequest
            {
                AppName = appDisplayName, Request = request,
                RequestData = new RequestData { Content = requestDetails }
            };
            return await CreateChannelAsync(content);
        }

        /// <inheritdoc/>
        public async Task<string> CreateChannelAsync(ChannelServiceRequest channelServiceRequest, object requestDetails)
        {
            var request = channelServiceRequest.Action;

            var appDisplayName = m_AppNamespaceProvider.GetAppNamespace();
            var content = new ChannelRequest
            {
                AppName = appDisplayName, Request = request,
                RequestData = new RequestData { Content = JsonSerialization.Serialize(requestDetails) }
            };

            return await CreateChannelAsync(content);
        }

        async Task<string> CreateChannelAsync(ChannelRequest request)
        {
            var contentStr = JsonSerialization.Serialize(request);
            var contentBody = new StringContent(contentStr, Encoding.UTF8, "application/json");

            var requestUri = m_ServiceHostResolver.GetResolvedRequestUri($"/app-linking/v1/channels");

            try
            {
                var response = await m_HttpClient.PostAsync(requestUri, contentBody);
                response.EnsureSuccessStatusCode();
                var channelIdResponse = await response.JsonDeserializeAsync<ChannelIdResponseJson>();
                return channelIdResponse.channelId;
            }
            catch (Exception ex)
            {
                s_Logger.LogError($"Error creating channel: {requestUri}, {ex}");

                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ChannelInfo> GetChannelAsync(string channelId)
        {
            var requestUri = m_ServiceHostResolver.GetResolvedRequestUri($"/app-linking/v1/channels/{channelId}");

            var response = await m_HttpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            var channel = await response.JsonDeserializeAsync<ChannelJson>();
            if (channel.channel.Response.Equals(k_UnauthorizedResponse))
            {
                throw new UnauthorizedException("Channel response was set to unauthorized");
            }

            return channel.channel;
        }
    }
}

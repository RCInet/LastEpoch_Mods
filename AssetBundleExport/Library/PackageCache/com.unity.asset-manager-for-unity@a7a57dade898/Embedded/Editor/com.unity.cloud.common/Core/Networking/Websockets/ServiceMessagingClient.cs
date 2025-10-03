using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// A derived class of <see cref="HttpHeaders"/> specifically for web socket headers.
    /// </summary>
    class WebSocketHeaders : HttpHeaders { }

    /// <inheritdoc cref="IServiceMessagingClient" />
    class ServiceMessagingClient : IServiceMessagingClient, IDisposable
    {
        readonly IWebSocketClient m_WebSocketClient;
        readonly IServiceAuthorizer m_ServiceAuthorizer;
        readonly IAppIdProvider m_AppIdProvider;
        readonly HashSet<string> m_SourceHeaders = new ();
        long m_CheckpointEpochMilliseconds;

        /// <inheritdoc />
        public ConnectionState State => m_WebSocketClient.State;

        /// <inheritdoc />
        public event Action<Exception> ConnectionErrorOccured;
        /// <inheritdoc />
        public event Action<Message> MessageReceived;
        /// <inheritdoc />
        public event Action<ConnectionState> ConnectionStateChanged;

        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<ServiceMessagingClient>();

        /// <summary>
        /// Creates an instance of <see cref="ServiceMessagingClient"/>
        /// </summary>
        /// <param name="webSocketClient">IWebSocketClient provided. </param>
        /// <param name="serviceAuthorizer">Authorizer to add authorization information to requests. </param>
        /// <param name="appIdProvider">Provider of the application identifier. </param>
        public ServiceMessagingClient(IWebSocketClient webSocketClient, IServiceAuthorizer serviceAuthorizer = null, IAppIdProvider appIdProvider = null)
        {
            m_WebSocketClient = webSocketClient;
            m_ServiceAuthorizer = serviceAuthorizer;
            m_AppIdProvider = appIdProvider;

            m_WebSocketClient.ConnectionErrorOccured += OnWebSocketClientError;
            m_WebSocketClient.MessageReceived += OnWebSocketClientReceived;
            m_WebSocketClient.ConnectionStateChanged += OnWebSocketStateChanged;
        }

        void OnWebSocketClientError(Exception err) => ConnectionErrorOccured?.Invoke(err);
        void OnWebSocketStateChanged(ConnectionState obj) => ConnectionStateChanged?.Invoke(obj);

        /// <inheritdoc />
        public async Task ConnectAsync(Uri uri, ServiceMessagingClientOptions options = default, CancellationToken cancellationToken = default)
        {
            var headers = new WebSocketHeaders();
            ApplyDefaultHeaders(headers, m_SourceHeaders);

            if (m_ServiceAuthorizer != null)
                await m_ServiceAuthorizer.AddAuthorization(headers);

            headers.AddAppIdAndClientTrace(m_AppIdProvider?.GetAppId() ?? AppId.None, ServiceHttpClient.ClientTrace);

#if UNITY_WEBGL
            uri = uri.AddHeadersAsQuery(headers);
#endif

            await options.RetryPolicy.ExecuteAsyncWithExceptionValidation(ct => m_WebSocketClient.ConnectAsync(uri, headers),
                exception => true, cancellationToken);
        }

        /// <inheritdoc />
        public Task DisconnectAsync() => m_WebSocketClient.DisconnectAsync();

        /// <inheritdoc />
        public Task SendAsync(Message message)
        {
           return SendAsync(new[] { message });
        }

        /// <inheritdoc />
        public Task SendAsync(IEnumerable<Message> messages)
        {
            var messageCollection = new MessageCollection(messages, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            var json = JsonSerialization.Serialize(messageCollection);
            return m_WebSocketClient.SendAsync(json);
        }

        /// <inheritdoc />
        public void AddApiSourceVersion(ApiSourceVersion apiSourceVersion)
        {
            if (apiSourceVersion == null)
                throw new ArgumentNullException(nameof(apiSourceVersion));

            if (string.IsNullOrWhiteSpace(apiSourceVersion.Name))
                throw new ArgumentNullException($"{apiSourceVersion.Name}");

            if (string.IsNullOrWhiteSpace(apiSourceVersion.Version))
                throw new ArgumentNullException($"{apiSourceVersion.Version}");

            m_SourceHeaders.Add(apiSourceVersion.GetHeaderValue());
        }

        /// <inheritdoc />
        public void AddApiSourceVersion(string name, string version)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentNullException(nameof(version));

            AddApiSourceVersion(new ApiSourceVersion(name, version));
        }

        void UpdateCheckpointTimestamp(long checkpointEpochMilliseconds) =>
            m_CheckpointEpochMilliseconds = checkpointEpochMilliseconds;

        void OnWebSocketClientReceived(string jsonMessages)
        {
            var messageCollection = JsonSerialization.Deserialize<MessageCollection>(jsonMessages);

            if (messageCollection?.Messages == null || !messageCollection.Messages.Any())
            {
                s_Logger?.LogDebug("Received empty message collection.");
                return;
            }

            var messages = messageCollection.Messages.Where(message => message.IsValid());
            s_Logger?.LogDebug($"Received valid messages count: {messages.Count()}.");

            UpdateCheckpointTimestamp(messageCollection.CheckpointEpochMilliseconds);
            s_Logger?.LogDebug($"Updated checkpoint timestamp from server - " +
                $"{m_CheckpointEpochMilliseconds}.");

            foreach (var message in messages)
            {
                try
                {
                    MessageReceived?.Invoke(message);
                }
                catch (Exception err)
                {
                    s_Logger.LogError(err);
                }
            }
        }

        /// <summary>
        /// Ensure disposal of any IDisposable references.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Ensure internal disposal of any IDisposable references.
        /// </summary>
        /// <param name="disposing">Dispose pattern boolean value received from public Dispose method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_WebSocketClient != null)
                {
                    m_WebSocketClient.ConnectionErrorOccured -= OnWebSocketClientError;
                    m_WebSocketClient.MessageReceived -= OnWebSocketClientReceived;
                    m_WebSocketClient.ConnectionStateChanged -= OnWebSocketStateChanged;
                }
                m_WebSocketClient?.Dispose();
            }
        }

        static void ApplyDefaultHeaders(WebSocketHeaders headers, IEnumerable<string> headersToApply)
        {
            foreach (var headerValue in headersToApply)
            {
                headers.Add(ServiceHeaderUtils.k_ApiSourceHeader, headerValue);
            }
        }
    }
}

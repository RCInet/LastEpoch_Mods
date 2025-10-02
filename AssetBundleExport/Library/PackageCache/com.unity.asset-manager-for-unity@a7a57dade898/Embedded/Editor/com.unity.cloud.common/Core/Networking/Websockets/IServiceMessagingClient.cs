using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// A class to configure options for a <see cref="IServiceMessagingClient"/>
    /// </summary>
    struct ServiceMessagingClientOptions
    {
        IRetryPolicy m_RetryPolicy;

        /// <summary>
        /// The <see cref="IRetryPolicy"/> to be used by the client.
        /// </summary>
        public IRetryPolicy RetryPolicy => m_RetryPolicy ??= new ExponentialBackoffRetryPolicy();

        /// <summary>
        /// Initializes and returns an instance of <see cref="ServiceMessagingClientOptions"/>.
        /// </summary>
        /// <param name="retryPolicy">The retry policy to use for the client.</param>
        public ServiceMessagingClientOptions(IRetryPolicy retryPolicy)
        {
            m_RetryPolicy = retryPolicy;
        }

        /// <summary>
        /// Initializes and returns an instance of <see cref="ServiceMessagingClientOptions"/> with default settings.
        /// </summary>
        /// <returns>The specific client options.</returns>
        public static ServiceMessagingClientOptions Default() =>
            new ServiceMessagingClientOptions(new ExponentialBackoffRetryPolicy());

        /// <summary>
        /// Initializes and returns an instance of <see cref="ServiceMessagingClientOptions"/> with no retry policy.
        /// </summary>
        /// <returns>The specific client options.</returns>
        public static ServiceMessagingClientOptions NoRetryOption() =>
            new ServiceMessagingClientOptions(new NoRetryPolicy());
    }

    /// <summary>
    /// Interface for a messaging client used to connect to a service and send/receive messages
    /// </summary>
    interface IServiceMessagingClient
    {
        /// <summary>
        /// Event invoked when a connection error occurs
        /// </summary>
        event Action<Exception> ConnectionErrorOccured;

        /// <summary>
        /// Event invoked when a message is received from the server
        /// </summary>
        event Action<Message> MessageReceived;

        /// <summary>
        /// Event invoked when the client's connection state changes
        /// </summary>
        event Action<ConnectionState> ConnectionStateChanged;

        /// <summary>
        /// Awaitable method to open a connection to a service using the given url using messaging client options
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <param name="options">The selected options</param>
        /// <param name="cancellationToken">Optional cancellation token that will try to cancel the operation.</param>
        /// <returns>A <see cref="Task"/> for the connection operation.</returns>
        Task ConnectAsync(Uri uri, ServiceMessagingClientOptions options = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Awaitable method to close a connection to a server
        /// </summary>
        /// <returns>A <see cref="Task"/> for the disconnection operation.</returns>
        Task DisconnectAsync();

        /// <summary>
        /// Awaitable method to send a message to a server
        /// </summary>
        /// <param name="message"></param>
        /// <returns>A <see cref="Task"/> for the send operation.</returns>
        Task SendAsync(Message message);

        /// <summary>
        /// Awaitable method to send a collection of messages to a server
        /// </summary>
        /// <param name="messages"></param>
        /// <returns>A <see cref="Task"/> for the send operation.</returns>
        Task SendAsync(IEnumerable<Message> messages);

        /// <summary>
        /// Adds an <see cref="ApiSourceVersion"/> to the client.
        /// </summary>
        /// <param name="apiSourceVersion">The version information for the API using this client.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="apiSourceVersion"/> is null or any of its fields are null or whitespace.</exception>
        void AddApiSourceVersion(ApiSourceVersion apiSourceVersion);

        /// <summary>
        /// Adds an <see cref="ApiSourceVersion"/> to the client.
        /// </summary>
        /// <param name="name">The API source name.</param>
        /// <param name="version">The API source version.</param>
        /// <exception cref="ArgumentNullException">Thrown is <paramref name="name"/> or <paramref name="version"/> are null or whitespace.</exception>
        void AddApiSourceVersion(string name, string version);

        /// <summary>
        /// Reference to the current state of the connection
        /// </summary>
        ConnectionState State { get; }
    }

    /// <summary>
    /// Enum to track different client states
    /// </summary>
    enum ConnectionState
    {
        /// <summary>
        /// The client is connecting.
        /// </summary>
        Connecting = 0,

        /// <summary>
        /// The client is connected.
        /// </summary>
        Connected = 1,

        /// <summary>
        /// The client is disconnecting.
        /// </summary>
        Disconnecting = 2,

        /// <summary>
        /// The client is disconnected.
        /// </summary>
        Disconnected = 3
    }
}

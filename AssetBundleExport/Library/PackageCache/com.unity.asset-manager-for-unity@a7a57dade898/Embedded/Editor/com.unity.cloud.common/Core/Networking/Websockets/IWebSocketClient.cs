using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// An interface for using a Messaging client and listen to events.
    /// </summary>
    interface IWebSocketClient : IDisposable
    {
        /// <summary>
        /// Event invoked when a connection error occurs
        /// </summary>
        event Action<Exception> ConnectionErrorOccured;

        /// <summary>
        /// Event invoked when a message is received from the server
        /// </summary>
        event Action<string> MessageReceived;

        /// <summary>
        /// Event invoked when binary data is received from the server
        /// </summary>
        event Action<ArraySegment<byte>> DataReceived;

        /// <summary>
        /// Event invoked when the state changes
        /// </summary>
        event Action<ConnectionState> ConnectionStateChanged;

        /// <summary>
        /// Attempts opening a connection to a server
        /// </summary>
        /// <param name="uri">The uri to connect to.</param>
        /// <param name="headers">The HTTP headers for the connection request.</param>
        /// <returns>The connection task.</returns>
        Task ConnectAsync(Uri uri, HttpHeaders headers = null);

        /// <summary>
        /// Closes a connection to a server
        /// </summary>
        /// <returns>The disconnection task.</returns>
        Task DisconnectAsync();

        /// <summary>
        /// Sends a message to a server.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>The send message task.</returns>
        Task SendAsync(string message);

        /// <summary>
        /// Method to send binary data to a server
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>The send message task.</returns>
        Task SendAsync(ArraySegment<byte> message);

        /// <summary>
        /// Reference to the current state of the connection
        /// </summary>
        ConnectionState State { get; }
    }
}

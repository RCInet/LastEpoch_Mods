using System;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// Enum of all supported protocols to connect to a Server.
    /// </summary>
    enum ServiceProtocol
    {
        /// <summary>
        /// HTTP protocol.
        /// </summary>
        Http,

        /// <summary>
        /// Websocket protocol.
        /// </summary>
        WebSocket,
        /// <summary>
        /// Websocket secure protocol.
        /// </summary>
        WebSocketSecure
    }
}

using System;

namespace Unity.Cloud.CommonEmbedded.Runtime
{
    /// <summary>
    /// Create WebSocket Client.
    /// </summary>
    interface IWebSocketClientFactory
    {
        /// <summary>
        /// Create an instance of <see cref="IWebSocketClient"/>.
        /// </summary>
        IWebSocketClient Create();
    }

    /// <inheritdoc/>
    class WebSocketClientFactory : IWebSocketClientFactory
    {
        /// <inheritdoc/>
        public IWebSocketClient Create()
        {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
            return new NativeWebSocketClient();
#elif UNITY_WEBGL
            return new WebglWebSocketClient();
#else
            throw new NotImplementedException("No WebSocket Client implemented for the current platform");
#endif
        }
    }
}

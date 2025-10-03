#if (!UNITY_WEBGL || UNITY_EDITOR)
using System.Net.WebSockets;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Cloud.CommonEmbedded.Runtime
{
    /// <summary>
    /// IWebSocketClient implementation for Unity editor and desktop standalone builds
    /// </summary>
    sealed class NativeWebSocketClient : IWebSocketClient
    {
        public event Action<Exception> ConnectionErrorOccured;
        public event Action<string> MessageReceived;
        public event Action<ArraySegment<byte>> DataReceived;
        public event Action<ConnectionState> ConnectionStateChanged;

        ClientWebSocket m_Client;

        CancellationTokenSource m_CancellationTokenSource;
        CancellationToken m_CancellationToken;

        readonly MainThreadInvoker m_Invoker;

        long m_CheckpointEpochMilliseconds = 0;

        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<NativeWebSocketClient>();

        ConnectionState m_State = ConnectionState.Disconnected;
        public ConnectionState State
        {
            get => m_State;
            private set
            {
                m_State = value;
                ConnectionStateChanged?.Invoke(m_State);
            }
        }

        public NativeWebSocketClient()
        {
            m_Invoker = new MainThreadInvoker(m_CancellationToken);
            State = ConnectionState.Disconnected;
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
        void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_Client?.Dispose();
            }
        }

        void CreateClient(HttpHeaders headers)
        {
            m_Client?.Dispose();
            m_Client = new ClientWebSocket();
            m_Client.Options.KeepAliveInterval = TimeSpan.FromSeconds(1);
            if (headers != null)
            {
                foreach (var (name, values) in headers)
                {
                    var valueString = values.Aggregate((v1, v2) => $"{v1}, {v2}");
                    m_Client.Options.SetRequestHeader(name, valueString);
                    s_Logger.LogDebug($"Adding header {name} - {valueString}");
                }
            }
        }

        // Connects to WebSocket. Non-blocking. Will start read-loop when open.
        public async Task ConnectAsync(Uri uri, HttpHeaders headers = null)
        {
            if (State == ConnectionState.Connected)
                throw new InvalidOperationException("Already connected");
            if (State == ConnectionState.Connecting)
                throw new InvalidOperationException("Already attempting connection");
            if (State == ConnectionState.Disconnecting)
                throw new InvalidOperationException("Currently attempting disconnection");

            if (m_CancellationTokenSource != null && m_CancellationToken.CanBeCanceled)
            {
                m_CancellationTokenSource.Cancel();
            }
            m_CancellationTokenSource = new CancellationTokenSource();
            m_CancellationToken = m_CancellationTokenSource.Token;

            State = ConnectionState.Connecting;
            try
            {
                await ConnectInternal(uri, headers);
            }
            catch (TaskCanceledException)
            {
                State = ConnectionState.Disconnected;
                return;
            }
            catch (Exception e)
            {
                HandleClientDisconnection(e);
                throw;
            }

            State = ConnectionState.Connected;

            // Start running parallel task for reading messages
            _ = Task.Run(RunReadLoopAsync, m_CancellationToken);
        }

        async Task ConnectInternal(Uri uri, HttpHeaders headers)
        {
            CreateClient(headers);
            uri = Utilities.GetUrlWithCheckpoint(uri, m_CheckpointEpochMilliseconds);
            s_Logger.LogDebug($"Connecting to URL => {uri}");

            if (headers != null)
            {
                foreach (var (name, values) in headers)
                {
                    s_Logger.LogDebug($"Header: {name} - {values.Aggregate((v1, v2) => $"{v1}, {v2}")}");
                }
            }
			try
            {
                await m_Client.ConnectAsync(uri, m_CancellationToken);
            }
            catch (Exception e)
            {
                s_Logger.LogError(new AggregateException("Websocket Connect Error", e));
                throw;
            }
            s_Logger.LogDebug("Opened connection to server.");
        }

        async Task RunReadLoopAsync()
        {
            while (!m_CancellationToken.IsCancellationRequested)
            {
                if (State != ConnectionState.Connected)
                    return;

                try
                {
                    using var ms = new MemoryStream();
                    WebSocketReceiveResult result;
                    do
                    {
                        var messageBuffer = WebSocket.CreateClientBuffer(1024, 16);
                        result = await m_Client.ReceiveAsync(messageBuffer, CancellationToken.None);
                        ms.Write(messageBuffer.Array, messageBuffer.Offset, result.Count);
                    }
                    while (!result.EndOfMessage);

                    switch (result.MessageType)
                    {
                        case WebSocketMessageType.Binary:
                            m_Invoker.InvokeMainThreadEventWithArg(OnDataReceived, ms.ToArray());
                            break;
                        case WebSocketMessageType.Close:
                            HandleClientDisconnection("Close frame received from server");
                            break;
                        case WebSocketMessageType.Text:
                            m_Invoker.InvokeMainThreadEventWithArg(OnMessageReceived, Encoding.UTF8.GetString(ms.ToArray()));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Position = 0;
                }
                catch (InvalidOperationException err)
                {
                    HandleClientDisconnection(err);
                    ReportError(err);
                }
            }
        }

        public async Task DisconnectAsync()
        {
            if (State == ConnectionState.Disconnected)
                throw new InvalidOperationException("Already disconnected");
            if (State == ConnectionState.Disconnecting)
                throw new InvalidOperationException("Already attempting disconnection");
            if (State == ConnectionState.Connecting)
                throw new InvalidOperationException("Currently attempting connection");

            try
            {
                State = ConnectionState.Disconnecting;
                if (m_Client.State == WebSocketState.Open || m_Client.State == WebSocketState.CloseReceived)
                {
                    await m_Client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
                m_CancellationTokenSource.Cancel();
            }
            finally
            {
                HandleClientDisconnection(string.Empty);
            }
        }

        public async Task SendAsync(string msg)=> await SendAsync(Encoding.UTF8.GetBytes(msg), WebSocketMessageType.Binary);
        public async Task SendAsync(ArraySegment<byte> data) => await SendAsync(data, WebSocketMessageType.Binary);
        async Task SendAsync(ArraySegment<byte> data, WebSocketMessageType type)
        {
            if (m_Client.State != WebSocketState.Open || State != ConnectionState.Connected)
            {
                throw new InvalidOperationException("Connection closed");
            }

            try
            {
                await m_Client.SendAsync(data, type, true, m_CancellationToken);
            }
            catch (Exception e)
            {
                HandleClientDisconnection(e);
                throw;
            }
        }

        async Task<string> ReadMessages()
        {
            var msgString = "";
            try
            {
                using var ms = new MemoryStream();
                WebSocketReceiveResult result;
                do
                {
                    var messageBuffer = WebSocket.CreateClientBuffer(4096, 16);
                    result = await m_Client.ReceiveAsync(messageBuffer, CancellationToken.None);
                    ms.Write(messageBuffer.Array, messageBuffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    msgString = Encoding.UTF8.GetString(ms.ToArray());
                }
                ms.Seek(0, SeekOrigin.Begin);
                ms.Position = 0;
            }
            catch (InvalidOperationException err)
            {
                HandleClientDisconnection(err);
                ReportError(err);
            }

            return msgString;
        }

        void ReportError(Exception err)
        {
            s_Logger.LogError(err.Message);
            m_Invoker.InvokeMainThreadEventWithArg(OnErrorReceived, err);
        }

        void OnDataReceived(byte[] data)
        {
            DataReceived?.Invoke(data);
        }

        void OnMessageReceived(string jsonMessages)
        {
            MessageReceived?.Invoke(jsonMessages);
        }

        void OnErrorReceived(Exception err)
        {
            s_Logger.LogError("Connection Error.");
            ConnectionErrorOccured?.Invoke(err);
        }

        void HandleClientDisconnection(Exception e)
        {
            State = ConnectionState.Disconnected;
            s_Logger.LogDebug("Closed connection to server.");
            if(e != null)
                s_Logger.LogError(e);
        }
        void HandleClientDisconnection(string msg)
        {
            State = ConnectionState.Disconnected;
            s_Logger.LogDebug("Closed connection to server. " + msg);
        }

        void SendStateChangedEvent(ConnectionState state)
        {
            ConnectionStateChanged?.Invoke(state);
        }
    }
}
#endif

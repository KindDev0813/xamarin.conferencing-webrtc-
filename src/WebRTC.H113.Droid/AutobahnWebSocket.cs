using System;
using System.Collections.Generic;
using IO.Crossbar.Autobahn.Websocket;
using IO.Crossbar.Autobahn.Websocket.Interfaces;
using IO.Crossbar.Autobahn.Websocket.Types;
using WebRTC.H113.Signaling.WebSocket;

namespace WebRTC.H113.Droid
{
    internal class AutobahnWebSocket : IWebSocketConnection
    {
        public const int CloseCannotConnect = 2;
        public const int CloseConnectionLost = 3;
        public const int CloseInternalError = 5;
        public const int CloseNormal = 1;
        public const int CloseProtocolError = 4;
        public const int CloseReconnect = 7;
        public const int CloseServerError = 6;

        private readonly WebSocketConnection _webSocket;

        public AutobahnWebSocket()
        {
            _webSocket = new WebSocketConnection();
        }

        public event EventHandler OnOpened;
        public event EventHandler<(int code, string reason)> OnClosed;
        public event EventHandler<Exception> OnError;
        public event EventHandler<string> OnMessage;

        public void Dispose()
        {
            _webSocket.Dispose();
        }

        public bool IsConnected => _webSocket.IsConnected;

        public void Open(string url, string protocol = null, string authToken = null)
        {
            try
            {
                var options = new WebSocketOptions();
                options.SetTLSEnabledProtocols(new[] {"TLSv1.1", "TLSv1.2"});
                options.ReconnectInterval = 0;
                _webSocket.Connect(url, null, new WebSocketObserver(this), options, new Dictionary<string, string>
                {
                    ["Sec-WebSocket-Protocol"] = protocol
                });
            }
            catch (Exception ex)
            {
                SendOnError(ex);
            }
        }

        public void Close()
        {
            _webSocket.SendClose();
        }

        public void Send(string message)
        {
            _webSocket.SendMessage(message);
        }

        private void SendOnOpened()
        {
            OnOpened?.Invoke(this, EventArgs.Empty);
        }

        private void SendOnClose(int code, string reason)
        {
            OnClosed?.Invoke(this, (code, reason));
        }

        private void SendOnError(Exception error)
        {
            OnError?.Invoke(this, error);
        }

        private void SendOnMessage(string message)
        {
            OnMessage?.Invoke(this, message);
        }

        private void SendPong()
        {
            _webSocket.SendPong();
        }

        private void SendPing()
        {
            _webSocket.SendPing();
        }

        private class WebSocketObserver : Java.Lang.Object, IWebSocketConnectionHandler
        {
            private readonly AutobahnWebSocket _webSocketConnectionEx;

            public WebSocketObserver(AutobahnWebSocket webSocketConnectionEx)
            {
                _webSocketConnectionEx = webSocketConnectionEx;
            }

            public void OnClose(int code, string reason)
            {
                _webSocketConnectionEx.SendOnClose(code, reason);
            }

            public void OnConnect(ConnectionResponse p0)
            {
            }

            public void OnMessage(byte[] p0, bool p1)
            {
            }

            public void OnMessage(string p0)
            {
                _webSocketConnectionEx.SendOnMessage(p0);
            }

            public void OnOpen()
            {
                _webSocketConnectionEx.SendOnOpened();
            }

            public void OnPing()
            {
                _webSocketConnectionEx.SendPong();
            }

            public void OnPing(byte[] p0)
            {
            }

            public void OnPong()
            {
                _webSocketConnectionEx.SendPing();
            }

            public void OnPong(byte[] p0)
            {
            }

            public void SetConnection(WebSocketConnection p0)
            {
            }
        }
    }
}
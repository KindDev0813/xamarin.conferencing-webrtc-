using System;
using System.Collections.Generic;
using IO.Crossbar.Autobahn.Websocket;
using IO.Crossbar.Autobahn.Websocket.Interfaces;
using IO.Crossbar.Autobahn.Websocket.Types;
using WebRTC.AppRTC.Abstraction;
using WebSocketConnectionHandler = IO.Crossbar.Autobahn.Websocket.WebSocketConnectionHandler;

namespace WebRTC.H113.Droid
{
    internal class AutobahnWebSocket : IWebSocketConnection
    {
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

        public void Open(string url, string protocol = null, string authToken = null)
        {
            try
            {
                var options = new WebSocketOptions();
                options.SetTLSEnabledProtocols(new[] {"TLSv1.1", "TLSv1.2"});
                _webSocket.Connect(url,null, new WebSocketObserver(this), options,new Dictionary<string, string>
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

            public void OnClose(int p0, string p1)
            {
                if (p0 == WebSocketConnectionHandler.InterfaceConsts.CloseServerError)
                {
                    _webSocketConnectionEx.SendOnError(new Exception(p1));
                }
                else
                {
                    _webSocketConnectionEx.SendOnClose(p0, p1);
                }
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
using System;
using CoreFoundation;
using Foundation;
using Square.SocketRocket;
using WebRTC.AppRTC;

namespace WebRTC.iOS.Demo
{
    public class WebSocketConnection : IWebSocketConnection
    {
        private WebSocket _webSocket;


        public event EventHandler OnOpened;
        public event EventHandler OnClosed;
        public event EventHandler<Exception> OnError;
        public event EventHandler<string> OnMessage;
        public bool IsOpen { get; private set; }

        public void Dispose()
        {
            if (_webSocket != null)
                UnWire(_webSocket);
            Close();
            _webSocket?.Dispose();
            _webSocket = null;
        }

        public void Open(string url, string protocol = null, string authToken = null)
        {
            if (_webSocket != null)
            {
                UnWire(_webSocket);
            }

            Close();

            var nsUrl = NSUrl.FromString(url);
            _webSocket = !string.IsNullOrEmpty(protocol)
                ? new WebSocket(nsUrl, new NSObject[] {new NSString(protocol)})
                : new WebSocket(nsUrl);
            Wire(_webSocket);
            _webSocket.Open();
        }

        public void Close()
        {
            if (!IsOpen)
                return;
            IsOpen = false;
            _webSocket?.Close();
        }

        public void Send(string message)
        {
            _webSocket.Send(new NSString(message, NSStringEncoding.UTF8));
        }

        private void Wire(WebSocket socket)
        {
            socket.WebSocketOpened += WebSocketDidOpen;
            socket.WebSocketFailed += WebSocketDidFailWithError;
            socket.WebSocketClosed += WebSocketDidClose;
            socket.ReceivedMessage += WebSocketDidReceiveMessage;
        }

        private void UnWire(WebSocket socket)
        {
            socket.WebSocketOpened -= WebSocketDidOpen;
            socket.WebSocketFailed -= WebSocketDidFailWithError;
            socket.WebSocketClosed -= WebSocketDidClose;
            socket.ReceivedMessage -= WebSocketDidReceiveMessage;
        }

        private void WebSocketDidOpen(object sender, EventArgs e)
        {
            IsOpen = true;
            DispatchQueue.MainQueue.DispatchAsync(() => OnOpened?.Invoke(this, EventArgs.Empty));
        }

        private void WebSocketDidReceiveMessage(object sender, WebSocketReceivedMessageEventArgs e)
        {
            DispatchQueue.MainQueue.DispatchAsync(() => OnMessage?.Invoke(this, e.Message?.ToString()));
        }

        private void WebSocketDidFailWithError(object sender, WebSocketFailedEventArgs e)
        {
            IsOpen = false;
            DispatchQueue.MainQueue.DispatchAsync(() =>
                OnError?.Invoke(this, new Exception(e.Error.LocalizedDescription)));
        }

        private void WebSocketDidClose(object sender, WebSocketClosedEventArgs e)
        {
            IsOpen = false;
            DispatchQueue.MainQueue.DispatchAsync(() => OnClosed?.Invoke(this, EventArgs.Empty));
        }
    }
}
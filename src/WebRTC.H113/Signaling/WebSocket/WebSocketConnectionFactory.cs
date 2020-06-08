using System;

namespace WebRTC.H113.Signaling.WebSocket
{
    public static class WebSocketConnectionFactory
    {
        public static Func<IWebSocketConnection> Factory { get; set; }
        public static IWebSocketConnection CreateWebSocketConnection() => Factory();
    }
}
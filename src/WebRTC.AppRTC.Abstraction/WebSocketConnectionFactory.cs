using System;

namespace WebRTC.AppRTC.Abstraction
{
    public static class WebSocketConnectionFactory
    {
        public static Func<IWebSocketConnection> Factory { get; set; }
        public static IWebSocketConnection CreateWebSocketConnection() => Factory();
    }
}
using System;

namespace WebRTC.H113.Signaling.WebSocket
{
    /// <summary>
    /// </summary>
    public interface IWebSocketConnection : IDisposable
    {
        event EventHandler OnOpened;
        event EventHandler<(int code, string reason)> OnClosed;
        event EventHandler<Exception> OnError;
        event EventHandler<string> OnMessage;
        
        bool IsConnected { get; }
        void Open(string url, string protocol = null, string authToken = null);
        void Close();
        
        void Send(string message);
    }
}
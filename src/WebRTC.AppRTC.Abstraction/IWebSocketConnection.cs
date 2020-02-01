using System;

namespace WebRTC.AppRTC.Abstraction
{
    /// <summary>
    /// </summary>
    public interface IWebSocketConnection : IDisposable
    {
        event EventHandler OnOpened;
        event EventHandler<(int code, string reason)> OnClosed;
        event EventHandler<Exception> OnError;
        event EventHandler<string> OnMessage;

        void Open(string url, string protocol = null, string authToken = null);
        void Close();
        void Send(string message);
    }
}
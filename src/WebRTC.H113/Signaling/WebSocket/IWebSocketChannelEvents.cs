namespace WebRTC.H113.Signaling.WebSocket
{
    public interface IWebSocketChannelEvents
    {
        void OnWebSocketOpen();
        void OnWebSocketReconnected();
        void OnWebSocketClose(int code, string reason);
        void OnWebSocketMessage(string message);
        void OnWebSocketError(string description);
    }
}
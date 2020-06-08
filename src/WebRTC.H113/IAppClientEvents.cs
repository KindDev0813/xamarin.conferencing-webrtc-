using WebRTC.Abstraction;

namespace WebRTC.H113
{
    public interface IAppClientEvents
    {
        void OnPeerFactoryCreated(IPeerConnectionFactory factory);

        /// <summary>
        /// Will be called when WebRTC connection it's establish
        /// </summary>
        void OnConnected();

        void OnDisconnect(DisconnectType disconnectType);

        void ShowNotification(int type, string title, string message);

        IVideoCapturer CreateVideoCapturer(IPeerConnectionFactory factory, IVideoSource videoSource);

        void ReadyToStart();

        void OnError(string description);
    }
}
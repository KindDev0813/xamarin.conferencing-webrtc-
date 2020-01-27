using WebRTC.Abstraction;

namespace WebRTC.AppRTC
{
    public interface IH113ClientEvents
    {
        void OnPeerFactoryCreated(IPeerConnectionFactory factory);
        
        void OnDisconnect(DisconnectType disconnectType);

        void ReadyToCall();

        IVideoCapturer CreateVideoCapturer(IPeerConnectionFactory factory, IVideoSource videoSource);

        void OnError(string description);
    }


    public enum DisconnectType
    {
        PeerConnection,
        WebSocket
    }

    public partial class H113Client
    {
        private const string TAG = nameof(H113Client);

        private readonly IExecutor _executor;

        private readonly IH113ClientEvents _events;
        private readonly ILogger _logger;

        private IAppRTCCClient _rtcClient;

        private PeerConnectionClient _peerConnectionClient;


        public H113Client(IH113ClientEvents events, ILogger logger = null)
        {
            _executor = ExecutorServiceFactory.MainExecutor;


            _logger = logger ?? new ConsoleLogger();
            _rtcClient = new WebSocketRTCClient(this, _logger);

            _events = new H113ClientEventProxy(events, _executor);
        }

        public void Connect(ConnectionParameters connectionParameters)
        {
            _rtcClient = new WebSocketRTCClient(this, _logger);
            _rtcClient.Connect(connectionParameters);
        }

        public void Disconnect()
        {
            _rtcClient?.Disconnect();
            _rtcClient = null;
            _peerConnectionClient?.Close();
            _peerConnectionClient = null;
        }

        public void StartVideoCall(IVideoRenderer localRenderer, IVideoRenderer remoteRenderer)
        {
            _peerConnectionClient.CreatePeerConnection(localRenderer, remoteRenderer);
        }

        public void SwitchCamera()
        {
            _peerConnectionClient.SwitchCamera();
        }

        public void SetVideoEnabled(bool enable)
        {
            _peerConnectionClient.SetVideoEnabled(enable);
        }

        public void SetAudioEnabled(bool enable)
        {
            _peerConnectionClient.SetAudioEnabled(enable);
        }

        private class H113ClientEventProxy : IH113ClientEvents
        {
            private readonly IExecutor _executor;
            private readonly IH113ClientEvents _events;

            public H113ClientEventProxy(IH113ClientEvents events, IExecutor executor)
            {
                _events = events;
                _executor = executor;
            }

            public void OnPeerFactoryCreated(IPeerConnectionFactory factory) =>
                _executor.Execute(() => _events.OnPeerFactoryCreated(factory));

            public void OnDisconnect(DisconnectType disconnectType) =>
                _executor.Execute(() => _events.OnDisconnect(disconnectType));

            public void ReadyToCall() => _executor.Execute(ReadyToCall);

            public IVideoCapturer CreateVideoCapturer(IPeerConnectionFactory factory, IVideoSource videoSource) =>
                _events.CreateVideoCapturer(factory, videoSource);

            public void OnError(string description) => _executor.Execute(() => _events.OnError(description));
        }

       
    }
}
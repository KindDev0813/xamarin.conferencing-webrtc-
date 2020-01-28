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

            _events = events;
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
            _peerConnectionClient.CreateOffer();
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
    }
}
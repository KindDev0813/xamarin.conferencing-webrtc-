using WebRTC.Abstraction;

namespace WebRTC.AppRTC
{
    public enum AppClientState
    {
        Disconnected,
        Connecting,
        Connected
    }

    public interface IAppRTCClientListener
    {
        void DidCreatePeerConnection(IPeerConnection peerConnection);
        void DidOpenDataChannel(IDataChannel dataChannel);
        void DidChangeState(AppClientState state);
    }

    public class AppRTCClientConfig
    {
        public AppRTCClientConfig()
        {
            
        }
        public AppRTCClientConfig(string token, string wssUrl)
        {
            Token = token;
            WssUrl = wssUrl;
        }

        public string Token { get;  }
        public string WssUrl { get; }
        public bool IsLoopback { get; set; }
    }

    public class AppRTCClient
    {
        private readonly AppRTCClientListenerSchedulerProxy _listener;
        private readonly AppRTCClientConfig _config;

        private AppClientState _appClientState;
        private WebSocketClient _webSocketClient;

        public AppRTCClient(AppRTCClientConfig config, IScheduler scheduler = null)
        {
            _listener = new AppRTCClientListenerSchedulerProxy(scheduler ?? AppRTC.DefaultScheduler);
            _config = config;
        }

        public IAppRTCClientListener Listener
        {
            get => _listener.Listener;
            set => _listener.Listener = value;
        }

        public AppClientState State
        {
            get => _appClientState;
            set
            {
                if (_appClientState == value)
                    return;
                _appClientState = value;
                Listener?.DidChangeState(value);
            }
        }

        public bool IsLoopback { get; set; }

        public void Connect()
        {
            
        }

        public void Disconnect()
        {
        }


        private class AppRTCClientListenerSchedulerProxy : IAppRTCClientListener
        {
            private readonly IScheduler _scheduler;

            public AppRTCClientListenerSchedulerProxy(IScheduler scheduler)
            {
                _scheduler = scheduler;
            }

            public IAppRTCClientListener Listener { get; set; }

            public void DidCreatePeerConnection(IPeerConnection peerConnection)
            {
                _scheduler.Schedule(() => Listener?.DidCreatePeerConnection(peerConnection));
            }

            public void DidOpenDataChannel(IDataChannel dataChannel)
            {
                _scheduler.Schedule(() => Listener?.DidOpenDataChannel(dataChannel));
            }

            public void DidChangeState(AppClientState state)
            {
                _scheduler.Schedule(() => Listener?.DidChangeState(state));
            }
        }
    }
}
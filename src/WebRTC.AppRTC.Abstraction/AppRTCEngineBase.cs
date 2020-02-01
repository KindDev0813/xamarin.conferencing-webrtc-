using WebRTC.Abstraction;

namespace WebRTC.AppRTC.Abstraction
{
    public interface IAppRTCEngineEvents
    {
        void OnPeerFactoryCreated(IPeerConnectionFactory factory);

        void OnDisconnect(DisconnectType disconnectType);

        IVideoCapturer CreateVideoCapturer(IPeerConnectionFactory factory, IVideoSource videoSource);

        void ReadyToStart();

        void OnError(string description);
    }


    public enum DisconnectType
    {
        PeerConnection,
        WebSocket
    }

    public abstract class AppRTCEngineBase : ISignalingEvents, IPeerConnectionEvents
    {
        private const string TAG = nameof(AppRTCEngineBase);

        protected readonly IExecutor Executor;
        protected readonly IAppRTCEngineEvents Events;
        protected readonly ILogger Logger;
        
        protected IAppRTCCClient RTCClient { get; private set; }
        protected PeerConnectionClient PeerConnectionClient { get; private set; }

        protected AppRTCEngineBase(IAppRTCEngineEvents events, ILogger logger = null)
        {
            Events = events;
            Logger = logger ?? new ConsoleLogger();

            Executor = ExecutorServiceFactory.MainExecutor;
        }

        
        public bool Connected { get; private set; }
        
        protected abstract IAppRTCCClient CreateClient();

        protected abstract PeerConnectionParameters CreatePeerConnectionParameters(
            ISignalingParameters signalingParameters);

        protected abstract void OnChannelConnectedInternal(ISignalingParameters signalingParameters);

        public void Connect(IConnectionParameters connectionParameters)
        {
            RTCClient = CreateClient();
            RTCClient.Connect(connectionParameters);
        }

        public void Disconnect()
        {
            Connected = false;
            RTCClient?.Disconnect();
            RTCClient = null;
            PeerConnectionClient?.Close();
            PeerConnectionClient = null;
        }

        public void StartVideoCall(IVideoRenderer localRenderer, IVideoRenderer remoteRenderer)
        {
            PeerConnectionClient.CreatePeerConnection(localRenderer, remoteRenderer);
        }

        public void ChangeCaptureFormat(int width, int height, int framerate)
        {
            PeerConnectionClient?.ChangeCaptureFormat(width,height,framerate);
        }

        public void SwitchCamera()
        {
            PeerConnectionClient?.SwitchCamera();
        }

        public void SetVideoEnabled(bool enable)
        {
            PeerConnectionClient?.SetVideoEnabled(enable);
        }

        public void SetAudioEnabled(bool enable)
        {
            PeerConnectionClient?.SetAudioEnabled(enable);
        }

        public void OnChannelConnected(ISignalingParameters signalingParameters)
        {
            Logger.Debug(TAG, "Creating PeerConnectionClient");
            Executor.Execute(() =>
            {
                var peerConnectionClientParams = CreatePeerConnectionParameters(signalingParameters);
                PeerConnectionClient =
                    new PeerConnectionClient(peerConnectionClientParams, this,
                        Logger);
                PeerConnectionClient.CreatePeerConnectionFactory();
                OnChannelConnectedInternal(signalingParameters);
                Events.ReadyToStart();
                Logger.Debug(TAG, "Created PeerConnectionClient");
            });
        }

        public void OnChannelClose()
        {
            Executor.Execute(() => Events.OnDisconnect(DisconnectType.WebSocket));
        }

        public void OnChannelError(string description)
        {
            Executor.Execute(() => Events.OnError(description));
        }

        public void OnRemoteDescription(SessionDescription sdp)
        {
            Executor.Execute(() =>
            {
                if (PeerConnectionClient == null)
                {
                    Logger.Error(TAG, "Received remote SDP for non-initilized peer connection.");
                    return;
                }

                PeerConnectionClient.SetRemoteDescription(sdp);
            });
        }

        public void OnRemoteIceCandidate(IceCandidate candidate)
        {
            Executor.Execute(() =>
            {
                if (PeerConnectionClient == null)
                {
                    Logger.Error(TAG, "Received remote SDP for non-initilized peer connection.");
                    return;
                }

                PeerConnectionClient.AddRemoteIceCandidate(candidate);
            });
        }

        public void OnRemoteIceCandidatesRemoved(IceCandidate[] candidates)
        {
            Executor.Execute(() =>
            {
                if (PeerConnectionClient == null)
                {
                    Logger.Error(TAG, "Received remote SDP for non-initilized peer connection.");
                    return;
                }

                PeerConnectionClient.RemoveRemoteIceCandidates(candidates);
            });
        }

        public void OnPeerFactoryCreated(IPeerConnectionFactory factory)
        {
            Executor.Execute(() => { Events.OnPeerFactoryCreated(factory); });
        }

        public void OnConnected()
        {
        }

        public void OnDisconnected()
        {
            Executor.Execute(() => { Events.OnDisconnect(DisconnectType.PeerConnection); });
        }

        public void OnLocalDescription(SessionDescription sdp)
        {
            Executor.Execute(() =>
            {
                RTCClient?.SendOfferSdp(sdp);
            });
        }

        public void OnIceCandidate(IceCandidate candidate)
        {
            Executor.Execute(() =>
            {
                RTCClient?.SendLocalIceCandidate(candidate);
            });        }

        public void OnIceCandidateRemoved(IceCandidate[] candidates)
        {
            Executor.Execute(() =>
            {
                RTCClient?.SendLocalIceCandidateRemovals(candidates);
            });
        }

        public void OnIceConnected()
        {
        }

        public void OnIceDisconnected()
        {
        }

        public void OnPeerConnectionClosed()
        {
        }

        public void OnPeerConnectionError(string description)
        {
            Executor.Execute(() => Events.OnError(description));
        }

        public IVideoCapturer CreateVideoCapturer(IPeerConnectionFactory factory, IVideoSource videoSource)
        {
            return Events.CreateVideoCapturer(factory, videoSource);
        }
    }
}
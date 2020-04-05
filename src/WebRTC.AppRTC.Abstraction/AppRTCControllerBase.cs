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

    public interface IAppRTCController
    {
        bool IsVideoEnable { get; }
        bool IsAudioEnable { get; }
        public void SwitchCamera();
        public void SetVideoEnabled(bool enable);
        public void SetAudioEnabled(bool enable);
        void Disconnect();
    }

    public abstract class AppRTCControllerBase<TConnectionParam, TSignalParam> : ISignalingEvents<TSignalParam>,
        IPeerConnectionEvents,IAppRTCController
        where TSignalParam : ISignalingParameters
        where TConnectionParam : IConnectionParameters
    {
        private const string TAG = nameof(AppRTCControllerBase<TConnectionParam, TSignalParam>);

        private TSignalParam _signalingParameters;

        protected AppRTCControllerBase(IAppRTCEngineEvents events, ILogger logger = null)
        {
            Events = events;
            Logger = logger ?? new ConsoleLogger();

            Executor = ExecutorServiceFactory.MainExecutor;
        }

        protected readonly IExecutor Executor;
        protected readonly IAppRTCEngineEvents Events;
        protected readonly ILogger Logger;

        protected IAppRTCCClient<TConnectionParam> RTCClient { get; private set; }
        protected PeerConnectionClient PeerConnectionClient { get; private set; }

        public bool Connected { get; private set; }

        protected abstract bool IsInitiator { get; }

        protected TSignalParam SignalingParameters { get; private set; }

        protected abstract IAppRTCCClient<TConnectionParam> CreateClient();

        protected abstract PeerConnectionParameters CreatePeerConnectionParameters(
            TSignalParam signalingParameters);


        protected abstract void OnChannelConnectedInternal(TSignalParam signalingParameters);

        public void Connect(TConnectionParam connectionParameters)
        {
            RTCClient = CreateClient();
            RTCClient.Connect(connectionParameters);
        }

        public virtual void Disconnect()
        {
            Connected = false;
            RTCClient?.Disconnect();
            RTCClient = null;
            PeerConnectionClient?.Close();
            PeerConnectionClient = null;
        }

        public void StartVideoCall(IVideoRenderer localRenderer, IVideoRenderer remoteRenderer)
        {
            Executor.Execute(() =>
            {
                PeerConnectionClient.CreatePeerConnection(localRenderer, remoteRenderer);
                OnChannelConnectedInternal(_signalingParameters);
            });
        }

        public void ChangeCaptureFormat(int width, int height, int framerate)
        {
            PeerConnectionClient?.ChangeCaptureFormat(width, height, framerate);
        }

        public bool IsVideoEnable => PeerConnectionClient?.IsVideoEnable ?? false;
        public bool IsAudioEnable => PeerConnectionClient?.IsAudioEnable ?? false;
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

        public void OnChannelConnected(TSignalParam signalingParameters)
        {
            Logger.Debug(TAG, "Creating PeerConnectionClient");
            SignalingParameters = signalingParameters;
            Executor.Execute(() =>
            {
                _signalingParameters = signalingParameters;

                PeerConnectionClient?.Close();

                var peerConnectionClientParams = CreatePeerConnectionParameters(signalingParameters);
                PeerConnectionClient =
                    new PeerConnectionClient(peerConnectionClientParams, this,
                        Logger);
                PeerConnectionClient.CreatePeerConnectionFactory();
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

        public void OnPeerConnectionCreated(IPeerConnection peerConnection)
        {
            Executor.Execute(()=>  OnPeerConnectionCreatedInternal(peerConnection));
        }

        public void OnConnected()
        {
            Connected = true;
        }

        public void OnDisconnected()
        {
            Connected = false;
            Executor.Execute(() => { Events.OnDisconnect(DisconnectType.PeerConnection); });
        }

        public void OnLocalDescription(SessionDescription sdp)
        {
            Executor.Execute(() =>
            {
                Logger.Debug(TAG, $"Sending {sdp.Type}");
                if (IsInitiator)
                    RTCClient?.SendOfferSdp(sdp);
                else
                    RTCClient?.SendAnswerSdp(sdp);
            });
        }

        public void OnIceCandidate(IceCandidate candidate)
        {
            Executor.Execute(() => { RTCClient?.SendLocalIceCandidate(candidate); });
        }

        public void OnIceCandidateRemoved(IceCandidate[] candidates)
        {
            Executor.Execute(() => { RTCClient?.SendLocalIceCandidateRemovals(candidates); });
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

        protected virtual void OnPeerConnectionCreatedInternal(IPeerConnection peerConnection)
        {
            
        }
    }
}
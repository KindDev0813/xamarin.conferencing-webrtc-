using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebRTC.Abstraction;
using WebRTC.Abstraction.Extensions;
using WebRTC.AppRTC.Extensions;
using Xamarin.Essentials;

namespace WebRTC.AppRTC
{
    public enum AppClientState
    {
        Disconnected,
        Connecting,
        Connected
    }

    public enum AppErrorCode
    {
        Unknown,
        CreateSdp,
        SetSdp,
        WebSocket,
    }

    public class AppRTCException : Exception
    {
        public AppRTCException(string message) : this(message, null, AppErrorCode.Unknown)
        {
        }

        public AppRTCException(Exception ex) : this("Unknown error.", ex, AppErrorCode.Unknown)
        {
        }


        public AppRTCException(string message, AppErrorCode errorCode) : this(message, null, errorCode)
        {
        }

        public AppRTCException(string message, Exception ex, AppErrorCode errorCode) : base(message, ex)
        {
            ErrorCode = errorCode;
        }

        public AppErrorCode ErrorCode { get; }
    }

    public class AppRTCClientConfig
    {
        public AppRTCClientConfig(string token, string wssUrl)
        {
            Token = token;
            WssUrl = wssUrl;
        }

        public string Token { get; }
        public string WssUrl { get; }
    }

    public interface IAppRTCClientListener
    {
        void DidCreatePeerConnection(IPeerConnection peerConnection);
        void DidOpenDataChannel(IDataChannel dataChannel);
        void DidChangeState(AppClientState state);
        void DidReceiveLocalVideoTrack(IVideoTrack videoTrack);

        void DidReceiveRemoteVideoTrack(IVideoTrack videoTrack);

        void DidCreateCapturer(IVideoCapturer videoCapturer);

        void DidRegisterWithCollider();

        void OnError(AppRTCException error);
    }

    public class AppRTCClient : ISignalingChannelListener, IPeerConnectionListener
    {
        private const string TAG = nameof(AppRTCClient);

        private const string MediaStreamId = "ARDAMS";
        private const string VideoTrackId = "ARDAMSv0";
        private const string AudioTrackId = "ARDAMSa0";
        private const string VideoTrackType = "video";

        private readonly IScheduler _scheduler;

        private readonly AppRTCClientListenerSchedulerProxy _listener;
        private readonly AppRTCClientConfig _config;


        private readonly SignalingChannel _channel;

        private readonly ILogger _logger;

        private readonly List<IceServer> _iceServers = new List<IceServer>();

        private AppClientState _appClientState;


        private IPeerConnectionFactory _factory;
        private IPeerConnection _peerConnection;
        private IVideoTrack _localVideoTrack;

        public AppRTCClient(AppRTCClientConfig config, IAppRTCFactory appRTCFactory, ILogger logger = null,
            IScheduler scheduler = null)
        {
            AppRTC.Init(appRTCFactory);

            _scheduler = scheduler ?? AppRTC.DefaultScheduler;
            _listener = new AppRTCClientListenerSchedulerProxy(_scheduler);
            _logger = logger ?? AppRTC.Logger;
            _config = config;


            _channel = new WebSocketClient(config.WssUrl, config.Token) {Listener = this};
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

        public async Task<bool> ConnectAsync(string phone)
        {
            if (!await CheckPermissionAsync<Permissions.LocationWhenInUse>())
            {
                _logger.Debug(TAG, "Location permission denied!");
                return false;
            }

            if (!await CheckPermissionAsync<Permissions.Camera>())
            {
                _logger.Debug(TAG, "Camera permission denied!");
                return false;
            }

            if (!_channel.IsOpen)
            {
                try
                {
                    await _channel.OpenAsync();
                }
                catch (Exception ex)
                {
                    _logger.Error(TAG, "Failed to open WS.", ex);
                    return false;
                }
            }

            var location = await Geolocation.GetLastKnownLocationAsync();

            //_channel.SendMessage(new RegisterMessage(phone, location.Longitude, location.Latitude));

            _channel.SendMessage(new RegisterMessage(phone, 54.23, 12.12));

            StartSignalingIfReady();

            return true;
        }


        public void Disconnect()
        {
            _factory?.Dispose();
            _factory = null;
            _peerConnection?.Dispose();
            _peerConnection = null;
            _localVideoTrack?.Dispose();
            _localVideoTrack = null;
            _channel?.CloseAsync().GetAwaiter().GetResult();
        }

        public void DidChangeState(SignalingChannel channel, SignalingChannelState state)
        {
            if (state == SignalingChannelState.Registered)
            {
                StartSignalingIfReady();
                Listener?.DidRegisterWithCollider();
            }
        }

        public void DidReceiveMessage(SignalingChannel channel, SignalingMessage message)
        {
            switch (message)
            {
                case RegisteredMessage registeredMessage:
                    _iceServers.Clear();
                    _iceServers.AddRange(registeredMessage.GetIceServers());
                    break;
                case IceCandidateMessage iceCandidateMessage:
                    var iceCandidate = iceCandidateMessage.IceCandidate;
                    _peerConnection.AddIceCandidate(new IceCandidate(iceCandidate.Sdp, iceCandidate.SdpMid,
                        iceCandidate.SdpMLineIndex));
                    break;
                case SessionDescriptionMessage sessionDescriptionMessage:
                    _peerConnection.SetRemoteDescription(
                        new SessionDescription(SdpType.Answer, sessionDescriptionMessage.Description.Sdp),
                        DidSetSessionDescriptionWithError);
                    break;
            }
        }

        public void OnSignalingChange(SignalingState signalingState)
        {
            _logger.Debug(TAG, $"OnSignalingChange -> {signalingState} ");
        }

        public void OnIceConnectionChange(IceConnectionState iceConnectionState)
        {
            _logger.Debug(TAG, $"OnIceConnectionChange -> {iceConnectionState} ");
        }

        public void OnConnectionChange(PeerConnectionState newState)
        {
            _logger.Debug(TAG, $"OnConnectionChange -> {newState} ");
        }

        public void OnIceGatheringChange(IceGatheringState iceGatheringState)
        {
            _logger.Debug(TAG, $"OnIceGatheringChange -> {iceGatheringState} ");
        }

        public void OnIceCandidate(IceCandidate iceCandidate)
        {
            _logger.Debug(TAG, "OnIceCandidate");
            _channel.SendMessage(new IceCandidateMessage(iceCandidate));
        }

        public void OnIceCandidatesRemoved(IceCandidate[] iceCandidates)
        {
            _logger.Debug(TAG, $"OnIceCandidatesRemoved -> {iceCandidates.Length}");
            _peerConnection.RemoveIceCandidates(iceCandidates);
        }

        public void OnAddStream(IMediaStream mediaStream)
        {
            _logger.Debug(TAG, $"OnAddStream -> {mediaStream.StreamId}");
            _logger.Debug(TAG,
                $"Stream({mediaStream.StreamId}) with {mediaStream.VideoTracks.Length} video tracks and {mediaStream.AudioTracks.Length} audio tracks");
        }

        public void OnRemoveStream(IMediaStream mediaStream)
        {
            _logger.Debug(TAG, $"OnRemoveStream -> {mediaStream.StreamId}");
        }

        public void OnDataChannel(IDataChannel dataChannel)
        {
            _logger.Debug(TAG, $"OnDataChannel -> {dataChannel.Id}");
        }

        public void OnRenegotiationNeeded()
        {
            _logger.Debug(TAG, "OnRenegotiationNeeded");
        }

        public void OnAddTrack(IRtpReceiver rtpReceiver, IMediaStream[] mediaStreams)
        {
            _logger.Debug(TAG, "OnAddTrack");
        }

        public void OnTrack(IRtpTransceiver transceiver)
        {
            _logger.Debug(TAG, "OnTrack");
        }

        private void StartSignalingIfReady()
        {
            if (_channel.State != SignalingChannelState.Registered || State == AppClientState.Connected)
                return;

            State = AppClientState.Connected;

            _factory = NativeFactory.CreatePeerConnectionFactory();

            var config = new RTCConfiguration(_iceServers.ToArray());
            config.Certificate = RTCCertificate.GenerateCertificate(EncryptionKeyType.Rsa, 100000);


            _peerConnection = _factory.CreatePeerConnection(config, this);

            if (_peerConnection == null)
            {
                Listener?.OnError(new AppRTCException("Invalid config for PeerConnectionFactory"));
                return;
            }

            Listener?.DidCreatePeerConnection(_peerConnection);

            CreateMediaSenders();

            var mandatoryConstraints = new Dictionary<string, string>
            {
                ["OfferToReceiveAudio"] = "true",
                ["OfferToReceiveVideo"] = "true"
            };

            _peerConnection.CreateOffer(new MediaConstraints(mandatoryConstraints, null), DidCreateSessionDescription);
        }

        private void DidCreateSessionDescription(SessionDescription sdp, Exception error)
        {
            if (error != null)
            {
                var msg = "Failed to create session description.";
                _logger.Error(TAG, msg, error);
                Disconnect();

                Listener?.OnError(new AppRTCException(msg, error,
                    AppErrorCode.CreateSdp));
                return;
            }

            _peerConnection.SetLocalDescription(sdp, DidSetSessionDescriptionWithError);
            _channel.SendMessage(new SessionDescriptionMessage(sdp));
        }

        private void DidSetSessionDescriptionWithError(Exception error)
        {
            if (error == null)
            {
                _logger.Debug(TAG, "Successfully set session description.");
                return;
            }

            var msg = "Failed to set session description.";
            _logger.Error(TAG, msg, error);
            Disconnect();
            Listener?.OnError(new AppRTCException(msg, error, AppErrorCode.CreateSdp));
        }

        private void CreateMediaSenders()
        {
            var source = _factory.CreateAudioSource(new MediaConstraints());
            var track = _factory.CreateAudioTrack(AudioTrackId, source);

            _peerConnection.AddTrack(track, new[] {MediaStreamId});

            _localVideoTrack = CreateLocalVideoTrack();

            if (_localVideoTrack == null)
                return;
            _peerConnection.AddTrack(_localVideoTrack, new[] {MediaStreamId});
            Listener?.DidReceiveLocalVideoTrack(_localVideoTrack);

            var receiver = _peerConnection.GetVideoTransceiver()?.Receiver;

            if (receiver != null)
                Listener?.DidReceiveRemoteVideoTrack(receiver?.Track as IVideoTrack);
        }

        private IVideoTrack CreateLocalVideoTrack()
        {
            var source = _factory.CreateVideoSource(false);
            var capturer = _factory.CreateCameraCapturer(source, true);
            var videoTrack = _factory.CreateVideoTrack(VideoTrackId, source);
            Listener.DidCreateCapturer(capturer);
            return videoTrack;
        }


        private static async Task<bool> CheckPermissionAsync<T>() where T : Permissions.BasePermission, new()
        {
            var result = await Permissions.RequestAsync<T>();
            return result == PermissionStatus.Granted;
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

            public void DidReceiveLocalVideoTrack(IVideoTrack videoTrack)
            {
                _scheduler.Schedule(() => Listener?.DidReceiveLocalVideoTrack(videoTrack));
            }

            public void DidReceiveRemoteVideoTrack(IVideoTrack videoTrack)
            {
                _scheduler.Schedule(() => Listener?.DidReceiveRemoteVideoTrack(videoTrack));
            }

            public void DidCreateCapturer(IVideoCapturer videoCapturer)
            {
                _scheduler.Schedule(() => Listener?.DidCreateCapturer(videoCapturer));
            }

            public void DidRegisterWithCollider()
            {
                _scheduler.Schedule(() => Listener?.DidRegisterWithCollider());
            }

            public void OnError(AppRTCException error)
            {
                _scheduler.Schedule(() => Listener?.OnError(error));
            }
        }
    }
}
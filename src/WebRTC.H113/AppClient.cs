using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WebRTC.Abstraction;
using WebRTC.H113.Extensions;
using WebRTC.H113.Schedulers;
using WebRTC.H113.Signaling;
using WebRTC.H113.Signaling.Models;

namespace WebRTC.H113
{
    public class AppClient : ISignalingChannelEvents, IPeerConnectionEvents, IDataChannelEvents
    {
        private const string TAG = nameof(AppClient);

        private readonly SerialDisposable _onLocationChangedDisposable = new SerialDisposable();


        private readonly IAppClientEvents _appClientEvents;
        private readonly ILocationService _locationService;
        private readonly ILogger _logger;
        private readonly IExecutor _executor;

        private SignalingChannel _signalingChannel;

        private PeerConnectionClient _peerConnectionClient;

        private DataChannel _dataChannel;

        private bool _disconnectInProgress;


        public AppClient(IAppClientEvents appClientEvents, ILocationService locationService, ILogger logger = null)
        {
            _appClientEvents = appClientEvents;
            _locationService = locationService;
            _logger = logger ?? new ConsoleLogger();

            _executor = ExecutorServiceFactory.MainExecutor;
        }

        public bool IsInitiator { get; } = true;
        public ConnectionParameters ConnectionParameters { get; private set; }

        public RegisteredMessage SignalingParameters { get; private set; }

        public bool IsWebRTCConnected { get; private set; }
        public bool IsWebSocketConnected { get; private set; }
        public ConnectionState ConnectionState { get; private set; }

        public bool IsVideoEnable => _peerConnectionClient?.IsVideoEnable ?? false;

        public bool IsAudioEnable => _peerConnectionClient?.IsAudioEnable ?? false;

        public void Connect(ConnectionParameters connectionParameters)
        {
            _executor.Execute(async () =>
            {
                if (ConnectionState == ConnectionState.Connected || ConnectionState == ConnectionState.Connecting)
                    throw new InvalidOperationException("It's already connected please Disconnect before trying again");
                ConnectionParameters = connectionParameters;
                ConnectionState = ConnectionState.Connecting;
                if (!await CreateSignalingChannelAsync(connectionParameters))
                {
                    ReportError("Failed to open WebSocket");
                    return;
                }

                var lastLocation = await _locationService.GetLastLocationAsync();

                _signalingChannel.SendMessage(new RegisterMessage(connectionParameters.Phone, lastLocation.Longitude,
                    lastLocation.Latitude));

                _onLocationChangedDisposable.Disposable = _locationService.OnLocationChanged
                    .Where(l => l != null)
                    .Subscribe(location =>
                    {
                        _executor.Execute(() =>
                        {
                            _logger.Debug(TAG, $"Updating location {location}");
                            if (_signalingChannel.State == SignalingChannelState.Registered &&
                                SignalingParameters != null)
                            {
                                _signalingChannel.SendMessage(new UpdateInfoMessage(SignalingParameters.SocketId,
                                    location));
                            }
                        });
                    });
            });
        }

        public void Disconnect()
        {
            _executor.Execute(() =>
            {
                if (ConnectionState != ConnectionState.Connected && ConnectionState != ConnectionState.Connecting)
                    return;
                DisconnectInternal(DisconnectType.UserDisconnect);
            });
        }

        public void StartVideoCall(IVideoRenderer localRenderer, IVideoRenderer remoteRenderer)
        {
            _executor.Execute(() => { _peerConnectionClient.CreatePeerConnection(localRenderer, remoteRenderer); });
        }

        public void SwitchCamera()
        {
            _peerConnectionClient?.SwitchCamera();
        }

        public void ShowNotification(int type, string title, string message)
        {
            _executor.Execute(() => _appClientEvents.ShowNotification(type, title, message));
        }

        public void SetVideoEnabled(bool enable)
        {
            _peerConnectionClient?.SetVideoEnabled(enable);
        }

        public void SetAudioEnabled(bool enable)
        {
            _peerConnectionClient?.SetAudioEnabled(enable);
        }

        void ISignalingChannelEvents.ChannelDidChangeState(SignalingChannel channel, SignalingChannelState state)
        {
            _executor.Execute(() =>
            {
                switch (state)
                {
                    case SignalingChannelState.New:
                    case SignalingChannelState.Closed:
                    case SignalingChannelState.Error:
                    case SignalingChannelState.Connecting:
                    case SignalingChannelState.Reconnecting:
                        IsWebSocketConnected = false;
                        break;
                    case SignalingChannelState.Open:
                    case SignalingChannelState.Registered:
                        IsWebSocketConnected = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }
            });
        }

        void ISignalingChannelEvents.ChannelDidReceiveMessage(SignalingChannel channel,
            SignalingMessage message)
        {
            _executor.Execute(() =>
            {
                if (channel.State != SignalingChannelState.Registered)
                {
                    ReportError("Got WebSocket message in non registered state.");
                    return;
                }

                switch (message.MessageType)
                {
                    case SignalingMessageType.Unknown:
                        _logger.Error(TAG, $"Got unknown message: {message}");
                        break;
                    case SignalingMessageType.Register:
                    case SignalingMessageType.Offer:
                    case SignalingMessageType.SendCandidate:
                        _logger.Error(TAG, $"Got wrong message type: {message.MessageType}");
                        break;
                    case SignalingMessageType.Registered:
                        var registerMessage = (RegisteredMessage) message;
                        SignalingParametersReady(registerMessage);
                        break;
                    case SignalingMessageType.Reconnecting:
                        var reconnectedMessage = (ReconnectingMessage) message;
                        //TODO(vali): impl this??
                        break;
                    case SignalingMessageType.ReceivedAnswer:
                        var answerMessage = (SessionDescriptionMessage) message;
                        OnRemoteDescription(answerMessage.Description);
                        break;
                    case SignalingMessageType.ReceiveCandidate:
                        var candidateMessage = (IceCandidateMessage) message;
                        var iceCandidate = candidateMessage.IceCandidate;
                        OnRemoteIceCandidate(new IceCandidate(iceCandidate.Sdp, iceCandidate.SdpMid,
                            iceCandidate.SdpMLineIndex));
                        break;
                    case SignalingMessageType.StopVideo:
                        Disconnect();
                        break;
                    case SignalingMessageType.CloseConnection:
                        Disconnect();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        void ISignalingChannelEvents.ChannelOnError(SignalingChannel channel, string description)
        {
            ReportError($"WebSocket failed - {description}");
        }

        void ISignalingChannelEvents.ChannelDidReconnect(SignalingChannel channel)
        {
            _executor.Execute(() =>
            {
                _logger.Debug(TAG, "WebSocket reconnected resetting IceCandidates");
                _peerConnectionClient.ResetIceConnection();
            });
        }

        void ISignalingChannelEvents.ChannelDidClose(SignalingChannel channel, int code, string reason)
        {
            _executor.Execute(() => { DisconnectInternal(DisconnectType.WebSocket); });
        }

        void IPeerConnectionEvents.OnPeerFactoryCreated(IPeerConnectionFactory factory)
        {
            _executor.Execute(() => _appClientEvents.OnPeerFactoryCreated(factory));
        }

        void IPeerConnectionEvents.OnPeerConnectionCreated(IPeerConnection peerConnection)
        {
            _executor.Execute(() =>
            {
                _peerConnectionClient.CreateOffer();
                _dataChannel = new DataChannel(this, peerConnection);
            });
        }

        void IPeerConnectionEvents.OnConnected()
        {
            _executor.Execute(() =>
            {
                IsWebRTCConnected = true;
                if (IsWebRTCConnected && IsWebSocketConnected)
                    ConnectionState = ConnectionState.Connected;
                else
                {
                    ConnectionState = ConnectionState.Error;
                    _appClientEvents.OnError("Invalid state WebRTC it's connected but WS is disconnected...");
                    return;
                }

                _appClientEvents.OnConnected();
            });
        }

        void IPeerConnectionEvents.OnDisconnected()
        {
            _executor.Execute(() =>
            {
                IsWebRTCConnected = false;
                DisconnectInternal(DisconnectType.PeerConnection);
            });
        }

        void IPeerConnectionEvents.OnLocalDescription(SessionDescription sdp)
        {
            _executor.Execute(() =>
            {
                if (ConnectionState != ConnectionState.Connecting)
                {
                    ReportError("Sending offer SDP in non connected state.");
                    return;
                }

                _logger.Debug(TAG, $"Sending {sdp.Type}");

                var offerMessage = new SessionDescriptionMessage(sdp)
                {
                    SocketId = SignalingParameters.SocketId,
                    MessageType = SignalingMessageType.Offer
                };
                _signalingChannel.SendMessage(offerMessage);
            });
        }

        void IPeerConnectionEvents.OnIceCandidate(IceCandidate candidate)
        {
            _executor.Execute(() =>
            {
                var iceCandidate = new IceCandidateMessage(candidate)
                {
                    SocketId = SignalingParameters.SocketId,
                };
                _signalingChannel.SendMessage(iceCandidate);
            });
        }

        void IPeerConnectionEvents.OnIceCandidateRemoved(IceCandidate[] candidates)
        {
            _logger.Debug(TAG, $"OnIceCandidateRemoved - {candidates.Length}");
        }

        void IPeerConnectionEvents.OnIceConnected()
        {
            _logger.Debug(TAG, "OnIceConnected");
        }

        void IPeerConnectionEvents.OnIceDisconnected()
        {
            _logger.Debug(TAG, "OnIceDisconnected");
        }

        bool IPeerConnectionEvents.OnIceFailedHandle()
        {
            _logger.Debug(TAG, "OnIceFailedHandle");
            return true;
        }

        void IPeerConnectionEvents.OnPeerConnectionClosed()
        {
            _logger.Debug(TAG, "OnPeerConnectionClosed");
        }

        void IPeerConnectionEvents.OnPeerConnectionError(string description)
        {
            _executor.Execute(() => ReportError(description));
        }

        IVideoCapturer IPeerConnectionEvents.CreateVideoCapturer(IPeerConnectionFactory factory,
            IVideoSource videoSource) => _appClientEvents.CreateVideoCapturer(factory, videoSource);

        private Task<bool> CreateSignalingChannelAsync(ConnectionParameters connectionParameters)
        {
            _signalingChannel =
                new SignalingChannel(this, connectionParameters.WsUrl, connectionParameters.Protocol, _logger);
            return _signalingChannel.ConnectAsync();
        }

        private void SignalingParametersReady(RegisteredMessage registeredMessage)
        {
            SignalingParameters = registeredMessage;
            _logger.Debug(TAG, "Creating PeerConnectionClient");
            var peerConnectionParameters = new PeerConnectionParameters(registeredMessage.GetIceServers())
            {
                VideoCallEnabled = true,
                AudioCallEnabled = false
            };
            _peerConnectionClient = new PeerConnectionClient(peerConnectionParameters, this, _logger);
            _peerConnectionClient.CreatePeerConnectionFactory();
            _appClientEvents.ReadyToStart();
            _logger.Debug(TAG, "Created PeerConnectionClient");
        }

        private void OnRemoteDescription(SessionDescription sdp)
        {
            if (_peerConnectionClient == null)
            {
                ReportError("Received remote SDP for non-initilized peer connection.");
                return;
            }

            _peerConnectionClient.SetRemoteDescription(sdp);
        }

        private void OnRemoteIceCandidate(IceCandidate iceCandidate)
        {
            if (_peerConnectionClient == null)
            {
                ReportError("Received remote IceCandidate for non-initilized peer connection.");
                return;
            }

            _peerConnectionClient.AddRemoteIceCandidate(iceCandidate);
        }

        private void DisconnectInternal(DisconnectType disconnectType)
        {
            if (_disconnectInProgress)
                return;
            _disconnectInProgress = true;
            _logger.Debug(TAG, $"Disconnecting {disconnectType}.");
            _appClientEvents.OnDisconnect(disconnectType);
            ClearAppState();
            _disconnectInProgress = false;
        }

        private void ClearAppState()
        {
            _logger.Debug(TAG, "Clearing app state.");

            CloseWebSocket();

            IsWebSocketConnected = false;
            IsWebRTCConnected = false;

            _dataChannel?.Dispose();
            _dataChannel = null;

            _peerConnectionClient?.Close();
            _peerConnectionClient = null;

            ConnectionState = ConnectionState.Closed;
        }

        private void CloseWebSocket()
        {
            if (_signalingChannel == null)
                return;
            if (_signalingChannel.State == SignalingChannelState.Open ||
                _signalingChannel.State == SignalingChannelState.Registered)
            {
                _logger.Debug(TAG, "Closing ws.");
                if (_signalingChannel.State == SignalingChannelState.Registered)
                    _signalingChannel.SendMessage(new CloseConnectionMessage());
                _signalingChannel.Close();
            }

            _signalingChannel = null;
        }

        private void ReportError(string errorMessage)
        {
            _logger.Error(TAG, errorMessage);
            _executor.Execute(() =>
            {
                if (ConnectionState == ConnectionState.Error)
                    return;
                ConnectionState = ConnectionState.Error;
                _appClientEvents.OnError(errorMessage);
            });
        }
    }
}
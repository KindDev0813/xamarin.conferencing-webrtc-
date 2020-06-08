using System;
using WebRTC.Abstraction;
using WebRTC.AppRTC.Abstraction;
using Xamarin.Essentials;

namespace WebRTC.H113
{
    public class H113RTCClient : IAppRTCCClient<ConnectionParameters>, IWebSocketChannelEvents
    {
        private const string TAG = nameof(H113RTCClient);

        private readonly IH113SignalingEvents _signalingEvents;
        private readonly IExecutorService _executor;
        private readonly ILogger _logger;

        private H113WebSocketClient _wsClient;

        private ConnectionParameters _connectionParameters;

        private string _socketId;

        public H113RTCClient(IH113SignalingEvents signalingEvents, ILogger logger = null)
        {
            _signalingEvents = signalingEvents;
            _executor = ExecutorServiceFactory.CreateExecutorService(nameof(H113RTCClient));
            _logger = logger ?? new ConsoleLogger();
            State = ConnectionState.New;
        }

        public ConnectionState State { get; private set; }

        public void Connect(ConnectionParameters connectionParameters)
        {
            _connectionParameters = connectionParameters;

            _executor.Execute(ConnectInternal);
        }

        public void Disconnect()
        {
            _executor.Execute(DisconnectInternal);
        }

        public void SendOfferSdp(SessionDescription sdp)
        {
            _executor.Execute(() =>
            {
                if (State != ConnectionState.Connected)
                {
                    ReportError("Sending offer SDP in non connected state.");
                    return;
                }

                var offerMessage = new SessionDescriptionMessage(sdp)
                {
                    SocketId = _socketId,
                    MessageType = SignalingMessageType.Offer
                };
                _wsClient.Send(offerMessage);
            });
        }

        public void SendAnswerSdp(SessionDescription sdp)
        {
            throw new NotImplementedException();
        }

        public void SendLocalIceCandidate(IceCandidate candidate)
        {
            _executor.Execute(() =>
            {
                var iceCandidate = new IceCandidateMessage(candidate)
                {
                    SocketId = _socketId,
                    MessageType = SignalingMessageType.SendCandidate
                };
                _wsClient.Send(iceCandidate);
            });
        }

        public void SendLocalIceCandidateRemovals(IceCandidate[] candidates)
        {
            //TODO: impl in feature?
            _executor.Execute(() => { });
        }
        
        public void UpdateInfoMessage(Location location)
        {
            _executor.Execute(() =>
            {
                if (State != ConnectionState.Connected)
                {
                    ReportError("Sending UpdateInfoMessage in non connected state.");

                    return;
                }

                var message = new UpdateInfoMessage(_socketId, location)
                {
                    SocketId = _socketId,
                    MessageType = SignalingMessageType.UpdateInfo
                };

                _logger.Debug(TAG, message.ToString());

                _wsClient.Send(message);
            });
        }

        public void OnWebSocketOpen()
        {
            _logger.Debug(TAG, "Websocket open...");
        }

        public void OnWebSocketClose()
        {
            _logger.Debug(TAG, "Websocket closed...");
            _executor.Execute(() => _signalingEvents.OnChannelClose());
        }

        public void OnWebSocketMessage(string message)
        {
            if (_wsClient.State != WebSocketConnectionState.Registered)
            {
                _logger.Error(TAG, "Got WebSocket message in non registered state.");
                return;
            }

            var msg = SignalingMessage.FromJson(message);

            if (msg == null) //jls
                return;

            switch (msg.MessageType)
            {
                case SignalingMessageType.Unknown:
                    _logger.Error(TAG, $"Got unknown message: {message}");
                    break;
                case SignalingMessageType.Register:
                case SignalingMessageType.Offer:
                case SignalingMessageType.SendCandidate:
                    _logger.Error(TAG, $"Got wrong message type: {msg.MessageType}");
                    break;
                case SignalingMessageType.Registered:
                    var registerMessage = (RegisteredMessage) msg;
                    SignalingParametersReady(registerMessage);
                    break;
                case SignalingMessageType.ReceivedAnswer:
                    var answerMessage = (SessionDescriptionMessage) msg;
                    _signalingEvents.OnRemoteDescription(answerMessage.Description);
                    break;
                case SignalingMessageType.ReceiveCandidate:
                    var candidateMessage = (IceCandidateMessage) msg;
                    var iceCandidate = candidateMessage.IceCandidate;
                    _signalingEvents.OnRemoteIceCandidate(new IceCandidate(iceCandidate.Sdp, iceCandidate.SdpMid,
                        iceCandidate.SdpMLineIndex));
                    break;
                case SignalingMessageType.Reconnecting:
                    var reconnectingMessage = (ReconnectingMessage) msg;
                    _socketId = reconnectingMessage.Id;
                    break;
                case SignalingMessageType.CloseConnection:
                    OnRequestCloseConnection();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnWebSocketError(string description)
        {
            ReportError(description);
        }

        private void ConnectInternal()
        {
            State = ConnectionState.New;
            _wsClient = new H113WebSocketClient(_executor, this, _logger);
            _wsClient.Connect(_connectionParameters.WsUrl, _connectionParameters.Protocol);
            _wsClient.Register(new RegisterMessage(_connectionParameters.Phone));
        }

        private void DisconnectInternal()
        {
            _logger.Debug(TAG, "Disconnect.");
            if (State == ConnectionState.Connected)
            {
                _logger.Debug(TAG, "Closing ws.");
            }

            State = ConnectionState.Closed;
            _wsClient?.Disconnect(true);
            _executor.Release();
        }

        private void SignalingParametersReady(RegisteredMessage registeredMessage)
        {
            _logger.Debug(TAG, "Connection completed.");

            if (registeredMessage.SocketId == null)
            {
                ReportError("Failed to register with WS");
                return;
            }

            _socketId = registeredMessage.SocketId;

            State = ConnectionState.Connected;

            _signalingEvents.OnChannelConnected(registeredMessage);
        }

        private void OnRequestCloseConnection()
        {
            _logger.Debug(TAG, "Received close command...");
            _executor.Execute(() => _signalingEvents.OnRequestCloseConnection());
        }

        private void ReportError(string errorMessage)
        {
            _logger.Error(TAG, errorMessage);
            _executor.Execute(() =>
            {
                if (State == ConnectionState.Error)
                    return;
                State = ConnectionState.Error;
                _signalingEvents.OnChannelError(errorMessage);
            });
        }
    }
}
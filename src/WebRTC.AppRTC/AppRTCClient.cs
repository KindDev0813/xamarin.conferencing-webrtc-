using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WebRTC.Abstraction;

namespace WebRTC.AppRTC
{
    public class AppRTCClient : IAppRTCCClient, IWebSocketChannelEvents
    {
        private const string TAG = nameof(AppRTCClient);
        private const string ROOM_JOIN = "join";
        private const string ROOM_MESSAGE = "message";
        private const string ROOM_LEAVE = "leave";

        private readonly ISignalingEventsEx _signalingEvents;
        private readonly IExecutorService _executor;
        private readonly ILogger _logger;

        private WebSocketChannelClientEx _wsClient;

        private RoomConnectionParameters _connectionParameters;

        private bool _initiator;
        private string _messageUrl;
        private string _leaveUrl;


        public AppRTCClient(ISignalingEventsEx signalingEvents, ILogger logger = null)
        {
            _signalingEvents = signalingEvents;
            _executor = ExecutorServiceFactory.CreateExecutorService(nameof(AppRTCClient));
            _logger = logger ?? new ConsoleLogger();
            State = ConnectionState.New;
        }

        public ConnectionState State { get; private set; }
        
       
        public void Connect(IConnectionParameters connectionParameters)
        {
            _connectionParameters = (RoomConnectionParameters) connectionParameters;
            _executor.Execute(ConnectToRoomInternal);
        }

        public void Disconnect()
        {
            _executor.Execute(DisconnectFromRoomInternal);
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

                var json = JsonConvert.SerializeObject(new
                {
                    sdp = sdp.Sdp,
                    type = "offer"
                });
                SendPostMessage(MessageType.Message,_messageUrl,json);

                if (_connectionParameters.IsLoopback)
                {
                    // In loopback mode rename this offer to answer and route it back.
                    var sdpAnswer = new SessionDescription(SdpType.Answer, sdp.Sdp);
                    _signalingEvents.OnRemoteDescription(sdpAnswer);
                }
            });
        }

        public void SendAnswerSdp(SessionDescription sdp)
        {
            _executor.Execute(() =>
            {
                if (_connectionParameters.IsLoopback)
                {
                    _logger.Error(TAG,"Sending answer in loopback mode.");
                    return;
                }

                var json = JsonConvert.SerializeObject(new
                {
                    sdp = sdp.Sdp,
                    type = "answer"
                });
                _wsClient.Send(json);
            });
        }

        public void SendLocalIceCandidate(IceCandidate candidate)
        {
            _executor.Execute(() =>
            {
                var json = ARDSignalingMessage.CreateJson(candidate);
                if (_initiator)
                {
                    if (State != ConnectionState.Connected)
                    {
                        ReportError("Sending ICE candidate in non connected state.");
                        return;
                    }
                    
                    SendPostMessage(MessageType.Message,_messageUrl,json);
                }
                else
                {
                    _wsClient.Send(json);
                }
            });
        }

        public void SendLocalIceCandidateRemovals(IceCandidate[] candidates)
        {
            _executor.Execute(() =>
            {
                var json = ARDSignalingMessage.CreateJson(candidates);

                if (_initiator)
                {
                    if (State != ConnectionState.Connected)
                    {
                        ReportError("Sending ICE candidate removals in non connected state.");
                        return;
                    }
                    
                    SendPostMessage(MessageType.Message,_messageUrl,json);
                    if (_connectionParameters.IsLoopback)
                    {
                        _signalingEvents.OnRemoteIceCandidatesRemoved(candidates);
                    }
                }
                else
                {
                    _wsClient.Send(json);
                }
            });
        }

        public void OnWebSocketClose()
        {
            _signalingEvents.OnChannelClose();
        }

        public void OnWebSocketMessage(string message)
        {
            if (_wsClient.State != WebSocketConnectionState.Registered)
            {
                _logger.Error(TAG,"Got WebSocket message in non registered state.");
                return;
            }

            var msg = ARDSignalingMessage.MessageFromJSONString(message);

            switch (msg.Type)
            {
                case ARDSignalingMessageType.Candidate:
                    var candidate = (ARDICECandidateMessage) msg;
                    _signalingEvents.OnRemoteIceCandidate(candidate.Candidate);
                    break;
                case ARDSignalingMessageType.CandidateRemoval:
                    var candidates = (ARDICECandidateRemovalMessage) msg;
                    _signalingEvents.OnRemoteIceCandidatesRemoved(candidates.Candidates);
                    break;
                case ARDSignalingMessageType.Offer:
                    if (!_initiator)
                    {
                        var sdp = (ARDSessionDescriptionMessage) msg;
                        _signalingEvents.OnRemoteDescription(sdp.Description);
                    }
                    else
                    {
                        ReportError($"Received offer for call receiver : {message}");
                    }
                    break;
                case ARDSignalingMessageType.Answer:
                    if (_initiator)
                    {
                        var sdp = (ARDSessionDescriptionMessage) msg;
                        _signalingEvents.OnRemoteDescription(sdp.Description);
                    }
                    else
                    {
                        ReportError($"Received answer for call initiator: {message}");
                    }
                    break;
                case ARDSignalingMessageType.Bye:
                    _signalingEvents.OnChannelClose();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnWebSocketError(string description)
        {
            ReportError($"WebSocket error: {description}");
        }

        // Connects to room - function runs on a local looper thread.
        private void ConnectToRoomInternal()
        {
            var connectionUrl = GetConnectionUrl(_connectionParameters);
            _logger.Debug(TAG, $"Connect to room: {connectionUrl}");

            State = ConnectionState.New;
            _wsClient = new WebSocketChannelClientEx(_executor,this,_logger);
            
            var roomParametersFetcher = new RoomParametersFetcher(connectionUrl,null,_logger);
            roomParametersFetcher.MakeRequest((parameters, description) =>
            {
                _executor.Execute(() =>
                {
                    if (description != null)
                    {
                        ReportError(description);
                        return;
                    }

                    SignalingParametersReady(parameters);
                });
            });
        }

        // Disconnect from room and send bye messages - runs on a local looper thread.
        private void DisconnectFromRoomInternal()
        {
            _logger.Debug(TAG, "Disconnect. Room state: " + State);
            if (State == ConnectionState.Connected) {
                _logger.Debug(TAG, "Closing room.");
                SendPostMessage(MessageType.Leave, _leaveUrl, null);
            }
            State = ConnectionState.Closed;
            _wsClient?.Disconnect(true);
        }

        private void SendPostMessage(MessageType messageType, string url, string message)
        {
            var logInfo = url;
            
            if (message != null)
                logInfo += $". Message: {message}";
            
            _logger.Debug(TAG,$"C->GAE: {logInfo}");
            
            var httpConnection = new AsyncHttpURLConnection(MethodType.Post,url,message,((response, errorMessage) =>
            {
                _executor.Execute(() =>
                {
                    if (errorMessage != null)
                    {
                        ReportError($"GAE POST error : {errorMessage}");
                        return;
                    }

                    if (messageType != MessageType.Message) 
                        return;
                    try
                    {
                        var msg = JsonConvert.DeserializeObject<ARDMessageResponse>(response);
                        if (msg.Type != ARDMessageResultType.Success)
                        {
                            ReportError($"GAE POST error : {response}");
                        }
                    }
                    catch (JsonException e)
                    {
                        ReportError($"GAE POST JSON error: {e.Message}");
                    }
                });
            }));
            httpConnection.Send();
        }

        private void SignalingParametersReady(SignalingParameters signalingParameters)
        {
            _logger.Debug(TAG,"Room connection completed.");

            if (_connectionParameters.IsLoopback &&
                (!signalingParameters.IsInitiator || signalingParameters.OfferSdp !=null))
            {
                ReportError("Loopback room is busy.");
                return;
            }

            if (!_connectionParameters.IsLoopback && !signalingParameters.IsInitiator &&
                signalingParameters.OfferSdp == null)
            {
                _logger.Warning(TAG,"No offer SDP in room response.");
            }

            _initiator = signalingParameters.IsInitiator;
            _messageUrl = GetMessageUrl(_connectionParameters, signalingParameters);
            _leaveUrl = GetLeaveUrl(_connectionParameters, signalingParameters);
            
            _logger.Debug(TAG,$"Message URL: {_messageUrl}");
            _logger.Debug(TAG,$"Leave URL: {_leaveUrl}");

            State = ConnectionState.Connected;
            
            _signalingEvents.OnConnectedToRoom(signalingParameters);

            _wsClient.Connect(signalingParameters.WssUrl, signalingParameters.WssPostUrl);
            _wsClient.Register(_connectionParameters.RoomId, signalingParameters.ClientId);
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

        private static string GetConnectionUrl(RoomConnectionParameters connectionParameters)
        {
            return connectionParameters.RoomUrl + "/" + ROOM_JOIN + "/" + connectionParameters.RoomId +
                   GetQueryString(connectionParameters);
        }

        private static string GetMessageUrl(RoomConnectionParameters connectionParameters,
            SignalingParameters signalingParameters)
        {
            return connectionParameters.RoomUrl + "/" + ROOM_MESSAGE + "/" + connectionParameters.RoomId + "/" +
                   signalingParameters.ClientId + GetQueryString(connectionParameters);
        }

        private static string GetLeaveUrl(RoomConnectionParameters connectionParameters,
            SignalingParameters signalingParameters)
        {
            return connectionParameters.RoomUrl + "/" + ROOM_LEAVE + "/" + connectionParameters.RoomId + "/" +
                   signalingParameters.ClientId + GetQueryString(connectionParameters);
        }

        private static string GetQueryString(RoomConnectionParameters roomConnectionParameters)
        {
            return roomConnectionParameters.UrlParameters != null ? $"?{roomConnectionParameters.UrlParameters}" : "";
        }
        

        private enum MessageType
        {
            Message,
            Leave
        }

        public interface ISignalingEventsEx : ISignalingEvents
        {
            void OnConnectedToRoom(SignalingParameters signalingParameters);
        }

        public class RoomConnectionParameters : IConnectionParameters
        {
            public string RoomUrl { get; set; }
            public string RoomId { get; set; }
            
            public bool IsLoopback { get; set; }
            public string UrlParameters { get; set; }
        }

        public class SignalingParameters:ISignalingParameters
        {
            public IceServer[] IceServers { get; set; }
            public bool IsInitiator { get; set; }
            public string ClientId { get; set; }
            public string WssUrl { get; set; }
            public string WssPostUrl { get; set; }
            public SessionDescription OfferSdp { get; set; }
            public IceCandidate[] IceCandidates { get; set; }
        }

        private class WebSocketChannelClientEx : WebSocketChannelClient
        {
            private string _postServerUrl;

            private string _roomId;
            private string _clientId;
            
            
            public WebSocketChannelClientEx(IExecutor executor, IWebSocketChannelEvents events, ILogger logger = null) : base(executor, events, logger)
            {
            }

            public override void Connect(string wsUrl, string postUrl)
            {
                base.Connect(wsUrl,null);
                _postServerUrl = postUrl;
            }
            
            public void Register(string roomId, string clientId)
            {
                CheckIfCalledOnValidThread();
                _roomId = roomId;
                _clientId = clientId;

                if (State != WebSocketConnectionState.Connected)
                {
                    Logger.Debug(TAG,$"WebSocket register() in state {State}");
                    return;
                }
                
                var registerMessage = new
                {
                    cmd = "register",
                    roomid = roomId,
                    clientid = clientId
                };

                var json = JsonConvert.SerializeObject(registerMessage);
                
                
                Logger.Debug(TAG,$"C->WSS: {json}");
                
                WebSocketConnection.Send(json);
                
                State = WebSocketConnectionState.Registered;

                foreach (var sendMessage in WsSendQueue)
                {
                    Send(sendMessage);
                }
                
                WsSendQueue.Clear();
            }

            protected override string GetRegisterMessage(string message)
            {
                 return JsonConvert.SerializeObject(new
                {
                    cmd = "send",
                    msg = message
                });
            }

            protected override void SendByeMessage()
            {
                Send(ARDSignalingMessage.CreateByeJson());
                SendWSSMessage(MethodType.Delete,"");
            }

            protected override void OnConnectionOpen()
            {
                if (_clientId != null && _roomId != null)
                {
                    Register(_roomId,_clientId);
                }
            }

            protected override void OnMessageReceived(string message)
            {

                var webSocketMessage = JsonConvert.DeserializeObject<Dictionary<string,string>>(message);

                if (webSocketMessage.ContainsKey("error"))
                {
                    var error = webSocketMessage["error"];
                    if (!string.IsNullOrEmpty(error))
                    {
                        ReportError($"WebSocket error message: {error}");
                        return;
                    }
                }

                if (!webSocketMessage.TryGetValue("msg", out string msg) || string.IsNullOrEmpty(msg))
                {
                    ReportError($"Unexpected WebSocket message: {message}");
                    return;
                }
                
                
                base.OnMessageReceived(msg);
            }

            private void SendWSSMessage(MethodType method, string message)
            {
                var postUrl = $"{_postServerUrl}/{_roomId}/{_clientId}";
                Logger.Debug(TAG,$"WS {method} : {postUrl} : {message}");
                
                var httpConnection = new AsyncHttpURLConnection(method,postUrl,message,(response, errorMessage) =>
                {
                    if(errorMessage != null)
                        ReportError($"WS {method} error: {errorMessage}");
                });
                
                httpConnection.Send();
            }
        }
    }
}
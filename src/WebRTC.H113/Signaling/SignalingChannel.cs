using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebRTC.H113.Signaling.Models;
using WebRTC.H113.Signaling.WebSocket;

namespace WebRTC.H113.Signaling
{
    public class SignalingChannel : IWebSocketChannelEvents
    {
        private const string TAG = nameof(SignalingChannel);

        private readonly List<SignalingMessage> _queue = new List<SignalingMessage>();

        private readonly ISignalingChannelEvents _signalingChannelEvents;
        private readonly ILogger _logger;
        private readonly WebSocketClient _webSocketClient;

        private TaskCompletionSource<bool> _completionSourceConnect;

        private SignalingChannelState _state;
        private bool _isDrainingQueue;

        private ConnectionParameters _connectionParameters;
        private string _socketId;

        public SignalingChannel(ISignalingChannelEvents signalingChannelEvents,
            ILogger logger = null)
        {
            _signalingChannelEvents = signalingChannelEvents;
            _logger = logger;
            _webSocketClient = new WebSocketClient(this);

            State = SignalingChannelState.New;
        }

        public SignalingChannelState State
        {
            get => _state;
            private set
            {
                if (_state == value)
                    return;
                _logger.Debug(TAG, $"Changing state {_state} -> {value}");
                _state = value;
                _signalingChannelEvents.ChannelDidChangeState(this, value);
            }
        }

        public Task<bool> ConnectAsync(ConnectionParameters connectionParameters)
        {
            if (_completionSourceConnect != null)
                throw new InvalidOperationException("Connect async was called already!");
            _completionSourceConnect = new TaskCompletionSource<bool>();
            _connectionParameters = connectionParameters;
            _logger.Debug(TAG, $"Connecting to WS - {connectionParameters.WsUrl}");
            _webSocketClient.Connect(connectionParameters.WsUrl, connectionParameters.Protocol);
            return _completionSourceConnect.Task;
        }

        public void Close()
        {
            _webSocketClient.Disconnect(true);
        }

        public void SendMessage(SignalingMessage message)
        {
            switch (State)
            {
                case SignalingChannelState.New:
                case SignalingChannelState.Open:
                case SignalingChannelState.Reconnecting:
                    _logger.Debug(TAG, $"WS ACC: {message}");
                    AddToQueue(message);
                    break;
                case SignalingChannelState.Registered:
                    if (!_isDrainingQueue)
                        DrainQueue();
                    _webSocketClient.Send(message.ToJson());
                    break;
                case SignalingChannelState.Closed:
                case SignalingChannelState.Error:
                    _logger.Error(TAG, $"WebSocket send() in error or closed state: {message}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void IWebSocketChannelEvents.OnWebSocketOpen()
        {
            State = SignalingChannelState.Open;
            _logger.Debug(TAG, "Connected to WS");
            _completionSourceConnect.TrySetResult(true);
            _completionSourceConnect = null;

            DrainQueue();
        }
        
        void IWebSocketChannelEvents.OnWebSocketReconnected()
        {
            _logger.Debug(TAG, "Reconnected to WS");
            var message = new ReconnectingMessage(_socketId, _connectionParameters.Phone);
            _logger.Debug(TAG, $"C->WSS: {message}");
            _webSocketClient.Send(message.ToJson());
            State = SignalingChannelState.Registered;
            DrainQueue();
        }
        
        void IWebSocketChannelEvents.OnWebSocketClose(int code, string reason)
        {
            if (code == WebSocketClient.CloseReconnect)
            {
                _logger.Debug(TAG, $"Reconnecting to channel - {reason}");
                State = SignalingChannelState.Reconnecting;
                return;
            }

            State = SignalingChannelState.Closed;
            _signalingChannelEvents.ChannelDidClose(this, code, reason);
        }

        void IWebSocketChannelEvents.OnWebSocketMessage(string message)
        {
            var msg = SignalingMessageFactory.FromJson(message);

            if (msg is RegisteredMessage)
            {
                _socketId = msg.SocketId;
                State = SignalingChannelState.Registered;
            }

            if (msg == null)
                _logger.Warning(TAG, $"Failed to parse ${message}");
            else
                _signalingChannelEvents.ChannelDidReceiveMessage(this, msg);
        }

        void IWebSocketChannelEvents.OnWebSocketError(string description)
        {
            State = SignalingChannelState.Error;
            _signalingChannelEvents.ChannelOnError(this, description);
            if (_completionSourceConnect == null)
                return;
            _logger.Error(TAG, $"Failed to connect to WS - {description}");
            _completionSourceConnect.TrySetResult(false);
            _completionSourceConnect = null;
        }

        private void AddToQueue(SignalingMessage message)
        {
            lock (_queue)
            {
                if (message is RegisterMessage)
                {
                    if (State == SignalingChannelState.New)
                        _queue.Insert(0, message);
                    else
                    {
                        _webSocketClient.Send(message.ToJson());
                        State = SignalingChannelState.Registered;
                    }
                }
                else
                {
                    _queue.Add(message);
                }
            }
        }

        private void DrainQueue()
        {
            lock (_queue)
            {
                if (_queue.Count == 0)
                    return;
                _logger.Debug(TAG, $"Draining queue - {_queue.Count}");
                var items = _queue.ToArray();
                _isDrainingQueue = true;
                foreach (var message in items)
                {
                    SendMessage(message);
                }

                foreach (var message in items)
                {
                    _queue.Remove(message);
                }

                _queue.Clear();
                _isDrainingQueue = false;
            }
        }
    }
}
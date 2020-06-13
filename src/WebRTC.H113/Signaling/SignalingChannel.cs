#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using WebRTC.H113.Schedulers;
using WebRTC.H113.Signaling.Models;
using WebSocketSharp;
using Xamarin.Essentials;

namespace WebRTC.H113.Signaling
{
    public class SignalingChannel
    {
        private const string TAG = nameof(SignalingChannel);

        private readonly List<SignalingMessage> _queue = new List<SignalingMessage>();

        private readonly ISignalingChannelEvents _signalingChannelEvents;
        private readonly IExecutor _executor;
        private readonly ILogger _logger;

        private readonly string _wsUrl;
        private readonly string[] _protocols;

        private WebSocket? _webSocket;

        private SignalingChannelState _state;
        private bool _isDrainingQueue;

        private TaskCompletionSource<bool>? _completionSourceConnect;

        private string? _phone;
        private string? _socketId;

        private bool _active;
        private bool _prevConnected;
        private bool _onCloseCalled;

        public SignalingChannel(ISignalingChannelEvents signalingChannelEvents, string wsUrl, string token,
            ILogger logger = null)
        {
            _signalingChannelEvents = signalingChannelEvents;
            _wsUrl = wsUrl;
            _protocols = new[] {token};
            _logger = logger;

            _executor = ExecutorServiceFactory.MainExecutor;

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

        public Task<bool> ConnectAsync()
        {
            if (_completionSourceConnect != null)
                return _completionSourceConnect.Task;
            _completionSourceConnect = new TaskCompletionSource<bool>();
            _executor.Execute(ConnectInternal);
            return _completionSourceConnect.Task;
        }

        public void Close()
        {
            _executor.Execute(CloseInternal);
        }

        public void SendMessage(SignalingMessage message)
        {
            _executor.Execute(() =>
            {
                if (message is RegisterMessage registerMessage)
                {
                    _phone = registerMessage.PhoneNumber;
                }

                switch (State)
                {
                    case SignalingChannelState.New:
                    case SignalingChannelState.Open:
                    case SignalingChannelState.Reconnecting:
                    case SignalingChannelState.Connecting:
                        _logger.Debug(TAG, $"WS ACC: {message}");
                        AddToQueue(message);
                        break;
                    case SignalingChannelState.Registered:
                        if (!_isDrainingQueue)
                            DrainQueue();
                        _logger.Debug(TAG, $"C->WSS: {message}");
                        _webSocket?.SendAsync(message.ToJson(), _ => { });
                        break;
                    case SignalingChannelState.Closed:
                    case SignalingChannelState.Error:
                        _logger.Error(TAG, $"WebSocket send() in error or closed state: {message}");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        private void ConnectInternal()
        {
            if (_webSocket != null)
            {
                CloseInternal();
            }

            _logger.Debug(TAG, $"Connecting to WS - {_wsUrl}");

            _webSocket = new WebSocket(_wsUrl, _protocols);
            _webSocket.SslConfiguration.EnabledSslProtocols =
                SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

            _webSocket.Log.Level = WebSocketSharp.LogLevel.Trace;
            _webSocket.Log.Output = (log, file) => _logger.Debug("WebSocket", log.ToString());

            WireEvents();

            _active = true;
            _onCloseCalled = false;

            if (State != SignalingChannelState.Reconnecting)
                State = SignalingChannelState.Connecting;

            _webSocket.ConnectAsync();
        }

        private void CloseInternal()
        {
            UnWireEvents();
            _webSocket?.Close();
            _active = false;
            _prevConnected = false;
            _onCloseCalled = true;
        }

        private void WireEvents()
        {
            if (_webSocket == null)
                return;
            _webSocket.OnOpen += WebSocketOnOnOpen;
            _webSocket.OnClose += WebSocketOnOnClose;
            _webSocket.OnError += WebSocketOnOnError;
            _webSocket.OnMessage += WebSocketOnOnMessage;
        }

        private void UnWireEvents()
        {
            if (_webSocket == null)
                return;
            _webSocket.OnOpen -= WebSocketOnOnOpen;
            _webSocket.OnClose -= WebSocketOnOnClose;
            _webSocket.OnError -= WebSocketOnOnError;
            _webSocket.OnMessage -= WebSocketOnOnMessage;
        }

        private void WebSocketOnOnOpen(object? sender, EventArgs e)
        {
            _executor.Execute(() =>
            {
                if (State == SignalingChannelState.Reconnecting && !string.IsNullOrEmpty(_socketId) &&
                    !string.IsNullOrEmpty(_phone))
                {
                    _logger.Debug(TAG, "Reconnected successfully");

                    State = SignalingChannelState.Registered;

                    _webSocket?.SendAsync(new DoReconnectMessage(_phone, _socketId).ToJson(), success =>
                    {
                        _signalingChannelEvents.ChannelDidReconnect(this);
                        DrainQueue();
                    });
                }
                else
                {
                    _prevConnected = true;

                    State = SignalingChannelState.Open;
                    _completionSourceConnect?.TrySetResult(true);
                    DrainQueue();
                }
            });
        }

        private void WebSocketOnOnClose(object? sender, CloseEventArgs e)
        {
            _executor.Execute(() =>
            {
                _logger.Debug(TAG, $"WebSocket close - {e.Code} {e.Reason}");

                var reconnect = false;
                if (!e.WasClean)
                {
                    reconnect = ScheduleReconnect();
                }

                State = reconnect ? SignalingChannelState.Reconnecting : SignalingChannelState.Closed;
                _completionSourceConnect?.TrySetResult(false);

                if (!reconnect)
                    _signalingChannelEvents.ChannelDidClose(this, e.Code, e.Reason);
            });
        }

        private void WebSocketOnOnError(object? sender, ErrorEventArgs e)
        {
            _executor.Execute(() =>
            {
                _logger.Debug(TAG, $"WebSocket error - {e.Message} \n {e.Exception}");
                _signalingChannelEvents.ChannelOnError(this, e.Message);
            });
        }

        private void WebSocketOnOnMessage(object? sender, MessageEventArgs e)
        {
            _executor.Execute(() =>
            {
                if (_onCloseCalled)
                {
                    _logger.Debug(TAG, $"Received message while closing - {e.Data}");
                    return;
                }

                var message = e.Data;
                _logger.Debug(TAG, $"WSS->C: {message}");
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
            });
        }

        private void Reconnect()
        {
            if (_webSocket == null)
            {
                _logger.Debug(TAG, "Reconnecting - false  webSocket == null");
                return;
            }

            _onCloseCalled = false;
            if (_webSocket.ReadyState == WebSocketState.Open || _webSocket.ReadyState == WebSocketState.Connecting)
            {
                _logger.Debug(TAG, $"Reconnecting - false  WebSocket state - {_webSocket.ReadyState}");
                return;
            }

            _logger.Debug(TAG, $"Reconnecting to ${_wsUrl}");
            _webSocket.ConnectAsync();
        }

        private bool ScheduleReconnect()
        {
            if (!_active || !_prevConnected || !Connectivity.ConnectionProfiles.Any())
                return false;
            _logger.Debug(TAG, "Scheduling Reconnect");
            _executor.Execute(async () =>
            {
                await Task.Delay(500);
                Reconnect();
            });
            return true;
        }

        private void AddToQueue(SignalingMessage message)
        {
            lock (_queue)
            {
                switch (message)
                {
                    case RegisterMessage _:
                        if (State == SignalingChannelState.Open)
                        {
                            _webSocket?.Send(message.ToJson());
                            State = SignalingChannelState.Registered;
                        }
                        else
                        {
                            _queue.Insert(0, message);
                        }

                        break;
                    default:
                        _queue.Add(message);
                        break;
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
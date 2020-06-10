using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace WebRTC.H113.Signaling.WebSocket
{
    public class WebSocketClient
    {
        public const int CloseReconnect = 7;

        private const string TAG = nameof(WebSocketClient);

        private const int CloseTimeout = 1000;
        private readonly IWebSocketChannelEvents _events;
        private readonly IWebSocketConnection _webSocketConnection;

        private readonly ILogger _logger;

        private ManualResetEvent _mre;

        private string _wsUrl;
        private string _protocol;

        private bool _active;
        private bool _prevConnected;
        private bool _onCloseCalled;

        private int _retries;

        public WebSocketClient(IWebSocketChannelEvents events, ILogger logger = null)
        {
            _events = events;
            _webSocketConnection = WebSocketConnectionFactory.CreateWebSocketConnection();
            _logger = logger ?? new ConsoleLogger();

            State = WebSocketConnectionState.New;
        }


        public WebSocketConnectionState State { get; private set; }

        public bool IsConnected => _webSocketConnection.IsConnected;

        public void Connect(string wsUrl, string protocol)
        {
            WireEvents();
            _wsUrl = wsUrl;
            _protocol = protocol;
            _active = true;
            _onCloseCalled = false;
            _logger.Debug(TAG, $"Connecting WebSocket to:{wsUrl}. Protocol :{protocol}");
            _webSocketConnection.Open(wsUrl, protocol);
        }

        public void Disconnect(bool waitForComplete)
        {
            UnWireEvents();
            _onCloseCalled = true;
            _active = false;
            _prevConnected = false;
            _logger.Debug(TAG, $"Disconnect WebSocket. State: {State}");
            if (State == WebSocketConnectionState.Connected || State == WebSocketConnectionState.Error)
            {
                if (waitForComplete)
                    _mre = new ManualResetEvent(false);
                _webSocketConnection.Close();
                State = WebSocketConnectionState.Closed;

                if (waitForComplete)
                {
                    try
                    {
                        _mre.WaitOne(CloseTimeout);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(TAG, $"Wait error:{ex}");
                    }
                }
            }

            State = WebSocketConnectionState.Closed;
            _logger.Debug(TAG, "Disconnecting WebSocket done.");
        }

        public void Send(string message)
        {
            switch (State)
            {
                case WebSocketConnectionState.New:
                case WebSocketConnectionState.Connected:
                    _logger.Debug(TAG, $"C->WSS: {message}");
                    _webSocketConnection.Send(message);
                    break;
                case WebSocketConnectionState.Closed:
                case WebSocketConnectionState.Error:
                    _logger.Error(TAG, $"WebSocket send() in error or closed state: {State} -> {message}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnConnectionOpen()
        {
            _retries = 0;
            _onCloseCalled = false;

            if (_prevConnected)
            {
                _events.OnWebSocketReconnected();
                return;
            }

            _prevConnected = true;
            _events.OnWebSocketOpen();
        }

        private void OnMessageReceived(string message)
        {
            _events.OnWebSocketMessage(message);
        }

        private void ReportError(string errorMessage)
        {
            _logger.Error(TAG, errorMessage);
            if (State == WebSocketConnectionState.Error)
                return;
            State = WebSocketConnectionState.Error;
            _events.OnWebSocketError(errorMessage);
        }

        private void WireEvents()
        {
            _webSocketConnection.OnOpened += WebSocketConnectionOnOnOpened;
            _webSocketConnection.OnClosed += WebSocketConnectionOnOnClosed;
            _webSocketConnection.OnError += WebSocketConnectionOnOnError;
            _webSocketConnection.OnMessage += WebSocketConnectionOnOnMessage;
        }

        private void UnWireEvents()
        {
            _webSocketConnection.OnOpened -= WebSocketConnectionOnOnOpened;
            _webSocketConnection.OnClosed -= WebSocketConnectionOnOnClosed;
            _webSocketConnection.OnError -= WebSocketConnectionOnOnError;
            _webSocketConnection.OnMessage -= WebSocketConnectionOnOnMessage;
        }

        private void WebSocketConnectionOnOnOpened(object sender, EventArgs e)
        {
            _logger.Debug(TAG, $"WebSocket connection opened to:{_wsUrl}");
            State = WebSocketConnectionState.Connected;
            OnConnectionOpen();
        }

        private void WebSocketConnectionOnOnMessage(object sender, string e)
        {
            _logger.Debug(TAG, $"WSS->C: {e}");
            if (State == WebSocketConnectionState.Connected && !_onCloseCalled)
                OnMessageReceived(e);
        }

        private void WebSocketConnectionOnOnError(object sender, Exception e)
        {
            ReportError(e.Message);
        }

        private void WebSocketConnectionOnOnClosed(object sender, (int code, string reason) e)
        {
            var (code, reason) = e;
            _logger.Debug(TAG, $"WebSocket connection closed. Code: {code}. Reason: {reason}. State: {State}");
            _mre?.Set();
            var reconnecting = ScheduleReconnect();
            if (State == WebSocketConnectionState.Closed && !reconnecting)
            {
                _events.OnWebSocketClose(code, reason);
                return;
            }
            State = WebSocketConnectionState.Closed;
            _events.OnWebSocketClose(reconnecting ? CloseReconnect : code, reason);
            _onCloseCalled = true;
        }

        private void Reconnect()
        {
            if (IsConnected || string.IsNullOrEmpty(_wsUrl))
            {
                _logger.Debug(TAG, $"Ignore reconnect because IsConnected -> {IsConnected} or wsUrl is null or empty.");
                return;
            }

            _logger.Debug(TAG, $"Reconnecting {_retries - 1}");
            Connect(_wsUrl, _protocol);
        }

        private bool ScheduleReconnect()
        {
            var haveValidConnection = Connectivity.ConnectionProfiles.Any();
            var need = _active && _prevConnected && haveValidConnection && _retries <= 3;
            if (!need)
                return false;
            _logger.Debug(TAG, "Reconnection scheduled");
            _retries++;
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500 * _retries));
                Reconnect();
            });
            return true;
        }
    }
}
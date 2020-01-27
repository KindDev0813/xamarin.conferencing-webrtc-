using System;
using System.Collections.Generic;
using System.Threading;

namespace WebRTC.AppRTC
{
   

    public enum WebSocketConnectionState
    {
        New,
        Connected,
        Registered,
        Closed,
        Error
    }

    public interface IWebSocketChannelEvents
    {
        void OnWebSocketClose();
        void OnWebSocketMessage(string message);
        void OnWebSocketError(string description);
    }
    
    public class WebSocketChannelClient
    {
        private const string TAG = nameof(WebSocketChannelClient);

        private const int CloseTimeout = 1000;


        private readonly List<string> _wsSendQueue = new List<string>();

        private readonly IExecutor _executor;
        private readonly IWebSocketChannelEvents _events;
        private readonly IWebSocketConnection _webSocketConnection;
        private readonly ILogger _logger;

        private ManualResetEvent _mre;
        private string _wsUrl;

        private string _registerMessage;

        public WebSocketChannelClient(IExecutor executor, IWebSocketChannelEvents events, ILogger logger = null)
        {
            _executor = executor;
            _events = events;
            _webSocketConnection = WebSocketConnectionFactory.CreateWebSocketConnection();
            _logger = logger ?? new ConsoleLogger();

            State = WebSocketConnectionState.New;
        }

        public WebSocketConnectionState State { get; private set; }

        public void Connect(string wsUrl, string protocol)
        {
            CheckIfCalledOnValidThread();
            WireEvents();

            _wsUrl = wsUrl;
            _logger.Debug(TAG, $"Connecting WebSocket to:{wsUrl}. Protocol :{protocol}");
            _webSocketConnection.Open(wsUrl, protocol);
        }

        public void Disconnect(bool waitForComplete)
        {
            CheckIfCalledOnValidThread();
            _logger.Debug(TAG, $"Disconnect WebSocket. State: {State}");
            if (State == WebSocketConnectionState.Registered)
            {
                // Send "bye" to WebSocket server.
                State = WebSocketConnectionState.Connected;
            }

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

            UnWireEvents();

            _logger.Debug(TAG, "Disconnecting WebSocket done.");
        }

        public void Send(string message)
        {
            CheckIfCalledOnValidThread();
            switch (State)
            {
                case WebSocketConnectionState.New:
                case WebSocketConnectionState.Connected:
                    _logger.Debug(TAG, $"WS ACC: {message}");
                    _wsSendQueue.Add(message);
                    break;
                case WebSocketConnectionState.Closed:
                case WebSocketConnectionState.Error:
                    _logger.Error(TAG, $"WebSocket send() in error or closed state: {message}");
                    break;
                case WebSocketConnectionState.Registered:
                    _webSocketConnection.Send(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Register(string message)
        {
            CheckIfCalledOnValidThread();
            _registerMessage = message;
            if (State != WebSocketConnectionState.Connected)
            {
                _logger.Warning(TAG, $"WebSocket register() in state {State}");
                return;
            }

            _logger.Debug(TAG, $"C->WSS: {message}");

            State = WebSocketConnectionState.Registered;

            foreach (var sendMessage in _wsSendQueue)
            {
                Send(sendMessage);
            }

            _wsSendQueue.Clear();
            _registerMessage = null;
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
            _executor.Execute(() =>
            {
                State = WebSocketConnectionState.Connected;
                if (_registerMessage != null)
                    Register(_registerMessage);
            });
        }

        private void WebSocketConnectionOnOnMessage(object sender, string e)
        {
            _logger.Debug(TAG, $"WSS->C: {e}");
            _executor.Execute(() =>
            {
                if (State == WebSocketConnectionState.Connected || State == WebSocketConnectionState.Registered)
                    _events.OnWebSocketMessage(e);
            });
        }

        private void WebSocketConnectionOnOnError(object sender, Exception e)
        {
            ReportError(e.Message);
        }

        private void WebSocketConnectionOnOnClosed(object sender, (int code, string reason) e)
        {
            _logger.Debug(TAG, $"WebSocket connection closed. Code: {e.code}. Reason: {e.reason}. State: {State}");
            _mre.Set();
            _executor.Execute(() =>
            {
                if (State == WebSocketConnectionState.Closed)
                    return;
                State = WebSocketConnectionState.Closed;
                _events.OnWebSocketClose();
            });
        }


        private void CheckIfCalledOnValidThread()
        {
            if (!_executor.IsCurrentExecutor)
                throw new InvalidOperationException("WebSocket method is not called on valid executor.");
        }

        private void ReportError(string errorMessage)
        {
            _logger.Error(TAG, errorMessage);
            _executor.Execute(() =>
            {
                if (State == WebSocketConnectionState.Error)
                    return;
                State = WebSocketConnectionState.Error;
                _events.OnWebSocketError(errorMessage);
            });
        }
    }
}
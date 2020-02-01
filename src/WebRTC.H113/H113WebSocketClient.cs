using WebRTC.AppRTC.Abstraction;

namespace WebRTC.H113
{
    public class H113WebSocketClient : WebSocketChannelClientBase
    {
        private const string TAG = nameof(H113WebSocketClient);
        
        private string _registerMessage;

        
        public H113WebSocketClient(IExecutor executor, IWebSocketChannelEvents events, ILogger logger = null) : base(executor, events, logger)
        {
        }
        
        public void Register(string message)
        {
            CheckIfCalledOnValidThread();
            _registerMessage = message;
            if (State != WebSocketConnectionState.Connected)
            {
                Logger.Warning(TAG, $"WebSocket register() in state {State}");
                return;
            }

            Logger.Debug(TAG, $"C->WSS: {message}");

            Send(message);

            State = WebSocketConnectionState.Registered;

            foreach (var sendMessage in WsSendQueue)
            {
                Send(sendMessage);
            }

            WsSendQueue.Clear();
            _registerMessage = null;
        }

        protected override void SendByeMessage()
        {
            //TODO(vali): impl in feature if you use bye message for WS
        }

        protected override void OnConnectionOpen()
        {
            Register(_registerMessage);
        }
    }
}
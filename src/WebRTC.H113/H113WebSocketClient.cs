using Polly;
using WebRTC.AppRTC.Abstraction;

namespace WebRTC.H113
{
    public class H113WebSocketClient : WebSocketChannelClientBase
    {
        private const string TAG = nameof(H113WebSocketClient);
        private bool _didRegister;

        public H113WebSocketClient(IExecutor executor, IWebSocketChannelEvents events, ILogger logger = null) : base(
            executor, events, logger)
        {
        }
        public void Register(RegisterMessage message)
        {
            CheckIfCalledOnValidThread();
            _didRegister = true;
            if (State != WebSocketConnectionState.Connected)
            {
                Logger.Warning(TAG, $"WebSocket register() in state {State}");
            }
            else
            {
                State = WebSocketConnectionState.Registered;
            }
            Send(message);
        }

        public void Send(SignalingMessage message)
        {
            if (message != null)
                Send(message.ToJson());
        }


        protected override void SendByeMessage()
        {
            //TODO(vali): impl in feature if you use bye message for WS
        }

        protected override void OnConnectionOpen()
        {
            if (_didRegister)
            {
                State = WebSocketConnectionState.Registered;
            }

            base.OnConnectionOpen();
        }

        protected override bool ShouldIgnoreDisconnect(int code, string reason)
        {
            return true;
        }
    }
}
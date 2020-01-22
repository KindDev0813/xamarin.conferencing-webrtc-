using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebRTC.Abstraction;

namespace WebRTC.AppRTC
{
    public interface IWebSocketConnection : IDisposable
    {
        event EventHandler OnOpened;
        event EventHandler OnClosed;
        event EventHandler<Exception> OnError;
        event EventHandler<string> OnMessage;

        bool IsOpen { get; }

        void Open(string url, string protocol = null, string authToken = null);
        void Close();
        void Send(string message);
    }


    public class WebSocketClient : SignalingChannel
    {
        private readonly IWebSocketConnection _webSocketConnection;
        private readonly string _url;
        private readonly string _protocol;
        
        private TaskCompletionSource<int> _openTask = new TaskCompletionSource<int>();

        public WebSocketClient(string url, string protocol)
        {
            _webSocketConnection = AppRTC.AppRTCFactory.CreateWebSocketConnection();
            _url = url;
            _protocol = protocol;

            WireEvents();
        }

        public string SocketId { get; private set; }

        public override bool IsOpen => _webSocketConnection.IsOpen;

        public override void Dispose()
        {
            base.Dispose();
            if (_webSocketConnection.IsOpen)
                _webSocketConnection.Close();
            UnWireEvents();
            _webSocketConnection.Dispose();
        }

        public override Task OpenAsync()
        {
            if(IsOpen)
                return Task.CompletedTask;
            _webSocketConnection.Open(_url,_protocol);
            return _openTask.Task;
        }

        public override Task CloseAsync()
        {
            _webSocketConnection.Close();
            return Task.CompletedTask;
        }


        public override void SendMessage(SignalingMessage message)
        {
            message.SocketId = SocketId;
            var json = JsonConvert.SerializeObject(message);

            AppRTC.Logger.Debug(TAG,$"C->WSS:{json}");
            _webSocketConnection.Send(json);
        }

        protected override void OnReceivedMessage(SignalingMessage message)
        {
            base.OnReceivedMessage(message);
            if (message is RegisteredMessage)
            {
                SocketId = message.SocketId;
            }
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

        private void WebSocketConnectionOnOnMessage(object sender, string e)
        {
            OnReceivedMessage(e);
        }

        private void WebSocketConnectionOnOnError(object sender, Exception e)
        {
            _openTask.SetException(e);
            State = SignalingChannelState.Error;
        }

        private void WebSocketConnectionOnOnClosed(object sender, EventArgs e)
        {
            State = SignalingChannelState.Closed;
        }

        private void WebSocketConnectionOnOnOpened(object sender, EventArgs e)
        {
            _openTask.SetResult(0);
            State = SignalingChannelState.Open;
        }
    }

    

    public class WebSocketSignalingChannelLoopback : WebSocketClient, ISignalingChannelListener
    {
        public WebSocketSignalingChannelLoopback(string url, string protocol) : base(url, protocol)
        {
            Listener = new SignalingChannelListenerSchedulerProxy(this, AppRTC.DefaultScheduler);
        }

        public void DidChangeState(SignalingChannel channel, SignalingChannelState state)
        {
        }

        public void DidReceiveMessage(SignalingChannel channel, SignalingMessage message)
        {
            switch (message)
            {
                case SessionDescriptionMessage sendSessionDescriptionMessage:
                    var description = sendSessionDescriptionMessage.Description;
                    var dsc = description.Sdp;
                    dsc = dsc.Replace("offer", "answer");
                    SendMessage(new SessionDescriptionMessage(new SessionDescription(SdpType.Answer, dsc))
                    {
                        MessageType = SignalingMessageType.ReceivedAnswer
                    });
                    break;
                case IceCandidateMessage iceCandidateMessage:
                    SendMessage(iceCandidateMessage);
                    break;
            }
        }
    }
}
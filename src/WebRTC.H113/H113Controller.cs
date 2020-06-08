using System;
using System.Text;
using WebRTC.Abstraction;
using WebRTC.AppRTC.Abstraction;
using WebRTC.H113.Extensions;
using Xamarin.Essentials;

namespace WebRTC.H113
{
    public interface IH113AppRTCEngineEvents : IAppRTCEngineEvents
    {
        //jls
        void ShowNotification(int type, string title, string message);
    }

    public interface IH113SignalingEvents : ISignalingEvents<RegisteredMessage>
    {
        void OnRequestCloseConnection();
    }

    public class H113Controller : AppRTCControllerBase<ConnectionParameters, RegisteredMessage>, IH113SignalingEvents
    {
        private const string TAG = nameof(H113Controller);

        private const string SwitchCameraCommand = "flip cam";
        private const string ActivateAppCommand = "notify";
        
        private readonly IH113AppRTCEngineEvents _events;

        private IDataChannel _dataChannel;
        public H113Controller(IH113AppRTCEngineEvents events, ILogger logger = null) : base(events, logger)
        {
            _events = events;
        }

        protected override bool IsInitiator => true;
        

        //jls
        public void ShowNotification(int type, string title, string message)
        {
            Executor.Execute(() => _events.ShowNotification(type, title, message));
        }

        public void SendLocation(Location location)
        {
            (RTCClient as H113RTCClient)?.UpdateInfoMessage(location);
        }

        public void OnRequestCloseConnection()
        {
            Logger.Debug(TAG, "OnRequestCloseConnection...");
            Executor.Execute(Disconnect);
        }

        public override bool OnIceFailedHandle()
        {
            PeerConnectionClient?.ResetIceConnection();
            return true;
        }

        protected override void OnTearDown()
        {
            if (_dataChannel != null)
                _dataChannel.OnMessage -= DataChannelOnOnMessage;
            base.OnTearDown();
        }

        protected override IAppRTCCClient<ConnectionParameters> CreateClient() => new H113RTCClient(this, Logger);

        protected override PeerConnectionParameters CreatePeerConnectionParameters(
            RegisteredMessage signalingParameters)
        {
            var registeredMessage = signalingParameters;
            return new PeerConnectionParameters(registeredMessage.GetIceServers())
            {
                VideoCallEnabled = true,
                AudioCallEnabled = false
            };
        }

        protected override void OnChannelConnectedInternal(RegisteredMessage signalingParameters)
        {
            PeerConnectionClient?.CreateOffer();
        }

        protected override void OnPeerConnectionCreatedInternal(IPeerConnection peerConnection)
        {
            base.OnPeerConnectionCreatedInternal(peerConnection);
            _dataChannel = peerConnection.CreateDataChannel("sendChannel", new DataChannelConfiguration());
            if (_dataChannel != null)
                _dataChannel.OnMessage += DataChannelOnOnMessage;
        }

        private void DataChannelOnOnMessage(object sender, DataBuffer e)
        {
            var command = Encoding.UTF8.GetString(e.Data);
            switch (command)
            {
                case SwitchCameraCommand:
                    SwitchCamera();
                    break;
                case ActivateAppCommand:
                    ShowNotification(1, "Title", "Message body");
                    break;
            }
        }
    }
}
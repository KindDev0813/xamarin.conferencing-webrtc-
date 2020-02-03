using System;
using System.Text;
using WebRTC.Abstraction;
using WebRTC.AppRTC.Abstraction;
using WebRTC.H113.Extensions;

namespace WebRTC.H113
{
    public class H113Controller : AppRTCControllerBase<ConnectionParameters,RegisteredMessage>
    {
        private const string SwitchCameraCommand = "flip cam";
        
        private IDataChannel _dataChannel;
        public H113Controller(IAppRTCEngineEvents events, ILogger logger = null) : base(events, logger)
        {
        }

        public override void Disconnect()
        {
            if (_dataChannel != null)
                _dataChannel.OnMessage -= DataChannelOnOnMessage;
            base.Disconnect();
        }

        protected override bool IsInitiator => true;
        protected override IAppRTCCClient<ConnectionParameters> CreateClient() => new H113RTCClient(this,Logger);
        
        protected override PeerConnectionParameters CreatePeerConnectionParameters(RegisteredMessage signalingParameters)
        {
            var registeredMessage = signalingParameters;
            return new PeerConnectionParameters(registeredMessage.GetIceServers())
            {
                VideoCallEnabled = true
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
            if(_dataChannel != null)
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
            }
        }
    }
}
using WebRTC.AppRTC.Abstraction;
using WebRTC.H113.Extensions;

namespace WebRTC.H113
{
    public class H113Controller : AppRTCControllerBase<ConnectionParameters,RegisteredMessage>
    {
        public H113Controller(IAppRTCEngineEvents events, ILogger logger = null) : base(events, logger)
        {
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
    }
}
using WebRTC.AppRTC.Abstraction;
using WebRTC.H113.Extensions;

namespace WebRTC.H113
{
    public class H113Engine : AppRTCEngineBase
    {
        public H113Engine(IAppRTCEngineEvents events, ILogger logger = null) : base(events, logger)
        {
        }

        protected override IAppRTCCClient CreateClient() => new H113RTCClient(this, Logger);

        protected override PeerConnectionParameters CreatePeerConnectionParameters(ISignalingParameters signalingParameters)
        {
            var registeredMessage = (RegisteredMessage)signalingParameters;
            return new PeerConnectionParameters(registeredMessage.GetIceServers())
            {
                VideoCallEnabled = true
            };
        }

        protected override void OnChannelConnectedInternal(ISignalingParameters signalingParameters)
        {
        }
        
        
    }
}
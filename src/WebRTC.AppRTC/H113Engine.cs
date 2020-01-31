using WebRTC.AppRTC.Extensions;

namespace WebRTC.AppRTC
{
    public class H113Engine : AppRTCEngineBase
    {
        public H113Engine(IAppRTCEngineEvents events, ILogger logger = null) : base(events, logger)
        {
        }

        public override IAppRTCCClient CreateClient() => new H113RTCClient(this, Logger);

        public override PeerConnectionParameters CreatePeerConnectionParameters(ISignalingParameters signalingParameters)
        {
            var registeredMessage = (RegisteredMessage)signalingParameters;
            return new PeerConnectionParameters(registeredMessage.GetIceServers())
            {
                VideoCallEnabled = true
            };
        }
    }
}
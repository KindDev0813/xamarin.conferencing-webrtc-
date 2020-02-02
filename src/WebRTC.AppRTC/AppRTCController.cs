using WebRTC.AppRTC.Abstraction;

namespace WebRTC.AppRTC
{
    public class AppRTCController : AppRTCControllerBase<RoomConnectionParameters,SignalingParameters>
    {
        public AppRTCController(IAppRTCEngineEvents events, ILogger logger = null) : base(events, logger)
        {
        }

        protected override bool IsInitiator => SignalingParameters.IsInitiator;
        protected override IAppRTCCClient<RoomConnectionParameters> CreateClient() => new AppRTCClient(this);
       
        protected override PeerConnectionParameters CreatePeerConnectionParameters(SignalingParameters signalingParameters)
        {
            var signalingParam = signalingParameters;
            return new PeerConnectionParameters(signalingParam.IceServers)
            {
                VideoCallEnabled = true
            };
        }

        protected override void OnChannelConnectedInternal(SignalingParameters signalingParameters)
        {
            OnConnectedToRoomInternal(signalingParameters);
        }

        private void OnConnectedToRoomInternal(SignalingParameters signalingParameters)
        {
            if (signalingParameters.IsInitiator)
            {
                PeerConnectionClient.CreateOffer();
            }
            else
            {
                if (signalingParameters.OfferSdp != null)
                {
                    PeerConnectionClient.SetRemoteDescription(signalingParameters.OfferSdp);
                    PeerConnectionClient.CreateAnswer();
                }

                if (signalingParameters.IceCandidates != null)
                {
                    foreach (var candidate in signalingParameters.IceCandidates)
                    {
                        PeerConnectionClient.AddRemoteIceCandidate(candidate);
                    }
                }
            }
        }
    }
}
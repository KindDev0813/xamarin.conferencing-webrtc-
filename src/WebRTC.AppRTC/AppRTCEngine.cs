using WebRTC.AppRTC.Abstraction;

namespace WebRTC.AppRTC
{
    public class AppRTCEngine : AppRTCEngineBase
    {
        public AppRTCEngine(IAppRTCEngineEvents events, ILogger logger = null) : base(events, logger)
        {
        }

        protected override IAppRTCCClient CreateClient() => new AppRTCClient(this, Logger);


        protected override PeerConnectionParameters CreatePeerConnectionParameters(
            ISignalingParameters signalingParameters)
        {
            var signalingParam = (SignalingParameters) signalingParameters;
            return new PeerConnectionParameters(signalingParam.IceServers)
            {
                VideoCallEnabled = true
            };
        }

        protected override void OnChannelConnectedInternal(ISignalingParameters signalingParameters)
        {
            OnConnectedToRoomInternal((SignalingParameters) signalingParameters);
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
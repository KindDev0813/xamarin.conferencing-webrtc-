namespace WebRTC.AppRTC
{
    public class AppRTCEngine : AppRTCEngineBase, AppRTCClient.ISignalingEventsEx
    {
        public AppRTCEngine(IAppRTCEngineEvents events, ILogger logger = null) : base(events, logger)
        {
        }

        public override IAppRTCCClient CreateClient() => new AppRTCClient(this, Logger);


        public override PeerConnectionParameters CreatePeerConnectionParameters(
            ISignalingParameters signalingParameters)
        {
            var signalingParam = (AppRTCClient.SignalingParameters) signalingParameters;
            return new PeerConnectionParameters(signalingParam.IceServers)
            {
                VideoCallEnabled = true
            };
        }

        public void OnConnectedToRoom(AppRTCClient.SignalingParameters signalingParameters)
        {
            Executor.Execute(() => OnConnectedToRoomInternal(signalingParameters));
        }

        private void OnConnectedToRoomInternal(AppRTCClient.SignalingParameters signalingParameters)
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
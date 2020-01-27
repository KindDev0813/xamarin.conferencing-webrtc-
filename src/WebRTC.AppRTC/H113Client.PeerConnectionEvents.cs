using WebRTC.Abstraction;

namespace WebRTC.AppRTC
{
    public partial class H113Client : IPeerConnectionEvents
    {
        IVideoCapturer IPeerConnectionEvents.CreateVideoCapturer(IPeerConnectionFactory factory,
            IVideoSource videoSource) => _events.CreateVideoCapturer(factory, videoSource);

        void IPeerConnectionEvents.OnPeerFactoryCreated(IPeerConnectionFactory factory)
        {
            _events.OnPeerFactoryCreated(factory);
        }

        void IPeerConnectionEvents.OnConnected()
        {
        }

        void IPeerConnectionEvents.OnDisconnected()
        {
            _events.OnDisconnect(DisconnectType.PeerConnection);
        }

        void IPeerConnectionEvents.OnLocalDescription(SessionDescription sdp)
        {
            _executor.Execute(() =>
            {
                _rtcClient?.SendOfferSdp(sdp);
            });
        }

        void IPeerConnectionEvents.OnIceCandidate(IceCandidate candidate)
        {
            _executor.Execute(() =>
            {
                _rtcClient?.SendLocalIceCandidate(candidate);
            });
        }

        void IPeerConnectionEvents.OnIceCandidateRemoved(IceCandidate[] candidates)
        {
            _executor.Execute(() =>
            {
                _rtcClient?.SendLocalIceCandidateRemovals(candidates);
            });
        }

        void IPeerConnectionEvents.OnIceConnected()
        {
            
        }

        void IPeerConnectionEvents.OnIceDisconnected()
        {
        }

        void IPeerConnectionEvents.OnPeerConnectionClosed()
        {
        }

        void IPeerConnectionEvents.OnPeerConnectionError(string description)
        {
            _events.OnError(description);
        }
    }
}
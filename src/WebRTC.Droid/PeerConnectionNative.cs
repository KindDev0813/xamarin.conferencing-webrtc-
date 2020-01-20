using WebRTC.Abstraction;

namespace WebRTC.Droid
{
    public class PeerConnectionNative : IPeerConnection
    {
        private readonly Org.Webrtc.PeerConnection _peerConnection;

        public PeerConnectionNative(Org.Webrtc.PeerConnection peerConnection)
        {
            _peerConnection = peerConnection;
        }
    }
}
using WebRTC.Abstraction;

namespace WebRTC.AppRTC
{
    public class WebSocketRTCClient : IAppRTCCClient
    {
        public enum ConnectionState
        {
            New,
            Connected,
            Closed,
            Error
        }
        
        public void Connect()
        {
            
        }

        public void Disconnect()
        {
        }

        public void SendOfferSdp(SessionDescription sdp)
        {
        }

        public void SendAnswerSdp(SessionDescription sdp)
        {
        }

        public void SendLocalIceCandidate(IceCandidate candidate)
        {
        }

        public void SendLocalIceCandidateRemovals(IceCandidate[] candidates)
        {
        }
    }
}
using WebRTC.Abstraction;

namespace WebRTC.AppRTC
{
    public interface IAppRTCCClient
    {
        void Connect();

        void Disconnect();
        
        void SendOfferSdp(SessionDescription sdp);
        void SendAnswerSdp(SessionDescription sdp);

        void SendLocalIceCandidate(IceCandidate candidate);
        void SendLocalIceCandidateRemovals(IceCandidate[] candidates);
    }

    public interface ISignalingEvents
    {
        void OnChannelConnected();
        void OnChannelError(string description);

        void OnRemoteDescription(SessionDescription sdp);
        void OnRemoteIceCandidate(IceCandidate candidate);
        void OnRemoteIceCandidatesRemoved(IceCandidate[] candidates);
    }
}
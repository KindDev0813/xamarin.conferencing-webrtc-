using WebRTC.Abstraction;

namespace WebRTC.AppRTC.Abstraction
{
    public enum ConnectionState
    {
        New,
        Connected,
        Closed,
        Error
    }
    
    public interface IAppRTCCClient
    {
        
        ConnectionState State { get; }
        
        
        void Connect(IConnectionParameters connectionParameters);

        void Disconnect();
        
        void SendOfferSdp(SessionDescription sdp);

        void SendAnswerSdp(SessionDescription sdp);
        
        void SendLocalIceCandidate(IceCandidate candidate);
        void SendLocalIceCandidateRemovals(IceCandidate[] candidates);
    }
    
    

    public interface ISignalingEvents
    {
        void OnChannelConnected(ISignalingParameters signalingParameters);

        void OnChannelClose();
        void OnChannelError(string description);

        void OnRemoteDescription(SessionDescription sdp);
        void OnRemoteIceCandidate(IceCandidate candidate);
        void OnRemoteIceCandidatesRemoved(IceCandidate[] candidates);
    }

    public interface IConnectionParameters
    {
        
    }

    public interface ISignalingParameters
    {
        
    }
    
}
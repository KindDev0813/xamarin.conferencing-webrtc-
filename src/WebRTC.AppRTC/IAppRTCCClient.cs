using WebRTC.Abstraction;

namespace WebRTC.AppRTC
{
    public interface IAppRTCCClient
    {
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
    
    public class ConnectionParameters : IConnectionParameters
    {
        public ConnectionParameters(string wsUrl, string protocol, string phone, double lat, double log)
        {
            WsUrl = wsUrl;
            Protocol = protocol;
            Phone = phone;
            Lat = lat;
            Log = log;
        }

        public string WsUrl { get; }
        public string Protocol { get; }

        public string Phone { get; }
        public double Lat { get; }
        public double Log { get; }
    }
}
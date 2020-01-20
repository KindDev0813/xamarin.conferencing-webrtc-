namespace WebRTC.Abstraction
{
    public interface IPeerConnectionFactory
    {
        IPeerConnection CreatePeerConnection(RTCConfiguration configuration,IPeerConnectionListener peerConnectionListener);
    }
}
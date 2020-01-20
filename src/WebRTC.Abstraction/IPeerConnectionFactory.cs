namespace WebRTC.Abstraction
{
    public interface IPeerConnectionFactory : INativeObject
    {
        IPeerConnection CreatePeerConnection(RTCConfiguration configuration,
            IPeerConnectionListener peerConnectionListener);
    }
}
using WebRTC.Abstraction;

namespace WebRTC.Droid
{
    public class NativeWebRTCFactory : INativeWebRTCFactory
    {
        public IPeerConnectionFactory CreateFactory() => new PeerConnectionFactory();
    }
}
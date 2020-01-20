using WebRTC.Abstraction;

namespace WebRTC.Droid
{
    public class WebRTCFactory : IWebRTCFactory
    {
        public IPeerConnectionFactory CreateFactory() => new PeerConnectionFactory();
    }
}
using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.Droid.Extensions;

namespace WebRTC.Droid
{
    public class PeerConnectionFactory : IPeerConnectionFactory
    {
        private readonly Org.Webrtc.PeerConnectionFactory _factory;

        public PeerConnectionFactory()
        {
            EglBase = Org.Webrtc.EglBase.Create();
            NativeObject = _factory = Org.Webrtc.PeerConnectionFactory
                .NewBuilder()
                .CreatePeerConnectionFactory();
        }

        public object NativeObject { get; }
        public IEglBase EglBase { get; }

        public IPeerConnection CreatePeerConnection(RTCConfiguration configuration,
            IPeerConnectionListener peerConnectionListener)
        {
            return new PeerConnectionNative(_factory.CreatePeerConnection(configuration.ToNative(),
                new PeerConnectionListenerProxy(peerConnectionListener)));
        }
    }
}
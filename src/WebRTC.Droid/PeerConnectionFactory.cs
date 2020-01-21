using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.Droid.Extensions;
using MediaConstraints = WebRTC.Abstraction.MediaConstraints;

namespace WebRTC.Droid
{
    public class PeerConnectionFactory : NativeObjectBase, IPeerConnectionFactory
    {
        private readonly Org.Webrtc.PeerConnectionFactory _factory;

        private PeerConnectionFactory(Org.Webrtc.PeerConnectionFactory factory) : base(factory)
        {
            _factory = factory;
        }

        public PeerConnectionFactory() : this(CreateFactory())
        {
            EglBase = Org.Webrtc.EglBase.Create();
        }


        public IEglBase EglBase { get; }

        public IPeerConnection CreatePeerConnection(RTCConfiguration configuration,
            IPeerConnectionListener peerConnectionListener)
        {
            return new PeerConnectionNative(_factory.CreatePeerConnection(configuration.ToNative(),
                new PeerConnectionListenerProxy(peerConnectionListener)));
        }

        public IAudioSource CreateAudioSource(MediaConstraints mediaConstraints)
        {
            return new AudioSourceNative(_factory.CreateAudioSource(mediaConstraints.ToNative()));
        }

        public IAudioTrack CreateAudioTrack(string id, IAudioSource audioSource)
        {
            return new AudioTrackNative(_factory.CreateAudioTrack(id, audioSource.ToNative<AudioSource>()));
        }

        public IVideoTrack CreateVideoTrack(string id, IVideoSource videoSource)
        {
            return new VideoTrackNative(_factory.CreateVideoTrack(id, videoSource.ToNative<VideoSource>()));
        }

        private static Org.Webrtc.PeerConnectionFactory CreateFactory()
        {
            return Org.Webrtc.PeerConnectionFactory
                .NewBuilder()
                .CreatePeerConnectionFactory();
        }
    }
}
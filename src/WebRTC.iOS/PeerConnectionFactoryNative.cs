using Foundation;
using WebRTC.Abstraction;
using WebRTC.iOS.Extensions;
using WebRTC.iOS.Binding;


namespace WebRTC.iOS
{
    internal class PeerConnectionFactoryNative : NativeObjectBase, IPeerConnectionFactory
    {
        private readonly RTCPeerConnectionFactory _factory;

        public PeerConnectionFactoryNative()
        {
            var decoderFactory = new RTCDefaultVideoDecoderFactory();
            var encoderFactory = new RTCDefaultVideoEncoderFactory();

            NativeObject = _factory = new RTCPeerConnectionFactory(encoderFactory, decoderFactory);
        }

        public IPeerConnection CreatePeerConnection(Abstraction.RTCConfiguration configuration,
            IPeerConnectionListener peerConnectionListener)
        {
            var rtcConfiguration = configuration.ToNative();
            var constraints = new RTCMediaConstraints(null,
                new NSDictionary<NSString, NSString>(new NSString("DtlsSrtpKeyAgreement"),
                    new NSString(configuration.EnableDtlsSrtp ? "false" : "true")));

            var peerConnection = _factory.PeerConnectionWithConfiguration(rtcConfiguration, constraints,
                new PeerConnectionListenerProxy(peerConnectionListener));
            if (peerConnection == null)
                return null;
            return new PeerConnectionNative(peerConnection,configuration, this);
        }

        public IAudioSource CreateAudioSource(MediaConstraints mediaConstraints)
        {
            var audioSource = _factory.AudioSourceWithConstraints(mediaConstraints.ToNative());
            if (audioSource == null)
                return null;
            return new AudioSourceNative(audioSource);
        }

        public IAudioTrack CreateAudioTrack(string id, IAudioSource audioSource)
        {
            var audioTrack = _factory.AudioTrackWithSource(audioSource.ToNative<RTCAudioSource>(), id);
            if (audioTrack == null)
                return null;
            return new AudioTrackNative(audioTrack);
        }

        public IVideoSource CreateVideoSource(bool isScreencast) => new VideoSourceNative(_factory.VideoSource);

        public IVideoTrack CreateVideoTrack(string id, IVideoSource videoSource)
        {
            var videoTrack = _factory.VideoTrackWithSource(videoSource.ToNative<RTCVideoSource>(), id);
            if (videoTrack == null)
                return null;
            return new VideoTrackNative(videoTrack);
        }

        public ICameraVideoCapturer CreateCameraCapturer(IVideoSource videoSource, bool frontCamera)
        {
            var capturer = new RTCCameraVideoCapturer(videoSource.ToNative<RTCVideoSource>());
            return new CameraVideoCapturerNative(capturer, frontCamera);
        }

        public IFileVideoCapturer CreateFileCapturer(IVideoSource videoSource, string file)
        {
            var capturer = new RTCFileVideoCapturer(videoSource.ToNative<RTCVideoSource>());
            return new FileVideoCapturerNative(capturer, file);
        }

        public bool StartAecDump(string file, int fileSizeLimitBytes)
        {
            return _factory.StartAecDumpWithFilePath(file, fileSizeLimitBytes);
        }

        public void StopAecDump()
        {
            _factory.StopAecDump();
        }
    }
}
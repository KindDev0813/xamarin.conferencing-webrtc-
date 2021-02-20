using Foundation;
using WebRTC.Abstraction;
using WebRTC.Abstraction.Extensions;
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

            var peerConnectionDelegate = new PeerConnectionListenerProxy(peerConnectionListener);

            var peerConnection = _factory.PeerConnectionWithConfiguration(rtcConfiguration, constraints,
                peerConnectionDelegate);
            return peerConnection == null ? null : new PeerConnectionNative(peerConnection, configuration, this,peerConnectionDelegate);
        }

        public IAudioSource CreateAudioSource(MediaConstraints mediaConstraints)
        {
            var audioSource = _factory.AudioSourceWithConstraints(mediaConstraints.ToNative());
            return audioSource == null ? null : new AudioSourceNative(audioSource);
        }

        public IAudioTrack CreateAudioTrack(string id, IAudioSource audioSource)
        {
            var audioTrack = _factory.AudioTrackWithSource(audioSource.ToNative<RTCAudioSource>(), id);
            return audioTrack == null ? null : new AudioTrackNative(audioTrack);
        }

        public IVideoSource CreateVideoSource(bool isScreencast) => new VideoSourceNative(_factory.VideoSource);

        public IVideoTrack CreateVideoTrack(string id, IVideoSource videoSource)
        {
            var videoTrack = _factory.VideoTrackWithSource(videoSource.ToNative<RTCVideoSource>(), id);
            return videoTrack == null ? null : new VideoTrackNative(videoTrack);
        }

        public ICameraVideoCapturer CreateCameraCapturer(IVideoSource videoSource, bool frontCamera)
        {
            var capturer = new RTCCameraVideoCapturer(videoSource.ToNative<RTCVideoSource>());
            return new CameraVideoCapturerNative(capturer, frontCamera);
        }

        public ICameraVideoCapturer CreateCameraCapturer(IVideoSource videoSource, RTCCameraDevice cameraDevice)
        {
            var capturer = new RTCCameraVideoCapturer(videoSource.ToNative<RTCVideoSource>());
            return new CameraVideoCapturerNative(capturer, cameraDevice);
        }

        public IFileVideoCapturer CreateFileCapturer(IVideoSource videoSource, string file)
        {
            return new FileVideoCapturer(videoSource, file);
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
namespace WebRTC.Abstraction
{
    public interface IPeerConnectionFactory : INativeObject
    {
        /// <summary>
        /// All callback will be on UI thread
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="peerConnectionListener"></param>
        /// <returns></returns>
        IPeerConnection CreatePeerConnection(RTCConfiguration configuration,
            IPeerConnectionListener peerConnectionListener);

        IAudioSource CreateAudioSource(MediaConstraints mediaConstraints);

        IAudioTrack CreateAudioTrack(string id, IAudioSource audioSource);

        IVideoSource CreateVideoSource(bool isScreencast);

        IVideoTrack CreateVideoTrack(string id, IVideoSource videoSource);

        ICameraVideoCapturer CreateCameraCapturer(IVideoSource videoSource,bool frontCamera);

        IFileVideoCapturer CreateFileCapturer(IVideoSource videoSource,string file);
    }
}
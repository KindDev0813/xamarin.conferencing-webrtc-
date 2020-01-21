namespace WebRTC.Abstraction
{
    public interface IPeerConnectionFactory : INativeObject
    {
        IPeerConnection CreatePeerConnection(RTCConfiguration configuration,
            IPeerConnectionListener peerConnectionListener);

        IAudioSource CreateAudioSource(MediaConstraints mediaConstraints);

        IAudioTrack CreateAudioTrack(string id, IAudioSource audioSource);

        IVideoTrack CreateVideoTrack(string id, IVideoSource videoSource);
    }
}
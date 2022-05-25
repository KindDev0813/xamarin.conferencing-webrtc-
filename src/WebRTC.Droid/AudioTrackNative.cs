using WebRTC.Abstraction;
using Org.Webrtc;

namespace WebRTC.Droid
{
    internal class AudioTrackNative : MediaStreamTrackNative, IAudioTrack
    {
        private readonly AudioTrack _audioTrack;

        public AudioTrackNative(AudioTrack audioTrack) : base(audioTrack)
        {
            _audioTrack = audioTrack;
        }

        public float Volume
        {
            get => 0f;
            set => _audioTrack.SetVolume(value);
        }
    }
}
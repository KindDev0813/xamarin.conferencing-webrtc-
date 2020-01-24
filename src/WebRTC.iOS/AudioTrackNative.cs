using System;
using WebRTC.Abstraction;

namespace WebRTC.iOS
{
    internal class AudioTrackNative :MediaStreamTrackNative, IAudioTrack
    {
        private readonly RTCAudioTrack _audioTrack;
        public AudioTrackNative(RTCAudioTrack audioTrack):base(audioTrack)
        {
            _audioTrack = audioTrack;
        }

        public float Volume
        {
            get => (float) _audioTrack.Source.Volume;
            set => _audioTrack.Source.Volume = value;
        }
    }
}
using WebRTC.Abstraction;

namespace WebRTC.iOS
{
    internal class AudioSourceNative : MediaSourceNative, IAudioSource
    {
        public AudioSourceNative(RTCAudioSource audioSource) : base(audioSource)
        {
        }
    }
}
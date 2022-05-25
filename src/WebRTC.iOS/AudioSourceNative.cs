using WebRTC.Abstraction;
using WebRTC.iOS.Binding;

namespace WebRTC.iOS
{
    internal class AudioSourceNative : MediaSourceNative, IAudioSource
    {
        public AudioSourceNative(RTCAudioSource audioSource) : base(audioSource)
        {
        }
    }
}
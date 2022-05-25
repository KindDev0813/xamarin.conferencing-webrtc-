using Org.Webrtc;
using WebRTC.Abstraction;

namespace WebRTC.Droid
{
    internal class AudioSourceNative : MediaSourceNative,IAudioSource
    {
        public AudioSourceNative(AudioSource audioSource) : base(audioSource)
        {
        }
    }
}
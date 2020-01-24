using WebRTC.Abstraction;

namespace WebRTC.iOS.Extensions
{
    internal static class MediaConstraintsExtensions
    {
        public static RTCMediaConstraints ToNative(this MediaConstraints self)
        {
            return new RTCMediaConstraints(self.Mandatory.ToNative(), self.Optional.ToNative());
        }
    }
}
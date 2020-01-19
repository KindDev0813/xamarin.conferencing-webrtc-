using Org.Webrtc;

namespace WebRTC.Droid.Extensions
{
    public static class IceCandidateExtension
    {
        public static IceCandidate ToNative(this Common.IceCandidate self)
        {
            return new IceCandidate(self.SdpMid,self.SdpMLineIndex,self.Sdp);
        }

        public static Common.IceCandidate ToNet(this IceCandidate self)
        {
            return new Common.IceCandidate(self.Sdp,self.SdpMid,self.SdpMLineIndex);
        }
    }
}
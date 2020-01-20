using IceCandidate = Org.Webrtc.IceCandidate;

namespace WebRTC.Droid.Extensions
{
    public static class IceCandidateExtension
    {
        public static IceCandidate ToNative(this Abstraction.IceCandidate self)
        {
            return new IceCandidate(self.SdpMid,self.SdpMLineIndex,self.Sdp);
        }

        public static Abstraction.IceCandidate ToNet(this IceCandidate self)
        {
            return new Abstraction.IceCandidate(self.Sdp,self.SdpMid,self.SdpMLineIndex);
        }
    }
}
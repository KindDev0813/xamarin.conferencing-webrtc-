using WebRTC.Abstraction;
using Org.Webrtc;

namespace WebRTC.Droid.Extensions
{
    internal static class RTCCertificateExtension
    {
        public static RtcCertificatePem ToNative(this RTCCertificate self) =>
            new RtcCertificatePem(self.PrivateKey, self.Certificate);

        public static RTCCertificate ToNet(this RtcCertificatePem self) =>
            new RTCCertificate(self.PrivateKey, self.Certificate);
    }
}
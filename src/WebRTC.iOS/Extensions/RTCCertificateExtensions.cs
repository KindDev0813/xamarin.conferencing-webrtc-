using WebRTC.iOS.Binding;

namespace WebRTC.iOS.Extensions
{
    internal static class RTCCertificateExtensions
    {
        public static RTCCertificate ToNative(this Abstraction.RTCCertificate self) =>
            new RTCCertificate(self.PrivateKey, self.Certificate);

        public static Abstraction.RTCCertificate ToNet(this RTCCertificate self) =>
            new Abstraction.RTCCertificate(self.Private_key, self.Certificate);
    }
}
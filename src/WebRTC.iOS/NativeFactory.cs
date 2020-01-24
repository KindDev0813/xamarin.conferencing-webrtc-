using Foundation;
using WebRTC.Abstraction;
using WebRTC.iOS.Extensions;

namespace WebRTC.iOS
{
    public class NativeFactory : INativeFactory
    {
        public IPeerConnectionFactory CreatePeerConnectionFactory() => new PeerConnectionFactoryNative();

        public WebRTC.Abstraction.RTCCertificate GenerateCertificate(EncryptionKeyType keyType, long expires)
        {
            return RTCCertificate.GenerateCertificateWithParams(new NSDictionary<NSString, NSObject>(
                new[] {"expires".ToNative(), "name".ToNative()},
                new NSObject[] {new NSNumber(expires), keyType.ToStringNative()}
            )).ToNet();
        }
    }
}
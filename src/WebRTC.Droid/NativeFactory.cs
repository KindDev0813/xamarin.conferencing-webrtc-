using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.Droid.Extensions;

namespace WebRTC.Droid
{
    public class NativeFactory : INativeFactory
    {
        public IPeerConnectionFactory CreatePeerConnectionFactory() => new PeerConnectionFactoryNative();

        public RTCCertificate GenerateCertificate(EncryptionKeyType keyType, long expires) =>
            RtcCertificatePem.GenerateCertificate(keyType.ToNative(), expires).ToNet();
    }
}
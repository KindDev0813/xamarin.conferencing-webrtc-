using Android.Content;
using WebRTC.Abstraction;
using Org.Webrtc;
using WebRTC.Droid.Extensions;
using PeerConnectionFactory = Org.Webrtc.PeerConnectionFactory;

namespace WebRTC.Droid
{
    internal class NativeFactory : INativeFactory
    {
        private readonly Context _context;

        public NativeFactory(Context context)
        {
            _context = context;
        }

        public IPeerConnectionFactory CreatePeerConnectionFactory() => new PeerConnectionFactoryNative(_context);

        public RTCCertificate GenerateCertificate(EncryptionKeyType keyType, long expires) =>
            RtcCertificatePem.GenerateCertificate(keyType.ToNative(), expires).ToNet();


        public void StopInternalTracingCapture()
        {
            PeerConnectionFactory.StopInternalTracingCapture();
        }

        public void ShutdownInternalTracer()
        {
            PeerConnectionFactory.ShutdownInternalTracer();
        }
    }
}
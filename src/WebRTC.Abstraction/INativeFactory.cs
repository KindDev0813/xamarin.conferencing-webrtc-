using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("WebRTC.Droid")]
[assembly: InternalsVisibleTo("WebRTC.iOS")]
namespace WebRTC.Abstraction
{
    internal interface INativeFactory
    {
        IPeerConnectionFactory CreatePeerConnectionFactory();
        RTCCertificate GenerateCertificate(EncryptionKeyType keyType, long expires);
        
        void StopInternalTracingCapture();
        void ShutdownInternalTracer();
    }
}
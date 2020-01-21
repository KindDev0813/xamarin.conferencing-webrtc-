namespace WebRTC.Abstraction
{
    public interface INativeFactory 
    {
        IPeerConnectionFactory CreatePeerConnectionFactory();
        
        RTCCertificate GenerateCertificate(EncryptionKeyType keyType, long expires);
    }
}
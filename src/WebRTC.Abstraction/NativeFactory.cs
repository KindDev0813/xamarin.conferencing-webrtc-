namespace WebRTC.Abstraction
{
    public static class NativeFactory
    {
        private static INativeFactory _factory;

        public static RTCCertificate GenerateCertificate(EncryptionKeyType keyType, long expires) =>
            _factory.GenerateCertificate(keyType, expires);

        public static IPeerConnectionFactory CreatePeerConnectionFactory() => _factory.CreatePeerConnectionFactory();

        public static void Init(INativeFactory factory)
        {
            _factory = factory;
        }
    }
}
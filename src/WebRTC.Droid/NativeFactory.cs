using System.Diagnostics.CodeAnalysis;
using Android.Content;
using Android.Util;
using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.Droid.Extensions;

namespace WebRTC.Droid
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class NativeFactory : INativeFactory
    {
        public const string DISABLE_WEBRTC_AGC_FIELDTRIAL = "WebRTC-Audio-MinimizeResamplingOnMobile/Enabled/";

        public const string VIDEO_FLEXFEC_FIELDTRIAL =
            "WebRTC-FlexFEC-03-Advertised/Enabled/WebRTC-FlexFEC-03/Enabled/";

        private readonly Context _context;

        public NativeFactory(Context context)
        {
            _context = context;
            PeerConnectionFactory.Initialize(
                PeerConnectionFactory.InitializationOptions.InvokeBuilder(context)
                .CreateInitializationOptions());
            
        }


        public IPeerConnectionFactory CreatePeerConnectionFactory() => new PeerConnectionFactoryNative(_context);

        public RTCCertificate GenerateCertificate(EncryptionKeyType keyType, long expires) =>
            RtcCertificatePem.GenerateCertificate(keyType.ToNative(), expires).ToNet();

        public static void Init(Context context)
        {
            WebRTC.Abstraction.NativeFactory.Init(new NativeFactory(context));
        }
    }
}
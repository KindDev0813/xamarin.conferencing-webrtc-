using System.Linq;
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
            CameraDevices = GetSupportedCameraDevices(context);
        }

        public IPeerConnectionFactory CreatePeerConnectionFactory() => new PeerConnectionFactoryNative(_context);

        public RTCCertificate GenerateCertificate(EncryptionKeyType keyType, long expires) =>
            RtcCertificatePem.GenerateCertificate(keyType.ToNative(), expires).ToNet();

        public RTCCameraDevice[] CameraDevices { get; }


        public void StopInternalTracingCapture()
        {
            PeerConnectionFactory.StopInternalTracingCapture();
        }

        public void ShutdownInternalTracer()
        {
            PeerConnectionFactory.ShutdownInternalTracer();
        }

        private static RTCCameraDevice[] GetSupportedCameraDevices(Context context)
        {
            var allCameras = context.GetAllCameras();

            return allCameras.GetDeviceNames()
                .Select(camera => new RTCCameraDevice(camera, allCameras.IsFrontFacing(camera))).ToArray();
        }
    }
}
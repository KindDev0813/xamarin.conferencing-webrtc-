using System.Linq;
using AVFoundation;
using Foundation;
using ObjCRuntime;
using WebRTC.iOS.Extensions;
using WebRTC.Abstraction;
using WebRTC.iOS.Binding;

namespace WebRTC.iOS
{
    internal class NativeFactory : INativeFactory
    {
        public NativeFactory(bool mockSimulatorCamera)
        {
            CameraDevices = GetSupportedCameraDevices(mockSimulatorCamera);
        }

        public IPeerConnectionFactory CreatePeerConnectionFactory() => new PeerConnectionFactoryNative();

        public Abstraction.RTCCertificate GenerateCertificate(EncryptionKeyType keyType, long expires)
        {
            return WebRTC.iOS.Binding.RTCCertificate.GenerateCertificateWithParams(new NSDictionary<NSString, NSObject>(
                new[] {"expires".ToNative(), "name".ToNative()},
                new NSObject[] {new NSNumber(expires), keyType.ToStringNative()}
            )).ToNet();
        }

        public RTCCameraDevice[] CameraDevices { get; }

        public void ShutdownInternalTracer()
        {
            RTCTracing.RTCShutdownInternalTracer();
        }

        public void StopInternalTracingCapture()
        {
            RTCTracing.RTCStopInternalCapture();
        }

        private static RTCCameraDevice[] GetSupportedCameraDevices(bool mockSimulatorCamera)
        {
            if (Runtime.Arch == Arch.SIMULATOR && mockSimulatorCamera)
            {
                return new[] {new RTCCameraDevice("1", false)};
            }

            return RTCCameraVideoCapturer.CaptureDevices.Select(i =>
                    new RTCCameraDevice(i.LocalizedName, i.Position == AVCaptureDevicePosition.Front, i.Description))
                .ToArray();
        }
    }
}
using Android.Content;
using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.AppRTC;
using WebRTC.Droid.Extensions;
using IVideoCapturer = WebRTC.Abstraction.IVideoCapturer;

namespace WebRTC.Droid.Demo
{
    public class VideoCapturerFactory : IVideoCapturerFactory
    {
        private readonly Context _context;


        public VideoCapturerFactory(Context context)
        {
            _context = context;
        }

        public IVideoCapturer CreateVideoCapturer()
        {
            Org.Webrtc.IVideoCapturer videoCapturer;

            videoCapturer = UseCamera2()
                ? CreateCameraCapturer(new Camera2Enumerator(_context))
                : CreateCameraCapturer(new Camera1Enumerator(false));

            if (videoCapturer == null)
                return null;

            return new VideoCapturerNative(videoCapturer);
        }

        public IVideoSource CreateVideoSource(IVideoCapturer videoCapturer, IPeerConnectionFactory factory)
        {
            var nativeFactory = factory.ToNative<PeerConnectionFactory>();
            return new VideoSourceNative(
                nativeFactory.CreateVideoSource(videoCapturer.ToNative<Org.Webrtc.IVideoCapturer>().IsScreencast));
        }

        private Org.Webrtc.IVideoCapturer CreateCameraCapturer(ICameraEnumerator cameraEnumerator)
        {
            var devicesNames = cameraEnumerator.GetDeviceNames();
            foreach (var devicesName in devicesNames)
            {
                if (cameraEnumerator.IsFrontFacing(devicesName))
                {
                    var videoCapturer = cameraEnumerator.CreateCapturer(devicesName, null);
                    if (videoCapturer != null)
                        return videoCapturer;
                }
            }

            foreach (var devicesName in devicesNames)
            {
                var videoCapturer = cameraEnumerator.CreateCapturer(devicesName, null);
                if (videoCapturer != null)
                    return videoCapturer;
            }

            return null;
        }

        private bool UseCamera2() => Camera2Enumerator.IsSupported(_context);
    }
}
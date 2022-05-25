using Android.Content;
using Org.Webrtc;
using WebRTC.Abstraction;
using ICameraVideoCapturer = Org.Webrtc.ICameraVideoCapturer;

namespace WebRTC.Droid
{
    internal class CameraVideoCapturerNative : VideoCapturerNative, Abstraction.ICameraVideoCapturer
    {
        private readonly ICameraVideoCapturer _videoCapturer;

        public CameraVideoCapturerNative(ICameraVideoCapturer videoCapturer, Context context, VideoSource videoSource,
            IEglBaseContext eglBaseContext) : base(videoCapturer, context, videoSource, eglBaseContext)
        {
            _videoCapturer = videoCapturer;
        }

        public void SwitchCamera()
        {
            _videoCapturer.SwitchCamera(null);
        }

        public void SwitchCamera(RTCCameraDevice device)
        {
            _videoCapturer.SwitchCamera(null, device.DeviceId);
        }
    }
}
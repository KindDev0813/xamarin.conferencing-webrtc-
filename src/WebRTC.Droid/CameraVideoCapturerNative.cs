using Org.Webrtc;

namespace WebRTC.Droid
{
    public class CameraVideoCapturerNative : VideoCapturerNative,Abstraction.ICameraVideoCapturer
    {
        private readonly ICameraVideoCapturer _videoCapturer;
        
        public CameraVideoCapturerNative(ICameraVideoCapturer videoCapturer) : base(videoCapturer)
        {
            _videoCapturer = videoCapturer;
        }

        public void SwitchCamera()
        {
            _videoCapturer.SwitchCamera(null);
        }
    }
}
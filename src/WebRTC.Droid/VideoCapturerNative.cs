using WebRTC.Abstraction;

namespace WebRTC.Droid
{
    public class VideoCapturerNative : NativeObjectBase,IVideoCapturer
    {
        private readonly Org.Webrtc.IVideoCapturer _videoCapturer;
        
        public VideoCapturerNative(Org.Webrtc.IVideoCapturer videoCapturer) : base(videoCapturer)
        {
            _videoCapturer = videoCapturer;
        }

        public void StartCapture(int videoWidth, int videoHeight, int fps)
        {
            _videoCapturer.StartCapture(videoWidth, videoHeight, fps);
        }

        public void StopCapture()
        {
            _videoCapturer.StopCapture();
        }
    }
}
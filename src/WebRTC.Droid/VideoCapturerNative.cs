using Android.Content;
using WebRTC.Abstraction;
using Org.Webrtc;
using IVideoCapturer = WebRTC.Abstraction.IVideoCapturer;

namespace WebRTC.Droid
{
    internal abstract class VideoCapturerNative : NativeObjectBase, IVideoCapturer
    {
        private readonly Org.Webrtc.IVideoCapturer _videoCapturer;
        private readonly SurfaceTextureHelper _surfaceTextureHelper;


        public VideoCapturerNative(Org.Webrtc.IVideoCapturer videoCapturer, Context context, VideoSource videoSource,
            IEglBaseContext eglBaseContext) : base(videoCapturer)
        {
            _videoCapturer = videoCapturer;

            _surfaceTextureHelper = SurfaceTextureHelper.Create("CaptureThread", eglBaseContext);
            videoCapturer.Initialize(_surfaceTextureHelper, context, videoSource.CapturerObserver);
        }

        public override void Dispose()
        {
            _surfaceTextureHelper?.Dispose();
            base.Dispose();
        }

        public bool IsScreencast => _videoCapturer.IsScreencast;

        public void StartCapture()
        {
            StartCapture(0, 0, 0);
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
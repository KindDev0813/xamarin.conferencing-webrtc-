using Android.Content;
using WebRTC.Abstraction;
using Org.Webrtc;

namespace WebRTC.Droid
{
    internal class FileVideoCapturerNative : VideoCapturerNative, IFileVideoCapturer
    {
        private readonly FileVideoCapturer _fileVideoCapturer;

        public FileVideoCapturerNative(FileVideoCapturer fileVideoCapturer, Context context, VideoSource videoSource,
            IEglBaseContext eglBaseContext) : base(fileVideoCapturer, context, videoSource, eglBaseContext)
        {
            _fileVideoCapturer = fileVideoCapturer;
        }
    }
}
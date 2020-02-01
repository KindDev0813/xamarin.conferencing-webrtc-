using WebRTC.Abstraction;
using Org.Webrtc;

namespace WebRTC.Droid
{
    internal class VideoSourceNative : MediaSourceNative, IVideoSource
    {
        private readonly VideoSource _videoSource;

        public VideoSourceNative(VideoSource videoSource) : base(videoSource)
        {
            _videoSource = videoSource;
        }

        public void AdaptOutputFormat(int width, int height, int fps)
        {
            _videoSource.AdaptOutputFormat(width, height, fps);
        }
    }
}
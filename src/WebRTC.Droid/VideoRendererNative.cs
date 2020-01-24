using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.Droid.Extensions;

namespace WebRTC.Droid
{
    public class VideoRendererProxy : Java.Lang.Object, IVideoSink, IVideoRenderer
    {
        public object NativeObject => VideoSink;
        public IVideoSink VideoSink { get; set; }


        public void OnFrame(VideoFrame p0)
        {
            VideoSink?.OnFrame(p0);
        }

    }
}
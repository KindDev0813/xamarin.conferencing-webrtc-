using Org.Webrtc;
using WebRTC.Abstraction;

namespace WebRTC.Droid
{
    public class VideoTrackNative : MediaStreamTrackNative,IVideoTrack
    {
        public VideoTrackNative(VideoTrack videoTrack) : base(videoTrack)
        {
            
        }
    }
}
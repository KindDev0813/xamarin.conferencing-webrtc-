using WebRTC.Abstraction;
using WebRTC.Abstraction.Extensions;
using Org.Webrtc;

namespace WebRTC.Droid
{
    internal class VideoTrackNative : MediaStreamTrackNative, IVideoTrack
    {
        private readonly VideoTrack _videoTrack;

        public VideoTrackNative(VideoTrack videoTrack) : base(videoTrack)
        {
            _videoTrack = videoTrack;
        }

        public void AddRenderer(IVideoRenderer videoRenderer)
        {
            _videoTrack.AddSink(videoRenderer.ToNative<IVideoSink>());
        }

        public void RemoveRenderer(IVideoRenderer videoRenderer)
        {
            _videoTrack.RemoveSink(videoRenderer.ToNative<IVideoSink>());
        }
    }
}
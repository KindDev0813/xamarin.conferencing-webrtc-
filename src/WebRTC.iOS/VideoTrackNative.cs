using System;
using WebRTC.Abstraction;

namespace WebRTC.iOS
{
    internal class VideoTrackNative : MediaStreamTrackNative, IVideoTrack
    {
        private readonly RTCVideoTrack _videoTrack;

        public VideoTrackNative(RTCVideoTrack videoTrack) : base(videoTrack)
        {
            _videoTrack = videoTrack;
        }
        

        public void AddRenderer(IVideoRenderer videoRenderer)
        {
            throw new NotImplementedException();
        }

        public void RemoveRenderer(IVideoRenderer videoRenderer)
        {
            throw new NotImplementedException();
        }
    }
}
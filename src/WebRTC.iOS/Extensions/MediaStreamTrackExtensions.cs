using System;
using WebRTC.Abstraction;

namespace WebRTC.iOS.Extensions
{
    internal static class MediaStreamTrackExtensions
    {
        public static RTCMediaStreamTrack ToNative(this IMediaStreamTrack self)
        {
            return self.ToNative<RTCMediaStreamTrack>();
        }
        
        public static IMediaStreamTrack ToNet(this RTCMediaStreamTrack self)
        {
            switch (self.Kind)
            {
                case Constants.AudioTrackKind:
                    return new AudioTrackNative((RTCAudioTrack) self);
                case Constants.VideoTrackKind:
                    return new VideoTrackNative((RTCVideoTrack) self);
                default:
                    throw new ArgumentOutOfRangeException(nameof(self),self,null);
            }
        }
    }
}
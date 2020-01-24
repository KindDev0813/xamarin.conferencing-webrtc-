using System;
using Org.Webrtc;
using WebRTC.Abstraction;

namespace WebRTC.Droid.Extensions
{
    internal static class MediaStreamTrackExtensions
    {
        public static MediaStreamTrack ToNative(this IMediaStreamTrack self)
        {
            return self.ToNative<MediaStreamTrack>();
        }
        
        public static IMediaStreamTrack ToNet(this MediaStreamTrack self)
        {
            switch (self.Kind())
            {
                case MediaStreamTrack.AudioTrackKind:
                    return new AudioTrackNative((AudioTrack) self);
                case MediaStreamTrack.VideoTrackKind:
                    return new VideoTrackNative((VideoTrack) self);
                default:
                    throw new ArgumentOutOfRangeException(nameof(self),self,null);
            }
        }
    }
}
using System;
using WebRTC.Abstraction;
using WebRTC.Abstraction.Extensions;
using Org.Webrtc;

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
                case Constants.AudioTrackKind:
                    return new AudioTrackNative((AudioTrack) self);
                case Constants.VideoTrackKind:
                    return new VideoTrackNative((VideoTrack) self);
                default:
                    throw new ArgumentOutOfRangeException(nameof(self),self,null);
            }
        }
    }
}
using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.Droid.Extensions;

namespace WebRTC.Droid
{
    public class MediaStreamTrackNative : IMediaStreamTrack
    {
        public MediaStreamTrackNative(MediaStreamTrack mediaStreamTrack)
        {
            NativeTrack = mediaStreamTrack;
        }
        
        public MediaStreamTrack NativeTrack { get; }
        
        public string Kind => NativeTrack.Kind();
        
        public string Label => NativeTrack.Id();
        
        public bool Enable
        {
            get => NativeTrack.Enabled();
            set => NativeTrack.SetEnabled(value);
        }

        public SourceState State => NativeTrack.InvokeState().ToNet();
    }
}